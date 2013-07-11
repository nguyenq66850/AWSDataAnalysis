using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;

namespace AWSDataAnalysis
{
    public partial class Form1 : Form
    {
        private
            string AWSAccessKey = System.Configuration.ConfigurationSettings.AppSettings["AWSAccessKey"];
            string AWSSecretKey = System.Configuration.ConfigurationSettings.AppSettings["AWSSecretKey"];
            DateTime startdate;
            DateTime enddate;
            int days = 1; // total number of days between start date and end date, initialized 1
            int period = 300; // 5 minutes by default
            AmazonCloudWatch cw;
            List<Metric> metricList = new List<Metric>();
            List<string> namespaceList = new List<string>()
                {
                    "All Metrics",
                    "AWS/EBS",
                    "AWS/EC2",
                    "AWS/ELB",
                    "AWS/RDS",
                    "AWS/SNS"
                };
            List<string> statList = new List<string>()
                {
                    "Average",
                    "Minimum",
                    "Maximum",
                    "Sum",
                    "SampleCount"
                };
            List<Color> preferredColor = new List<Color>()
                {
                    Color.Blue,
                    Color.Green,
                    Color.Red,
                    Color.BlueViolet,
                    Color.Maroon,
                    Color.Lime,
                    Color.Black,
                    Color.Chocolate,
                    Color.GreenYellow,
                    Color.Aquamarine
                };
            // List<List<Metric>> metricbyNameSpace = new List<List<Metric>>();
            List<List<Datapoint>> plottedDataPoints = new List<List<Datapoint>>();
            List<int> plottedMetricIDs = new List<int>();
            List<int> plottedStat = new List<int>();
            List<int> plottedColorID = new List<int>();
            List<int> searchResult = new List<int>();
            int[] fromindex;
            int[] toindex;
            bool raisedFromCode = false; // by default, an item in the List View is checked by a user
            int dpc = 0; // total number of data points

        public Form1()
        {
            InitializeComponent();

            // Initialize name space combo box
            foreach (string ns in namespaceList)
            {
                selectNSCBox.Items.Add(ns);
            }

            // Each name space contains metrics between from and to indices in the metricList
            fromindex = new int[namespaceList.Count];
            toindex = new int[namespaceList.Count];

            // Select All metrics by default
            selectNSCBox.SelectedIndex = 0;

            // Set default dates
            enddate = DateTime.Today;
            startdate = enddate.AddDays(-1);
            resetDate();
            
            // Select 5-minute period by default
            periodCBox.SelectedIndex = 1;
            period = 300;

            // Allow chart zoom and set DateTime format
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd/yy hh:mm:ss";
            // chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0.00";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // Initialize Cloud Watch with region APNorthEast1 (Tokyo)
            cw = Amazon.AWSClientFactory.CreateAmazonCloudWatchClient(AWSAccessKey, AWSSecretKey,
                                                                      RegionEndpoint.APNortheast1);

            // Retrieve the list of Metrics
            ListMetricsRequest metricsRequest = new ListMetricsRequest();
            for (int i = 1; i < namespaceList.Count; i++)
            {
                fromindex[i] = metricList.Count;
                metricsRequest.Namespace = namespaceList[i];
                metricsRequest.NextToken = null;
                try
                {
                    var result = cw.ListMetrics(metricsRequest).ListMetricsResult;
                    var tempList = result.Metrics;
                    while (result.NextToken != null)
                    {
                        metricsRequest.NextToken = result.NextToken;
                        result = cw.ListMetrics(metricsRequest).ListMetricsResult;
                        tempList.AddRange(result.Metrics);
                    }
                    tempList = tempList.OrderBy(a => a.Dimensions.Count)
                        .ThenBy(a => a.MetricName)
                        .ThenBy(a => a.Dimensions.Count > 0 ? a.Dimensions[0].Name : "")
                        .ThenBy(a => a.Dimensions.Count > 0 ? a.Dimensions[0].Value : "")
                        .ThenBy(a => a.Dimensions.Count > 1 ? a.Dimensions[1].Name : "")
                        .ThenBy(a => a.Dimensions.Count > 1 ? a.Dimensions[1].Value : "").ToList();
                    metricList.AddRange(tempList);
                    toindex[i] = metricList.Count;
                    // Debug.WriteLine(metricList.Count);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Close();
                    return;
                }
            }

            // For all metrics
            fromindex[0] = 0;
            toindex[0] = metricList.Count;

            // Initialize plottedStat
            for (int i = 0; i < metricList.Count; i++)
            {
                plottedStat.Add(-1); // no stat was plotted
            }
        }

