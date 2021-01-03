using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    public partial class TextEditorForm : Form
    {
        private const string SavedFileFilter = "KML files (*.kml)|*.kml|All files (*.*)|*.*";
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = SavedFileFilter;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    editorTB.Text = File.ReadAllText(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    reflectException(ex);
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = SavedFileFilter;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    File.WriteAllText(saveFileDialog.FileName, editorTB.Text);
                } catch(Exception ex)
                {
                    reflectException(ex);
                }
            }

        }

        private void reflectException(Exception e)
        {
            MessageBox.Show($"Details:\n{e.Message}", "Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}
