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
            int dpc = 0; // Total number of data points

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

            // Select 5-minute period by default
            periodCBox.SelectedIndex = 1;

            // Allow chart zoom and set DateTime format
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd/yy hh:mm:ss";
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
                var result = cw.ListMetrics(metricsRequest).ListMetricsResult;
                var tempList = result.Metrics;
                // metricList.AddRange(result.Metrics);
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

            // For all metrics
            fromindex[0] = 0;
            toindex[0] = metricList.Count;

            // Initialize plottedStat
            for (int i = 0; i < metricList.Count; i++)
            {
                plottedStat.Add(-1); // no stat was plotted
            }
        }

        private void zoomTBar_Scroll(object sender, EventArgs e)
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
                    exp[i] = exp[i - 1]*0.7;
                }
                chart1.ChartAreas[0].AxisX.ScaleView.Size = dTemp*exp[zoomTBar.Value];
                chart1.ChartAreas[0].AxisX.ScaleView.MinSize = dTemp*exp[zoomTBar.Value];
                chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = (dTemp*exp[zoomTBar.Value])/10;
                chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = (dTemp*exp[zoomTBar.Value])/10;
            }
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
                                           point.YValues[0],
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

        // Change the stat of a plotted metric
        private void statCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox statCBox = (ComboBox) sender;
            int metricID = Convert.ToInt32(statCBox.Name);
            plottedStat[metricID] = statCBox.SelectedIndex;
            refreshButton_Click(sender, e);
        }
        
        // Add a combo box for a plotted metric to select the desired statistics
        private void addCBox(int foundmetricID, int searchLViewID, bool usedefault)
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

            if (usedefault)
            {
                plottedStat[foundmetricID] = 0; // Choose Average stat by default
            }
            statCBox.SelectedIndex = plottedStat[foundmetricID];

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
                    addCBox(foundmetricID, i, false);
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
            searchLView.Controls.RemoveByKey(metricID.ToString());
            statCBox.Dispose();
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
                    plottedMetricIDs.Add(metricID);
                    addCBox(metricID, e.Item.Index, true);
                    refreshButton_Click(sender, e);
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
                    plottedMetricIDs.RemoveAt(pID);
                    refreshButton_Click(sender, e);
                    removeCBox(metricID);
                }
                // If the logic is correct, this else branch will never be reached
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // For Debug
            //foreach (int ID in plottedMetricID)
            //{
            //    Debug.Write(ID.ToString() + " ");
            //}
            //Debug.WriteLine(plottedMetricID.Count);

            try
            {
                DateTime.TryParse(startTBox.Text, out startdate);
                DateTime.TryParse(endTBox.Text, out enddate);

                int days = (int) (enddate - startdate).TotalDays;

                // Allow up to 100 days for data retrieval, modify if needed
                if (days <= 0 || days > 100)
                {
                    MessageBox.Show("Check your dates");
                    return;
                }

                int period = 0;
                switch (periodCBox.Text)
                {
                    case "1 Minute":
                        period = 1*60;
                        break;
                    case "5 Minutes":
                        period = 5*60;
                        break;
                    case "15 Minutes":
                        period = 15*60;
                        break;
                    case "1 Hour":
                        period = 3600;
                        break;
                    case "6 Hours":
                        period = 6*3600;
                        break;
                    case "1 Day":
                        period = 24*3600;
                        break;
                }

                // Clear the chart
                chart1.Series.Clear();

                // Initialize the total number of data points
                dpc = 0;
                plottedDataPoints.Clear();

                for (int i = 0; i < plottedMetricIDs.Count; i++)
                {
                    int plottedMetricID = plottedMetricIDs[i];
                    Metric metric = metricList[plottedMetricID];
                    GetMetricStatisticsRequest request = new GetMetricStatisticsRequest();
                    request.MetricName = metric.MetricName;
                    request.Namespace = metric.Namespace;
                    request.Dimensions = metric.Dimensions;
                    request.Statistics.AddRange(statList);
                    request.Period = period;

                    string name = String.Format("{0} {1} {2}",
                                                request.MetricName,
                                                request.Namespace,
                                                statList[plottedStat[plottedMetricID]]);
                    foreach (var dim in request.Dimensions)
                    {
                        name += String.Format(" {0}={1}",
                                              dim.Name,
                                              dim.Value);
                    }
                    Series series = new Series(name);
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = 2;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.MarkerSize = 5;

                    if (i < preferredColor.Count)
                    {
                        series.Color = preferredColor[i];
                        series.MarkerColor = preferredColor[i];
                    }

                    List<Datapoint> datapoints = new List<Datapoint>();
                    for (int day = 0; day < days; day++)
                    {
                        request.StartTime = startdate.AddDays(day);
                        request.EndTime = startdate.AddDays(day + 1);
                        GetMetricStatisticsResult result = cw.GetMetricStatistics(request).GetMetricStatisticsResult;
                        datapoints.AddRange(result.Datapoints);
                    }
                    datapoints.Sort((p1, p2) => DateTime.Compare(p1.Timestamp, p2.Timestamp));
                    switch (plottedStat[plottedMetricID])
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

                    // Add series to chart
                    chart1.Series.Add(series);
                    chart1.Legends.Last().Docking = Docking.Bottom;
                    dpc += datapoints.Count();
                    plottedDataPoints.Add(datapoints);
                }
                
                // Set chart title
                chart1.Titles.Clear();
                chart1.Titles.Add(String.Format("{0} series, {1} data points",
                    plottedMetricIDs.Count,
                    dpc));

                zoomTBar_Scroll(sender, e);
                
                // Allow zoom feature
                zoomTBar.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