        private void updateZoom()
        {
            chart1.ChartAreas[0].RecalculateAxesScale();
            if (zoomTBar.Value == 0)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
            else
            {
                double dTemp = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum -
                               chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                //dTemp = (enddate - startdate).TotalDays;
                double[] exp = new double[30];
                exp[0] = 1;
                for (int i = 1; i < 30; i++)
                {
                    exp[i] = exp[i - 1] * 0.7;
                }
                chart1.ChartAreas[0].AxisX.ScaleView.Size = dTemp * exp[zoomTBar.Value];
                chart1.ChartAreas[0].AxisX.ScaleView.MinSize = dTemp * exp[zoomTBar.Value];
                chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = (dTemp * exp[zoomTBar.Value]) / 10;
                chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = (dTemp * exp[zoomTBar.Value]) / 10;
            }
        }
        
        private void zoomTBar_Scroll(object sender, EventArgs e)
        {
            updateZoom();
        }

        private void chart_Click(object sender, EventArgs e)
        {
            // Allow zoom feature
            zoomTBar.Select();
        }

        private void chart_MouseMove(object sender, MouseEventArgs e)
        {
            // Use HitTest to return information of a data point
            HitTestResult result = chart1.HitTest(e.X, e.Y);
            if (result.PointIndex >= 0 && result.ChartElementType == ChartElementType.DataPoint)
            {
                var tokens = result.Series.Name.Split(' ');
                DataPoint point = result.Series.Points[result.PointIndex];
                string tip = String.Format("Value: {0}\nTime: {1}\nMetric: {2}\nNameSpace: {3}\nStat: {4}",
                                           point.YValues[0].ToString("#,##0.00"),
                                           DateTime.FromOADate(point.XValue).ToString("MM/dd/yy HH:mm:ss"),
                                           tokens[0],
                                           tokens[1],
                                           tokens[2]);
                for (int i = 3; i < tokens.Count(); i++)
                {
                    var dim = tokens[i].Split('=');
                    tip += String.Format("\n{0}: {1}", dim[0], dim[1]);
                }
                toolTip1.SetToolTip(chart1, tip);
            }
        }

