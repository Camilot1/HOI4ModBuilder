using System;
using System.Threading;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.utils
{
    public static class DialogUtils
    {
        // Intentionally show shell dialogs on a dedicated STA thread.
        // In this WinForms/OpenTK app, showing them directly on the main UI thread can trigger
        // a focus/activation bug where the dialog becomes visible only after Alt+Tab.
        public static string ChooseFolder(string description, string selectedPath)
            => RunStaDialog(() =>
            {
                using (var dialog = Utils.PrepareFolderDialog(selectedPath))
                {
                    dialog.Description = description;
                    return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
                }
            });

        // Keep file selection here too so the project has one place for dialog-thread workarounds
        // if the same WinForms activation bug starts affecting OpenFileDialog later.
        public static string ChooseOpenFile(string title, string initialDirectory, string filter)
            => RunStaDialog(() =>
            {
                using (var dialog = new OpenFileDialog())
                {
                    Utils.PrepareFileDialog(dialog, title, initialDirectory, filter);
                    return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
                }
            });

        public static string ChooseSaveFile(string title, string initialDirectory, string filter)
            => RunStaDialog(() =>
            {
                using (var dialog = new SaveFileDialog())
                {
                    Utils.PrepareFileDialog(dialog, title, initialDirectory, filter);
                    return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
                }
            });

        // Use the same STA-dialog workaround for MessageBox as for file/folder dialogs.
        // In this app, some WinForms dialogs can fail to appear in front until Alt+Tab unless
        // they are shown from a dedicated STA thread isolated from the main OpenTK/WinForms UI.
        public static DialogResult ShowMessageBox(
            string text,
            string title,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            => RunStaDialog(() => MessageBox.Show(text, title, buttons, icon, defaultButton));

        private static T RunStaDialog<T>(Func<T> showDialogFunc)
        {
            T result = default(T);
            Exception dialogException = null;

            var thread = new Thread(() =>
            {
                try
                {
                    result = showDialogFunc();
                }
                catch (Exception ex)
                {
                    dialogException = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (dialogException != null)
                throw dialogException;

            return result;
        }
    }
}
