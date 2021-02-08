using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Media;
using System.ComponentModel;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    public partial class KmlLiveCamEditorForm : Form
    {
        private const string SavedFileFilter = (
            "Keyhole Markup Language files (*.kml)|*.kml" +
            "|C# Script files (*.csx)|*.csx" +
            "|All files (*.*)|*.*"
        );

        private KmlLiveCam _kmlLiveCam;

        public KmlLiveCam KmlLiveCam => _kmlLiveCam;

        public KmlLiveCamEditorForm(string alias, string selectedLensName, KmlLiveCam kmlLiveCam) {
            InitializeComponent();
            _kmlLiveCam = kmlLiveCam;
            Text = $"MSFS2020-PilotPathRecorder LiveCam Editor - '{alias}'";
            kmlLiveCam.LensNames.ToList().ForEach(
                lensName => editorPaneTC.TabPages.Add(
                    newLensTab(lensName, kmlLiveCam.GetLens(lensName).Template)
                )
            );
            PerformLayout();
            selectLens(alias, selectedLensName);
        }

        private void selectLens(string alias, string selectedLensName)
        {
            TabPage existingLensTabPage = (
                editorPaneTC
                .TabPages.Cast<TabPage>()
                .FirstOrDefault(
                    tp => lensTabTextToLensName(tp.Text) == selectedLensName
                )
            );
            
            if (existingLensTabPage != default(TabPage))
            {
                editorPaneTC.SelectedTab = existingLensTabPage;
                return;
            }
            
            if (
                !UserDialogUtils.obtainConfirmation(
                    "Unrecognized LiveCam Lens Reference", 
                    $"Add Lens '{selectedLensName}' to LiveCam '{alias}'?"
                )
            )
            {
                return;
            }
            
            editorPaneTC.SelectedTab = addNewLensTabPage(selectedLensName);
            TryUpdateLiveCam(out _);
        }

        private void Handle_KmlTextValidation_Event(object sender, CancelEventArgs e)
        {
            string[] diagnostics;
            if (!TryUpdateLiveCam(out diagnostics))
            {
                UserDialogUtils.displayError("Validation Error(s)", string.Join("\n", diagnostics));
                e.Cancel = true;
                return;
            }

        }

        private bool TryUpdateLiveCam(out string[] diagnostics)
        {
            string[] lensTabNames = editorPaneTC.TabPages.Cast<TabPage>().Select(tp => tp.Name).ToArray();

            KmlLiveCam updatedKmlLiveCam = new KmlLiveCam(
                new LiveCamEntity(
                    lensTabNames.Select(
                        lensTabName => new LiveCamLensEntity(
                            lensTabTextToLensName(editorPaneTC.TabPages[lensTabName].Text),
                            editorPaneTC.TabPages[lensTabName].Controls[0].Text
                        )
                    )
                    .ToArray()
                )
            );

            diagnostics = updatedKmlLiveCam.Diagnostics;
            if (diagnostics.Length > 0)
            {
                return false;
            }

            _kmlLiveCam = updatedKmlLiveCam;
            return true;
        }

        private void Handle_FileOpenMenuItemSelected_Event(object sender, EventArgs e)
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

                string textFromFile;
                try
                {
                    textFromFile = File.ReadAllText(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    UserDialogUtils.displayError("Exception Encountered", ex.Message);
                    return;
                }

                editorPaneTC.SelectedTab.Controls[0].Text = textFromFile;

            }

        }

        private void Handle_FileSaveMenuItemSelected_Event(object sender, EventArgs e)
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

                string textToWrite = editorPaneTC.SelectedTab.Controls[0].Text;
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, textToWrite);
                } catch(Exception ex)
                {
                    UserDialogUtils.displayError("Exception Encountered", ex.Message);
                }
            }

        }

        private void Handle_HelpRequested_Event(object sender, HelpEventArgs hlpevent)
        {
            showHelp();
        }

        private void Handle_HelpMenuItemSelected_Event(object sender, EventArgs e)
        {
            showHelp();
        }

        private void showHelp() {
            UserDialogUtils.showHelpPopupText(
                this,
                $@"Permitted substitution values:

Camera Template:

{"\t" + string.Join("\n\t", TextRenderer.Placeholders(typeof(KmlCameraParameterValues)))}

See: https://developers.google.com/kml/documentation/kmlreference"
            );
        }

        private TabPage newLensTab(string lensName, string template)
        {
            
            // TODO - use logic to determine these "magic" values (where do these values come from?)
            Point magicTabPageLocation = new Point(4, 22);
            Size magicTabPageMargin = new Size(8, 26);
            Size magicTextBoxMargin = new Size(6, 6);

            TabPage newTabPage = new TabPage();

            int pageNumber = editorPaneTC.TabPages.Count + 1;

            newTabPage.Name = lensNameToLensTabName(lensName);
            newTabPage.Text = lensNameToLensTabText(lensName);
            newTabPage.Padding = new Padding(3, 3, 3, 3);
            newTabPage.TabIndex = editorPaneTC.TabIndex + pageNumber;
            newTabPage.ToolTipText = $"Text of the '{lensName}' lens";
            newTabPage.UseVisualStyleBackColor = true;

            newTabPage.Location = magicTabPageLocation;
            newTabPage.Size = new Size(
                editorPaneTC.Width - magicTabPageMargin.Width, 
                editorPaneTC.Height - magicTabPageMargin.Height
            );

            TextBox newTextBox = new TextBox();
            newTextBox.Name = $"{newTabPage.Name}.TB";
            newTextBox.AcceptsReturn = true;
            newTextBox.AcceptsTab = true;
            newTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            newTextBox.Location = new Point(3, 3);
            newTextBox.Multiline = true;
            newTextBox.ScrollBars = ScrollBars.Both;
            newTextBox.Size = new Size(
                newTabPage.ClientSize.Width - magicTextBoxMargin.Width, 
                newTabPage.ClientSize.Height - magicTextBoxMargin.Height
            );
            newTextBox.TabStop = false;
            newTextBox.Text = template;
            newTextBox.Validating += new CancelEventHandler(this.Handle_KmlTextValidation_Event);

            newTabPage.Controls.Add(newTextBox);

            return newTabPage;
        }

        private void newBT_Click(object sender, EventArgs e)
        {
            string newLensName;
            if (
                UserDialogUtils.TryGetStringInput(
                    "Lens Name (Empty for Initial Link):", 
                    "Create LiveCam Lens", 
                    out newLensName,
                    LensNameInputKeyPressHandler
                )
            )
            {
                editorPaneTC.SelectedTab = addNewLensTabPage(newLensName);
            }
        }

        private TabPage addNewLensTabPage(string newLensName)
        {
            editorPaneTC.SuspendLayout();
            TabPage newLensTabPage = newLensTab(newLensName, "");
            editorPaneTC.TabPages.Add(newLensTabPage);
            editorPaneTC.ResumeLayout();
            return newLensTabPage;
        }

        private void deleteBT_Click(object sender, EventArgs e)
        {
            TabPage currentLensTab = editorPaneTC.SelectedTab;
            if (currentLensTab == null)
            {
                return;
            }
            if (
                UserDialogUtils.obtainConfirmation(
                    "Confirm Deletion of Lens", 
                    $"Delete Lens '{currentLensTab.Text}'?",
                    MessageBoxIcon.Warning
                )
            )
            {
                editorPaneTC.TabPages.Remove(currentLensTab);
                TryUpdateLiveCam(out _);
            }
        }

        private string lensTabTextToLensName(string lensTabText)
        {
            return lensTabText.Substring(1);
        }

        private string lensNameToLensTabText(string lensName)
        {
            return $"/{lensName}";
        }

        private string lensNameToLensTabName(string lensName)
        {
            return $"lens_{lensName}";
        }

        private void LensNameInputKeyPressHandler(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if (!char.IsControl(key) && !char.IsLetterOrDigit(key))
            {
                e.Handled = true;
                SystemSounds.Hand.Play();
            }
        }

    }

}
