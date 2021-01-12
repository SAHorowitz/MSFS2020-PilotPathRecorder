using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FS2020PlanePath
{
    public partial class TextEditorForm : Form
    {
        private const string SavedFileFilter = "KML files (*.kml)|*.kml|All files (*.*)|*.*";
        private Func<string, string> kmlValidator;

        public TextEditorForm(
            string title, 
            string cameraKmlText,
            string linkKmlText,
            Func<string, string> kmlValidator
        ) {
            InitializeComponent();

            this.Text = title;
            this.kmlValidator = kmlValidator;
            cameraEditorTB.Text = cameraKmlText;
            linkEditorTB.Text = linkKmlText;
        }

        public string EditorText { get { return cameraEditorTB.Text; } }
        public string LinkText { get { return linkEditorTB.Text; } }

        private void validateKmlTexts(object sender, CancelEventArgs e)
        {
            if (kmlValidator != null)
            {
                List<string> complaints = new List<string>();
                foreach (
                    string[] item
                    in
                    new string[][]
                    {
                        new string[] { "Camera KML", EditorText },
                        new string[] { "Link KML", LinkText }
                    }
                )
                {
                    string failureMessage = kmlValidator.Invoke(item[1]);
                    if (failureMessage != null)
                    {
                        complaints.Add($"{item[0]}: {failureMessage}");
                    }
                }
                if (complaints.Count > 0)
                {
                    displayError(
                        "Validation Failure(s)",
                        string.Join("\n", complaints)
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
                    cameraEditorTB.Text = File.ReadAllText(openFileDialog.FileName);
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

                string textToWrite;
                switch (editorPaneTC.SelectedTab.Name)
                {
                    case "cameraEditorTP":
                        textToWrite = cameraEditorTB.Text;
                        break;
                    case "linkEditorTP":
                        textToWrite = linkEditorTB.Text;
                        break;
                    default:
                        displayError("Oops", $"unknown tab({editorPaneTC.SelectedTab.Name})");
                        return;
                }

                try
                {
                    File.WriteAllText(saveFileDialog.FileName, textToWrite);
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

Camera Template:

{"\t" + string.Join("\n\t", TemplateRenderer.Placeholders(typeof(KmlCameraParameterValues)))}

Link Template:

{"\t" + string.Join("\n\t", TemplateRenderer.Placeholders(typeof(KmlNetworkLinkValues)))}

See: https://developers.google.com/kml/documentation/kmlreference",
                this.Location
            );
        }
    }

}
