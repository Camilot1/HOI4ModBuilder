using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.forms.messageForms;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms.actionForms
{
    public enum EnumCreateObjectType
    {
        STATE,
        REGION
    }
    public partial class CreateObjectForm : Form
    {
        private bool _forceType;
        private EnumCreateObjectType _type;
        private string _fileName;
        private string[] _fileTextLines;

        private Action<object> _onRedo;
        private Action<object> _onUndo;


        public CreateObjectForm()
        {
            InitializeComponent();
            _type = 0;
        }
        public CreateObjectForm(EnumCreateObjectType type, bool forceType)
            : this()
        {
            _type = type;
            _forceType = forceType;
        }

        public CreateObjectForm(EnumCreateObjectType type, bool forceType, Action<object> onRedo, Action<object> onUndo)
            : this(type, forceType)
        {
            _onRedo = onRedo;
            _onUndo = onUndo;

            if (_onRedo != null && _onUndo == null || _onRedo == null && _onUndo != null)
                throw new Exception("Both onRedo and onUndo should be Null or NotNull at the same time!");
        }
        public void InvokeAction(Action action)
            => Invoke((MethodInvoker)delegate { action(); });

        public void TryInvokeActionOrLog(Action tryAction, Action<Exception> catchAction)
            => Invoke((MethodInvoker)delegate { Logger.TryOrCatch(tryAction, catchAction); });

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadData();
            GuiLocManager.formsReinitEvents.Add(this, () =>
            {
                Invoke((MethodInvoker)delegate
                {
                    Controls.Clear();
                    InitializeComponent();
                    LoadData();
                });
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
            => Logger.TryOrLog(() =>
            {
                LoadPattern();
                UpdateDisplayID();

                TextBox_File_Name.Text = _fileName;

                ComboBox_ObjectType.Enabled = !_forceType;
                ComboBox_ObjectType.Items.Clear();
                foreach (var type in Enum.GetValues(typeof(EnumCreateObjectType)))
                    ComboBox_ObjectType.Items.Add(GuiLocManager.GetLoc(type.ToString()));
                ComboBox_ObjectType.SelectedIndex = (int)_type;

                RichTextBox_File_Text.Text = _fileTextLines == null ? "" : string.Join("\n", _fileTextLines);
            });

        private void UpdateDisplayID()
            => Label_ID.Text = "ID: " + GetFreeID();

        private ushort GetFreeID()
        {
            switch (_type)
            {
                case EnumCreateObjectType.STATE: return Utils.GetRecalculatedFirstEmptyID(StateManager.GetStatesIdsSorted());
                case EnumCreateObjectType.REGION: return Utils.GetRecalculatedFirstEmptyID(StrategicRegionManager.GetRegionsIdsSorted());
                default: throw new Exception("Not implemented type: " + _type);
            }
        }

        private string GetModFolder()
        {
            switch (_type)
            {
                case EnumCreateObjectType.STATE: return StateManager.FOLDER_PATH;
                case EnumCreateObjectType.REGION: return StrategicRegionManager.FOLDER_PATH;
                default: throw new Exception("Not implemented type: " + _type);
            }
        }

        private Action<FileInfo> GetLoadAction()
        {
            switch (_type)
            {
                case EnumCreateObjectType.STATE:
                    return (fileInfo) => StateManager.LoadFile(new GameParser(), new StateGameFile(fileInfo));
                case EnumCreateObjectType.REGION:
                    return (fileInfo) => StrategicRegionManager.LoadFile(fileInfo);
                default: throw new Exception("Not implemented type: " + _type);
            }
        }

        private Action<ushort> GetRemoveAction()
        {
            switch (_type)
            {
                case EnumCreateObjectType.STATE: return (id) => StateManager.RemoveState(id);
                case EnumCreateObjectType.REGION: return (id) => StrategicRegionManager.RemoveRegion(id);
                default: throw new Exception("Not implemented type: " + _type);
            }
        }

        private void ComboBox_ObjectType_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _type = (EnumCreateObjectType)ComboBox_ObjectType.SelectedIndex);

        private void Button_Create_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                var id = GetFreeID();
                var stringID = id.ToString();
                var fileName = _fileName.Replace("{id}", stringID);

                if (!fileName.EndsWith(".txt"))
                    fileName += ".txt";

                var filePath = FileManager.AssembleFilePath(new[] { SettingsManager.Settings.modDirectory, GetModFolder(), fileName });
                if (File.Exists(filePath))
                    throw new FileAlreadyExistsException(filePath);

                if (_fileTextLines == null)
                    _fileTextLines = new string[0];

                var copy = new string[_fileTextLines.Length];
                for (int i = 0; i < _fileTextLines.Length; i++)
                    copy[i] = _fileTextLines[i];

                var loadAction = GetLoadAction();
                var removeAction = GetRemoveAction();

                MapManager.ActionHistory.Add(
                    () =>
                    {
                        for (int i = 0; i < copy.Length; i++)
                            copy[i] = copy[i].Replace("{id}", stringID);
                        File.WriteAllLines(filePath, copy);
                        loadAction.Invoke(new FileInfo(fileName, filePath, true));
                        _onRedo.Invoke(id);

                        InvokeAction(() => UpdateDisplayID());
                    },
                    () =>
                    {
                        _onUndo?.Invoke(id);
                        removeAction.Invoke(id);
                        File.Delete(filePath);

                        InvokeAction(() => UpdateDisplayID());
                    }
                );
            });

        private void Button_Pattern_Save_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                var pattern = SettingsManager.Settings.GetCreateObjectPattern(_type);
                pattern.fileName = _fileName;
                pattern.fileTextLines = _fileTextLines;
                SettingsManager.SaveSettings();
            });

        private void Button_Pattern_Load_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                LoadPattern();
                LoadData();
            });

        private void LoadPattern()
            => Logger.TryOrLog(() =>
            {
                var pattern = SettingsManager.Settings.GetCreateObjectPattern(_type);
                _fileName = pattern.fileName;
                _fileTextLines = pattern.fileTextLines;
            });

        private void TextBox_File_Name_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _fileName = TextBox_File_Name.Text);


        private void RichTextBox_File_Text_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _fileTextLines = RichTextBox_File_Text.Lines);

        private void RichTextBox_File_Text_KeyDown(object sender, KeyEventArgs e)

            => Logger.TryOrLog(() =>
            {
                var rtb = RichTextBox_File_Text;

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
            });
    }
}
