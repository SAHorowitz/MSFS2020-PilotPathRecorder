using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    public partial class KmlLiveCamEditorForm : Form
    {
        private const string SavedKmlLiveCamFileFilter = (
            "KML LiveCam Definition (*.json)|*.json" +
            "|All files (*.*)|*.*"
        );

        private const string SavedTemplateFileFilter = (
            "Keyhole Markup Language files (*.kml)|*.kml" +
            "|C# Script files (*.csx)|*.csx" +
            "|All files (*.*)|*.*"
        );

        private string alias;
        private string selectedLensName;
        private KmlLiveCam initialKmlLiveCam;
        private KmlLiveCam kmlLiveCam;
        private bool textChanged;
        private Func<string, KmlLiveCam> loadKmlLiveCamFromFileFn;
        private Action<string, KmlLiveCam> saveKmlLiveCamToFileFn;
        private Func<string, bool> isAliasAvailableFn;

        public string Alias => alias;

        public KmlLiveCam KmlLiveCam => kmlLiveCam;

        public KmlLiveCamEditorForm(
            string alias,
            string selectedLensName,
            KmlLiveCam kmlLiveCam,
            Func<string, KmlLiveCam> loadKmlLiveCamFromFileFn,
            Action<string, KmlLiveCam> saveKmlLiveCamToFileFn,
            Func<string, bool> isAliasAvailableFn
        )
        {
            InitializeComponent();
            
            this.alias = alias;
            this.selectedLensName = selectedLensName;
            this.kmlLiveCam = kmlLiveCam;
            this.loadKmlLiveCamFromFileFn = loadKmlLiveCamFromFileFn;
            this.saveKmlLiveCamToFileFn = saveKmlLiveCamToFileFn;
            this.isAliasAvailableFn = isAliasAvailableFn;
            this.initialKmlLiveCam = this.kmlLiveCam;

            InitializeKmlLiveCam();
        }

        private void Handle_KmlTextValidation_Event(object sender, CancelEventArgs e)
        {
            string[] diagnostics;
            if (!TryUpdateLiveCam(out diagnostics))
            {
                UserDialogUtils.displayError("Validation Error(s)", string.Join("\n", diagnostics));
                e.Cancel = true;
            }

        }

        private void Handle_TextChanged_Event(object sender, EventArgs e)
        {
            textChanged = true;
        }

        private void Handle_LiveCamDefinitionLoadFromFile_Event(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = SavedKmlLiveCamFileFilter;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                executeWithinTryCatchBlock(
                    () =>
                    {
                        replaceLoadedLiveCam(loadKmlLiveCamFromFileFn.Invoke(openFileDialog.FileName));
                    }
                );

            }

        }

        private void Handle_LiveCamDefinitionSaveToFile_Event(object sender, EventArgs e)
        {

            TryUpdateLiveCam(out _);

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = SavedKmlLiveCamFileFilter;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string saveFilename = saveFileDialog.FileName;
                executeWithinTryCatchBlock(
                    () =>
                    {
                        saveKmlLiveCamToFileFn.Invoke(saveFilename, kmlLiveCam);
                        Console.WriteLine($"KML LiveCam '{alias}' saved to({saveFilename})");
                    }
                );

            }

        }

        private void Handle_LiveCamDefinitionRename_Event(object sender, EventArgs e)
        {
            string duplicateAlias;
            if (
                !UserDialogUtils.TryGetStringInput(
                    "Enter new alias",
                    "Rename LiveCam Definition",
                    out duplicateAlias,
                    UserDialogUtils.LettersOrDigitsOnlyInputKeyPressHandler,
                    proposedNewAlias => {
                        if (!isAliasAvailableFn.Invoke(proposedNewAlias))
                        {
                            UserDialogUtils.displayError(
                                "Unavailable Alias",
                                $"Alias '{proposedNewAlias}' is not available.\nPlease choose another."
                            );
                            return false;
                        }
                        return true;
                    }
                )
            )
            {
                return;
            }

            alias = duplicateAlias;
            InitializeForAlias();

        }

        private void Handle_LensTemplateLoadFrom_Event(object sender, EventArgs e)
        {
            if (!IsLosingChangesOkay())
            {
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = SavedTemplateFileFilter;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                executeWithinTryCatchBlock(
                    () => editorPaneTC.SelectedTab.Controls[0].Text = File.ReadAllText(openFileDialog.FileName)
                );

            }

        }

        private void Handle_LensTemplateSaveAs_Event(object sender, EventArgs e)
        {

            TryUpdateLiveCam(out _);

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = SavedTemplateFileFilter;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                executeWithinTryCatchBlock(
                    () => {
                        File.WriteAllText(
                            saveFileDialog.FileName,
                            editorPaneTC.SelectedTab.Controls[0].Text
                        );
                    }
                );

            }
        }
        private void newBT_Click(object sender, EventArgs e)
        {
            string newLensName;
            if (
                UserDialogUtils.TryGetStringInput(
                    "Lens Name (leave empty for initial lens):",
                    "Create LiveCam Lens",
                    out newLensName,
                    UserDialogUtils.LettersOrDigitsOnlyInputKeyPressHandler,
                    proposedLensName => {
                        if (IsLensDefined(proposedLensName))
                        {
                            UserDialogUtils.displayError(
                                "Unavailable Lens Name",
                                $"Lens '{proposedLensName}' is already defined for this LiveCam.\nPlease choose another."
                            );
                            return false;
                        }
                        return true;
                    }
                )
            )
            {
                editorPaneTC.SelectedTab = addNewLensTabPage(newLensName);
            }
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

        private void KmlLiveCamEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel && !IsLosingChangesOkay())
            {
                e.Cancel = true;
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

        private void InitializeForAlias()
        {
            Text = $"MSFS2020-PilotPathRecorder LiveCam Editor - '{alias}'";
        }

        private void InitializeKmlLiveCam()
        {
            InitializeForAlias();
            editorPaneTC.TabPages.Clear();
            kmlLiveCam.LensNames.ToList().ForEach(
                lensName => editorPaneTC.TabPages.Add(
                    newLensTab(lensName, kmlLiveCam.GetLens(lensName).Template)
                )
            );
            PerformLayout();
            selectLens(alias, selectedLensName);
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

        private bool IsLensDefined(string lensName)
        {
            return (
                default(TabPage) != editorPaneTC
                .TabPages.Cast<TabPage>()
                .FirstOrDefault(
                    tp => lensTabTextToLensName(tp.Text) == lensName
                )
            );
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
            newTabPage.ToolTipText = $"Text of the '{lensName}' lens template";
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
            newTextBox.TextChanged += new EventHandler(this.Handle_TextChanged_Event);

            newTabPage.Controls.Add(newTextBox);

            return newTabPage;
        }

        private bool TryUpdateLiveCam(out string[] diagnostics)
        {
            if (!textChanged)
            {
                diagnostics = new string[0];
                return true;
            }

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

            kmlLiveCam = updatedKmlLiveCam;
            textChanged = false;

            return true;
        }

        private TabPage addNewLensTabPage(string newLensName)
        {
            editorPaneTC.SuspendLayout();
            TabPage newLensTabPage = newLensTab(newLensName, "");
            editorPaneTC.TabPages.Add(newLensTabPage);
            editorPaneTC.ResumeLayout();
            return newLensTabPage;
        }

        private bool IsLosingChangesOkay()
        {
            if (initialKmlLiveCam.Equals(kmlLiveCam) && !textChanged)
            {
                return true;
            }
            return (
                UserDialogUtils.obtainConfirmation(
                    "Confirm Abandon Changes",
                    "Changes will be lost.\nDo you wish to continue?"
                )
            );
        }

        private static void executeWithinTryCatchBlock(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                UserDialogUtils.displayError("Exception Encountered", ex.Message);
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

        private void replaceLoadedLiveCam(KmlLiveCam newKmlLiveCam)
        {
            Console.WriteLine($"loading new LiveCam({newKmlLiveCam})");
            kmlLiveCam = newKmlLiveCam;
            InitializeKmlLiveCam();
        }

    }

}
