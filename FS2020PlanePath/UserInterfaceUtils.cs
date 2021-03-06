using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;

namespace FS2020PlanePath
{

    public enum ToggleState
    {
        Out,
        In
    }

    public interface IButtonStateModel<ButtonState>
    {
        ButtonState State { get; set; }

        bool IsEnabled { get; set; }
    }

    public class MultiButtonStateModel<ButtonState> : IButtonStateModel<ButtonState> where ButtonState : Enum
    {
        private Button button;
        private string[] typeNames;
        private ButtonState type;

        public MultiButtonStateModel(Button button, bool enabled, ButtonState type, params string[] typeNames)
        {
            Debug.Assert(typeof(ButtonState).GetEnumValues().Length == typeNames.Length);
            this.button = button;
            this.typeNames = typeNames;
            button.Enabled = enabled;
            IsEnabled = button.Enabled;
            State = type;
        }

        public ButtonState State
        {
            get => type;
            set
            {
                type = value;
                button.Text = typeNames[Convert.ToInt32(type)];
            }

        }

        public bool IsEnabled
        {
            get => button.Enabled;
            set => button.Enabled = value;
        }
    }

    public static class UserDialogUtils
    {

        public static void displayError(string caption, string details)
        {
            MessageBox.Show($"Details:\n{details}", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void displayMessage(string caption, string details, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(
                details,
                caption,
                MessageBoxButtons.OK,
                icon
            );
        }

        public static bool obtainConfirmation(string caption, string details, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            DialogResult confirmationAnswer = MessageBox.Show(
                details,
                caption,
                MessageBoxButtons.YesNoCancel,
                icon,
                MessageBoxDefaultButton.Button3
            );
            return (DialogResult.Yes == confirmationAnswer);
        }

        public static void LettersOrDigitsOnlyInputKeyPressHandler(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if (!char.IsControl(key) && !char.IsLetterOrDigit(key))
            {
                e.Handled = true;
                SystemSounds.Hand.Play();
            }
        }

        public static bool TryGetStringInput(
            string text,
            string caption,
            out string input,
            KeyPressEventHandler keyPressHandler = default(KeyPressEventHandler),
            Func<string, bool> validator = default(Func<string, bool>)
        )
        {
            using (
                Form prompt = new Form()
                {
                    Text = caption,
                    Height = 180,
                    Width = 370,
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
                            graphics.DrawImage(SystemIcons.Question.ToBitmap(), new Point(275, 40));
                        }
                    }
                );

                prompt.Controls.Add(
                    new Label()
                    {
                        Text = text,
                        Top = 32,
                        Left = 30,
                        Width = 220
                    }
                );

                TextBox textBox = new TextBox()
                {
                    Top = 55,
                    Left = 30,
                    Width = 220
                };
                prompt.Controls.Add(textBox);
                if (keyPressHandler != null)
                {
                    textBox.KeyPress += keyPressHandler;
                }
                if (validator != default(Func<string, bool>))
                {
                    textBox.Validating += (
                        (object sender, CancelEventArgs e) =>
                        {
                            if (!validator.Invoke(textBox.Text))
                            {
                                e.Cancel = true;
                            }
                        }
                    );
                }

                Button okButton = new Button()
                {
                    Top = 90,
                    Left = 30,
                    Width = 60,
                    Text = "&OK",
                    DialogResult = DialogResult.OK
                };
                okButton.Click += closeFn;
                prompt.Controls.Add(okButton);
                prompt.AcceptButton = okButton;

                Button cancelButton = new Button()
                {
                    Top = 90,
                    Left = 100,
                    Width = 60,
                    Text = "&Cancel",
                    DialogResult = DialogResult.Cancel
                };
                cancelButton.Click += closeFn;
                cancelButton.CausesValidation = false;
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    }

}
