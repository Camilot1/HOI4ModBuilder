using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.scripts;
using HOI4ModBuilder.src.scripts.commands;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace HOI4ModBuilder.src.forms.scripts
{
    public partial class ScriptsForm : Form
    {
        public static ScriptsForm Instance { get; private set; }
        private string _filePath;
        private string[] _scriptLines;

        public ScriptsForm()
        {
            InitializeComponent();

            RichTextBox_Script.AllowDrop = true;
            RichTextBox_Script.DragEnter += RichTextBox_Script_DragEnter;
            RichTextBox_Script.DragDrop += RichTextBox_Script_DragDrop;

            Instance?.Invoke((MethodInvoker)delegate { Instance?.Close(); });
            Instance = this;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RefreshData();
            GuiLocManager.formsReinitEvents.Add(this, () =>
            {
                Invoke((MethodInvoker)delegate
                {
                    Controls.Clear();
                    InitializeComponent();
                    RefreshData();
                });
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void RefreshData()
            => Logger.TryOrLog(() => RichTextBox_Script.Text = _scriptLines == null ? "" : string.Join("\n", _scriptLines));

        private void Button_ChooseFile_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                string filePath;
                var fd = new OpenFileDialog();
                var dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "scripts" });
                Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.SCRIPTS_CHOOSE_FILE), dialogPath, "TXT files (*.txt)|*.txt");
                if (fd.ShowDialog() == DialogResult.OK)
                    filePath = fd.FileName;
                else
                    return;

                textBox1.Text = filePath;
                _filePath = filePath;
                UpdateFilePathRender();
                _scriptLines = File.ReadAllLines(filePath);
                RefreshData();
            });

        private void Button_Save_Click(object sender, EventArgs e)
        {

        }

        private void Button_Load_Click(object sender, EventArgs e)
        {

        }

        private void RichTextBox_Script_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                bool allTxt = files.All(f =>
                    Path.GetExtension(f)
                        .Equals(".txt", StringComparison.OrdinalIgnoreCase));

                e.Effect = allTxt
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void RichTextBox_Script_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                if (Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    textBox1.Text = file;
                    _filePath = file;
                    UpdateFilePathRender();
                    _scriptLines = File.ReadAllLines(file);
                    RefreshData();
                }
            }
        }

        private void ChangeExecutionState(bool flag)
        {
            Button_Execute.Enabled = flag;
        }

        private void Button_Execute_Click(object sender, EventArgs e)
        {
            Logger.TryOrCatch(() =>
            {
                ClearConsole();
                var executeTask = new Task(() =>
                {
                    Logger.TryOrCatch(() =>
                    {
                        var action = ScriptParser.Parse(_scriptLines, out var _);
                        action?.Invoke();
                    },
                    (ex) =>
                    {
                        Logger.LogSingleErrorMessage(ex.Message);
                        Logger.Log(ex.ToString());
                    });
                });
                executeTask.ContinueWith((t) =>
                {
                    Action action = () => ChangeExecutionState(true);
                    Invoke(action);
                });

                ScriptParser.DebugConsumer = (command, lineIndex, varsScope) =>
                {
                    Action action = () => UpdateDebug(command, lineIndex, varsScope);
                    Invoke(action);
                };
                ChangeExecutionState(false);
                executeTask.Start();
            },
            (ex) =>
            {
                ChangeExecutionState(true);
                Logger.LogException(ex);
            });
        }

        public void PrintToConsole(string value)
        {
            Action action = () => RichTextBox_Console.AppendText(value);
            Invoke(action);
        }

        public void ClearConsole()
        {
            Action action = () => RichTextBox_Console.Clear();
            Invoke(action);
        }

        private void UpdateDebug(ScriptCommand command, int lineIndex, VarsScope varsScope)
        {
            RichTextBox_Debug.Clear();

            int level = 0;
            var sb = new StringBuilder();

            var tempVarsScope = varsScope;
            while (tempVarsScope != null)
            {
                sb.Append("> Level: ").Append(level).Append("\n\n");
                level++;
                foreach (var entry in tempVarsScope.GetVars())
                {
                    sb.Append(entry.Key).Append(" = ").Append(entry.Value.ToString()).Append("\n\n");
                }
                tempVarsScope = tempVarsScope.Prev;
            }

            RichTextBox_Debug.Text = sb.ToString();

            GroupBox_Debug.Text = $"Debug: {ScriptParser.IsDebug}; Line: {lineIndex + 1}";

            var scriptLines = RichTextBox_Script.Text.Split('\n');
            var currentLineMark = ">>>";
            for (int i = 0; i < scriptLines.Length; i++)
            {
                var scriptLine = scriptLines[i];
                if (lineIndex == i)
                {
                    if (!scriptLine.StartsWith(currentLineMark))
                        scriptLine = currentLineMark + scriptLine;
                }
                else if (scriptLine.StartsWith(currentLineMark))
                {
                    scriptLine = scriptLine.Substring(currentLineMark.Length);
                }

                scriptLines[i] = scriptLine;
            }
            RichTextBox_Script.Text = string.Join("\n", scriptLines);
        }

        private void RichTextBox_Script_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _scriptLines = RichTextBox_Script.Lines);

        private void Button_Debug_Flip_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                ScriptParser.IsDebug = !ScriptParser.IsDebug;
                GroupBox_Debug.Text = "Debug: " + ScriptParser.IsDebug;
            });

        private void Button_Debug_NextStep_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                ScriptParser.NextStep = true;
            });

        private void Button_Debug_Terminate_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                ScriptParser.IsTerminated = true;
                GroupBox_Debug.Text = "Debug: " + ScriptParser.IsDebug;
            });

        private void RichTextBox_Script_KeyDown(object sender, KeyEventArgs e)
        {
            var rtb = RichTextBox_Script;

            if (e.Control && e.KeyCode == Keys.D)
            {
                e.SuppressKeyPress = true;

                int lineIndex = rtb.GetLineFromCharIndex(rtb.SelectionStart);
                int lineStart = rtb.GetFirstCharIndexFromLine(lineIndex);
                string lineText = rtb.Lines[lineIndex];

                rtb.SelectionStart = lineStart + lineText.Length;
                rtb.SelectionLength = 0;

                rtb.SelectedText = Environment.NewLine + lineText;

                int newLineStart = rtb.GetFirstCharIndexFromLine(lineIndex + 1);
                rtb.SelectionStart = newLineStart + lineText.Length;
            }

            if (e.Control && e.KeyCode == Keys.X)
            {
                e.SuppressKeyPress = true;

                int lineIndex = rtb.GetLineFromCharIndex(rtb.SelectionStart);
                int lineStart = rtb.GetFirstCharIndexFromLine(lineIndex);
                string textOfLine = rtb.Lines[lineIndex];

                bool isLastLine = lineIndex == rtb.Lines.Length - 1;
                int nextLineStart = isLastLine
                    ? lineStart + textOfLine.Length
                    : rtb.GetFirstCharIndexFromLine(lineIndex + 1);

                int selLength = nextLineStart - lineStart;

                rtb.SelectionStart = lineStart;
                rtb.SelectionLength = selLength;
                Clipboard.SetText('\n' + rtb.SelectedText);

                rtb.SelectedText = "";

                int prevLine = Math.Max(0, lineIndex - 1);
                int prevStart = rtb.GetFirstCharIndexFromLine(prevLine);
                rtb.SelectionStart = prevStart + rtb.Lines[prevLine].Length;
            }
        }

        private void GroupBox_Script_Resize(object sender, EventArgs e)
        {
            UpdateFilePathRender();
        }

        private void UpdateFilePathRender()
        {
            GroupBox_Script.Text = Utils.TruncatePath(_filePath, GroupBox_Script.Font, GroupBox_Script.Width - 12);
            textBox1.SelectionStart = 0;
            textBox1.ScrollToCaret();
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
    }
}
