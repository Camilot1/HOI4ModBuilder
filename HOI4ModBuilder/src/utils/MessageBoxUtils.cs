using System.Windows.Forms;

namespace HOI4ModBuilder.src.utils
{
    public class MessageBoxUtils
    {
        public static DialogResult ShowWarningChooseAction(string text, MessageBoxButtons buttons)
            => Show(text, GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), buttons, MessageBoxIcon.Warning);
        public static DialogResult ShowQuestionChooseAction(string text, MessageBoxButtons buttons)
            => Show(text, GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), buttons, MessageBoxIcon.Question);
        public static DialogResult ShowExclamationChooseAction(string text, MessageBoxButtons buttons)
            => Show(text, GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), buttons, MessageBoxIcon.Exclamation);

        public static DialogResult ShowInformation(string text, string title, MessageBoxButtons buttons)
            => Show(text, title, buttons, MessageBoxIcon.Information);
        public static DialogResult ShowInfoMessage(string text, MessageBoxButtons buttons)
            => Show(text, GuiLocManager.GetLoc(EnumLocKey.INFO_MESSAGE), buttons, MessageBoxIcon.Information);

        public static DialogResult Show(string text, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var result = MessageBox.Show(
                text, title, buttons, icon,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly
            );

            if (MainForm.IsFirstInited)
            {
                MainForm.Instance.InvokeAction(() =>
                {
                    var mainForm = MainForm.Instance;
                    var prevTopMost = mainForm.TopMost;

                    mainForm.TopMost = true;
                    mainForm.Activate();
                    mainForm.BringToFront();
                    mainForm.Focus();

                    mainForm.TopMost = prevTopMost;
                });
            }

            return result;
        }
    }
}
