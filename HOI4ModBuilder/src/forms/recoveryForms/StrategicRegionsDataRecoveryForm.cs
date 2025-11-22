using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.dataRecovery;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms.recoveryForms
{
    public partial class StrategicRegionsDataRecoveryForm : Form
    {
        public static StrategicRegionsDataRecoveryForm instance;
        public static string text = null;

        private static string _directoryPath = "";
        private static Dictionary<ushort, StrategicRegion> _oldRegions = new Dictionary<ushort, StrategicRegion>();
        private static Dictionary<ushort, StrategicRegion> _oldFilteredRegions = new Dictionary<ushort, StrategicRegion>();

        private static string _filterText = "";

        private static readonly TransferInfo transferInfo = new TransferInfo();

        public StrategicRegionsDataRecoveryForm()
        {
            InitializeComponent();
            instance?.Invoke((MethodInvoker)delegate { instance?.Close(); });
            instance = this;
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
            instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
        {
            Logger.TryOrLog(() => DisplayData());
        }

        private void DisplayData()
        {
            Label_OldDirectoryPath.Text = GuiLocManager.GetLoc(
                EnumLocKey.FORM_STRATEGIC_REGION_DATA_RECOVERY_DIRECTORY_PATH_LABEL,
                new Dictionary<string, string> { { "{directoryPath}", _directoryPath } }
            );

            TextBox_OldRegionsIds.Text = GetIdsRanges();

            Label_FoundOldRegionsCount.Text = GuiLocManager.GetLoc(
                EnumLocKey.FORM_STRATEGIC_REGION_DATA_RECOVERY_LOADED_REGION_COUNT_LABEL,
                new Dictionary<string, string> { { "{count}", $"{_oldRegions.Count}" } }
            );

            TextBox_IdsFilter.Text = _filterText;

            Label_FilteredOldRegionsCount.Text = GuiLocManager.GetLoc(
                EnumLocKey.FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTRED_REGION_COUNT_LABEL,
                new Dictionary<string, string> { { "{count}", $"{_oldFilteredRegions.Count}" } }
            );

            Label_FilteredOldRegionsThatDontExistCount.Text = GuiLocManager.GetLoc(
                EnumLocKey.FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTRED_REGION_THAT_DONT_EXISTS_COUNT_LABEL,
                new Dictionary<string, string> { { "{count}", $"{transferInfo.NotExistingRegionsCount}" } }
            );

            CheckBox_TransferFilesIfRegionIdNotFound.Checked = transferInfo.TransferFilesIfRegionIdNotFound;

            for (int i = 0; i < CheckedListBox_ParamsToRecover.Items.Count; i++)
            {
                var items = CheckedListBox_ParamsToRecover.Items;
                switch (items[i])
                {
                    case "name": CheckedListBox_ParamsToRecover.SetSelected(i, transferInfo.Name); break;
                    case "naval_terrain": CheckedListBox_ParamsToRecover.SetSelected(i, transferInfo.NavalTerrain); break;
                    case "provinces": CheckedListBox_ParamsToRecover.SetSelected(i, transferInfo.Provinces); break;
                    case "static_modifiers": CheckedListBox_ParamsToRecover.SetSelected(i, transferInfo.StaticModifiers); break;
                    case "weather": CheckedListBox_ParamsToRecover.SetSelected(i, transferInfo.Weather); break;
                    default:
                        throw new Exception($"Unknown parameter \"{items[i]}\"");
                }
            }
        }

        private static string GetIdsRanges()
        {
            //Пустой диапазон
            if (_oldRegions.Count == 0)
                return "";

            //Диапазон из 1 id
            if (_oldRegions.Count == 1)
                return $"{_oldFilteredRegions.First().Key}";

            List<ushort> regionsIds = new List<ushort>(_oldRegions.Count);
            regionsIds.AddRange(_oldRegions.Keys);

            regionsIds.Sort();

            ushort min = regionsIds[0];
            ushort current = min;

            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < regionsIds.Count; i++)
            {
                ushort regionId = regionsIds[i];
                if (current + 1 == regionId) current++;
                else
                {
                    if (min == current) sb.Append(min).Append(';');
                    else sb.Append(min).Append('-').Append(current).Append(';');

                    min = regionId;
                    current = regionId;
                }
            }

            if (min == current) sb.Append(min);
            else sb.Append(min).Append('-').Append(current);

            return sb.ToString();
        }

        private void Button_ChooseOldRegionsDirectory_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!MainForm.IsFirstLoaded)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_CANT_EXECUTE_BECAUSE_NO_DATA_WAS_LOADED
                    ));

                var thread = new Thread(() =>
                {
                    var dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "recovery", "strategic_regions" });
                    var fd = Utils.PrepareFolderDialog(dialogPath);
                    if (fd.ShowDialog() != DialogResult.OK) return;

                    Invoke(new Action(() =>
                    {
                        _directoryPath = fd.SelectedPath;
                        LoadOldData();
                        FilterOldData();
                        Utils.CleanUpMemory();
                    }));
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            });
        }

        private void Button_LoadOldRegionsDirectory_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                LoadOldData();
                FilterOldData();
                Utils.CleanUpMemory();
            });
        }

        private void Button_FilterHelp_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var text = GuiLocManager.GetLoc(
                    EnumLocKey.SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_FILTER_HELP,
                    new Dictionary<string, string> {
                        { "{exampleFilter}", "1-3;4;7" },
                        { "{exampleIds}", "1, 2, 3, 4, 7" }
                    }
                );
                MessageBoxUtils.ShowInfoMessage(text, MessageBoxButtons.OK);
                Focus();
            });
        }

        private void Button_FilterById_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                FilterOldData();
                Utils.CleanUpMemory();
            });
        }

        private void LoadOldData()
        {
            if (string.IsNullOrEmpty(_directoryPath) || !Directory.Exists(_directoryPath)) return;

            _oldRegions = new Dictionary<ushort, StrategicRegion>();

            var fileInfoPairs = FileManager.ReadFileInfos(_directoryPath, FileManager.TXT_FORMAT, false);
            foreach (var filePair in fileInfoPairs)
            { //Проходим в цикле по файлам со старыми данными
                Logger.TryOrCatch(
                    () =>
                    {
                        using (var fs = new FileStream(filePair.Value.filePath, FileMode.Open))
                            ParadoxParser.Parse(fs, new StrategicRegionFile(true, filePair.Value, _oldRegions));
                    },
                    (ex) => Logger.LogAdditionalException(ex)
                );
            }

            Logger.DisplayAdditionalExceptions();

            var text = GuiLocManager.GetLoc(
                EnumLocKey.SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_LOAD_RESULT,
                new Dictionary<string, string> {
                { "{count}", $"{_oldRegions.Count}" },
                { "{allCount}", $"{fileInfoPairs.Count}" }
                }
            );
            MessageBoxUtils.ShowInfoMessage(text, MessageBoxButtons.OK);
            Focus();
        }

        private void FilterOldData()
        {
            _oldFilteredRegions = new Dictionary<ushort, StrategicRegion>();

            var idRanges = GetIdRanges(_filterText);

            foreach (var oldRegion in _oldRegions.Values)
            {
                if (Check(idRanges, oldRegion.Id))
                    _oldFilteredRegions[oldRegion.Id] = oldRegion;
            }

            var NotExistingRegions = GetNotExistingRegions();
            transferInfo.NotExistingRegionsCount = NotExistingRegions.Count;

            DisplayData();

            var text = GuiLocManager.GetLoc(
                EnumLocKey.SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_FILTER_RESULT,
                new Dictionary<string, string> {
                { "{count}", $"{_oldFilteredRegions.Count}" },
                { "{allCount}", $"{_oldRegions.Count}" }
                }
            );

            MessageBoxUtils.ShowInfoMessage(text, MessageBoxButtons.OK);

            Focus();

            bool Check(List<ushort[]> ranges, ushort regionId)
            {
                if (ranges.Count == 0) return true;

                foreach (var range in ranges)
                {
                    if (range[0] <= regionId && regionId <= range[1]) return true;
                }

                return false;
            }
        }

        private void TransferOldData()
        {
            Logger.TryOrLog(() =>
            {
                int successCounter = 0;
                foreach (var oldFilteredRegion in _oldFilteredRegions.Values)
                    Logger.TryOrCatch(
                        () =>
                        {
                            bool result = StrategicRegionsDataRecoveryTool.TransferRegionInfo(transferInfo, oldFilteredRegion);
                            if (result) successCounter++;
                        },
                        (ex) => Logger.LogAdditionalException(ex)
                    );

                var text = GuiLocManager.GetLoc(
                    EnumLocKey.SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_RECOVERY_RESULT,
                    new Dictionary<string, string> {
                        { "{count}", $"{successCounter}" },
                        { "{allCount}", $"{_oldFilteredRegions.Count}" }
                    }
                );
                MessageBoxUtils.ShowInfoMessage(text, MessageBoxButtons.OK);

                Focus();
            });
        }

        private static List<StrategicRegion> GetNotExistingRegions()
        {
            List<StrategicRegion> regions = new List<StrategicRegion>();

            foreach (var pair in _oldFilteredRegions)
            {
                if (!StrategicRegionManager.Contains(pair.Key)) regions.Add(pair.Value);
            }

            return regions;
        }

        private static List<ushort[]> GetIdRanges(string filter)
        {
            string[] ranges = filter.Split(';');
            List<ushort[]> list = new List<ushort[]>();

            foreach (var range in ranges)
            {
                if (range == "") continue;

                string[] data = range.Split('-');
                if (data.Length > 2)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTER_INCORRECT_RANGE,
                        new Dictionary<string, string> { { "{value}", range } }
                    ));

                if (data.Length == 1) data = new string[] { data[0], data[0] };

                if (ushort.TryParse(data[0], out ushort min) && ushort.TryParse(data[1], out ushort max))
                    list.Add(new ushort[] { min, max });
                else
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTER_INCORRECT_RANGE,
                        new Dictionary<string, string> { { "{value}", range } }
                    ));
            }

            return list;
        }

        private void Button_ExecuteRecovery_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!MainForm.IsFirstLoaded)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_CANT_EXECUTE_BECAUSE_NO_DATA_WAS_LOADED
                    ));

                LoadOldData();
                FilterOldData();
                TransferOldData();
                Utils.CleanUpMemory();
            });
        }

        private void CheckedListBox_ParamsToRecover_MouseUp(object sender, MouseEventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                for (int i = 0; i < CheckedListBox_ParamsToRecover.Items.Count; i++)
                {
                    var items = CheckedListBox_ParamsToRecover.Items;
                    var value = CheckedListBox_ParamsToRecover.GetSelected(i);
                    switch (items[i])
                    {
                        case "name": transferInfo.Name = value; break;
                        case "naval_terrain": transferInfo.NavalTerrain = value; break;
                        case "provinces": transferInfo.Provinces = value; break;
                        case "static_modifiers": transferInfo.StaticModifiers = value; break;
                        case "weather": transferInfo.Weather = value; break;
                        default:
                            throw new Exception($"Unknown parameter \"{items[i]}\"");
                    }
                }
            });
        }

        private void CheckBox_TransferFilesIfRegionIdNotFound_Click(object sender, EventArgs e)
            => transferInfo.TransferFilesIfRegionIdNotFound = CheckBox_TransferFilesIfRegionIdNotFound.Checked;

        private void TextBox_IdsFilter_TextChanged(object sender, EventArgs e)
            => _filterText = TextBox_IdsFilter.Text;
    }

    class TransferInfo
    {
        public bool HasAnyTransferInfo
            => TransferFilesIfRegionIdNotFound && NotExistingRegionsCount > 0 ||
                Name || NavalTerrain || Provinces || StaticModifiers || Weather;

        public int NotExistingRegionsCount { get; set; }
        public bool TransferFilesIfRegionIdNotFound { get; set; }

        public bool Name { get; set; }
        public bool NavalTerrain { get; set; }
        public bool Provinces { get; set; }
        public bool StaticModifiers { get; set; }
        public bool Weather { get; set; }

        public bool HasAnyTransferFlag => Name | NavalTerrain | Provinces | StaticModifiers | Weather;
    }
}
