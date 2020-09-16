namespace FS2020PlanePath
{
    partial class MainPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            this.label2 = new System.Windows.Forms.Label();
            this.ThresholdLogWriteFreqTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.KMLFolderBrowser = new System.Windows.Forms.Button();
            this.KMLFilePathTBRO = new System.Windows.Forms.TextBox();
            this.SimConnectStatusLabel = new System.Windows.Forms.Label();
            this.SimConnetStatusTextBox = new System.Windows.Forms.Label();
            this.StartLoggingBtn = new System.Windows.Forms.Button();
            this.StopLoggingBtn = new System.Windows.Forms.Button();
            this.CreateKMLButton = new System.Windows.Forms.Button();
            this.PauseLoggingBtn = new System.Windows.Forms.Button();
            this.ContinueLogginBtn = new System.Windows.Forms.Button();
            this.RetrySimConnectionBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ThresholdMinAltTB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.FlightPickerLV = new System.Windows.Forms.ListView();
            this.DeleteFlight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(242, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Above Threshold Frequency to Write Entry (secs):";
            // 
            // ThresholdLogWriteFreqTB
            // 
            this.ThresholdLogWriteFreqTB.Location = new System.Drawing.Point(256, 90);
            this.ThresholdLogWriteFreqTB.Name = "ThresholdLogWriteFreqTB";
            this.ThresholdLogWriteFreqTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdLogWriteFreqTB.TabIndex = 5;
            this.ThresholdLogWriteFreqTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LogWriteFreqTB_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "KML File Path:";
            // 
            // KMLFolderBrowser
            // 
            this.KMLFolderBrowser.Location = new System.Drawing.Point(422, 360);
            this.KMLFolderBrowser.Name = "KMLFolderBrowser";
            this.KMLFolderBrowser.Size = new System.Drawing.Size(117, 23);
            this.KMLFolderBrowser.TabIndex = 18;
            this.KMLFolderBrowser.Text = "KML Path Browse...";
            this.KMLFolderBrowser.UseVisualStyleBackColor = true;
            this.KMLFolderBrowser.Click += new System.EventHandler(this.LogFolderBrowser_Click);
            // 
            // KMLFilePathTBRO
            // 
            this.KMLFilePathTBRO.Location = new System.Drawing.Point(125, 362);
            this.KMLFilePathTBRO.Name = "KMLFilePathTBRO";
            this.KMLFilePathTBRO.ReadOnly = true;
            this.KMLFilePathTBRO.Size = new System.Drawing.Size(272, 20);
            this.KMLFilePathTBRO.TabIndex = 17;
            // 
            // SimConnectStatusLabel
            // 
            this.SimConnectStatusLabel.AutoSize = true;
            this.SimConnectStatusLabel.Location = new System.Drawing.Point(119, 9);
            this.SimConnectStatusLabel.Name = "SimConnectStatusLabel";
            this.SimConnectStatusLabel.Size = new System.Drawing.Size(27, 13);
            this.SimConnectStatusLabel.TabIndex = 1;
            this.SimConnectStatusLabel.Text = "N/A";
            // 
            // SimConnetStatusTextBox
            // 
            this.SimConnetStatusTextBox.AutoSize = true;
            this.SimConnetStatusTextBox.Location = new System.Drawing.Point(9, 9);
            this.SimConnetStatusTextBox.Name = "SimConnetStatusTextBox";
            this.SimConnetStatusTextBox.Size = new System.Drawing.Size(100, 13);
            this.SimConnetStatusTextBox.TabIndex = 0;
            this.SimConnetStatusTextBox.Text = "SimConnect Status:";
            // 
            // StartLoggingBtn
            // 
            this.StartLoggingBtn.Enabled = false;
            this.StartLoggingBtn.Location = new System.Drawing.Point(13, 157);
            this.StartLoggingBtn.Name = "StartLoggingBtn";
            this.StartLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.StartLoggingBtn.TabIndex = 8;
            this.StartLoggingBtn.Text = "Start Logging";
            this.StartLoggingBtn.UseVisualStyleBackColor = true;
            this.StartLoggingBtn.Click += new System.EventHandler(this.StartLoggingBtn_Click);
            // 
            // StopLoggingBtn
            // 
            this.StopLoggingBtn.Enabled = false;
            this.StopLoggingBtn.Location = new System.Drawing.Point(442, 157);
            this.StopLoggingBtn.Name = "StopLoggingBtn";
            this.StopLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.StopLoggingBtn.TabIndex = 11;
            this.StopLoggingBtn.Text = "Stop Logging";
            this.StopLoggingBtn.UseVisualStyleBackColor = true;
            this.StopLoggingBtn.Click += new System.EventHandler(this.StopLoggingBtn_Click);
            // 
            // CreateKMLButton
            // 
            this.CreateKMLButton.Location = new System.Drawing.Point(139, 417);
            this.CreateKMLButton.Name = "CreateKMLButton";
            this.CreateKMLButton.Size = new System.Drawing.Size(97, 23);
            this.CreateKMLButton.TabIndex = 19;
            this.CreateKMLButton.Text = "Create KML File";
            this.CreateKMLButton.UseVisualStyleBackColor = true;
            this.CreateKMLButton.Click += new System.EventHandler(this.CreateKMLButton_Click);
            // 
            // PauseLoggingBtn
            // 
            this.PauseLoggingBtn.Enabled = false;
            this.PauseLoggingBtn.Location = new System.Drawing.Point(156, 157);
            this.PauseLoggingBtn.Name = "PauseLoggingBtn";
            this.PauseLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.PauseLoggingBtn.TabIndex = 9;
            this.PauseLoggingBtn.Text = "Pause Logging";
            this.PauseLoggingBtn.UseVisualStyleBackColor = true;
            this.PauseLoggingBtn.Click += new System.EventHandler(this.PauseLoggingBtn_Click);
            // 
            // ContinueLogginBtn
            // 
            this.ContinueLogginBtn.Enabled = false;
            this.ContinueLogginBtn.Location = new System.Drawing.Point(299, 157);
            this.ContinueLogginBtn.Name = "ContinueLogginBtn";
            this.ContinueLogginBtn.Size = new System.Drawing.Size(98, 23);
            this.ContinueLogginBtn.TabIndex = 10;
            this.ContinueLogginBtn.Text = "Continue Logging";
            this.ContinueLogginBtn.UseVisualStyleBackColor = true;
            this.ContinueLogginBtn.Click += new System.EventHandler(this.ContinueLogginBtn_Click);
            // 
            // RetrySimConnectionBtn
            // 
            this.RetrySimConnectionBtn.Enabled = false;
            this.RetrySimConnectionBtn.Location = new System.Drawing.Point(303, 4);
            this.RetrySimConnectionBtn.Name = "RetrySimConnectionBtn";
            this.RetrySimConnectionBtn.Size = new System.Drawing.Size(125, 23);
            this.RetrySimConnectionBtn.TabIndex = 2;
            this.RetrySimConnectionBtn.Text = "Retry Sim Connection";
            this.RetrySimConnectionBtn.UseVisualStyleBackColor = true;
            this.RetrySimConnectionBtn.Click += new System.EventHandler(this.RetrySimConnectionBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(455, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Logs will be written once a second below alitude threshold.  Above threshold set " +
    "options below.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(238, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Threshold Minimum Altitude Above Ground (feet):";
            // 
            // ThresholdMinAltTB
            // 
            this.ThresholdMinAltTB.Location = new System.Drawing.Point(256, 121);
            this.ThresholdMinAltTB.Name = "ThresholdMinAltTB";
            this.ThresholdMinAltTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdMinAltTB.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(12, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(530, 2);
            this.label5.TabIndex = 12;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "KML Export";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 246);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Choose Flight";
            // 
            // FlightPickerLV
            // 
            this.FlightPickerLV.FullRowSelect = true;
            this.FlightPickerLV.GridLines = true;
            this.FlightPickerLV.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FlightPickerLV.HideSelection = false;
            this.FlightPickerLV.Location = new System.Drawing.Point(125, 236);
            this.FlightPickerLV.Name = "FlightPickerLV";
            this.FlightPickerLV.Size = new System.Drawing.Size(370, 97);
            this.FlightPickerLV.TabIndex = 15;
            this.FlightPickerLV.UseCompatibleStateImageBehavior = false;
            this.FlightPickerLV.View = System.Windows.Forms.View.Details;
            // 
            // DeleteFlight
            // 
            this.DeleteFlight.Location = new System.Drawing.Point(289, 417);
            this.DeleteFlight.Name = "DeleteFlight";
            this.DeleteFlight.Size = new System.Drawing.Size(152, 23);
            this.DeleteFlight.TabIndex = 20;
            this.DeleteFlight.Text = "Delete Flight from Database";
            this.DeleteFlight.UseVisualStyleBackColor = true;
            this.DeleteFlight.Click += new System.EventHandler(this.DeleteFlight_Click);
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 455);
            this.Controls.Add(this.DeleteFlight);
            this.Controls.Add(this.FlightPickerLV);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ThresholdMinAltTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RetrySimConnectionBtn);
            this.Controls.Add(this.ContinueLogginBtn);
            this.Controls.Add(this.PauseLoggingBtn);
            this.Controls.Add(this.CreateKMLButton);
            this.Controls.Add(this.StopLoggingBtn);
            this.Controls.Add(this.StartLoggingBtn);
            this.Controls.Add(this.SimConnectStatusLabel);
            this.Controls.Add(this.SimConnetStatusTextBox);
            this.Controls.Add(this.KMLFilePathTBRO);
            this.Controls.Add(this.KMLFolderBrowser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ThresholdLogWriteFreqTB);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "Pilot Path Recorder v1.0.2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainPage_FormClosing);
            this.Shown += new System.EventHandler(this.MainPage_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ThresholdLogWriteFreqTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button KMLFolderBrowser;
        private System.Windows.Forms.TextBox KMLFilePathTBRO;
        private System.Windows.Forms.Label SimConnectStatusLabel;
        private System.Windows.Forms.Label SimConnetStatusTextBox;
        private System.Windows.Forms.Button StartLoggingBtn;
        private System.Windows.Forms.Button StopLoggingBtn;
        private System.Windows.Forms.Button CreateKMLButton;
        private System.Windows.Forms.Button PauseLoggingBtn;
        private System.Windows.Forms.Button ContinueLogginBtn;
        private System.Windows.Forms.Button RetrySimConnectionBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ThresholdMinAltTB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListView FlightPickerLV;
        private System.Windows.Forms.Button DeleteFlight;
    }
}

