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
            this.validator = validator;
            editorTB.Text = text;
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
                    displayError("Validation Failure", failureMessage);
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
                    displayError("Exception Encountered", ex.Message);
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
                    displayError("Exception Encountered", ex.Message);
                }
            }

        }

        private void displayError(string caption, string details)
        {
            MessageBox.Show($"Details:\n{details}", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void TextEditorForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            showHelp();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHelp();
        }

        private void showHelp() {
            Help.ShowPopup(this,
                $@"Permitted substitution values:

{"\t" + string.Join("\n\t", TemplateRenderer.Placeholders(typeof(KmlCameraParameterValues)))}

See: https://developers.google.com/kml/documentation/kmlreference",
                this.Location
            );
        }

    }

}
