
namespace FS2020PlanePath
{
    partial class TextEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorForm));
            this.cameraEditorTB = new System.Windows.Forms.TextBox();
            this.okBT = new System.Windows.Forms.Button();
            this.cancelBT = new System.Windows.Forms.Button();
            this.menuSP = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripDividerLB = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.editorPaneTC = new System.Windows.Forms.TabControl();
            this.cameraEditorTP = new System.Windows.Forms.TabPage();
            this.linkEditorTP = new System.Windows.Forms.TabPage();
            this.linkEditorTB = new System.Windows.Forms.TextBox();
            this.menuSP.SuspendLayout();
            this.editorPaneTC.SuspendLayout();
            this.cameraEditorTP.SuspendLayout();
            this.linkEditorTP.SuspendLayout();
            this.SuspendLayout();
            // 
            // cameraEditorTB
            // 
            this.cameraEditorTB.AcceptsReturn = true;
            this.cameraEditorTB.AcceptsTab = true;
            this.cameraEditorTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraEditorTB.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cameraEditorTB.Location = new System.Drawing.Point(3, 3);
            this.cameraEditorTB.Multiline = true;
            this.cameraEditorTB.Name = "cameraEditorTB";
            this.cameraEditorTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cameraEditorTB.Size = new System.Drawing.Size(417, 239);
            this.cameraEditorTB.TabIndex = 2;
            this.cameraEditorTB.TabStop = false;
            this.cameraEditorTB.Validating += new System.ComponentModel.CancelEventHandler(this.validateKmlTexts);
            // 
            // okBT
            // 
            this.okBT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBT.Location = new System.Drawing.Point(12, 38);
            this.okBT.Name = "okBT";
            this.okBT.Size = new System.Drawing.Size(54, 30);
            this.okBT.TabIndex = 1;
            this.okBT.Text = "&OK";
            this.toolTip1.SetToolTip(this.okBT, "Accept and immediately apply the updated KML template");
            this.okBT.UseVisualStyleBackColor = true;
            // 
            // cancelBT
            // 
            this.cancelBT.CausesValidation = false;
            this.cancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBT.Location = new System.Drawing.Point(12, 74);
            this.cancelBT.Name = "cancelBT";
            this.cancelBT.Size = new System.Drawing.Size(54, 30);
            this.cancelBT.TabIndex = 0;
            this.cancelBT.Text = "&Cancel";
            this.toolTip1.SetToolTip(this.cancelBT, "Discard any change(s) made to the KML template");
            this.cancelBT.UseVisualStyleBackColor = true;
            // 
            // menuSP
            // 
            this.menuSP.BackColor = System.Drawing.SystemColors.Control;
            this.menuSP.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuSP.Location = new System.Drawing.Point(0, 0);
            this.menuSP.Name = "menuSP";
            this.menuSP.ShowItemToolTips = true;
            this.menuSP.Size = new System.Drawing.Size(520, 24);
            this.menuSP.TabIndex = 4;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.ToolTipText = "Allows Loading or Saving of KML Template";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Load...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save &As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.ToolTipText = "Invokes \'Pop Up\' Help Panel";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // menuStripDividerLB
            // 
            this.menuStripDividerLB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStripDividerLB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.menuStripDividerLB.Location = new System.Drawing.Point(0, 25);
            this.menuStripDividerLB.Margin = new System.Windows.Forms.Padding(0);
            this.menuStripDividerLB.Name = "menuStripDividerLB";
            this.menuStripDividerLB.Size = new System.Drawing.Size(520, 2);
            this.menuStripDividerLB.TabIndex = 5;
            // 
            // editorPaneTC
            // 
            this.editorPaneTC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorPaneTC.Controls.Add(this.cameraEditorTP);
            this.editorPaneTC.Controls.Add(this.linkEditorTP);
            this.editorPaneTC.Location = new System.Drawing.Point(77, 38);
            this.editorPaneTC.Name = "editorPaneTC";
            this.editorPaneTC.SelectedIndex = 0;
            this.editorPaneTC.ShowToolTips = true;
            this.editorPaneTC.Size = new System.Drawing.Size(431, 271);
            this.editorPaneTC.TabIndex = 6;
            // 
            // cameraEditorTP
            // 
            this.cameraEditorTP.Controls.Add(this.cameraEditorTB);
            this.cameraEditorTP.Location = new System.Drawing.Point(4, 22);
            this.cameraEditorTP.Name = "cameraEditorTP";
            this.cameraEditorTP.Padding = new System.Windows.Forms.Padding(3);
            this.cameraEditorTP.Size = new System.Drawing.Size(423, 245);
            this.cameraEditorTP.TabIndex = 0;
            this.cameraEditorTP.Text = "Camera Template";
            this.cameraEditorTP.ToolTipText = "You may edit the \"Camera\" KML Template text in this tab.";
            this.cameraEditorTP.UseVisualStyleBackColor = true;
            // 
            // linkEditorTP
            // 
            this.linkEditorTP.Controls.Add(this.linkEditorTB);
            this.linkEditorTP.Location = new System.Drawing.Point(4, 22);
            this.linkEditorTP.Name = "linkEditorTP";
            this.linkEditorTP.Padding = new System.Windows.Forms.Padding(3);
            this.linkEditorTP.Size = new System.Drawing.Size(423, 245);
            this.linkEditorTP.TabIndex = 1;
            this.linkEditorTP.Text = "Link Template";
            this.linkEditorTP.ToolTipText = "You may edit the \"Link\" KML Template text in this tab.";
            this.linkEditorTP.UseVisualStyleBackColor = true;
            // 
            // linkEditorTB
            // 
            this.linkEditorTB.AcceptsReturn = true;
            this.linkEditorTB.AcceptsTab = true;
            this.linkEditorTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkEditorTB.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkEditorTB.Location = new System.Drawing.Point(3, 3);
            this.linkEditorTB.Multiline = true;
            this.linkEditorTB.Name = "linkEditorTB";
            this.linkEditorTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.linkEditorTB.Size = new System.Drawing.Size(417, 239);
            this.linkEditorTB.TabIndex = 3;
            this.linkEditorTB.TabStop = false;
            this.linkEditorTB.Validating += new System.ComponentModel.CancelEventHandler(this.validateKmlTexts);
            // 
            // TextEditorForm
            // 
            this.AcceptButton = this.okBT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancelBT;
            this.ClientSize = new System.Drawing.Size(520, 321);
            this.Controls.Add(this.editorPaneTC);
            this.Controls.Add(this.menuStripDividerLB);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.okBT);
            this.Controls.Add(this.menuSP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuSP;
            this.Name = "TextEditorForm";
            this.Text = "Text Editor";
            this.toolTip1.SetToolTip(this, "Allows customization of the \'Live Camera\' KML Template");
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.TextEditorForm_HelpRequested);
            this.menuSP.ResumeLayout(false);
            this.menuSP.PerformLayout();
            this.editorPaneTC.ResumeLayout(false);
            this.cameraEditorTP.ResumeLayout(false);
            this.cameraEditorTP.PerformLayout();
            this.linkEditorTP.ResumeLayout(false);
            this.linkEditorTP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cameraEditorTB;
        private System.Windows.Forms.Button okBT;
        private System.Windows.Forms.Button cancelBT;
        private System.Windows.Forms.MenuStrip menuSP;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Label menuStripDividerLB;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TabControl editorPaneTC;
        private System.Windows.Forms.TabPage cameraEditorTP;
        private System.Windows.Forms.TextBox linkEditorTB;
        private System.Windows.Forms.TabPage linkEditorTP;
    }
}