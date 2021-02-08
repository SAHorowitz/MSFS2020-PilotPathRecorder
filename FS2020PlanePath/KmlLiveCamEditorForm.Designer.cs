
namespace FS2020PlanePath
{
    partial class KmlLiveCamEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KmlLiveCamEditorForm));
            this.okBT = new System.Windows.Forms.Button();
            this.cancelBT = new System.Windows.Forms.Button();
            this.menuSP = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripDividerLB = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.newBT = new System.Windows.Forms.Button();
            this.deleteBT = new System.Windows.Forms.Button();
            this.editorPaneTC = new System.Windows.Forms.TabControl();
            this.menuSP.SuspendLayout();
            this.SuspendLayout();
            // 
            // okBT
            // 
            this.okBT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBT.Location = new System.Drawing.Point(12, 38);
            this.okBT.Name = "okBT";
            this.okBT.Size = new System.Drawing.Size(54, 30);
            this.okBT.TabIndex = 1;
            this.okBT.Text = "&OK";
            this.toolTip1.SetToolTip(this.okBT, "Accept and immediately apply the updated LiveCam");
            this.okBT.UseVisualStyleBackColor = true;
            // 
            // cancelBT
            // 
            this.cancelBT.CausesValidation = false;
            this.cancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBT.Location = new System.Drawing.Point(12, 73);
            this.cancelBT.Name = "cancelBT";
            this.cancelBT.Size = new System.Drawing.Size(54, 30);
            this.cancelBT.TabIndex = 0;
            this.cancelBT.Text = "&Cancel";
            this.toolTip1.SetToolTip(this.cancelBT, "Discard any change(s) made to the LiveCam");
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
            this.fileToolStripMenuItem.ToolTipText = "Allows Loading or Saving of LiveCam Lens Template";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Load...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.Handle_FileOpenMenuItemSelected_Event);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save &As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.Handle_FileSaveMenuItemSelected_Event);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.ToolTipText = "Invokes \'Pop Up\' Help Panel";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.Handle_HelpMenuItemSelected_Event);
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
            // newBT
            // 
            this.newBT.CausesValidation = false;
            this.newBT.Location = new System.Drawing.Point(12, 109);
            this.newBT.Name = "newBT";
            this.newBT.Size = new System.Drawing.Size(54, 30);
            this.newBT.TabIndex = 7;
            this.newBT.Text = "&New";
            this.toolTip1.SetToolTip(this.newBT, "Create a new LiveCam Lens");
            this.newBT.UseVisualStyleBackColor = true;
            this.newBT.Click += new System.EventHandler(this.newBT_Click);
            // 
            // deleteBT
            // 
            this.deleteBT.CausesValidation = false;
            this.deleteBT.Location = new System.Drawing.Point(12, 145);
            this.deleteBT.Name = "deleteBT";
            this.deleteBT.Size = new System.Drawing.Size(54, 30);
            this.deleteBT.TabIndex = 8;
            this.deleteBT.Text = "&Delete";
            this.toolTip1.SetToolTip(this.deleteBT, "Delete selected LiveCam Lens");
            this.deleteBT.UseVisualStyleBackColor = true;
            this.deleteBT.Click += new System.EventHandler(this.deleteBT_Click);
            // 
            // editorPaneTC
            // 
            this.editorPaneTC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorPaneTC.Location = new System.Drawing.Point(77, 38);
            this.editorPaneTC.Name = "editorPaneTC";
            this.editorPaneTC.SelectedIndex = 0;
            this.editorPaneTC.ShowToolTips = true;
            this.editorPaneTC.Size = new System.Drawing.Size(431, 271);
            this.editorPaneTC.TabIndex = 6;
            // 
            // KmlLiveCamEditorForm
            // 
            this.AcceptButton = this.okBT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancelBT;
            this.ClientSize = new System.Drawing.Size(520, 321);
            this.Controls.Add(this.deleteBT);
            this.Controls.Add(this.newBT);
            this.Controls.Add(this.editorPaneTC);
            this.Controls.Add(this.menuStripDividerLB);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.okBT);
            this.Controls.Add(this.menuSP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuSP;
            this.Name = "KmlLiveCamEditorForm";
            this.Text = "LiveCam Editor";
            this.toolTip1.SetToolTip(this, "Allows customization of the LiveCam Template");
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Handle_HelpRequested_Event);
            this.menuSP.ResumeLayout(false);
            this.menuSP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.Button newBT;
        private System.Windows.Forms.Button deleteBT;
    }
}