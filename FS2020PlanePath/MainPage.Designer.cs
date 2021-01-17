﻿namespace FS2020PlanePath
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
            this.components = new System.ComponentModel.Container();
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
            this.GoogleEarthAppRB = new System.Windows.Forms.RadioButton();
            this.GoogleEarthWebRB = new System.Windows.Forms.RadioButton();
            this.GoogleEarthGB = new System.Windows.Forms.GroupBox();
            this.SpeedUpVideoPlaybackCB = new System.Windows.Forms.CheckBox();
            this.ErrorTBRO = new System.Windows.Forms.TextBox();
            this.AutomaticLoggingCB = new System.Windows.Forms.CheckBox();
            this.LoggingThresholdGroundVelTB = new System.Windows.Forms.TextBox();
            this.LiveCameraCB = new System.Windows.Forms.CheckBox();
            this.LiveCameraHostPortTB = new System.Windows.Forms.TextBox();
            this.LiveCameraKmlBT = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.geLinkBT = new System.Windows.Forms.Button();
            this.LiveCameraKmlResetBT = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(242, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Above Threshold Frequency to Write Entry (secs):";
            // 
            // ThresholdLogWriteFreqTB
            // 
            this.ThresholdLogWriteFreqTB.Location = new System.Drawing.Point(256, 79);
            this.ThresholdLogWriteFreqTB.Name = "ThresholdLogWriteFreqTB";
            this.ThresholdLogWriteFreqTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdLogWriteFreqTB.TabIndex = 6;
            this.ThresholdLogWriteFreqTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LogWriteFreqTB_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "KML File Path:";
            // 
            // KMLFolderBrowser
            // 
            this.KMLFolderBrowser.Location = new System.Drawing.Point(422, 361);
            this.KMLFolderBrowser.Name = "KMLFolderBrowser";
            this.KMLFolderBrowser.Size = new System.Drawing.Size(117, 23);
            this.KMLFolderBrowser.TabIndex = 22;
            this.KMLFolderBrowser.Text = "KML Path Browse...";
            this.KMLFolderBrowser.UseVisualStyleBackColor = true;
            this.KMLFolderBrowser.Click += new System.EventHandler(this.LogFolderBrowser_Click);
            // 
            // KMLFilePathTBRO
            // 
            this.KMLFilePathTBRO.Location = new System.Drawing.Point(125, 363);
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
            this.StartLoggingBtn.Location = new System.Drawing.Point(13, 157);
            this.StartLoggingBtn.Name = "StartLoggingBtn";
            this.StartLoggingBtn.Size = new System.Drawing.Size(98, 23);
            this.StartLoggingBtn.TabIndex = 11;
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
            this.StopLoggingBtn.TabIndex = 14;
            this.StopLoggingBtn.Text = "Stop Logging";
            this.StopLoggingBtn.UseVisualStyleBackColor = true;
            this.StopLoggingBtn.Click += new System.EventHandler(this.StopLoggingBtn_Click);
            // 
            // CreateKMLButton
            // 
            this.CreateKMLButton.Location = new System.Drawing.Point(125, 479);
            this.CreateKMLButton.Name = "CreateKMLButton";
            this.CreateKMLButton.Size = new System.Drawing.Size(97, 23);
            this.CreateKMLButton.TabIndex = 27;
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
            this.PauseLoggingBtn.TabIndex = 12;
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
            this.ContinueLogginBtn.TabIndex = 13;
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
            this.label1.TabIndex = 4;
            this.label1.Text = "Logs will be written once a second below alitude threshold.  Above threshold set " +
    "options below.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(238, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Threshold Minimum Altitude Above Ground (feet):";
            // 
            // ThresholdMinAltTB
            // 
            this.ThresholdMinAltTB.Location = new System.Drawing.Point(256, 102);
            this.ThresholdMinAltTB.Name = "ThresholdMinAltTB";
            this.ThresholdMinAltTB.Size = new System.Drawing.Size(48, 20);
            this.ThresholdMinAltTB.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(12, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(530, 2);
            this.label5.TabIndex = 15;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "KML Export";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 284);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Choose Flight";
            // 
            // FlightPickerLV
            // 
            this.FlightPickerLV.FullRowSelect = true;
            this.FlightPickerLV.GridLines = true;
            this.FlightPickerLV.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FlightPickerLV.HideSelection = false;
            this.FlightPickerLV.Location = new System.Drawing.Point(125, 249);
            this.FlightPickerLV.Name = "FlightPickerLV";
            this.FlightPickerLV.Size = new System.Drawing.Size(370, 97);
            this.FlightPickerLV.TabIndex = 18;
            this.FlightPickerLV.UseCompatibleStateImageBehavior = false;
            this.FlightPickerLV.View = System.Windows.Forms.View.Details;
            // 
            // DeleteFlight
            // 
            this.DeleteFlight.Location = new System.Drawing.Point(284, 479);
            this.DeleteFlight.Name = "DeleteFlight";
            this.DeleteFlight.Size = new System.Drawing.Size(152, 23);
            this.DeleteFlight.TabIndex = 28;
            this.DeleteFlight.Text = "Delete Flight from Database";
            this.DeleteFlight.UseVisualStyleBackColor = true;
            this.DeleteFlight.Click += new System.EventHandler(this.DeleteFlight_Click);
            // 
            // GoogleEarthAppRB
            // 
            this.GoogleEarthAppRB.AutoSize = true;
            this.GoogleEarthAppRB.Location = new System.Drawing.Point(132, 407);
            this.GoogleEarthAppRB.Name = "GoogleEarthAppRB";
            this.GoogleEarthAppRB.Size = new System.Drawing.Size(142, 17);
            this.GoogleEarthAppRB.TabIndex = 24;
            this.GoogleEarthAppRB.TabStop = true;
            this.GoogleEarthAppRB.Text = "Google Earth Application";
            this.GoogleEarthAppRB.UseVisualStyleBackColor = true;
            // 
            // GoogleEarthWebRB
            // 
            this.GoogleEarthWebRB.AutoSize = true;
            this.GoogleEarthWebRB.Location = new System.Drawing.Point(284, 407);
            this.GoogleEarthWebRB.Name = "GoogleEarthWebRB";
            this.GoogleEarthWebRB.Size = new System.Drawing.Size(113, 17);
            this.GoogleEarthWebRB.TabIndex = 25;
            this.GoogleEarthWebRB.TabStop = true;
            this.GoogleEarthWebRB.Text = "Google Earth Web";
            this.GoogleEarthWebRB.UseVisualStyleBackColor = true;
            // 
            // GoogleEarthGB
            // 
            this.GoogleEarthGB.Location = new System.Drawing.Point(122, 389);
            this.GoogleEarthGB.Name = "GoogleEarthGB";
            this.GoogleEarthGB.Size = new System.Drawing.Size(282, 46);
            this.GoogleEarthGB.TabIndex = 23;
            this.GoogleEarthGB.TabStop = false;
            this.GoogleEarthGB.Text = "Export KML For Use In:";
            // 
            // SpeedUpVideoPlaybackCB
            // 
            this.SpeedUpVideoPlaybackCB.AutoSize = true;
            this.SpeedUpVideoPlaybackCB.Location = new System.Drawing.Point(125, 441);
            this.SpeedUpVideoPlaybackCB.Name = "SpeedUpVideoPlaybackCB";
            this.SpeedUpVideoPlaybackCB.Size = new System.Drawing.Size(361, 17);
            this.SpeedUpVideoPlaybackCB.TabIndex = 26;
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
            this.ErrorTBRO.TextChanged += new System.EventHandler(this.ErrorTBRO_TextChanged);
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
            // LiveCameraCB
            // 
            this.LiveCameraCB.AccessibleDescription = "";
            this.LiveCameraCB.AutoSize = true;
            this.LiveCameraCB.Location = new System.Drawing.Point(13, 213);
            this.LiveCameraCB.Name = "LiveCameraCB";
            this.LiveCameraCB.Size = new System.Drawing.Size(85, 17);
            this.LiveCameraCB.TabIndex = 15;
            this.LiveCameraCB.Text = "Live Camera";
            this.toolTip1.SetToolTip(this.LiveCameraCB, "Check to enable \'Live Camera\' listener (see documentation)");
            this.LiveCameraCB.UseVisualStyleBackColor = true;
            this.LiveCameraCB.CheckedChanged += new System.EventHandler(this.LiveCameraCB_CheckedChanged);
            // 
            // LiveCameraHostPortTB
            // 
            this.LiveCameraHostPortTB.Location = new System.Drawing.Point(125, 213);
            this.LiveCameraHostPortTB.Name = "LiveCameraHostPortTB";
            this.LiveCameraHostPortTB.Size = new System.Drawing.Size(283, 20);
            this.LiveCameraHostPortTB.TabIndex = 16;
            this.LiveCameraHostPortTB.Text = "http://localhost:8000/kmlcam";
            this.toolTip1.SetToolTip(this.LiveCameraHostPortTB, "Set \'Live Camera\' listener address");
            this.LiveCameraHostPortTB.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateNetworkLink);
            // 
            // LiveCameraKmlBT
            // 
            this.LiveCameraKmlBT.Location = new System.Drawing.Point(413, 213);
            this.LiveCameraKmlBT.Name = "LiveCameraKmlBT";
            this.LiveCameraKmlBT.Size = new System.Drawing.Size(38, 20);
            this.LiveCameraKmlBT.TabIndex = 29;
            this.LiveCameraKmlBT.Text = "Edit";
            this.toolTip1.SetToolTip(this.LiveCameraKmlBT, "Edit Selected  \'Live Camera\' Definition");
            this.LiveCameraKmlBT.UseVisualStyleBackColor = true;
            this.LiveCameraKmlBT.Click += new System.EventHandler(this.LiveCameraKml_Click);
            // 
            // geLinkBT
            // 
            this.geLinkBT.Location = new System.Drawing.Point(455, 213);
            this.geLinkBT.Name = "geLinkBT";
            this.geLinkBT.Size = new System.Drawing.Size(38, 20);
            this.geLinkBT.TabIndex = 30;
            this.geLinkBT.Text = "Link";
            this.toolTip1.SetToolTip(this.geLinkBT, "Install Selected Network Link (e.g.,  in Google Earth)");
            this.geLinkBT.UseVisualStyleBackColor = true;
            this.geLinkBT.Click += new System.EventHandler(this.geLinkBT_Click);
            // 
            // LiveCameraKmlResetBT
            // 
            this.LiveCameraKmlResetBT.Location = new System.Drawing.Point(497, 213);
            this.LiveCameraKmlResetBT.Name = "LiveCameraKmlResetBT";
            this.LiveCameraKmlResetBT.Size = new System.Drawing.Size(43, 20);
            this.LiveCameraKmlResetBT.TabIndex = 31;
            this.LiveCameraKmlResetBT.Text = "Reset";
            this.toolTip1.SetToolTip(this.LiveCameraKmlResetBT, "Reset Selected Live Camera Definition (i.e. Discard Edits)");
            this.LiveCameraKmlResetBT.UseVisualStyleBackColor = true;
            this.LiveCameraKmlResetBT.Click += new System.EventHandler(this.Handle_LiveCameraKmlResetBT_Click);
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 518);
            this.Controls.Add(this.LiveCameraKmlResetBT);
            this.Controls.Add(this.geLinkBT);
            this.Controls.Add(this.LiveCameraKmlBT);
            this.Controls.Add(this.LiveCameraHostPortTB);
            this.Controls.Add(this.LiveCameraCB);
            this.Controls.Add(this.LoggingThresholdGroundVelTB);
            this.Controls.Add(this.AutomaticLoggingCB);
            this.Controls.Add(this.ErrorTBRO);
            this.Controls.Add(this.SpeedUpVideoPlaybackCB);
            this.Controls.Add(this.GoogleEarthWebRB);
            this.Controls.Add(this.GoogleEarthAppRB);
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
            this.Controls.Add(this.GoogleEarthGB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "Pilot Path Recorder v";
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
        private System.Windows.Forms.RadioButton GoogleEarthAppRB;
        private System.Windows.Forms.RadioButton GoogleEarthWebRB;
        private System.Windows.Forms.GroupBox GoogleEarthGB;
        private System.Windows.Forms.CheckBox SpeedUpVideoPlaybackCB;
        private System.Windows.Forms.TextBox ErrorTBRO;
        private System.Windows.Forms.CheckBox AutomaticLoggingCB;
        private System.Windows.Forms.TextBox LoggingThresholdGroundVelTB;
        private System.Windows.Forms.CheckBox LiveCameraCB;
        private System.Windows.Forms.TextBox LiveCameraHostPortTB;
        private System.Windows.Forms.Button LiveCameraKmlBT;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button geLinkBT;
        private System.Windows.Forms.Button LiveCameraKmlResetBT;
    }
}

