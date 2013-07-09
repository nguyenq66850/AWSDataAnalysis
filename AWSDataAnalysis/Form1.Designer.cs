namespace AWSDataAnalysis
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.metricMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.plottedMetricsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startLabel = new System.Windows.Forms.Label();
            this.startTBox = new System.Windows.Forms.TextBox();
            this.endLabel = new System.Windows.Forms.Label();
            this.endTBox = new System.Windows.Forms.TextBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.zoomTBar = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.searchLView = new System.Windows.Forms.ListView();
            this.MetricName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameSpace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dim1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dim2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Statistics = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.selectNSCBox = new System.Windows.Forms.ComboBox();
            this.searchTBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.periodCBox = new System.Windows.Forms.ComboBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.metricMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTBar)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            this.chart1.ContextMenuStrip = this.metricMenu;
            this.chart1.Dock = System.Windows.Forms.DockStyle.Bottom;
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(0, 241);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(1016, 500);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart_Click);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
            // 
            // metricMenu
            // 
            this.metricMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plottedMetricsToolStripMenuItem,
            this.exportDataToolStripMenuItem});
            this.metricMenu.Name = "metricMenu";
            this.metricMenu.Size = new System.Drawing.Size(146, 48);
            // 
            // plottedMetricsToolStripMenuItem
            // 
            this.plottedMetricsToolStripMenuItem.Name = "plottedMetricsToolStripMenuItem";
            this.plottedMetricsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.plottedMetricsToolStripMenuItem.Text = "Plotted Metrics";
            this.plottedMetricsToolStripMenuItem.Click += new System.EventHandler(this.plottedMetricsToolStripMenuItem_Click);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.exportDataToolStripMenuItem.Text = "Export Data";
            this.exportDataToolStripMenuItem.Click += new System.EventHandler(this.exportDataToolStripMenuItem_Click);
            // 
            // startLabel
            // 
            this.startLabel.AutoSize = true;
            this.startLabel.Location = new System.Drawing.Point(450, 19);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(30, 13);
            this.startLabel.TabIndex = 1;
            this.startLabel.Text = "From";
            // 
            // startTBox
            // 
            this.startTBox.Location = new System.Drawing.Point(490, 16);
            this.startTBox.Name = "startTBox";
            this.startTBox.Size = new System.Drawing.Size(70, 20);
            this.startTBox.TabIndex = 2;
            this.startTBox.Text = "7/1/2013";
            this.startTBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.startTBox_KeyDown);
            // 
            // endLabel
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.Location = new System.Drawing.Point(570, 19);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(20, 13);
            this.endLabel.TabIndex = 3;
            this.endLabel.Text = "To";
            // 
            // endTBox
            // 
            this.endTBox.Location = new System.Drawing.Point(600, 16);
            this.endTBox.Name = "endTBox";
            this.endTBox.Size = new System.Drawing.Size(70, 20);
            this.endTBox.TabIndex = 4;
            this.endTBox.Text = "7/5/2013";
            this.endTBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.endTBox_KeyDown);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(770, 15);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(55, 23);
            this.refreshButton.TabIndex = 5;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // zoomTBar
            // 
            this.zoomTBar.Location = new System.Drawing.Point(830, 12);
            this.zoomTBar.Maximum = 18;
            this.zoomTBar.Name = "zoomTBar";
            this.zoomTBar.Size = new System.Drawing.Size(170, 42);
            this.zoomTBar.TabIndex = 6;
            this.zoomTBar.Scroll += new System.EventHandler(this.zoomTBar_Scroll);
            // 
            // searchLView
            // 
            this.searchLView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.searchLView.CheckBoxes = true;
            this.searchLView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MetricName,
            this.NameSpace,
            this.dim1,
            this.dim2,
            this.Statistics});
            this.searchLView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLView.FullRowSelect = true;
            this.searchLView.GridLines = true;
            this.searchLView.HoverSelection = true;
            this.searchLView.Location = new System.Drawing.Point(0, 53);
            this.searchLView.Name = "searchLView";
            this.searchLView.Size = new System.Drawing.Size(1016, 185);
            this.searchLView.TabIndex = 8;
            this.searchLView.UseCompatibleStateImageBehavior = false;
            this.searchLView.View = System.Windows.Forms.View.Details;
            this.searchLView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.searchLView_ColumnWidthChanged);
            this.searchLView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.searchLView_ItemChecked);
            // 
            // MetricName
            // 
            this.MetricName.Text = "MetricName";
            this.MetricName.Width = 250;
            // 
            // NameSpace
            // 
            this.NameSpace.Text = "NameSpace";
            this.NameSpace.Width = 80;
            // 
            // dim1
            // 
            this.dim1.Text = "";
            this.dim1.Width = 350;
            // 
            // dim2
            // 
            this.dim2.Text = "";
            this.dim2.Width = 200;
            // 
            // Statistics
            // 
            this.Statistics.Text = "Statistics";
            this.Statistics.Width = 100;
            // 
            // selectNSCBox
            // 
            this.selectNSCBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.selectNSCBox.FormattingEnabled = true;
            this.selectNSCBox.Location = new System.Drawing.Point(20, 16);
            this.selectNSCBox.Name = "selectNSCBox";
            this.selectNSCBox.Size = new System.Drawing.Size(120, 21);
            this.selectNSCBox.TabIndex = 9;
            this.selectNSCBox.SelectedIndexChanged += new System.EventHandler(this.selectNSCBox_SelectedIndexChanged);
            // 
            // searchTBox
            // 
            this.searchTBox.Location = new System.Drawing.Point(150, 16);
            this.searchTBox.Name = "searchTBox";
            this.searchTBox.Size = new System.Drawing.Size(150, 20);
            this.searchTBox.TabIndex = 10;
            this.searchTBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTBox_KeyDown);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(310, 15);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(55, 23);
            this.searchButton.TabIndex = 11;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // periodCBox
            // 
            this.periodCBox.FormattingEnabled = true;
            this.periodCBox.Items.AddRange(new object[] {
            "1 Minute",
            "5 Minutes",
            "15 Minutes",
            "1 Hour",
            "6 Hours",
            "1 Day"});
            this.periodCBox.Location = new System.Drawing.Point(680, 16);
            this.periodCBox.Name = "periodCBox";
            this.periodCBox.Size = new System.Drawing.Size(80, 21);
            this.periodCBox.TabIndex = 12;
            this.periodCBox.SelectedIndexChanged += new System.EventHandler(this.periodCBox_SelectedIndexChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 238);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1016, 3);
            this.splitter1.TabIndex = 13;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 50);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(1016, 3);
            this.splitter2.TabIndex = 14;
            this.splitter2.TabStop = false;
            this.splitter2.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1016, 50);
            this.panel1.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 741);
            this.Controls.Add(this.searchLView);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.periodCBox);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchTBox);
            this.Controls.Add(this.selectNSCBox);
            this.Controls.Add(this.zoomTBar);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.endTBox);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.startTBox);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AWS Data Analysis";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.metricMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zoomTBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.TextBox startTBox;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.TextBox endTBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.TrackBar zoomTBar;
        private System.Windows.Forms.ContextMenuStrip metricMenu;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListView searchLView;
        private System.Windows.Forms.ComboBox selectNSCBox;
        private System.Windows.Forms.TextBox searchTBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.ComboBox periodCBox;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader MetricName;
        private System.Windows.Forms.ColumnHeader NameSpace;
        private System.Windows.Forms.ColumnHeader dim1;
        private System.Windows.Forms.ColumnHeader dim2;
        private System.Windows.Forms.ToolStripMenuItem plottedMetricsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader Statistics;
    }
}

