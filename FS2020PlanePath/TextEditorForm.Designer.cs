
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorForm));
            this.editorTB = new System.Windows.Forms.TextBox();
            this.okBT = new System.Windows.Forms.Button();
            this.cancelBT = new System.Windows.Forms.Button();
            this.menuSP = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripDividerLB = new System.Windows.Forms.Label();
            this.menuSP.SuspendLayout();
            this.SuspendLayout();
            // 
            // editorTB
            // 
            this.editorTB.AcceptsReturn = true;
            this.editorTB.AcceptsTab = true;
            this.editorTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorTB.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editorTB.Location = new System.Drawing.Point(77, 38);
            this.editorTB.Multiline = true;
            this.editorTB.Name = "editorTB";
            this.editorTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.editorTB.Size = new System.Drawing.Size(431, 271);
            this.editorTB.TabIndex = 2;
            this.editorTB.TabStop = false;
            this.editorTB.Validating += new System.ComponentModel.CancelEventHandler(this.validateForm);
            // 
            // okBT
            // 
            this.okBT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBT.Location = new System.Drawing.Point(12, 38);
            this.okBT.Name = "okBT";
            this.okBT.Size = new System.Drawing.Size(54, 30);
            this.okBT.TabIndex = 1;
            this.okBT.Text = "&OK";
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
            this.cancelBT.UseVisualStyleBackColor = true;
            // 
            // menuSP
            // 
            this.menuSP.BackColor = System.Drawing.SystemColors.Control;
            this.menuSP.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuSP.Location = new System.Drawing.Point(0, 0);
            this.menuSP.Name = "menuSP";
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
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "&Load...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save &As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
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
            // TextEditorForm
            // 
            this.AcceptButton = this.okBT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancelBT;
            this.ClientSize = new System.Drawing.Size(520, 321);
            this.Controls.Add(this.menuStripDividerLB);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.okBT);
            this.Controls.Add(this.editorTB);
            this.Controls.Add(this.menuSP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuSP;
            this.Name = "TextEditorForm";
            this.Text = "Text Editor";
            this.menuSP.ResumeLayout(false);
            this.menuSP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox editorTB;
        private System.Windows.Forms.Button okBT;
        private System.Windows.Forms.Button cancelBT;
        private System.Windows.Forms.MenuStrip menuSP;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Label menuStripDividerLB;
    }
}