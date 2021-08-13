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
            this.AboveThresholdWriteFreqLabel = new System.Windows.Forms.Label();
            this.ThresholdLogWriteFreqTB = new System.Windows.Forms.TextBox();
            this.KMLFilePathLabel = new System.Windows.Forms.Label();
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
            this.LogInfoLabel = new System.Windows.Forms.Label();
            this.ThresholdMinAltLabel = new System.Windows.Forms.Label();
            this.ThresholdMinAltTB = new System.Windows.Forms.TextBox();
            this.SeparateLineLabel = new System.Windows.Forms.Label();
            this.KMLExportLabel = new System.Windows.Forms.Label();
            this.ChooseFlightLabel = new System.Windows.Forms.Label();
            this.FlightPickerLV = new System.Windows.Forms.ListView();
            this.DeleteFlight = new System.Windows.Forms.Button();
            this.GoogleEarthGB = new System.Windows.Forms.GroupBox();
            this.SpeedUpVideoPlaybackCB = new System.Windows.Forms.CheckBox();
            this.ErrorTBRO = new System.Windows.Forms.TextBox();
            this.AutomaticLoggingCB = new System.Windows.Forms.CheckBox();
            this.LoggingThresholdGroundVelTB = new System.Windows.Forms.TextBox();
            this.KMLFileNameGB = new System.Windows.Forms.GroupBox();
            this.FileNameTemplateLabel = new System.Windows.Forms.Label();
            this.KMLFileNameTemplateTB = new System.Windows.Forms.TextBox();
            this.KMLFileNameTBRO = new System.Windows.Forms.TextBox();
            this.FileNameExampleLabel = new System.Windows.Forms.Label();
            this.GoogleEarthWebRB = new System.Windows.Forms.RadioButton();
            this.GoogleEarthAppRB = new System.Windows.Forms.RadioButton();
            this.LoggingThresholdEngineOffCB = new System.Windows.Forms.CheckBox();
            this.GoogleEarthGB.SuspendLayout();
            this.KMLFileNameGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // AboveThresholdWriteFreqLabel
            // 
            this.AboveThresholdWriteFreqLabel.AutoSize = true;
            this.AboveThresholdWriteFreqLabel.Location = new System.Drawing.Point(10, 82);
            this.AboveThresholdWriteFreqLabel.Name = "AboveThresholdWriteFreqLabel";
            this.AboveThresholdWriteFreqLabel.Size = new System.Drawing.Size(242, 13);
            this.AboveThresholdWriteFreqLabel.TabIndex = 5;
            this.AboveThresholdWriteFreqLabel.Text = "Above Threshold Frequency to Write Entry (secs):";
            // 
            // ThresholdLogWriteFreqTB
            // 
            this.ThresholdLogWriteFreqTB.Location = new System.Drawing.Point(256, 79);
            this.ThresholdLogWriteFreqTB.Name = "ThresholdLogWriteFreqTB";
            this.ThresholdLogWriteFreqTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdLogWriteFreqTB.TabIndex = 6;
            this.ThresholdLogWriteFreqTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LogWriteFreqTB_KeyPress);
            // 
            // KMLFilePathLabel
            // 
            this.KMLFilePathLabel.AutoSize = true;
            this.KMLFilePathLabel.Location = new System.Drawing.Point(12, 353);
            this.KMLFilePathLabel.Name = "KMLFilePathLabel";
            this.KMLFilePathLabel.Size = new System.Drawing.Size(76, 13);
            this.KMLFilePathLabel.TabIndex = 20;
            this.KMLFilePathLabel.Text = "KML File Path:";
            // 
            // KMLFolderBrowser
            // 
            this.KMLFolderBrowser.Location = new System.Drawing.Point(422, 348);
            this.KMLFolderBrowser.Name = "KMLFolderBrowser";
            this.KMLFolderBrowser.Size = new System.Drawing.Size(117, 23);
            this.KMLFolderBrowser.TabIndex = 22;
            this.KMLFolderBrowser.Text = "KML Path Browse...";
            this.KMLFolderBrowser.UseVisualStyleBackColor = true;
            this.KMLFolderBrowser.Click += new System.EventHandler(this.LogFolderBrowser_Click);
            // 
            // KMLFilePathTBRO
            // 
            this.KMLFilePathTBRO.Location = new System.Drawing.Point(125, 350);
            this.KMLFilePathTBRO.Name = "KMLFilePathTBRO";
            this.KMLFilePathTBRO.ReadOnly = true;
            this.KMLFilePathTBRO.Size = new System.Drawing.Size(272, 20);
            this.KMLFilePathTBRO.TabIndex = 21;
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
            this.StartLoggingBtn.Location = new System.Drawing.Point(13, 185);
            this.StartLoggingBtn.Name = "StartLoggingBtn";
            this.StartLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.StartLoggingBtn.TabIndex = 12;
            this.StartLoggingBtn.Text = "Start Logging";
            this.StartLoggingBtn.UseVisualStyleBackColor = true;
            this.StartLoggingBtn.Click += new System.EventHandler(this.StartLoggingBtn_Click);
            // 
            // StopLoggingBtn
            // 
            this.StopLoggingBtn.Enabled = false;
            this.StopLoggingBtn.Location = new System.Drawing.Point(442, 185);
            this.StopLoggingBtn.Name = "StopLoggingBtn";
            this.StopLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.StopLoggingBtn.TabIndex = 15;
            this.StopLoggingBtn.Text = "Stop Logging";
            this.StopLoggingBtn.UseVisualStyleBackColor = true;
            this.StopLoggingBtn.Click += new System.EventHandler(this.StopLoggingBtn_Click);
            // 
            // CreateKMLButton
            // 
            this.CreateKMLButton.Location = new System.Drawing.Point(125, 577);
            this.CreateKMLButton.Name = "CreateKMLButton";
            this.CreateKMLButton.Size = new System.Drawing.Size(97, 23);
            this.CreateKMLButton.TabIndex = 26;
            this.CreateKMLButton.Text = "Create KML File";
            this.CreateKMLButton.UseVisualStyleBackColor = true;
            this.CreateKMLButton.Click += new System.EventHandler(this.CreateKMLButton_Click);
            // 
            // PauseLoggingBtn
            // 
            this.PauseLoggingBtn.Enabled = false;
            this.PauseLoggingBtn.Location = new System.Drawing.Point(156, 185);
            this.PauseLoggingBtn.Name = "PauseLoggingBtn";
            this.PauseLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.PauseLoggingBtn.TabIndex = 13;
            this.PauseLoggingBtn.Text = "Pause Logging";
            this.PauseLoggingBtn.UseVisualStyleBackColor = true;
            this.PauseLoggingBtn.Click += new System.EventHandler(this.PauseLoggingBtn_Click);
            // 
            // ContinueLogginBtn
            // 
            this.ContinueLogginBtn.Enabled = false;
            this.ContinueLogginBtn.Location = new System.Drawing.Point(299, 185);
            this.ContinueLogginBtn.Name = "ContinueLogginBtn";
            this.ContinueLogginBtn.Size = new System.Drawing.Size(98, 23);
            this.ContinueLogginBtn.TabIndex = 14;
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
            // LogInfoLabel
            // 
            this.LogInfoLabel.AutoSize = true;
            this.LogInfoLabel.Location = new System.Drawing.Point(10, 63);
            this.LogInfoLabel.Name = "LogInfoLabel";
            this.LogInfoLabel.Size = new System.Drawing.Size(455, 13);
            this.LogInfoLabel.TabIndex = 4;
            this.LogInfoLabel.Text = "Logs will be written once a second below alitude threshold.  Above threshold set " +
    "options below.";
            // 
            // ThresholdMinAltLabel
            // 
            this.ThresholdMinAltLabel.AutoSize = true;
            this.ThresholdMinAltLabel.Location = new System.Drawing.Point(9, 104);
            this.ThresholdMinAltLabel.Name = "ThresholdMinAltLabel";
            this.ThresholdMinAltLabel.Size = new System.Drawing.Size(238, 13);
            this.ThresholdMinAltLabel.TabIndex = 7;
            this.ThresholdMinAltLabel.Text = "Threshold Minimum Altitude Above Ground (feet):";
            // 
            // ThresholdMinAltTB
            // 
            this.ThresholdMinAltTB.Location = new System.Drawing.Point(256, 102);
            this.ThresholdMinAltTB.Name = "ThresholdMinAltTB";
            this.ThresholdMinAltTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdMinAltTB.TabIndex = 8;
            // 
            // SeparateLineLabel
            // 
            this.SeparateLineLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SeparateLineLabel.Location = new System.Drawing.Point(12, 224);
            this.SeparateLineLabel.Name = "SeparateLineLabel";
            this.SeparateLineLabel.Size = new System.Drawing.Size(530, 2);
            this.SeparateLineLabel.TabIndex = 16;
            this.SeparateLineLabel.Text = "label5";
            // 
            // KMLExportLabel
            // 
            this.KMLExportLabel.AutoSize = true;
            this.KMLExportLabel.Location = new System.Drawing.Point(9, 239);
            this.KMLExportLabel.Name = "KMLExportLabel";
            this.KMLExportLabel.Size = new System.Drawing.Size(62, 13);
            this.KMLExportLabel.TabIndex = 17;
            this.KMLExportLabel.Text = "KML Export";
            // 
            // ChooseFlightLabel
            // 
            this.ChooseFlightLabel.AutoSize = true;
            this.ChooseFlightLabel.Location = new System.Drawing.Point(9, 274);
            this.ChooseFlightLabel.Name = "ChooseFlightLabel";
            this.ChooseFlightLabel.Size = new System.Drawing.Size(71, 13);
            this.ChooseFlightLabel.TabIndex = 18;
            this.ChooseFlightLabel.Text = "Choose Flight";
            // 
            // FlightPickerLV
            // 
            this.FlightPickerLV.FullRowSelect = true;
            this.FlightPickerLV.GridLines = true;
            this.FlightPickerLV.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FlightPickerLV.HideSelection = false;
            this.FlightPickerLV.Location = new System.Drawing.Point(125, 239);
            this.FlightPickerLV.Name = "FlightPickerLV";
            this.FlightPickerLV.Size = new System.Drawing.Size(370, 97);
            this.FlightPickerLV.TabIndex = 19;
            this.FlightPickerLV.UseCompatibleStateImageBehavior = false;
            this.FlightPickerLV.View = System.Windows.Forms.View.Details;
            // 
            // DeleteFlight
            // 
            this.DeleteFlight.Location = new System.Drawing.Point(284, 577);
            this.DeleteFlight.Name = "DeleteFlight";
            this.DeleteFlight.Size = new System.Drawing.Size(152, 23);
            this.DeleteFlight.TabIndex = 27;
            this.DeleteFlight.Text = "Delete Flights from Database";
            this.DeleteFlight.UseVisualStyleBackColor = true;
            this.DeleteFlight.Click += new System.EventHandler(this.DeleteFlight_Click);
            // 
            // GoogleEarthGB
            // 
            this.GoogleEarthGB.Controls.Add(this.GoogleEarthWebRB);
            this.GoogleEarthGB.Controls.Add(this.GoogleEarthAppRB);
            this.GoogleEarthGB.Location = new System.Drawing.Point(122, 487);
            this.GoogleEarthGB.Name = "GoogleEarthGB";
            this.GoogleEarthGB.Size = new System.Drawing.Size(282, 46);
            this.GoogleEarthGB.TabIndex = 24;
            this.GoogleEarthGB.TabStop = false;
            this.GoogleEarthGB.Text = "Export KML For Use In:";
            // 
            // SpeedUpVideoPlaybackCB
            // 
            this.SpeedUpVideoPlaybackCB.AutoSize = true;
            this.SpeedUpVideoPlaybackCB.Location = new System.Drawing.Point(125, 539);
            this.SpeedUpVideoPlaybackCB.Name = "SpeedUpVideoPlaybackCB";
            this.SpeedUpVideoPlaybackCB.Size = new System.Drawing.Size(361, 17);
            this.SpeedUpVideoPlaybackCB.TabIndex = 25;
            this.SpeedUpVideoPlaybackCB.Text = "Speed Up First Person Flight Playback When Above Threshold Altitude";
            this.SpeedUpVideoPlaybackCB.UseVisualStyleBackColor = true;
            // 
            // ErrorTBRO
            // 
            this.ErrorTBRO.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ErrorTBRO.Location = new System.Drawing.Point(13, 30);
            this.ErrorTBRO.Multiline = true;
            this.ErrorTBRO.Name = "ErrorTBRO";
            this.ErrorTBRO.ReadOnly = true;
            this.ErrorTBRO.Size = new System.Drawing.Size(415, 29);
            this.ErrorTBRO.TabIndex = 3;
            this.ErrorTBRO.TabStop = false;
            // 
            // AutomaticLoggingCB
            // 
            this.AutomaticLoggingCB.AutoSize = true;
            this.AutomaticLoggingCB.Location = new System.Drawing.Point(13, 131);
            this.AutomaticLoggingCB.Name = "AutomaticLoggingCB";
            this.AutomaticLoggingCB.Size = new System.Drawing.Size(327, 17);
            this.AutomaticLoggingCB.TabIndex = 9;
            this.AutomaticLoggingCB.Text = "Automatic Logging - Logging Threshold Ground Velocity (knots):";
            this.AutomaticLoggingCB.UseVisualStyleBackColor = true;
            this.AutomaticLoggingCB.Click += new System.EventHandler(this.AutomaticLoggingCB_Click);
            // 
            // LoggingThresholdGroundVelTB
            // 
            this.LoggingThresholdGroundVelTB.Location = new System.Drawing.Point(346, 128);
            this.LoggingThresholdGroundVelTB.Name = "LoggingThresholdGroundVelTB";
            this.LoggingThresholdGroundVelTB.Size = new System.Drawing.Size(48, 20);
            this.LoggingThresholdGroundVelTB.TabIndex = 10;
            // 
            // KMLFileNameGB
            // 
            this.KMLFileNameGB.Controls.Add(this.KMLFileNameTBRO);
            this.KMLFileNameGB.Controls.Add(this.FileNameExampleLabel);
            this.KMLFileNameGB.Controls.Add(this.FileNameTemplateLabel);
            this.KMLFileNameGB.Controls.Add(this.KMLFileNameTemplateTB);
            this.KMLFileNameGB.Location = new System.Drawing.Point(5, 386);
            this.KMLFileNameGB.Name = "KMLFileNameGB";
            this.KMLFileNameGB.Size = new System.Drawing.Size(534, 95);
            this.KMLFileNameGB.TabIndex = 23;
            this.KMLFileNameGB.TabStop = false;
            this.KMLFileNameGB.Text = "KML File Name Template:";
            // 
            // FileNameTemplateLabel
            // 
            this.FileNameTemplateLabel.AutoSize = true;
            this.FileNameTemplateLabel.Location = new System.Drawing.Point(7, 63);
            this.FileNameTemplateLabel.Name = "FileNameTemplateLabel";
            this.FileNameTemplateLabel.Size = new System.Drawing.Size(54, 13);
            this.FileNameTemplateLabel.TabIndex = 2;
            this.FileNameTemplateLabel.Text = "Template:";
            // 
            // KMLFileNameTemplateTB
            // 
            this.KMLFileNameTemplateTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.KMLFileNameTemplateTB.Location = new System.Drawing.Point(120, 57);
            this.KMLFileNameTemplateTB.Name = "KMLFileNameTemplateTB";
            this.KMLFileNameTemplateTB.Size = new System.Drawing.Size(408, 23);
            this.KMLFileNameTemplateTB.TabIndex = 3;
            this.KMLFileNameTemplateTB.TextChanged += new System.EventHandler(this.KMLFileNameTemplateTB_TextChanged);
            // 
            // KMLFileNameTBRO
            // 
            this.KMLFileNameTBRO.Location = new System.Drawing.Point(120, 24);
            this.KMLFileNameTBRO.Name = "KMLFileNameTBRO";
            this.KMLFileNameTBRO.ReadOnly = true;
            this.KMLFileNameTBRO.Size = new System.Drawing.Size(408, 20);
            this.KMLFileNameTBRO.TabIndex = 1;
            // 
            // FileNameExampleLabel
            // 
            this.FileNameExampleLabel.AutoSize = true;
            this.FileNameExampleLabel.Location = new System.Drawing.Point(7, 26);
            this.FileNameExampleLabel.Name = "FileNameExampleLabel";
            this.FileNameExampleLabel.Size = new System.Drawing.Size(50, 13);
            this.FileNameExampleLabel.TabIndex = 0;
            this.FileNameExampleLabel.Text = "Example:";
            // 
            // GoogleEarthWebRB
            // 
            this.GoogleEarthWebRB.AutoSize = true;
            this.GoogleEarthWebRB.Location = new System.Drawing.Point(161, 18);
            this.GoogleEarthWebRB.Name = "GoogleEarthWebRB";
            this.GoogleEarthWebRB.Size = new System.Drawing.Size(113, 17);
            this.GoogleEarthWebRB.TabIndex = 1;
            this.GoogleEarthWebRB.TabStop = true;
            this.GoogleEarthWebRB.Text = "Google Earth Web";
            this.GoogleEarthWebRB.UseVisualStyleBackColor = true;
            // 
            // GoogleEarthAppRB
            // 
            this.GoogleEarthAppRB.AutoSize = true;
            this.GoogleEarthAppRB.Location = new System.Drawing.Point(9, 18);
            this.GoogleEarthAppRB.Name = "GoogleEarthAppRB";
            this.GoogleEarthAppRB.Size = new System.Drawing.Size(142, 17);
            this.GoogleEarthAppRB.TabIndex = 0;
            this.GoogleEarthAppRB.TabStop = true;
            this.GoogleEarthAppRB.Text = "Google Earth Application";
            this.GoogleEarthAppRB.UseVisualStyleBackColor = true;
            // 
            // LoggingThresholdEngineOffCB
            // 
            this.LoggingThresholdEngineOffCB.AutoSize = true;
            this.LoggingThresholdEngineOffCB.Location = new System.Drawing.Point(45, 154);
            this.LoggingThresholdEngineOffCB.Name = "LoggingThresholdEngineOffCB";
            this.LoggingThresholdEngineOffCB.Size = new System.Drawing.Size(288, 17);
            this.LoggingThresholdEngineOffCB.TabIndex = 11;
            this.LoggingThresholdEngineOffCB.Text = "Stop Automatic Logging Only When Engnes Turned Off";
            this.LoggingThresholdEngineOffCB.UseVisualStyleBackColor = true;
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 614);
            this.Controls.Add(this.LoggingThresholdEngineOffCB);
            this.Controls.Add(this.LoggingThresholdGroundVelTB);
            this.Controls.Add(this.AutomaticLoggingCB);
            this.Controls.Add(this.ErrorTBRO);
            this.Controls.Add(this.SpeedUpVideoPlaybackCB);
            this.Controls.Add(this.DeleteFlight);
            this.Controls.Add(this.FlightPickerLV);
            this.Controls.Add(this.ChooseFlightLabel);
            this.Controls.Add(this.KMLExportLabel);
            this.Controls.Add(this.SeparateLineLabel);
            this.Controls.Add(this.ThresholdMinAltTB);
            this.Controls.Add(this.ThresholdMinAltLabel);
            this.Controls.Add(this.LogInfoLabel);
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
            this.Controls.Add(this.KMLFilePathLabel);
            this.Controls.Add(this.ThresholdLogWriteFreqTB);
            this.Controls.Add(this.AboveThresholdWriteFreqLabel);
            this.Controls.Add(this.GoogleEarthGB);
            this.Controls.Add(this.KMLFileNameGB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "Pilot Path Recorder v";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainPage_FormClosing);
            this.Shown += new System.EventHandler(this.MainPage_Shown);
            this.GoogleEarthGB.ResumeLayout(false);
            this.GoogleEarthGB.PerformLayout();
            this.KMLFileNameGB.ResumeLayout(false);
            this.KMLFileNameGB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label AboveThresholdWriteFreqLabel;
        private System.Windows.Forms.TextBox ThresholdLogWriteFreqTB;
        private System.Windows.Forms.Label KMLFilePathLabel;
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
        private System.Windows.Forms.Label LogInfoLabel;
        private System.Windows.Forms.Label ThresholdMinAltLabel;
        private System.Windows.Forms.TextBox ThresholdMinAltTB;
        private System.Windows.Forms.Label SeparateLineLabel;
        private System.Windows.Forms.Label KMLExportLabel;
        private System.Windows.Forms.Label ChooseFlightLabel;
        private System.Windows.Forms.ListView FlightPickerLV;
        private System.Windows.Forms.Button DeleteFlight;
        private System.Windows.Forms.GroupBox GoogleEarthGB;
        private System.Windows.Forms.CheckBox SpeedUpVideoPlaybackCB;
        private System.Windows.Forms.TextBox ErrorTBRO;
        private System.Windows.Forms.CheckBox AutomaticLoggingCB;
        private System.Windows.Forms.TextBox LoggingThresholdGroundVelTB;
        private System.Windows.Forms.GroupBox KMLFileNameGB;
        private System.Windows.Forms.Label FileNameTemplateLabel;
        private System.Windows.Forms.TextBox KMLFileNameTemplateTB;
        private System.Windows.Forms.RadioButton GoogleEarthWebRB;
        private System.Windows.Forms.RadioButton GoogleEarthAppRB;
        private System.Windows.Forms.TextBox KMLFileNameTBRO;
        private System.Windows.Forms.Label FileNameExampleLabel;
        private System.Windows.Forms.CheckBox LoggingThresholdEngineOffCB;
    }
}

