using System;
using System.Drawing;
using System.Windows.Forms;

namespace FS2020PlanePath
{
    public static class UserDialogUtils
    {

        public static void displayError(string caption, string details)
        {
            MessageBox.Show($"Details:\n{details}", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void displayMessage(string caption, string details)
        {
            MessageBox.Show(
                details,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public static bool obtainConfirmation(string caption, string details, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            DialogResult confirmationAnswer = MessageBox.Show(
                details,
                caption,
                MessageBoxButtons.YesNo,
                icon
            );
            return (DialogResult.Yes == confirmationAnswer);
        }

        public static bool TryGetStringInput(
            string text, 
            string caption, 
            out string input,
            KeyPressEventHandler keyPressHandler = null
        )
        {
            using (
                Form prompt = new Form()
                {
                    Text = caption,
                    Height = 150,
                    Width = 340,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterScreen
                }
            )
            {
                EventHandler closeFn = (sender, e) => prompt.Close();
                PaintEventHandler drawIconFn = (
                    (sender, e) =>
                    {
                        using (Graphics graphics = prompt.CreateGraphics())
                        {
                            graphics.DrawImage(SystemIcons.Question.ToBitmap(), new Point(260, 25));
                        }
                    }
                );

                prompt.Controls.Add(
                    new Label()
                    {
                        Text = text,
                        Top = 17,
                        Left = 15,
                        Width = 220
                    }
                );

                TextBox textBox = new TextBox()
                {
                    Top = 40,
                    Left = 15,
                    Width = 220
                };
                prompt.Controls.Add(textBox);
                if (keyPressHandler != null)
                {
                    textBox.KeyPress += keyPressHandler;
                }

                Button okButton = new Button()
                {
                    Top = 75,
                    Left = 15,
                    Width = 50,
                    Text = "&Ok",
                    DialogResult = DialogResult.OK
                };
                okButton.Click += closeFn;
                prompt.Controls.Add(okButton);
                prompt.AcceptButton = okButton;

                Button cancelButton = new Button()
                {
                    Top = 75,
                    Left = 75,
                    Width = 50,
                    Text = "&Cancel",
                    DialogResult = DialogResult.Cancel
                };
                cancelButton.Click += closeFn;
                prompt.Controls.Add(cancelButton);
                prompt.CancelButton = cancelButton;

                prompt.Paint += drawIconFn;


                DialogResult dialogResult = prompt.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    input = "";
                    return false;
                }

                input = textBox.Text;
                return true;
            }

        }

        public static void showHelpPopupText(Control parent, string popupText)
        {
            Help.ShowPopup(parent, popupText, parent.Location);
        }

    }

}
