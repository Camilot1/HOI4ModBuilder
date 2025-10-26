﻿using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.hoiDataObjects.common.bookmarks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.ideologies;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames;
using HOI4ModBuilder.src.hoiDataObjects.common.units.equipment;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.data.exceptions;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace HOI4ModBuilder.managers
{
    class DataManager
    {
        public static DateTime[] currentDateStamp;

        public static void SaveAll()
        {
            Logger.TryOrLog(() =>
                {
                    if (!CanStartSave())
                        return;

                    MainForm.IsLoadingOrSaving = true;
                    SaveAllData(SettingsManager.Settings);
                }, FinishSave);
        }

        public static void LoadAll()
        {
            if (!CanStartLoad())
                return;

            MainForm.IsFirstLoaded = true;
            MainForm.IsLoadingOrSaving = true;

            Logger.Log("Loading...");

            if (MainForm.Instance.GLControl.Context == null)
            {
                Logger.LogSingleErrorMessage("Can't load data. glControl.Context == null");
                MainForm.IsLoadingOrSaving = false;
                return;
            }

            MainForm.PauseGLControl();
            MainForm.Instance.SetGroupBoxProgressBackColor(Color.White);

            Task.Run(() =>
            {
                Logger.TryOrLog(
                    () =>
                    {
                        var context = new GraphicsContext(GraphicsMode.Default, MainForm.Instance.GLControl.WindowInfo);
                        context.MakeCurrent(MainForm.Instance.GLControl.WindowInfo);

                        Logger.CloseAllTextBoxMessageForms();

                        var stopwatch = Stopwatch.StartNew();
                        LoadAllData(SettingsManager.Settings);
                        stopwatch.Stop();
                        Logger.Log("Loading time: " + stopwatch.ElapsedMilliseconds + " ms");

                        context.MakeCurrent(null);
                    },
                    () => MainForm.Instance.TryInvokeActionOrLog(
                        FinishLoad,
                        ex =>
                        {
                            Logger.LogException(ex);
                            FinishLoad();
                        })
                );
            });
        }
        public static void UpdateAll()
            => Logger.TryOrLog(() =>
            {
                if (!CanUpdate())
                    return;

                MainForm.IsLoadingOrSaving = true;
                SavePattern.LoadAll();
                MapManager.UpdateMapInfo();
            }, () => MainForm.IsLoadingOrSaving = false);

        private static void SaveAllData(BaseSettings settings)
        {
            MainForm.IsMapMainLayerChangeEnabled = false;

            Logger.Log("Saving...");

            LocalModDataManager.SaveLocalSettings(settings);
            DataManager.Save(settings);
            TextureManager.SaveAllMaps(settings);

            Utils.CleanUpMemory();

            MainForm.IsMapMainLayerChangeEnabled = true;
        }

        private static void LoadAllData(BaseSettings settings)
        {
            SettingsManager.Settings.LoadModDescriptors();

            LocalModDataManager.Load(settings);
            SavePattern.LoadAll();

            MainForm.IsMapMainLayerChangeEnabled = false;

            MapManager.ActionHistory.Clear();

            //Logger.Log($"Выполняю загрузку шрифтов");
            //FontManager.LoadFonts();

            Logger.Log($"Loading mod directory: {SettingsManager.Settings.modDirectory}");

            TextRenderManager.Instance.Load();
            DataManager.Load(SettingsManager.Settings);
            MapManager.Load(SettingsManager.Settings);

            MapPositionsManager.Load(settings);

            WarningsManager.Init();
            ErrorManager.Init();

            MainForm.IsMapMainLayerChangeEnabled = true;
        }

        public static void OnBookmarkChange()
        {
            Logger.TryOrLog(() =>
            {
                string[] value = MainForm.Instance.ToolStripComboBox_Data_Bookmark.Text.Split(']');
                if (value.Length <= 1)
                    return;

                string dateTimeString = value[0].Replace('[', ' ').Trim();

                Logger.TryOrCatch(() =>
                {
                    if (!Utils.TryParseDateTimeStamp(dateTimeString, out DateTime dateTime))
                        throw new BookmarkNotFoundException(dateTimeString);

                    currentDateStamp = new DateTime[] { dateTime };
                    CountryManager.UpdateByDateTimeStamp(dateTime);
                    StateManager.UpdateByDateTimeStamp(dateTime);

                    if (!MainForm.IsLoadingOrSaving)
                        MapManager.HandleMapMainLayerChange(true, MainForm.Instance.SelectedMainLayer, MainForm.Instance.GetParameter());
                }, ex => throw new BookmarkLoadingException(dateTimeString, ex));
            });
        }

        private static void Load(BaseSettings settings)
            => LoadManagers(settings);

        private static void LoadManagers(BaseSettings settings)
        {
            var stopwatch = Stopwatch.StartNew();
            currentDateStamp = null;

            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_REPLACE_TAGS, () => ReplaceTagsManager.Load(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_INFO_ARGS_BLOCKS, () => InfoArgsBlocksManager.Load(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_BOOKMARKS, () => BookmarkManager.Load(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TERRAINS, () => TerrainManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_CONTINENTS, () => ContinentManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_COUNTRIES, () => CountryManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_BUILDINGS, () => BuildingManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_BUILDINGS, () => InfoArgsBlocksManager.LoadBuildingsCustomArgsBlocks()),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_RESOURCES, () => ResourceManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_STATE_CATEGORIES, () => StateCategoryManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_IDEOLOGIES, () => IdeologyManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_DEFINITION, () => ProvinceManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_STATES, () => StateManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_REGIONS, () => StrategicRegionManager.Load(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_AI_AREAS, () => AiAreaManager.Load(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_UNITS, () => SubUnitManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_DIVISION_NAMES_GROUPS, () => DivisionNamesGroupManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_OOBS, () => OOBManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_EQUPMENTS, () => EquipmentManager.Load(settings)),
                //new LocalizedAction(EnumLocKey.NONE, () => LocalizationManager.Load(settings)),
            };

            MainForm.ExecuteActions(actions);

            stopwatch.Stop();
            Console.WriteLine("Загрузка DataManager = " + stopwatch.ElapsedMilliseconds + " ms.");
        }

        public static void Save(BaseSettings settings)
            => SaveManagers(settings);
        private static void SaveManagers(BaseSettings settings)
        {
            var stopwatch = Stopwatch.StartNew();

            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_PROVINCES_DEFINITION, () => ProvinceManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_ADJACENCIES, () => AdjacenciesManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_SUPPLIES, () => SupplyManager.SaveAll(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_STATES, () => StateManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_REGIONS, () => StrategicRegionManager.Save(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_AI_AREAS, () => AiAreaManager.Save(settings)),

                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_UNITS, () => SubUnitManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_DIVISION_NAMES_GROUPS, () => DivisionNamesGroupManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_OOBS, () => OOBManager.Save(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_EQUPMENTS, () => EquipmentManager.Save(settings)),
                //new LocalizedAction(EnumLocKey.NONE, () => LocalizationManager.Save(settings)),
            };

            MainForm.ExecuteActions(actions);

            stopwatch.Stop();
            Console.WriteLine("Загрузка DataManager = " + stopwatch.ElapsedMilliseconds + " ms.");
        }

        private static bool CanStartSave()
        {
            if (!MainForm.IsFirstLoaded)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_SAVE_BECAUSE_NO_DATA_WAS_LOADED);
            if (MainForm.ErrorsOrExceptionsDuringLoading)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_SAVE_BECAUSE_OF_LOADING_ERRORS_OR_EXCEPTIONS);
            if (MainForm.IsLoadingOrSaving)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_SAVE_BECAUSE_ALREADY_SAVING_OR_LOADING);
            if (!SettingsManager.Settings.IsModDirectorySelected())
                return LogErrorAndReturnFalse(EnumLocKey.CANT_SAVE_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS);
            return true;
        }

        private static bool CanStartLoad()
        {
            if (MainForm.IsLoadingOrSaving)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_LOAD_BECAUSE_ALREADY_SAVING_OR_LOADING);
            if (!SettingsManager.Settings.IsModDirectorySelected())
                return LogErrorAndReturnFalse(EnumLocKey.CANT_LOAD_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS);
            return true;
        }

        private static bool CanUpdate()
        {
            if (!MainForm.IsFirstLoaded)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_UPDATE_BECAUSE_NO_DATA_WAS_LOADED);
            if (MainForm.IsLoadingOrSaving)
                return LogErrorAndReturnFalse(EnumLocKey.CANT_UPDATE_BECAUSE_ALREADY_SAVING_OR_LOADING);
            return true;
        }

        private static bool LogErrorAndReturnFalse(EnumLocKey messageKey)
        {
            Logger.LogSingleErrorMessage(messageKey);
            return false;
        }

        private static void FinishSave()
        {
            var success = Logger.ExceptionsCount == 0;

            MainForm.DisplayProgress(
                success ? EnumLocKey.PROGRESSBAR_SAVED : EnumLocKey.PROGRESSBAR_SAVING_FAILED,
                BuildProgressSummary(includeTime: true),
                0
            );

            MainForm.Instance.SetGroupBoxProgressBackColor(success ? ResolveStatusColor() : Color.Red);

            MapManager.HandleMapMainLayerChange(true, MainForm.Instance.SelectedMainLayer, MainForm.Instance.GetParameter());

            AfterFinish();
        }

        private static void FinishLoad()
        {
            var success = Logger.ExceptionsCount == 0;

            MainForm.DisplayProgress(
                success ? EnumLocKey.PROGRESSBAR_LOADED : EnumLocKey.PROGRESSBAR_LOADING_FAILED,
                BuildProgressSummary(includeTime: false),
                0
            );

            MainForm.Instance.SetGroupBoxProgressBackColor(success ? ResolveStatusColor() : Color.Red);

            MainForm.ResumeGLControl();
            if (MainForm.Instance.ToolStripComboBox_Data_Bookmark.Items.Count > 0)
                MainForm.Instance.ToolStripComboBox_Data_Bookmark.SelectedIndex = 0;

            MainForm.Instance.UpdateSelectedMainLayerAndTool(true, false);
            MainForm.Instance.UpdateBordersType();

            MainForm.ErrorsOrExceptionsDuringLoading = Logger.ErrorsCount > 0 || Logger.ExceptionsCount > 0;
            AfterFinish();
        }

        private static void AfterFinish()
        {
            Logger.DisplayWarnings();
            Logger.DisplayErrors();
            Logger.DisplayExceptions();
            Utils.CleanUpMemory();
            MainForm.IsLoadingOrSaving = false;
        }

        private static Dictionary<string, string> BuildProgressSummary(bool includeTime)
        {
            var summary = new Dictionary<string, string>
            {
                { "{warningsCount}", $"{Logger.WarningsCount}" },
                { "{errorsCount}", $"{Logger.ErrorsCount}" },
                { "{exceptionsCount}", $"{Logger.ExceptionsCount}" },
            };

            if (includeTime)
                summary["{time}"] = DateTime.Now.ToLongTimeString();

            return summary;
        }

        private static Color ResolveStatusColor()
        {
            if (Logger.ErrorsCount > 0)
                return Color.OrangeRed;
            if (Logger.WarningsCount > 0)
                return Color.Yellow;
            return Color.White;
        }
    }
}
