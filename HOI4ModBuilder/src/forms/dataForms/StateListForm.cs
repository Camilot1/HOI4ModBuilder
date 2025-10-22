using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.forms.actionForms;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms
{
    partial class StateListForm : Form
    {
        public static StateListForm Instance { get; private set; }
        public static State currentState;
        public static string text = null;

        public StateListForm()
        {
            InitializeComponent();
            Instance?.Invoke((MethodInvoker)delegate { Instance?.Close(); });
            Instance = this;
        }

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
            Instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
        {
            var data = Instance.DataGridView_States;
            data.Rows.Clear();

            var assembleTask = new Task<DataGridViewRow[]>(
                () =>
                {
                    ushort id;
                    ushort[] ids = StateManager.GetStatesIds().OrderBy(x => x).ToArray();

                    var rows = new DataGridViewRow[ids.Length];

                    DataGridViewRow row;

                    for (int i = 0; i < ids.Length; i++)
                    {
                        id = ids[i];
                        StateManager.TryGetState(id, out State s);

                        row = new DataGridViewRow();
                        row.CreateCells(data, new object[] {
                            id,
                            s.CurrentName,
                            s.CurrentManpower,
                            s.CurrentStateCategory?.name,
                            s.owner?.Tag,
                            s.Provinces.Count,
                            s.pixelsCount
                        });
                        rows[i] = row;
                    }

                    return rows;
                }
            );

            assembleTask.ContinueWith(
                task => data.Rows.AddRange(task.Result),
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            assembleTask.Start();
        }

        private void Button_Find_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                ushort id = ushort.Parse(TextBox_Id.Text.Trim());
                FindState(id);
            });
        }

        public void FindState(ushort id)
        {
            Logger.TryOrLog(() =>
            {
                if (StateManager.TryGetState(id, out State state))
                {
                    currentState = state;
                    var sb = new StringBuilder();

                    var stateGameFile = (StateGameFile)state.GetParent().GetParent();

                    stateGameFile.Save(sb, "", null, default);
                    text = sb.ToString();
                    RichTextBox_StateInfo.Text = text;
                    TextBox_FileInfo.Text = stateGameFile.FileInfo.fileName;
                }
                else Logger.LogSingleErrorMessage(
                        EnumLocKey.STATE_NOT_FOUND_BY_ID,
                        new Dictionary<string, string> { { "{stateId}", $"{id}" } }
                    );
            });
        }

        private void Button_Load_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (currentState == null)
                    return;

                var stateGameFile = (StateGameFile)currentState.GetParent().GetParent();
                var sb = new StringBuilder();
                stateGameFile.Save(sb, "", null, default);
                RichTextBox_StateInfo.Text = sb.ToString();
                TextBox_FileInfo.Text = stateGameFile.FileInfo.fileName;
            });
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            //TODO reimplement
            if (true)
                return;

            Logger.TryOrLog(() =>
            {
                if (currentState == null)
                    return;
                if (text.Equals(RichTextBox_StateInfo.Text))
                    return;

                var newStateGameFile = new StateGameFile();
                var parser = new GameParser();

                parser.ParseFileFromString(newStateGameFile, RichTextBox_StateInfo.Text);

                var state = new State
                {
                    //fileInfo = currentState.fileInfo,
                    borders = currentState.borders,
                    center = currentState.center,
                    dislayCenter = currentState.dislayCenter,
                    pixelsCount = currentState.pixelsCount
                };

                //TODO
                //ParadoxParser.Parse(Utils.ToStream(RichTextBox_StateInfo.Text), state);

                var s = state;
                currentState.ClearData();
                StateManager.RemoveState(currentState.Id.GetValue());

                foreach (var p in currentState.Provinces)
                    p.State = null;

                currentState = state;

                foreach (var p in state.Provinces)
                    p.State = state;

                StateManager.AddState(state.Id.GetValue(), state);

                state.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                LoadData();

                var sb = new StringBuilder();
                //state.Save(sb); //TODO
                text = sb.ToString();
                RichTextBox_StateInfo.Text = text;

                MainForm.Instance?.TryInvokeActionOrLog(
                    () => MapManager.HandleMapMainLayerChange(true, MainForm.Instance.SelectedMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text),
                    (ex) => Logger.LogException(ex)
                );
            });
        }

        private void Button_Create_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => new CreateObjectForm(EnumCreateObjectType.STATE, true).ShowDialog());
    }
}