        // Check if a metric description contains a search string
        private bool text_in_metric(string text, Metric metric)
        {
            if (metric.MetricName.ToUpper().Contains(text) || metric.Namespace.ToUpper().Contains(text))
            {
                return true;
            }
            else
            {
                foreach (Dimension dim in metric.Dimensions)
                {
                    if (dim.Name.ToUpper().Contains(text) || dim.Value.ToUpper().Contains(text))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void addPointsToSeries(List<Datapoint> datapoints, Series series, int stat)
        {
            series.Points.Clear();
            switch (stat)
            {
                case 0:
                    foreach (Datapoint point in datapoints)
                    {
                        point.Timestamp = point.Timestamp.ToUniversalTime();
                        series.Points.AddXY(point.Timestamp, point.Average);
                    }
                    break;
                case 1:
                    foreach (Datapoint point in datapoints)
                    {
                        point.Timestamp = point.Timestamp.ToUniversalTime();
                        series.Points.AddXY(point.Timestamp, point.Minimum);
                    }
                    break;
                case 2:
                    foreach (Datapoint point in datapoints)
                    {
                        point.Timestamp = point.Timestamp.ToUniversalTime();
                        series.Points.AddXY(point.Timestamp, point.Maximum);
                    }
                    break;
                case 3:
                    foreach (Datapoint point in datapoints)
                    {
                        point.Timestamp = point.Timestamp.ToUniversalTime();
                        series.Points.AddXY(point.Timestamp, point.Sum);
                    }
                    break;
                case 4:
                    foreach (Datapoint point in datapoints)
                    {
                        point.Timestamp = point.Timestamp.ToUniversalTime();
                        series.Points.AddXY(point.Timestamp, point.SampleCount);
                    }
                    break;
            }
        }
        
        // Change the stat of a plotted metric
        private void statCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox statCBox = (ComboBox) sender;
            int metricID = Convert.ToInt32(statCBox.Name);
            int pID = plotID(metricID); // pID must be different than -1

            // Update series
            plottedStat[metricID] = statCBox.SelectedIndex; // new stat
            chart1.Series[pID].Name = getSeriesName(metricList[metricID], plottedStat[metricID]); // new name corresponding to new stat
            addPointsToSeries(plottedDataPoints[pID], chart1.Series[pID], plottedStat[metricID]); // new data
        }
        
        // Add a combo box for a plotted metric to select the desired statistics
        private void addCBox(int foundmetricID, int searchLViewID)
        {
            ComboBox statCBox = new ComboBox();
            statCBox.Name = foundmetricID.ToString();
            foreach (var stat in statList)
            {
                statCBox.Items.Add(stat);
            }

            int lastcolumn = searchLView.Columns.Count - 1;
            statCBox.Size = new Size(searchLView.Columns[lastcolumn].Width, 21);
            Point p = searchLView.Items[searchLViewID].Position;
            p.X = 1;
            for (int i = 0; i < lastcolumn; i++)
            {
                p.X += searchLView.Columns[i].Width;
            }
            p.Y -= 2;
            statCBox.Location = p;
            searchLView.Controls.Add(statCBox);

            // Switching these 2 lines will break the logic
            statCBox.SelectedIndex = plottedStat[foundmetricID]; // not invoke the event handler
            statCBox.SelectedIndexChanged += statCBox_SelectedIndexChanged;
        }
        
        // Search in the metricList with indices from and to
        private void search(string text, int from, int to)
        {
            // Clear old result from previous search
            searchLView.Items.Clear();
            searchLView.Controls.Clear();
            searchResult.Clear();

            // Special search for plotted metrics
            if (text == "PLOT")
            {
                searchResult.AddRange(plottedMetricIDs);
            }
            else
            {
                for (int i = from; i < to; i++)
                    if (text_in_metric(text, metricList[i]))
                    {
                        searchResult.Add(i);
                    }
            }

            Debug.WriteLine(String.Format("Found {0} metrics that contain {1}", searchResult.Count, text));

            // Use searchResult to create searchLView
            for (int i = 0; i < searchResult.Count; i++)
            {
                int foundmetricID = searchResult[i];
                var metric = metricList[foundmetricID];
                ListViewItem item = new ListViewItem(new string[]
                    {
                        // foundmetricID.ToString(), // for Debug
                        metric.MetricName,
                        metric.Namespace
                    });
                foreach (Dimension dim in metric.Dimensions)
                {
                    item.SubItems.Add(dim.Value);
                }

                // Plotted item gets a check mark
                item.Checked = (plotID(foundmetricID) >= 0);

                // Change raisedFromCode to avoid an adding-removing cycle of each checked item
                raisedFromCode = true;
                searchLView.Items.Add(item);
                raisedFromCode = false;

                // Add a combo box with the currently selected statistics
                if (item.Checked)
                {
                    addCBox(foundmetricID, i);
                }
            }
        }
        
        // If a metric was plotted, return its plotID, otherwise return -1
        private int plotID(int metricID)
        {
            for (int i = plottedMetricIDs.Count - 1; i >= 0; i--)
                if (plottedMetricIDs[i] == metricID)
                    {
                        return i;
                    }
            return -1;
        }
        
        private void searchButton_Click(object sender, EventArgs e)
        {
            int nsID = selectNSCBox.SelectedIndex;
            string text = searchTBox.Text.ToUpper();

            search(text, fromindex[nsID], toindex[nsID]);
        }

        private void searchTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchButton_Click(sender, e);
            }
        }

        private void selectNSCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchButton_Click(sender, e);
        }

        private void removeCBox(int metricID)
        {
            ComboBox statCBox = (ComboBox) searchLView.Controls[metricID.ToString()];
            searchLView.Controls.Remove(statCBox);
            statCBox.Dispose();
        }

        private void updateSeriesColor()
        {
            for (int pID = 0; pID < chart1.Series.Count; pID++ )
            {
                if (pID < preferredColor.Count)
                {
                    chart1.Series[pID].Color = preferredColor[pID];
                    chart1.Series[pID].MarkerColor = preferredColor[pID];
                }
                else break;
            }
        }
        
