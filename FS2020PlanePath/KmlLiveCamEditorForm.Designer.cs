
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
            this.liveCamDefinitionToolStripMenuItems = new System.Windows.Forms.ToolStripMenuItem();
            this.liveCamDefinitionRenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.liveCamDefinitionLoadFromFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveCamDefinitionSaveToFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lensTemplateToolStripMenuItems = new System.Windows.Forms.ToolStripMenuItem();
            this.lensTemplateLoadFromMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lensTemplateSaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.liveCamDefinitionToolStripMenuItems,
            this.toolStripSeparator1,
            this.lensTemplateToolStripMenuItems});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.ToolTipText = "Allows Loading or Saving of LiveCam Lens Template";
            // 
            // liveCamDefinitionToolStripMenuItems
            // 
            this.liveCamDefinitionToolStripMenuItems.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.liveCamDefinitionRenameMenuItem,
            this.toolStripSeparator2,
            this.liveCamDefinitionLoadFromFileMenuItem,
            this.liveCamDefinitionSaveToFileMenuItem});
            this.liveCamDefinitionToolStripMenuItems.Name = "liveCamDefinitionToolStripMenuItems";
            this.liveCamDefinitionToolStripMenuItems.Size = new System.Drawing.Size(180, 22);
            this.liveCamDefinitionToolStripMenuItems.Text = "LiveCam Definition";
            this.liveCamDefinitionToolStripMenuItems.ToolTipText = "Manage the LiveCam definition.";
            // 
            // liveCamDefinitionRenameMenuItem
            // 
            this.liveCamDefinitionRenameMenuItem.Name = "liveCamDefinitionRenameMenuItem";
            this.liveCamDefinitionRenameMenuItem.Size = new System.Drawing.Size(180, 22);
            this.liveCamDefinitionRenameMenuItem.Text = "&Rename...";
            this.liveCamDefinitionRenameMenuItem.ToolTipText = "Change the alias for the LiveCam definition";
            this.liveCamDefinitionRenameMenuItem.Click += new System.EventHandler(this.Handle_LiveCamDefinitionRename_Event);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // liveCamDefinitionLoadFromFileMenuItem
            // 
            this.liveCamDefinitionLoadFromFileMenuItem.Name = "liveCamDefinitionLoadFromFileMenuItem";
            this.liveCamDefinitionLoadFromFileMenuItem.Size = new System.Drawing.Size(180, 22);
            this.liveCamDefinitionLoadFromFileMenuItem.Text = "&Load From File...";
            this.liveCamDefinitionLoadFromFileMenuItem.ToolTipText = "Load the LiveCam definition from a file.";
            this.liveCamDefinitionLoadFromFileMenuItem.Click += new System.EventHandler(this.Handle_LiveCamDefinitionLoadFromFile_Event);
            // 
            // liveCamDefinitionSaveToFileMenuItem
            // 
            this.liveCamDefinitionSaveToFileMenuItem.Name = "liveCamDefinitionSaveToFileMenuItem";
            this.liveCamDefinitionSaveToFileMenuItem.Size = new System.Drawing.Size(180, 22);
            this.liveCamDefinitionSaveToFileMenuItem.Text = "&Save To File...";
            this.liveCamDefinitionSaveToFileMenuItem.ToolTipText = "Save the LiveCam definition to a file.";
            this.liveCamDefinitionSaveToFileMenuItem.Click += new System.EventHandler(this.Handle_LiveCamDefinitionSaveToFile_Event);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // lensTemplateToolStripMenuItems
            // 
            this.lensTemplateToolStripMenuItems.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lensTemplateLoadFromMenuItem,
            this.lensTemplateSaveAsMenuItem});
            this.lensTemplateToolStripMenuItems.Name = "lensTemplateToolStripMenuItems";
            this.lensTemplateToolStripMenuItems.Size = new System.Drawing.Size(180, 22);
            this.lensTemplateToolStripMenuItems.Text = "Lens Template";
            this.lensTemplateToolStripMenuItems.ToolTipText = "Manage the currently selected lens template.";
            // 
            // lensTemplateLoadFromMenuItem
            // 
            this.lensTemplateLoadFromMenuItem.Name = "lensTemplateLoadFromMenuItem";
            this.lensTemplateLoadFromMenuItem.Size = new System.Drawing.Size(180, 22);
            this.lensTemplateLoadFromMenuItem.Text = "&Load From File...";
            this.lensTemplateLoadFromMenuItem.ToolTipText = "Load the template for the currently selected lens from a file.";
            this.lensTemplateLoadFromMenuItem.Click += new System.EventHandler(this.Handle_LensTemplateLoadFrom_Event);
            // 
            // lensTemplateSaveAsMenuItem
            // 
            this.lensTemplateSaveAsMenuItem.Name = "lensTemplateSaveAsMenuItem";
            this.lensTemplateSaveAsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.lensTemplateSaveAsMenuItem.Text = "&Save To File...";
            this.lensTemplateSaveAsMenuItem.ToolTipText = "Save the template for the currently selected lens to a file.";
            this.lensTemplateSaveAsMenuItem.Click += new System.EventHandler(this.Handle_LensTemplateSaveAs_Event);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KmlLiveCamEditorForm_FormClosing);
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
        private System.Windows.Forms.Label menuStripDividerLB;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TabControl editorPaneTC;
        private System.Windows.Forms.Button newBT;
        private System.Windows.Forms.Button deleteBT;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem liveCamDefinitionToolStripMenuItems;
        private System.Windows.Forms.ToolStripMenuItem lensTemplateToolStripMenuItems;
        private System.Windows.Forms.ToolStripMenuItem liveCamDefinitionSaveToFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lensTemplateLoadFromMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lensTemplateSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveCamDefinitionLoadFromFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem liveCamDefinitionRenameMenuItem;
    }
}