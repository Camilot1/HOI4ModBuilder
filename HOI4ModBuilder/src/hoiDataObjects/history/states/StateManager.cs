using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{
    class StateManager : IParadoxRead
    {
        public static StateManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "history", "states" });
        private static FileInfo _currentFile;
        private static Dictionary<ushort, State> _statesById = new Dictionary<ushort, State>();
        public static void ForEachState(Action<State> action)
        {
            foreach (var s in _statesById.Values) action(s);
        }

        public static State SelectedState { get; set; }
        public static State RMBState { get; set; }

        private static HashSet<ProvinceBorder> _statesBorders = new HashSet<ProvinceBorder>();

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            Instance = new StateManager();
            _statesById = new Dictionary<ushort, State>();
            _statesBorders = new HashSet<ProvinceBorder>();

            var fileInfosPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            Logger.Log("Loading of States started");
            foreach (var fileInfo in fileInfosPairs.Values)
            {
                _currentFile = fileInfo;
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);
            }
            Logger.Log("Loading of States finished");
        }

        public static void Save(Settings settings)
        {
            var sb = new StringBuilder();

            foreach (var state in _statesById.Values)
            {
                if (state.fileInfo.needToSave)
                {
                    Logger.TryOrCatch(
                        () => state.Save(sb),
                        (ex) =>
                            throw new Exception(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_WHILE_STATE_SAVING,
                                new Dictionary<string, string> { { "{stateId}", $"{state.Id}" } }
                            ), ex)
                    );

                    File.WriteAllText(settings.modDirectory + FOLDER_PATH + state.fileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }

                if (state.fileInfo.needToDelete)
                    File.Delete(settings.modDirectory + FOLDER_PATH + state.fileInfo.fileName);
            }
        }

        public static void Draw(bool showCenters)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var state in _statesById.Values)
                {
                    if (state.dislayCenter) GL.Vertex2(state.center.x, state.center.y);
                }
                GL.End();
            }

            if (SelectedState != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedState.borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }
            }

            if (RMBState != null)
            {
                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(2.5f);

                foreach (var border in RMBState.borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }
            }
        }

        public static void AddStatesBorder(ProvinceBorder border) => _statesBorders.Add(border);

        public static bool TransferProvince(Province province, State src, State dest)
        {
            if (province == null || src == null && dest == null) return false;
            if (src != null && dest != null && src.Equals(dest)) return false;
            if (province != null && dest != null && province.State == dest) return false;

            src?.RemoveProvince(province);
            dest?.AddProvince(province);

            if (src == null || dest == null) return true;

            TransferProvinceHistory(province, src.startHistory, dest.startHistory);

            foreach (DateTime dateStamp in src.stateHistories.Keys)
            {
                var srcHistory = src.stateHistories[dateStamp];
                bool tempDestHistory = true;
                if (!dest.stateHistories.TryGetValue(dateStamp, out StateHistory destHistory))
                {
                    destHistory = new StateHistory(dateStamp, dest);
                    tempDestHistory = true;
                }

                if (TransferProvinceHistory(province, srcHistory, destHistory))
                {
                    if (tempDestHistory) dest.stateHistories[dateStamp] = destHistory;
                    if (!srcHistory.HasAnyData()) src.stateHistories.Remove(dateStamp);
                }
            }

            if (DataManager.currentDateStamp != null)
            {
                src.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                dest.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
            }

            return true;
        }

        private static bool TransferProvinceHistory(Province province, StateHistory src, StateHistory dest)
        {
            bool result = false;

            if (src.victoryPoints.TryGetValue(province, out uint value))
            {
                dest.victoryPoints[province] = value;
                src.victoryPoints.Remove(province);
                result = true;
            }

            if (src.TryGetProvinceBuildings(province, out Dictionary<Building, uint> dictionary))
            {
                dest.SetProvinceBuildings(province, dictionary);
                src.RemoveProvinceBuildings(province);
                result = true;
            }
            return result;
        }


        public static void UpdateByDateTimeStamp(DateTime dateTime)
        {
            foreach (var state in _statesById.Values) state.UpdateByDateTimeStamp(dateTime);
        }

        public static void GetMinMaxManpower(out int min, out int max)
        {
            min = 0;
            max = 0;

            foreach (var state in _statesById.Values)
            {
                min = state.manpower;
                max = state.manpower;
                break;
            }

            foreach (var state in _statesById.Values)
            {
                if (state.manpower > max) max = state.manpower;
                else if (state.manpower < min) min = state.manpower;
            }
        }

        public static bool TryAddState(string fileName, State state)
        {
            if (!_statesById.ContainsKey(state.Id))
            {
                _statesById[state.Id] = state;
                state.fileInfo.needToSave = true;
                return true;
            }
            else return false;
        }

        public static bool ContainsStateIdKey(ushort id) => _statesById.ContainsKey(id);
        public static Dictionary<ushort, State>.KeyCollection GetStatesIds() => _statesById.Keys;
        public static Dictionary<ushort, State>.ValueCollection GetStates() => _statesById.Values;

        public static bool TryGetState(ushort id, out State state) => _statesById.TryGetValue(id, out state);

        public static void AddState(ushort id, State state)
        {
            _statesById[id] = state;
            state.fileInfo.needToSave = true;
        }

        public static void RemoveState(ushort id)
        {
            var state = _statesById[id];
            if (state != null)
            {
                _statesById.Remove(id);
                state.fileInfo.needToSave = true;
            }
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "state")
            {
                var state = new State { fileInfo = _currentFile };

                try
                {
                    parser.Parse(state);

                    foreach (var p in state.provinces) p.State = state;

                    if (!_statesById.ContainsKey(state.Id)) _statesById[state.Id] = state;
                    else Logger.LogError(
                            EnumLocKey.ERROR_STATE_DUPLICATE_ID,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", state.fileInfo?.filePath },
                                { "{stateId}", $"{state.Id}" },
                                { "{otherFilePath}", _statesById[state.Id].fileInfo?.filePath }
                            }
                        );
                }
                catch (Exception ex)
                {
                    string idString = state.Id == 0 ? GuiLocManager.GetLoc(EnumLocKey.ERROR_STATE_UNSUCCESSFUL_STATE_ID_PARSE_RESULT) : $"{state.Id}";
                    Logger.LogExceptionAsError(
                        EnumLocKey.ERROR_WHILE_STATE_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", idString },
                            { "{filePath}", _currentFile.filePath }
                        },
                        ex
                    );
                }
            }
        }

        public static void InitStatesBorders()
        {
            _statesBorders = new HashSet<ProvinceBorder>(0);
            foreach (var state in _statesById.Values) state.InitBorders();
            TextureManager.InitStateBordersMap(_statesBorders);
        }

        public static void CalculateCenters()
        {
            foreach (var state in _statesById.Values) state.CalculateCenter();
        }

        public static State SelectState(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.State != null) SelectedState = province.State;
            else SelectedState = null;

            ProvinceManager.SelectedProvince = null;
            StrategicRegionManager.SelectedRegion = null;
            return SelectedState;
        }

        public static State SelectRMBState(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.State != null) RMBState = province.State;
            else RMBState = null;

            ProvinceManager.RMBProvince = null;
            StrategicRegionManager.RMBRegion = null;
            return RMBState;
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape()
        {
            SelectedState = null;
            RMBState = null;
        }
    }
}
