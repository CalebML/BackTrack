namespace BackTrack
{
    partial class BackTrack
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
            this.ReadData = new System.Windows.Forms.Button();
            this.clearSDCard = new System.Windows.Forms.Button();
            this.MainMap = new GMap.NET.WindowsForms.GMapControl();
            this.ZoomIn = new System.Windows.Forms.Button();
            this.ZoomOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveHike = new System.Windows.Forms.Button();
            this.LoadHike = new System.Windows.Forms.Button();
            this.openHikeDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveHikeDialog = new System.Windows.Forms.SaveFileDialog();
            this.AddPoint = new System.Windows.Forms.Button();
            this.RemovePoint = new System.Windows.Forms.Button();
            this.StartPoint = new System.Windows.Forms.ComboBox();
            this.EndPoint = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AvgSpeed = new System.Windows.Forms.Label();
            this.KmHr = new System.Windows.Forms.RadioButton();
            this.FtMin = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // ReadData
            // 
            this.ReadData.Location = new System.Drawing.Point(12, 12);
            this.ReadData.Name = "ReadData";
            this.ReadData.Size = new System.Drawing.Size(90, 44);
            this.ReadData.TabIndex = 0;
            this.ReadData.Text = "Read data from SD card";
            this.ReadData.UseVisualStyleBackColor = true;
            this.ReadData.Click += new System.EventHandler(this.ReadData_Click);
            // 
            // clearSDCard
            // 
            this.clearSDCard.Location = new System.Drawing.Point(108, 12);
            this.clearSDCard.Name = "clearSDCard";
            this.clearSDCard.Size = new System.Drawing.Size(90, 44);
            this.clearSDCard.TabIndex = 1;
            this.clearSDCard.Text = "Clear SD card";
            this.clearSDCard.UseVisualStyleBackColor = true;
            this.clearSDCard.Click += new System.EventHandler(this.clearSDCard_Click);
            // 
            // MainMap
            // 
            this.MainMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainMap.Bearing = 0F;
            this.MainMap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MainMap.CanDragMap = true;
            this.MainMap.EmptyTileColor = System.Drawing.Color.Navy;
            this.MainMap.GrayScaleMode = false;
            this.MainMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.MainMap.LevelsKeepInMemmory = 5;
            this.MainMap.Location = new System.Drawing.Point(12, 62);
            this.MainMap.MarkersEnabled = true;
            this.MainMap.MaxZoom = 18;
            this.MainMap.MinZoom = 2;
            this.MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.MainMap.Name = "MainMap";
            this.MainMap.NegativeMode = false;
            this.MainMap.PolygonsEnabled = true;
            this.MainMap.RetryLoadTile = 0;
            this.MainMap.RoutesEnabled = true;
            this.MainMap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.MainMap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.MainMap.ShowTileGridLines = false;
            this.MainMap.Size = new System.Drawing.Size(519, 305);
            this.MainMap.TabIndex = 2;
            this.MainMap.Zoom = 0D;
            // 
            // ZoomIn
            // 
            this.ZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoomIn.Location = new System.Drawing.Point(484, 13);
            this.ZoomIn.Name = "ZoomIn";
            this.ZoomIn.Size = new System.Drawing.Size(47, 43);
            this.ZoomIn.TabIndex = 3;
            this.ZoomIn.Text = "+";
            this.ZoomIn.UseVisualStyleBackColor = true;
            this.ZoomIn.Click += new System.EventHandler(this.ZoomIn_Click);
            // 
            // ZoomOut
            // 
            this.ZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZoomOut.Location = new System.Drawing.Point(538, 13);
            this.ZoomOut.Name = "ZoomOut";
            this.ZoomOut.Size = new System.Drawing.Size(44, 43);
            this.ZoomOut.TabIndex = 4;
            this.ZoomOut.Text = "-";
            this.ZoomOut.UseVisualStyleBackColor = true;
            this.ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(441, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Zoom:";
            // 
            // SaveHike
            // 
            this.SaveHike.Enabled = false;
            this.SaveHike.Location = new System.Drawing.Point(204, 13);
            this.SaveHike.Name = "SaveHike";
            this.SaveHike.Size = new System.Drawing.Size(90, 44);
            this.SaveHike.TabIndex = 6;
            this.SaveHike.Text = "Save Hike";
            this.SaveHike.UseVisualStyleBackColor = true;
            this.SaveHike.Click += new System.EventHandler(this.SaveHike_Click);
            // 
            // LoadHike
            // 
            this.LoadHike.Location = new System.Drawing.Point(300, 13);
            this.LoadHike.Name = "LoadHike";
            this.LoadHike.Size = new System.Drawing.Size(90, 44);
            this.LoadHike.TabIndex = 7;
            this.LoadHike.Text = "Load Hike from PC";
            this.LoadHike.UseVisualStyleBackColor = true;
            this.LoadHike.Click += new System.EventHandler(this.LoadHike_Click);
            // 
            // openHikeDialog
            // 
            this.openHikeDialog.FileName = "openHikeDialog";
            this.openHikeDialog.Filter = "BackTrack Hike Files (*.hike)|*.hike|All Files|*.*";
            // 
            // saveHikeDialog
            // 
            this.saveHikeDialog.Filter = "BackTrack Hike Files (*.hike)|*.hike";
            // 
            // AddPoint
            // 
            this.AddPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddPoint.Location = new System.Drawing.Point(538, 77);
            this.AddPoint.Name = "AddPoint";
            this.AddPoint.Size = new System.Drawing.Size(65, 44);
            this.AddPoint.TabIndex = 8;
            this.AddPoint.Text = " Add \r\nPoint";
            this.AddPoint.UseVisualStyleBackColor = true;
            // 
            // RemovePoint
            // 
            this.RemovePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemovePoint.Enabled = false;
            this.RemovePoint.Location = new System.Drawing.Point(538, 127);
            this.RemovePoint.Name = "RemovePoint";
            this.RemovePoint.Size = new System.Drawing.Size(65, 44);
            this.RemovePoint.TabIndex = 9;
            this.RemovePoint.Text = "Remove Point";
            this.RemovePoint.UseVisualStyleBackColor = true;
            this.RemovePoint.Click += new System.EventHandler(this.RemovePoint_Click);
            // 
            // StartPoint
            // 
            this.StartPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StartPoint.FormattingEnabled = true;
            this.StartPoint.Location = new System.Drawing.Point(538, 209);
            this.StartPoint.Name = "StartPoint";
            this.StartPoint.Size = new System.Drawing.Size(65, 21);
            this.StartPoint.TabIndex = 10;
            this.StartPoint.SelectedIndexChanged += new System.EventHandler(this.StartPoint_SelectedIndexChanged);
            // 
            // EndPoint
            // 
            this.EndPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EndPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EndPoint.FormattingEnabled = true;
            this.EndPoint.Location = new System.Drawing.Point(538, 249);
            this.EndPoint.Name = "EndPoint";
            this.EndPoint.Size = new System.Drawing.Size(65, 21);
            this.EndPoint.TabIndex = 11;
            this.EndPoint.SelectedIndexChanged += new System.EventHandler(this.EndPoint_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(537, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 26);
            this.label2.TabIndex = 12;
            this.label2.Text = "Avg Speed \r\n  between ";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(557, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "and";
            // 
            // AvgSpeed
            // 
            this.AvgSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AvgSpeed.AutoSize = true;
            this.AvgSpeed.Location = new System.Drawing.Point(545, 273);
            this.AvgSpeed.Name = "AvgSpeed";
            this.AvgSpeed.Size = new System.Drawing.Size(45, 13);
            this.AvgSpeed.TabIndex = 14;
            this.AvgSpeed.Text = "0 Km/hr";
            // 
            // KmHr
            // 
            this.KmHr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.KmHr.AutoSize = true;
            this.KmHr.Checked = true;
            this.KmHr.Location = new System.Drawing.Point(537, 289);
            this.KmHr.Name = "KmHr";
            this.KmHr.Size = new System.Drawing.Size(54, 17);
            this.KmHr.TabIndex = 15;
            this.KmHr.TabStop = true;
            this.KmHr.Text = "Km/hr";
            this.KmHr.UseVisualStyleBackColor = true;
            this.KmHr.CheckedChanged += new System.EventHandler(this.KmHr_CheckedChanged);
            // 
            // FtMin
            // 
            this.FtMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FtMin.AutoSize = true;
            this.FtMin.Location = new System.Drawing.Point(537, 312);
            this.FtMin.Name = "FtMin";
            this.FtMin.Size = new System.Drawing.Size(55, 17);
            this.FtMin.TabIndex = 16;
            this.FtMin.Text = "Ft/min";
            this.FtMin.UseVisualStyleBackColor = true;
            // 
            // BackTrack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 379);
            this.Controls.Add(this.FtMin);
            this.Controls.Add(this.KmHr);
            this.Controls.Add(this.AvgSpeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EndPoint);
            this.Controls.Add(this.StartPoint);
            this.Controls.Add(this.RemovePoint);
            this.Controls.Add(this.AddPoint);
            this.Controls.Add(this.LoadHike);
            this.Controls.Add(this.SaveHike);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ZoomOut);
            this.Controls.Add(this.ZoomIn);
            this.Controls.Add(this.MainMap);
            this.Controls.Add(this.clearSDCard);
            this.Controls.Add(this.ReadData);
            this.MinimumSize = new System.Drawing.Size(600, 220);
            this.Name = "BackTrack";
            this.Text = "BackTrack - The Software!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ReadData;
        private System.Windows.Forms.Button clearSDCard;
        private GMap.NET.WindowsForms.GMapControl MainMap;
        private GMap.NET.WindowsForms.GMapOverlay Points;
        private GMap.NET.WindowsForms.GMapOverlay Route;
        private System.Windows.Forms.Button ZoomIn;
        private System.Windows.Forms.Button ZoomOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveHike;
        private System.Windows.Forms.Button LoadHike;
        private System.Windows.Forms.OpenFileDialog openHikeDialog;
        private System.Windows.Forms.SaveFileDialog saveHikeDialog;
        private System.Windows.Forms.Button AddPoint;
        private System.Windows.Forms.Button RemovePoint;
        private System.Windows.Forms.ComboBox StartPoint;
        private System.Windows.Forms.ComboBox EndPoint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label AvgSpeed;
        private System.Windows.Forms.RadioButton KmHr;
        private System.Windows.Forms.RadioButton FtMin;
        //GMapOverlay test
    }
}