        private void removeSeries(int pID, int metricID)
        {
            chart1.Series.RemoveAt(pID);
            updateSeriesColor();

            plottedDataPoints.RemoveAt(pID);

            plottedMetricIDs.RemoveAt(pID);
            plottedStat[metricID] = -1; 

            removeCBox(metricID);
        }

        private void addSeries(int metricID)
        {
            int pID = plottedMetricIDs.Count; // pID = this old count before adding metricID to the plotted list
            plottedMetricIDs.Add(metricID);
            plottedStat[metricID] = 0; // Choose Average stat by default

            var datapoints = getData(metricID, startdate, days, period);
            plottedDataPoints.Add(datapoints);

            var series = plotSeries(pID, metricID);
            chart1.Series.Add(series);
            chart1.Legends.Last().Docking = Docking.Bottom;
        }

        private void searchLView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // The code takes care of the list of checked items by itself
            if (raisedFromCode) return;

            // If the ItemCheckedEvent is raised by a user
            int nsID = selectNSCBox.SelectedIndex;
            int metricID = searchResult[e.Item.Index];
            if (e.Item.Checked)
                if (plotID(metricID) < 0)
                {
                    addSeries(metricID);
                    addCBox(metricID, e.Item.Index);
                }
                else
                {
                    // Do nothing
                    // If the logic is correct, this else branch will never be reached
                }
            else // Item was unchecked
            {
                int pID = plotID(metricID);
                if (pID >= 0)
                {
                    removeSeries(pID, metricID);
                }
                // If the logic is correct, this else branch will never be reached
            }

            updateChart();

