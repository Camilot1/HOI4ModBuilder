using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static HOI4ModBuilder.src.tools.autotools.AutoToolRegenerateProvincesColors;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{
    public class StateManager
    {
        public static StateManager Instance { get; private set; }

        public static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "history", "states" });
        private static Dictionary<ushort, State> _statesById = new Dictionary<ushort, State>();

        public static void ForEachState(Action<State> action)
        {
            foreach (var s in _statesById.Values)
                action(s);
        }

        public static long GetSumManpower()
        {
            long sum = 0;
            foreach (var state in _statesById.Values)
                sum += state.CurrentManpower;
            return sum;
        }

        public static HashSet<State> GroupSelectedStates { get; private set; } = new HashSet<State>();
        public static Point2F GetGroupSelectedStatesCenter()
        {
            var commonCenter = new CommonCenter();
            foreach (var obj in GroupSelectedStates)
                commonCenter.Push(obj.pixelsCount, obj.center);
            commonCenter.Get(out var _, out var center);
            return center;
        }
        public static State SelectedState { get; set; }
        public static State RMBState { get; set; }

        private static HashSet<ProvinceBorder> _statesBorders = new HashSet<ProvinceBorder>();

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(BaseSettings settings)
        {
            Instance = new StateManager();

            Deselect();

            _statesById = new Dictionary<ushort, State>();
            _statesBorders = new HashSet<ProvinceBorder>();

            var fileInfosPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            var actions = new List<(string, Action)>(fileInfosPairs.Count);
            var addActions = new ConcurrentQueue<Action>();

            foreach (var fileInfo in fileInfosPairs.Values)
                actions.Add((null, () =>
                {
                    LoadFile(new GameParser(), new StateGameFile(fileInfo), out var addAction);
                    addActions.Enqueue(addAction);
                }
                ));

            MainForm.ExecuteActionsParallelNoDisplay(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_STATES, actions);
            //foreach (var action in actions) //DEBUG
            //    action.Item2();
            foreach (var addAction in addActions)
                addAction();
            foreach (var state in _statesById.Values)
                state.Validate(out var _);
        }

        public static void LoadFile(GameParser parser, StateGameFile stateFile)
        {
            LoadFile(parser, stateFile, out var addAction);
            addAction?.Invoke();
        }
        private static void LoadFile(GameParser parser, StateGameFile stateFile, out Action addAction)
        {
            addAction = () => { };
            try
            {
                parser.ParseFile(stateFile);

                var state = stateFile.State.GetValue();
                if (state == null)
                    return;

                addAction = () =>
                {
                    if (!_statesById.ContainsKey(state.Id.GetValue()))
                        _statesById[state.Id.GetValue()] = state;
                    else Logger.LogError(
                            EnumLocKey.ERROR_STATE_DUPLICATE_ID,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", state.GetGameFile().FileInfo?.filePath },
                                { "{stateId}", $"{state.Id.GetValue()}" },
                                { "{otherFilePath}", _statesById[state.Id.GetValue()].GetGameFile().FileInfo?.filePath }
                            }
                        );
                };
            }
            catch (Exception ex)
            {
                int id = 0;

                var state = stateFile.State.GetValue();
                if (state != null)
                    id = state.Id.GetValue();

                string idString = id == 0 ?
                    GuiLocManager.GetLoc(EnumLocKey.ERROR_STATE_UNSUCCESSFUL_STATE_ID_PARSE_RESULT) : $"{id}";
                Logger.LogExceptionAsError(
                    EnumLocKey.ERROR_WHILE_STATE_LOADING,
                    new Dictionary<string, string>
                    {
                            { "{stateId}", idString },
                            { "{parserCursorInfo}", $"{parser.GetCursorInfo()}" },
                            { "{filePath}", stateFile.FilePath }
                    },
                    ex
                );
            }
        }

        public static void SaveAll(BaseSettings settings)
        {
            var actions = new List<(string, Action)>(_statesById.Count);
            foreach (var state in _statesById.Values)
                actions.Add((null, () => Save(settings, state)));

            ParallelUtils.Execute(actions);
            //foreach (var actionEntry in actions) //DEBUG
            //    actionEntry.Item2();
        }

        public static void Save(BaseSettings settings, State state)
        {
            var file = (StateGameFile)state.GetParent().GetParent();

            if (file.IsNeedToSave())
            {
                var sb = new StringBuilder();
                Logger.TryOrCatch(
                    () => file.Save(sb, "", null, default),
                    (ex) =>
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_STATE_SAVING,
                            new Dictionary<string, string> { { "{stateId}", $"{state.Id.GetValue()}" } }
                        ), ex)
                );

                File.WriteAllText(settings.modDirectory + FOLDER_PATH + file.FileInfo.fileName, sb.ToString());
            }

            if (file.FileInfo.needToDelete)
                File.Delete(settings.modDirectory + FOLDER_PATH + file.FileInfo.fileName);
        }

        public static void Draw(bool showCenters, bool showCollisions)
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

            if (GroupSelectedStates != null && GroupSelectedStates.Count > 0)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(8f);

                foreach (var state in GroupSelectedStates)
                {
                    foreach (var border in state.borders)
                    {
                        if (border.pixels.Length == 1)
                            continue;

                        if (
                            border.provinceA.State != null &&
                            border.provinceA.State.Id.GetValue() == state.Id.GetValue() &&
                            GroupSelectedStates.Contains(border.provinceB.State)
                            ||
                            border.provinceB.State != null &&
                            border.provinceB.State.Id.GetValue() == state.Id.GetValue() &&
                            GroupSelectedStates.Contains(border.provinceA.State))
                            continue;

                        GL.Begin(PrimitiveType.LineStrip);
                        foreach (Value2S vertex in border.pixels)
                        {
                            GL.Vertex2(vertex.x, vertex.y);
                        }
                        GL.End();
                    }
                }
            }

            if (SelectedState != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedState.borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2S vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }

                if (showCollisions)
                {
                    GL.Color4(0f, 0f, 1f, 1f);
                    GL.LineWidth(3f);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Vertex2(SelectedState.bounds.left, SelectedState.bounds.top);
                    GL.Vertex2(SelectedState.bounds.right + 1, SelectedState.bounds.top);
                    GL.Vertex2(SelectedState.bounds.right + 1, SelectedState.bounds.bottom + 1);
                    GL.Vertex2(SelectedState.bounds.left, SelectedState.bounds.bottom + 1);
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
                    foreach (Value2S vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }
            }
        }

        public static void RegenerateColors(ProgressCallback progressCallback)
        {
            var stopwatch = Stopwatch.StartNew();
            var modSettings = SettingsManager.Settings.GetModSettings();

            var newColors = new HashSet<int>(512);
            var adjacentColors = new List<int>(16);

            var regeneratedStatesToColor = new Dictionary<State, int>(512);
            var hsvRanges = modSettings.GetStateHSVRanges();

            var states = AssembleStates(GetValues());
            int counter = 0;
            foreach (var state in states)
            {
                counter++;
                progressCallback?.Execute(counter, states.Count);

                adjacentColors.Clear();

                int newColor;
                foreach (var borderState in state.GetBorderStates())
                {
                    if (!regeneratedStatesToColor.TryGetValue(borderState, out newColor))
                        continue;
                    adjacentColors.Add(newColor);
                }

                newColor = ColorUtils.GenerateDistinctColor(
                    new Random(state.Id.GetValue()), adjacentColors, hsvRanges, c => !newColors.Contains(c)
                );

                regeneratedStatesToColor[state] = newColor;
                state.Color = newColor;
                newColors.Add(newColor);
            }
            Logger.Log($"RegenerateStatesColors = {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void AddBorder(ProvinceBorder border)
            => _statesBorders.Add(border);

        public static bool TransferProvince(Province province, State src, State dest)
        {
            if (province == null || src == null && dest == null)
                return false;
            if (src != null && dest != null && src.Equals(dest))
                return false;
            if (province != null && dest != null && province.State == dest)
                return false;

            src?.RemoveProvince(province);
            dest?.AddProvince(province);

            if (src == null || dest == null)
                return true;

            TransferProvinceHistory(province, src.History.GetValue(), dest.History.GetValue());

            var srcInnerHistories = src.History.GetValue().InnerHistories;
            var destInnerHistories = dest.History.GetValue().InnerHistories;

            foreach (DateTime dateStamp in srcInnerHistories.Keys)
            {
                var srcHistory = srcInnerHistories[dateStamp];
                bool tempDestHistory = true;
                if (!destInnerHistories.TryGetValue(dateStamp, out StateHistory destHistory))
                {
                    destHistory = new StateHistory(dest, dateStamp);
                    tempDestHistory = true;
                }

                if (TransferProvinceHistory(province, srcHistory, destHistory))
                {
                    if (tempDestHistory)
                        destInnerHistories[dateStamp] = destHistory;
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

            VictoryPoint victoryPoint = null;

            foreach (var vp in src.VictoryPoints)
            {
                if (vp.province == province)
                {
                    victoryPoint = vp;
                    break;
                }
            }

            if (victoryPoint != null)
            {
                src.VictoryPoints.Remove(victoryPoint);
                dest.VictoryPoints.Add(victoryPoint);
                result = true;
            }

            if (src.TryGetProvinceBuildings(province, out var provinceBuildings))
            {
                dest.SetProvinceBuildings(province, provinceBuildings);
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
            min = int.MaxValue;
            max = int.MinValue;

            foreach (var state in _statesById.Values)
            {
                if (state.CurrentManpower > max)
                    max = state.CurrentManpower;
                if (state.CurrentManpower < min)
                    min = state.CurrentManpower;
            }
        }

        public static void GetMinMaxWeightedManpower(out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (var state in _statesById.Values)
            {
                double weightedManpower = state.CurrentManpower / (double)state.pixelsCount;
                if (weightedManpower > max)
                    max = weightedManpower;
                if (weightedManpower < min)
                    min = weightedManpower;
            }
        }

        public static void GetMinMaxResourceCount(Resource resource, out uint min, out uint max)
        {
            min = uint.MaxValue;
            max = uint.MinValue;

            foreach (var state in _statesById.Values)
            {
                var count = state.GetResourceCount(resource);
                if (count > max)
                    max = count;
                if (count < min)
                    min = count;
            }
        }

        public static void GetMinMaxWeightedResourceCount(Resource resource, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (var state in _statesById.Values)
            {
                var count = state.GetResourceCount(resource);
                double weightedCount = count / (double)state.pixelsCount;
                if (weightedCount > max)
                    max = weightedCount;
                if (weightedCount < min)
                    min = weightedCount;
            }
        }

        public static bool TryAdd(string fileName, State state)
        {
            if (!_statesById.ContainsKey(state.Id.GetValue()))
            {
                _statesById[state.Id.GetValue()] = state;
                state.SetNeedToSave(true);
                return true;
            }
            else return false;
        }

        public static bool Contains(ushort id)
            => _statesById.ContainsKey(id);
        public static Dictionary<ushort, State>.KeyCollection GetIDs()
            => _statesById.Keys;
        public static List<ushort> GetIDsSorted()
        {
            var list = new List<ushort>(GetIDs());
            list.Sort();
            return list;
        }
        public static Dictionary<ushort, State>.ValueCollection GetValues()
            => _statesById.Values;

        public static bool TryGet(ushort id, out State state)
            => _statesById.TryGetValue(id, out state);
        public static State Get(ushort id)
            => TryGet(id, out var state) ? state : null;

        public static void Add(ushort id, State state)
        {
            _statesById[id] = state;
            state.SetNeedToSave(true);
        }

        public static void Remove(ushort id)
        {
            var state = _statesById[id];
            if (state != null)
            {
                _statesById.Remove(id);
                state.SetNeedToSave(true);
            }
        }

        public static ushort GetMaxID()
        {
            var list = new List<ushort>(_statesById.Keys);
            list.Sort();
            return list[list.Count - 1];
        }


        public static void InitBorders()
        {
            _statesBorders = new HashSet<ProvinceBorder>(0);
            foreach (var state in _statesById.Values)
                state.InitBorders();
            TextureManager.InitStateBordersMap(_statesBorders);
        }

        public static void CalculateCenters()
        {
            foreach (var state in _statesById.Values)
                state.CalculateCenter();
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape() => Deselect();

        public static void Deselect()
        {
            GroupSelectedStates.Clear();
            SelectedState = null;
            RMBState = null;
        }

        public static void Select(ushort[] ids)
        {
            GroupSelectedStates.Clear();

            if (ids.Length == 1)
            {
                if (TryGet(ids[0], out var state))
                    SelectedState = state;
            }

            foreach (var id in ids)
            {
                if (!TryGet(id, out var state))
                    continue;

                GroupSelectedStates.Add(state);
            }
        }

        public static State Select(int color)
        {
            if (ProvinceManager.TryGet(color, out Province province) && province.State != null)
            {
                if (MainForm.Instance.IsShiftPressed())
                {
                    if (SelectedState != null)
                        GroupSelectedStates.Add(SelectedState);

                    if (GroupSelectedStates.Contains(province.State))
                        GroupSelectedStates.Remove(province.State);
                    else
                        GroupSelectedStates.Add(province.State);

                    SelectedState = null;
                    return province.State;
                }
                else
                {
                    GroupSelectedStates.Clear();
                    SelectedState = province.State;
                }
            }
            else
            {
                SelectedState = null;
                GroupSelectedStates.Clear();
            }

            ProvinceManager.Deselect();
            StrategicRegionManager.Deselect();
            return SelectedState;
        }

        public static State SelectRMB(int color)
        {
            if (ProvinceManager.TryGet(color, out Province province) && province.State != null)
                RMBState = province.State;
            else
                RMBState = null;

            ProvinceManager.Deselect();
            StrategicRegionManager.Deselect();
            return RMBState;
        }

        public static void RemoveProvinceData(Province province)
        {
            if (province == null)
                return;

            foreach (var state in _statesById.Values)
                state.RemoveProvinceData(province);
        }
    }
}
