
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
            this.editorTB.Location = new System.Drawing.Point(77, 12);
            this.editorTB.Multiline = true;
            this.editorTB.Name = "editorTB";
            this.editorTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.editorTB.Size = new System.Drawing.Size(431, 261);
            this.editorTB.TabIndex = 0;
            this.editorTB.Validating += new System.ComponentModel.CancelEventHandler(this.validateForm);
            // 
            // okBT
            // 
            this.okBT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBT.Location = new System.Drawing.Point(12, 12);
            this.okBT.Name = "okBT";
            this.okBT.Size = new System.Drawing.Size(54, 30);
            this.okBT.TabIndex = 1;
            this.okBT.Text = "&OK";
            this.okBT.UseVisualStyleBackColor = true;
            // 
            // cancelBT
            // 
            this.cancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBT.Location = new System.Drawing.Point(12, 48);
            this.cancelBT.Name = "cancelBT";
            this.cancelBT.Size = new System.Drawing.Size(54, 30);
            this.cancelBT.TabIndex = 2;
            this.cancelBT.Text = "&Cancel";
            this.cancelBT.UseVisualStyleBackColor = true;
            // 
            // TextEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 285);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.okBT);
            this.Controls.Add(this.editorTB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextEditorForm";
            this.Text = "Text Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox editorTB;
        private System.Windows.Forms.Button okBT;
        private System.Windows.Forms.Button cancelBT;
    }
}