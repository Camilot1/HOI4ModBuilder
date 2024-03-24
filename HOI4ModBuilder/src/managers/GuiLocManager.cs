﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace HOI4ModBuilder.src.utils
{
    class GuiLocManager
    {
        private static Dictionary<EnumLocKey, string> _loc = new Dictionary<EnumLocKey, string>();
        private static readonly string[] allowedLanguages = new string[] { "ru", "en" };

        private static readonly IDeserializer yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public static readonly Dictionary<Form, Action> formsReinitEvents = new Dictionary<Form, Action>();

        public static void Init(Settings settings)
        {
            if (settings.language == null) settings.language = GetCurrentParentLanguageName;
            if (!allowedLanguages.Contains(settings.language)) settings.language = "en";

            SetCurrentUICulture(settings.language);
        }

        public static string GetCurrentParentLanguageName => Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

        public static void SetCurrentUICulture(string language)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Logger.TryOrLog(() => LoadLocalizationFile(SettingsManager.settings.language));
            foreach (var actionPair in new Dictionary<Form, Action>(formsReinitEvents)) actionPair.Value();
        }

        public static string GetLoc(EnumLocKey key)
        {
            if (_loc.TryGetValue(key, out var value)) return value;
            else return key.ToString();
        }

        public static string GetLoc(EnumLocKey key, Dictionary<string, string> replaceValues, string additionalText)
        {
            if (_loc.TryGetValue(key, out var value))
            {
                if (replaceValues != null)
                    foreach (var valuePair in replaceValues) value = value.Replace(valuePair.Key, valuePair.Value);
                if (additionalText != null) value += additionalText;

                return value;
            }
            else return $"{key}: {Utils.DictionaryToString(replaceValues)}";
        }

        public static string GetLoc(EnumLocKey key, Dictionary<string, string> replaceValues)
        {
            return GetLoc(key, replaceValues, null);
        }

        private static void LoadLocalizationFile(string language)
        {
            string filePath = $"localization\\{language}.yml";
            if (!File.Exists(filePath))
                throw new Exception($"LOC_FILE_NOT_FOUND_ERROR: {filePath}");

            try
            {
                var tempDictionary = yamlDeserializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filePath));

                var transferDictionary = new Dictionary<string, EnumLocKey>();

                //Подготавливаем таблицу соотношения ("Имя ключа" : EnumLocKey)
                foreach (EnumLocKey enumLocKey in Enum.GetValues(typeof(EnumLocKey)))
                    transferDictionary[enumLocKey.ToString()] = enumLocKey;

                //Переносим ключи и их локализацию из файла в словарь локализации
                _loc = new Dictionary<EnumLocKey, string>();
                foreach (var pair in tempDictionary)
                {
                    if (transferDictionary.TryGetValue(pair.Key, out var enumKey))
                        _loc[enumKey] = pair.Value;
                }

                //Проверяем, каких ключек локализации не было в файле
                foreach (var locKey in transferDictionary.Keys)
                {
                    if (!tempDictionary.TryGetValue(locKey, out _))
                        Logger.LogWarning(
                            EnumLocKey.WARNING_LOCALIZATION_KEY_NOT_FOUND,
                            new Dictionary<string, string> {
                                { "{filePath}", filePath },
                                { "{locKey}", locKey }
                            }
                        );
                }
                //Выводим отсутствующие ключи локализации
                Logger.DisplayWarnings();
            }
            catch (Exception ex)
            {
                Logger.LogSingleMessage($"EXCEPTION: Can't load localization file: {filePath}\n\n{ex}");
            }
        }
    }

    public enum EnumLocKey
    {
        WARNING_LOCALIZATION_KEY_NOT_FOUND,

        PROGRESSBAR_LOADED,
        PROGRESSBAR_LOADING_FAILED,
        PROGRESSBAR_SAVED,
        PROGRESSBAR_UPDATED,

        ERROR_HAS_OCCURED,
        EXCEPTION_HAS_OCCURED,

        CANT_SAVE_BECAUSE_NO_DATA_WAS_LOADED,
        CANT_SAVE_BECAUSE_OF_LOADING_ERRORS_OR_EXCEPTIONS,
        CANT_SAVE_BECAUSE_ALREADY_SAVING_OR_LOADING,
        CANT_SAVE_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS,

        CANT_LOAD_BECAUSE_ALREADY_SAVING_OR_LOADING,
        CANT_LOAD_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS,

        FOUND_WARNINGS_FORM_TITLE,
        FOUND_WARNINGS_COUNT,

        FOUND_ERRORS_FORM_TITLE,
        FOUND_ERRORS_COUNT,

        FOUND_EXCEPTIONS_FORM_TITLE,
        FOUND_EXCEPTIONS_COUNT,

        MAP_TAB_PROGRESSBAR_LOADING_REPLACE_TAGS,
        MAP_TAB_PROGRESSBAR_LOADING_INFO_ARGS_BLOCKS,
        MAP_TAB_PROGRESSBAR_LOADING_BOOKMARKS,
        MAP_TAB_PROGRESSBAR_LOADING_TERRAINS,
        MAP_TAB_PROGRESSBAR_LOADING_CONTINENTS,
        MAP_TAB_PROGRESSBAR_LOADING_BUILDINGS,
        MAP_TAB_PROGRESSBAR_LOADING_COUNTRIES,
        MAP_TAB_PROGRESSBAR_LOADING_RESOURCES,
        MAP_TAB_PROGRESSBAR_LOADING_STATE_CATEGORIES,
        MAP_TAB_PROGRESSBAR_LOADING_IDEOLOGIES,
        MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_DEFINITION,
        MAP_TAB_PROGRESSBAR_LOADING_STATES,
        MAP_TAB_PROGRESSBAR_LOADING_REGIONS,
        MAP_TAB_PROGRESSBAR_LOADING_UNITS,
        MAP_TAB_PROGRESSBAR_LOADING_OOBS,
        MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS,
        MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_BORDERS_TEXTURE_MAP,
        MAP_TAB_PROGRESSBAR_LOADING_ADDITIONAL_MAP_LAYERS,
        MAP_TAB_PROGRESSBAR_LOADING_ADJACENCIES_DEFINITION,
        MAP_TAB_PROGRESSBAR_LOADING_SUPPLIES,
        MAP_TAB_PROGRESSBAR_PROVINCES_BORDERS_ASSEMBLE,
        MAP_TAB_PROGRESSBAR_STATES_BORDERS_ASSEMBLE,
        MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE,
        MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING,
        MAP_TAB_PROGRESSBAR_MAP_ADDITIONAL_TEXTURE_LOADING,
        MAP_TAB_PROGRESSBAR_MAP_ADDITIONAL_TEXTURE_LOADED,

        AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_PROVINCES_BMP_FILE,
        AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_DEFINITION_CSV_FILE,
        AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TITLE,
        AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TEXT,

        WARNING_COUNTRY_TAGS_PARSING_INCORRECT_LINE,
        WARNING_COUNTRY_TAGS_PARSING_INLINE_FILEPATH_FILE_DOESNT_EXISTS,
        ERROR_ADJACENCY_LOADING_INCORRECT_PARAMS_COUNT,
        ERROR_ADJACENCY_LOADING_INCORRECT_FIRST_PROVINCE_ID,
        ERROR_ADJACENCY_LOADING_FIRST_PROVINCE_DOESNT_EXISTS,
        ERROR_ADJACENCY_LOADING_INCORRECT_SECOND_PROVINCE_ID,
        ERROR_ADJACENCY_LOADING_SECOND_PROVINCE_DOESNT_EXISTS,
        ERROR_ADJACENCY_LOADING_INCORRECT_TYPE,
        ERROR_ADJACENCY_LOADING_INCORRECT_THIRD_PROVINCE_ID,
        ERROR_ADJACENCY_LOADING_THIRD_PROVINCE_DOESNT_EXISTS,
        ERROR_ADJACENCY_LOADING_ADJACENCY_RULE_DOESNT_EXISTS,
        ERROR_REPLACE_TAG_INCORRECT_STRUCTURE,
        ERROR_REPLACE_TAG_INNER_TAGS_LIST_IS_EMPTY,
        WARNING_REPLACE_TAG_INNER_TAGS_LIST_ISNT_INITIALIZED,
        STATE_NOT_FOUND_BY_ID,
        EXCEPTION_WHILE_TEXTURE_LOADING,
        FOLDER_BROWSER_DIALOG_CHOOSE_GAME_DIRECTORY_TITLE,
        SINGLE_MESSAGE_NO_HOI4EXE_FILE_IN_GAME_DIRECTORY,
        FOLDER_BROWSER_DIALOG_CHOOSE_DIRECTORY_IN_DOCUMENTS_TITLE,
        SINGLE_MESSAGE_NO_SAVEGAMES_FOULDER_IN_DIRECTORY_IN_DOCUMENTS,
        FOLDER_BROWSER_DIALOG_CHOOSE_DIRECTORY_OF_MOD_TITLE,
        SINGLE_MESSAGE_NO_MOD_DESCRIPTOR_IN_DIRECTORY_OF_MOD,
        SINGLE_MESSAGE_MULTIPLE_MOD_DESCRIPTOR_IN_DIRECTORY_OF_MOD,
        EXCEPTION_WHILE_COUNTRY_GRAPHICS_LOADING,
        EXCEPTION_WHILE_ADJACENCIES_LOADING,
        ERROR_ADJACENCY_RULE_LOADING_REQUIRED_PROVINCE_DOESNT_EXISTS,
        ERROR_ADJACENCY_RULE_LOADING_ICON_PROVINCE_DOESNT_EXISTS,
        ERROR_ADJACENCY_RULE_LOADING_ICON_OFFSET_INCORRECT_VALUES_COUNT,
        ERROR_ADJACENCY_RULE_LOADING_UNKNOWN_TOKEN,
        ERROR_ADJACENCY_RULE_RELATION_UNKNOWN_TOKEN,
        ERROR_PROVINCE_INCORRECT_PARAMS_COUNT,
        ERROR_PROVINCE_INCORRECT_ID_VALUE,
        ERROR_PROVINCE_INCORRECT_ID_SEQUENCE,
        ERROR_PROVINCE_INCORRECT_COLOR_VALUE,
        ERROR_PROVINCE_INCORRECT_TYPE_VALUE,
        ERROR_PROVINCE_INCORRECT_IS_COASTAL_VALUE,
        ERROR_PROVINCE_INCORRECT_TERRAIN_VALUE,
        ERROR_PROVINCE_INCORRECT_CONTINENT_ID_VALUE,
        ERROR_PROVINCE_DUPLICATE_ID,
        ERROR_PROVINCE_DUPLICATE_COLOR,
        ERROR_PROVINCE_INCORRECT_LINE,
        EXCEPTION_PROCESS_PROVINCES_PIXELS_WAS_BEFORE_PROCESS_DEFITION_FILE,
        SINGLE_MESSAGE_NEW_PROVINCE_WITH_COLOR_WAS_CREATED,
        WARNING_PROVINCE_HAS_ZERO_PIXELS,
        WARNING_PROVINCE_HAS_CENTER_OUTSIDE_THE_MAP,
        WARNING_STATE_HAS_ZERO_PIXELS,
        WARNING_STATE_HAS_CENTER_OUTSIDE_THE_MAP,
        WARNING_REGION_HAS_ZERO_PIXELS,
        WARNING_REGION_HAS_CENTER_OUTSIDE_THE_MAP,
        EXCEPTION_PROVINCE_MERGE_NULL_PROVINCES_OR_SAME_PROVINCES_IDS,
        EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_BUILDINGS,
        EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_RAILWAYS,
        EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_ADJACENCIES,
        EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_SUPPLY_HUB,
        EXCEPTION_PROVINCE_MERGE_NO_PROVINCE_FOR_VACANT_ID,
        EXCEPTION_PROVINCE_MERGE_NEXT_VACANT_PROVINCE_ID_IS_ZERO,
        EXCEPTION_PROVINCE_MERGE_PROVINCE_TO_REPLACE_NOT_FOUND,
        CHOOSE_ACTION,
        EXCEPTION_WHILE_SETTINGS_FILE_LOADING,
        EXCEPTION_MOD_DESCRIPTOR_FILE_NOT_FOUND,
        EXCEPTION_BOOKMARK_NOT_FOUND,
        EXCEPTION_WHILE_BOOKMARK_LOADING,
        EXCEPTION_INCORRECT_COUNTRY_COLOR_PARAMS_COUNT,
        EXCEPTION_INCORRECT_COUNTRY_COLOR_FORMAT,
        ERROR_STATE_INCORRECT_ID_VALUE,
        ERROR_STATE_DUPLICATE_ID,
        ERROR_STATE_INCORRECT_MANPOWER,
        ERROR_STATE_INCORRECT_RESOURCE_NAME,
        ERROR_STATE_DUPLICATE_RESOURCE_STATEMENT,
        ERROR_STATE_INCORRECT_RESOURCE_COUNT,
        ERROR_STATE_INCORRECT_PROVINCE_ID,
        ERROR_STATE_PROVINCE_NOT_FOUND,
        ERROR_STATE_INCORRECT_BUILDINGS_MAX_LEVEL_FACTOR_VALUE,
        ERROR_STATE_STATE_CATEGORY_NOT_FOUND,
        ERROR_STATE_INCORRECT_LOCAL_SUPPLIES_VALUE,
        ERROR_STATE_UNSUCCESSFUL_STATE_ID_PARSE_RESULT,
        ERROR_WHILE_REGION_LOADING,
        ERROR_MAP_FILE_NOT_FOUND,
        EXCEPTION_DATA_ARGS_BLOCK_DONT_HAVE_ALL_MANDATORY_BLOCKS,
        EXCEPTION_DATA_ARGS_BLOCK_HAS_MORE_UNIVERSAL_PARAMS_THAN_ALLOWED,
        EXCEPTION_DATA_ARGS_BLOCK_HAS_COMMON_AND_UNIVERSAL_PARAMS_BUT_THIS_NOT_ALLOWED,
        SINGLE_MESSAGE_SEARCH_POSITION_INCORRECT_SEPARATOR,
        SINGLE_MESSAGE_SEARCH_POSITION_INCORRECT_COORDS,
        EXCEPTION_MOD_DIRECTORY_NOT_FOUND,
        EXCEPTION_USING_CURRENT_MOD_SETTINGS_AND_HOI4MODBUILDER_DIRECTORY_NOT_FOUND,
        CANT_SAVE_BECAUSE_HOI4MODBUILDER_DIRECTORY_DOESNT_EXISTS,
        EXCEPTION_SAVING_PROCESS_ABORTED,
        EXCEPTION_DATA_ARGS_BLOCK_NOT_ALLOWED_TOKEN,
        EXCEPTION_DATA_ARGS_BLOCK_UNIVERSAL_PARAM_CANT_BE_A_BLOCK,
        EXCEPTION_DATA_ARGS_BLOCK_PARAM_HAS_UNSUPPORTED_DEMILITER,
        EXCEPTION_DATA_ARGS_BLOCK_CANT_PARSE_VALUE,
        EXCEPTION_DATA_ARGS_BLOCK_HAS_NO_UNIVERSAL_PARAMS_ALLOWED_VALUE_TYPES,
        EXCEPTION_DATA_ARGS_BLOCK_HAS_INFO_ARGS_BLOCK,
        EXCEPTION_DATA_ARGS_BLOCK_HAS_NO_ALLOWED_VALUE_TYPES,
        EXCEPTION_INFO_ARGS_BLOCKS_WITH_REPLACE_TAGS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
        EXCEPTION_INFO_ARGS_BLOCKS_WITH_REPLACE_TAGS_DUPLICATE_BLOCK_NAME_IN_FILE,
        EXCEPTION_INFO_ARGS_BLOCKS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
        EXCEPTION_INFO_ARGS_BLOCKS_DUPLICATE_BLOCK_NAME_IN_FILE,
        EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_DEFINED_MODIFIER_IN_FILE,
        EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
        EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_CATEGORY_IN_FILE,
        EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_VALUE_TYPE_IN_FILE,
        EXCEPTION_TEXTURE_FILE_HAS_NO_NAME,
        EXCEPTION_WHILE_BUILDING_LOADING,
        EXCEPTION_WHILE_IDEOLOGY_GROUP_MODIFIERS_LOADING,
        EXCEPTION_IDEOLOGY_GROUP_DUPLICATE_GROUP_NAME_INF_FILE,
        EXCEPTION_WHILE_STATE_CATEGORY_LOADING,
        EXCEPTION_STATE_CATEGORY_DUPLICATE_NAME_IN_FILE,
        EXCEPTION_COUNTRY_COLORS_INCORRECT_COLOR_FORMAT_IN_FILE,
        EXCEPTION_STATE_ID_UPDATE_VALUE_IS_USED,
        WARNING_REPLACE_TAG_DUPLICATE_IN_FILE,
        ERROR_STATE_HISTORY_OWNER_COUNTRY_NOT_FOUND,
        ERROR_STATE_HISTORY_CONTROLLER_COUNTRY_NOT_FOUND,
        ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PARAMS_COUNT,
        ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PROVINCE_ID_VALUE,
        ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PROVINCE_NOT_FOUND,
        ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_POINTS_VALUE,
        ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_TOKEN,
        ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_PROVINCE_ID,
        ERROR_STATE_HISTORY_BUILDINGS_PROVINCIAL_BUILDING_IN_STATE,
        ERROR_STATE_HISTORY_BUILDINGS_NOT_PROVINCIAL_BUILDING_IN_PROVINCE,
        ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_TOKEN_IN_PROVINCE,
        ERROR_ADJACENCY_LOADING_ADJACENCY_CANT_HAVE_NEGATIVE_AND_POSITIVE_PROVINCE_ID,
        EXCEPTION_CANT_EXECUTE_BLOCK_FUNCTIONS,
        EXCEPTION_WHILE_STATE_HISTORY_LOADING,
        EXCEPTION_WHILE_ADJACENCY_RULE_IS_DISABLED_BLOCK_LOADING,
        EXCEPTION_WHILE_ADJACENCY_RULE_RELATION_BLOCK_LOADING,
        EXCEPTION_WHILE_STATE_SAVING,
        ERROR_ADJACENCY_RULE_DUPLICATE_NAME,
        EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_24BPP,
        EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_SAME_SIZE,
        EXCEPTION_PROVINCE_INCORRECT_TYPE_ID,
        EXCEPTION_PROVINCE_ID_UPDATE_VALUE_IS_USED,
        EXCEPTION_PROVINCE_COLOR_UPDATE_VALUE_IS_USED,
        EXCEPTION_REGION_ID_UPDATE_VALUE_IS_USED,
        EXCEPTION_INCORRECT_DATE_PERIOD,
        EXCEPTION_WHILE_REGION_SAVING,
        ERROR_REGION_UNSUCCESSFUL_REGION_ID_PARSE_RESULT,
        EXCEPTION_WHILE_TEXTURE_LOADING_INCORRECT_TEXTURE_PIXEL_FORMAT,
        EXCEPTION_WHILE_TEXTURE_LOADING_INCORRECT_TEXTURE_PIXEL_FORMAT_WITH_FILEPATH,
        ERROR_WHILE_STATE_LOADING,
        PROGRESSBAR_SAVING_FAILED,
        ERROR_ADJACENCY_RULE_LOADING_ICON_OFFSET_INCORRECT_VALUE,
        ERROR_REPLACE_TAG_DUPLICATE_IN_FILE,
        ERROR_STATE_HISTORY_VICTORY_POINTS_PROVINCE_ALREADY_HAS_VICTORY_POINTS,
        WARNINGS_TRIED_TO_PAINT_WITH_UNKNOWN_PROVINCE_COLOR,
        ERROR_REGION_UNKNOWN_TOKEN,
        ERROR_REGION_INCORRECT_NAVAL_TERRAIN_VALUE,
        ERROR_REGION_INCORRECT_PROVINCE_ID,
        ERROR_REGION_PROVINCE_NOT_FOUND,
        ERROR_REGION_WEATHER_UNKNOWN_TOKEN,
        ERROR_REGION_WEATHER_PERIOD_UNKNOWN_TOKEN,
        TOOL_CAN_JOIN_RAILWAYS_BUT_THEY_HAS_DIFFERENT_LEVELS_SHOULD_I_COUNTINUE,
        MERGE_PROVINCES_TOOL_SECOND_AND_LAST_PROVINCES_HAVE_VICTORY_POINTS,
        MERGE_PROVINCES_TOOL_SECOND_PROVINCE_HAS_VICTORY_POINTS,
        MERGE_PROVINCES_TOOL_LAST_PROVINCE_HAS_VICTORY_POINTS,
        FORM_STRATEGIC_REGION_DATA_RECOVERY_DIRECTORY_PATH_LABEL,
        FORM_STRATEGIC_REGION_DATA_RECOVERY_LOADED_REGION_COUNT_LABEL,
        FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTRED_REGION_COUNT_LABEL,
        FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTRED_REGION_THAT_DONT_EXISTS_COUNT_LABEL,
        EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_FILTER_INCORRECT_RANGE,
        EXCEPTION_FORM_STRATEGIC_REGION_DATA_RECOVERY_CANT_EXECUTE_BECAUSE_NO_DATA_WAS_LOADED,
        FOUND_ADDITIONAL_EXCEPTIONS_FORM_TITLE,
        FOUND_ADDITIONAL_EXCEPTIONS_COUNT,
        ERROR_REGION_DUPLICATE_ID,
        ERROR_MULTI_REGIONS_IN_FILE,
        ERROR_REGION_HAS_NO_FILE,
        SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_FILTER_HELP,
        SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_RECOVERY_RESULT,
        SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_LOAD_RESULT,
        SINGLE_MESSAGE_STRATEGIC_REGION_DATA_RECOVERY_FILTER_RESULT,
        INFO_MESSAGE,
        ERROR_WHILE_SUB_UNITS_LOADING,
        ERROR_WHILE_SUB_UNIT_LOADING,
        ERROR_UNIT_DUPLICATE_NAME,
        ERROR_WHILE_DIVISION_NAMES_GROUP_LOADING,
        ERROR_DIVISION_NAMES_GROUP_DUPLICATE_NAME,
        EXCEPTION_COORDINATES_UNKNOWN_TOKEN,
        ERROR_DIVISION_NAMES_GROUP_NOT_FOUND,
        EXCEPTION_WHILE_BLOCK_LOADING,
        ERROR_DIVISION_TEMPLATE_UNKNOWN_TOKEN,
        SUB_UNIT_NOT_FOUND,
        ERROR_DIVISION_TEMPLATE_INCORRECT_SUB_UNIT_COORDINATES,
        ERROR_DIVISION_TEMPLATE_SUB_UNITS_COORDINATES_OVERLAPPING,
        ERROR_DIVISION_NAME_INCORRECT_TOKEN,
        ERROR_DIVISION_PROVINCE_NOT_FOUND,
        WARNING_WHILE_DIVISION_VALIDATION_PARAMETER_VALUE_IS_OUT_OF_RANGE_IN_FILE,
        ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_NAME_NOR_DIVISION_NAME,
        ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NAME_AND_DIVISION_NAME,
        ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_LOCATION,
        ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_DIVISION_TEMPLATE,

        UNKNOWN_TOKEN,
        LAYERED_LEVELS_PARAMETER_VALUE_OVERRIDDEN,
        LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN,
        COUNTRY_NOT_FOUND,
        PROVINCE_NOT_FOUND,
        INCORRECT_VALUE,
        DIVISION_NAMES_GROUP_NOT_FOUND,
        BLOCK_DOESNT_HAVE_MANDATORY_INNER_PARAMETER,
        SUB_UNITS_COORDINATES_OVERLAPPING,
        ERROR_LAYERED_PREFIX,
        WARNING_LAYERED_PREFIX,
        BLOCK_HAS_SEVERAL_MUTUALLY_EXCLUSIVE_MANDATORY_INNER_PARAMETERS,
        BLOCK_DOESNT_HAVE_ANY_OF_SEVERAL_MUTUALLY_EXCLUSIVE_MANDATORY_INNER_PARAMETERS,
        PARAMETER_VALUE_IS_OUT_OF_RANGE,
        BLOCK_MUST_HAVE_AT_LEAST_ONE_MANDATORY_PARAMETER,
        EXCEPTION_COUNTRY_TAG_UPDATE_VALUE_IS_USED,
        DIVISION_TEMPLATE_MUST_HAVE_AT_LEAST_ONE_REGIMENT_OR_SUPPORT_SUB_UNIT,
        FLEET_MUST_HAVE_AT_LEAST_ONE_TASK_FORCE,
        TASK_FORCE_MUST_HAVE_AT_LEAST_ONE_SHIP,
        MAP_TAB_PROGRESSBAR_LOADING_DIVISION_NAMES_GROUPS,
        SUB_UNIT_DUPLICATE_DEFINITION_IN_CURRENT_FILE,
        SUB_UNIT_DUPLICATE_DEFINITION,
        TASK_FORCE_SHIP_NOT_FOUND,
        TASK_FORCES_USES_SHIP_THAT_DOESNT_DEFINED_ANYWHERE,
        EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_EFFECT_IN_FILE,
        EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
        EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_TRIGGER_IN_FILE,
        EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE
    }
}