            //// For Debug
            //foreach (int ID in plottedMetricIDs)
            //{
            //    Debug.Write(ID.ToString() + " ");
            //}
            //// These 3 counts must be the same
            //Debug.WriteLine(String.Format("Metric Count: {0}, Dataset Count: {1}, Series Count: {2}",
            //    plottedMetricIDs.Count,
            //    plottedDataPoints.Count,
            //    chart1.Series.Count));
        }

        private List<Datapoint> getData(int metricID, DateTime startdate, int days, int period)
        {
            List<Datapoint> datapoints = new List<Datapoint>();
            Metric metric = metricList[metricID];
            GetMetricStatisticsRequest request = new GetMetricStatisticsRequest();
            request.MetricName = metric.MetricName;
            request.Namespace = metric.Namespace;
            request.Dimensions = metric.Dimensions;
            request.Statistics.AddRange(statList);
            request.Period = period;

            try
            {
                for (int day = 0; day < days; day++)
                {
                    request.StartTime = startdate.AddDays(day);
                    request.EndTime = startdate.AddDays(day + 1);
                    GetMetricStatisticsResult result = cw.GetMetricStatistics(request).GetMetricStatisticsResult;
                    datapoints.AddRange(result.Datapoints);
                }
                datapoints.Sort((p1, p2) => DateTime.Compare(p1.Timestamp, p2.Timestamp));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return datapoints;
        }

        private string getSeriesName(Metric metric, int statID)
        {
            string name = String.Format("{0} {1} {2}",
                                        metric.MetricName,
                                        metric.Namespace,
                                        statList[statID]);
            foreach (var dim in metric.Dimensions)
            {
                name += String.Format(" {0}={1}",
                                        dim.Name,
                                        dim.Value);
            }
            return name;
        }

        private bool parseDate()
        {
            DateTime nstartdate;
            if (!DateTime.TryParse(startTBox.Text, out nstartdate))
            {
                MessageBox.Show("False start date! Resetting ...");
                return false;
            }

            DateTime nenddate;
            if (!DateTime.TryParse(endTBox.Text, out nenddate))
            {
                MessageBox.Show("False end date! Resetting ...");
                return false;
            }

            int ndays = (int)(nenddate - nstartdate).TotalDays;
            if (ndays <= 0)
            {
                MessageBox.Show("End date must be later than start date! Resetting ...");
                return false;
            }
            if (ndays > 100)
            {
                MessageBox.Show("Data retrieval is limited to no more than 100 days! Resetting ...");
                return false;
            }
            
            // Update
            startdate = nstartdate;
            enddate = nenddate;
            days = ndays;
            return true;
        }

        private void resetDate()
        {
            startTBox.Text = startdate.ToShortDateString();
            endTBox.Text = enddate.ToShortDateString();
            days = (int)(enddate - startdate).TotalDays;
        }

        private Series plotSeries(int pID, int metricID)
        {
            Series series = new Series(getSeriesName(metricList[metricID], plottedStat[metricID]));
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 2;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 5;

            // Use preferred color if possible
            if (pID < preferredColor.Count)
            {
                series.Color = preferredColor[pID];
                series.MarkerColor = preferredColor[pID];
            }

            addPointsToSeries(plottedDataPoints[pID], series, plottedStat[metricID]);

            return series;
        }

        private void updateChart()
        {
            dpc = plottedDataPoints.Sum(a => a.Count);
            // Set chart title
            chart1.Titles.Clear();
            chart1.Titles.Add(String.Format("{0} series, {1} data points",
                plottedMetricIDs.Count,
                dpc));
            updateZoom();
        }

        private int getPeriod(string text)
        {
            int period = 300;
            switch (periodCBox.Text)
            {
                case "1 Minute":
                    period = 1 * 60;
                    break;
                case "5 Minutes":
                    period = 5 * 60;
                    break;
                case "15 Minutes":
                    period = 15 * 60;
                    break;
                case "1 Hour":
                    period = 3600;
                    break;
                case "6 Hours":
                    period = 6 * 3600;
                    break;
                case "1 Day":
                    period = 24 * 3600;
                    break;
            }
            return period;
        }
        
        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (!parseDate())
            {
                resetDate();
                return;
            }

            period = getPeriod(periodCBox.Text);

            // Initialize the total number of data points and re-get data
            plottedDataPoints.Clear();
            for (int i = 0; i < plottedMetricIDs.Count; i++)
            {
                var datapoints = getData(plottedMetricIDs[i], startdate, days, period);
                plottedDataPoints.Add(datapoints);
            }

            // Clear the chart and readd series
            chart1.Series.Clear();
            for (int i = 0; i < plottedMetricIDs.Count; i++)
            {
                var series = plotSeries(i, plottedMetricIDs[i]);
                chart1.Series.Add(series);
                chart1.Legends.Last().Docking = Docking.Bottom;
            }

            updateChart();
                
            // Allow zoom feature
            zoomTBar.Select();
        }

        private void periodCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshButton_Click(sender, e);
        }

        private void endTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                refreshButton_Click(sender, e);
            }
        }

        private void startTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                refreshButton_Click(sender, e);
            }
        }

        private void plottedMetricsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search("PLOT", 0, 0);
        }

        private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dpc == 0)
            {
                MessageBox.Show("No data to export");
                return;
            }
            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = ".csv";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string filename = fd.FileName;
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false);
                    for (int i = 0; i < plottedMetricIDs.Count; i++)
                    {
                        Metric metric = metricList[plottedMetricIDs[i]];
                        sw.Write(String.Format("TimeStamp,Average,Minimum,Maximum,Sum,SampleCount,{0},{1}",metric.MetricName, metric.Namespace));
                        foreach (var dim in metric.Dimensions)
                        {
                            sw.Write(String.Format(",{0}={1}",
                                                   dim.Name,
                                                   dim.Value));
                        }
                        sw.WriteLine();
                        for (int j = 0; j < plottedDataPoints[i].Count; j++)
                        {
                            sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5}",
                                                       plottedDataPoints[i][j].Timestamp,
                                                       plottedDataPoints[i][j].Average,
                                                       plottedDataPoints[i][j].Minimum,
                                                       plottedDataPoints[i][j].Maximum,
                                                       plottedDataPoints[i][j].Sum,
                                                       plottedDataPoints[i][j].SampleCount));
                        }
                        sw.WriteLine();
                    }
                    sw.Close();
                    MessageBox.Show("Data exported");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void searchLView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            int XX = 1;
            int lastcolumn = searchLView.Columns.Count - 1;
            for (int i = 0; i < lastcolumn; i++)
            {
                XX += searchLView.Columns[i].Width;
            }
            for (int i = 0; i < searchLView.Controls.Count; i++)
            {
                Point p = searchLView.Controls[i].Location;
                p.X = XX;
                searchLView.Controls[i].Location = p;
                searchLView.Controls[i].Size = new Size(searchLView.Columns[lastcolumn].Width, 21);
            }
        }
    }
}
