using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    public partial class TextEditorForm : Form
    {

        private Func<string, string> validator;

        public TextEditorForm(
            string title, 
            string text,
            Func<string, string> validator
        ) {
            InitializeComponent();
            this.Text = title;
            editorTB.Text = text;
            this.validator = validator;
        }

        public string EditorText
        {
            get
            {
                return editorTB.Text;
            }
        }

        private void validateForm(object sender, CancelEventArgs e)
        {
            if (validator != null)
            {
                string failureMessage = validator.Invoke(EditorText);
                if (failureMessage != null)
                {
                    MessageBox.Show(
                        $"Details:{failureMessage}",
                        "Validation Failure",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    e.Cancel = true;
                }
            }
        }
    }

}
