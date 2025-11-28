using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    public class MapToolStrategicLocations : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STRATEGIC_LOCATION;

        public MapToolStrategicLocations(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { EnumMainLayer.STRATEGIC_LOCATIONS },
                  new HotKey
                  {
                      key = Keys.L,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool isHandlingMouseMove() => true;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES
        };
        public override Func<ICollection> GetParametersProvider()
            => () => new List<string>
            {
                GuiLocManager.GetLoc(EnumLocKey.PROVINCE),
                GuiLocManager.GetLoc(EnumLocKey.STATE),
            };
        public override Func<ICollection> GetParameterValuesProvider()
            => () => StrategicLocationManager.GetNamesSortedStartingWith("");

        // TODO: Refactor. Was made in a hurry
        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            /*
            StrategicLocationManager.TryGet(parameter, out var targetStrategicLocation);

            if (!ProvinceManager.TryGet(MapManager.GetColor(pos), out Province province))
                return false;

            if (province.State == null)
                return false;

            if (province.State.CurrentHistory == null)
                return false;

            bool provinceMode = GuiLocManager.GetLoc(EnumLocKey.PROVINCE) == parameter;
            bool stateMode = GuiLocManager.GetLoc(EnumLocKey.STATE) == parameter;

            if (!provinceMode && !stateMode)
                return false;

            var dict = provinceMode ? province.State.provinceStrategicLocations : province.State.stateStrategicLocations;
            var blockTag = provinceMode ? "strategic_province_location" : "strategic_state_location";
            var contains = false;

            if (dict.TryGetValue(province, out var list))
                foreach (var obj in list)
                    if (obj.Name == value)
                    {
                        contains = true;
                        break;
                    }

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (contains)
                    return false;

                int lastResultIndex = -1;
                var blocks = province.State.CurrentHistory.DynamicScriptBlocks;
                for (int i = blocks.Count - 1; i >= 0; i--)
                {
                    var block = blocks[i];
                    if (block.ScriptBlockInfo.GetBlockName() == blockTag)
                    {
                        lastResultIndex = i;
                        break;
                    }
                }

                Action<StateHistory, StrategicLocation> redoAction = null;
                Action<StateHistory, StrategicLocation> undoAction = null;

                // Если подходящих эффектов не было
                if (lastResultIndex == -1)
                {
                    redoAction = (stateHistory, location) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.HasAny(
                            (block) => block.ScriptBlockInfo.GetBlockName() == blockTag &&
                                block.GetValue() is GameList<ScriptBlockParseObject> innerList &&
                                innerList.HasAny(
                                    innerBlock => innerBlock.ScriptBlockInfo.GetBlockName() == location.Name
                                )
                         ))
                            return;

                        if (!stateHistory.DynamicScriptBlocks.TryGetLast(block => block.ScriptBlockInfo.GetBlockName() == blockTag, out var lastBlock))
                        {
                            lastBlock = ParserUtils.GetScriptBlockParseObject(
                                stateHistory.DynamicScriptBlocks, blockTag, new GameList<ScriptBlockParseObject>()
                            );
                            stateHistory.DynamicScriptBlocks.Add(lastBlock);
                        }

                        if (lastBlock.TryAddUniversalParams(new List<(string, dataObjects.argBlocks.EnumValueType, object)>
                        {
                            (value, dataObjects.argBlocks.EnumValueType.PROVINCE, province)
                        }))

                            province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.RemoveLastIf(
                            (block) =>
                                block.ScriptBlockInfo.GetBlockName() == "add_claim_by" &&
                                block.GetValue() == targetCountry
                        );
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                }
                else if (
                    blocks[lastResultIndex].ScriptBlockInfo.GetBlockName() == "remove_claim_by" &&
                    blocks[lastResultIndex].GetValue() == targetCountry)
                {
                    redoAction = (stateHistory, country) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.Count <= lastResultIndex)
                            return;

                        var block = stateHistory.DynamicScriptBlocks[lastResultIndex];
                        if (block.ScriptBlockInfo.GetBlockName() != "remove_claim_by" ||
                            block.GetValue() != country)
                            return;

                        stateHistory.DynamicScriptBlocks.RemoveAt(lastResultIndex);
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "remove_claim_by", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                }

                if (redoAction == null || undoAction == null)
                    return false;

                MapManager.ActionsBatch.AddWithExecute(
                    () => redoAction(province.State.CurrentHistory, targetCountry),
                    () => undoAction(province.State.CurrentHistory, targetCountry)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                //Если нет core_of на текущий момент
                if (!province.State.CurrentClaimsBy.Contains(targetCountry))
                    return false;

                // Удаляем все лишние add_claim_by или remove_claim_by
                var blocks = province.State.CurrentHistory.DynamicScriptBlocks;
                blocks.RemoveAllExceptLast(
                    (block) => // Ищем эффекты, связанные с выбранной страной
                    {
                        var blockName = block.ScriptBlockInfo.GetBlockName();
                        if (blockName != "add_claim_by" && blockName != "remove_claim_by")
                            return false;

                        return block.GetValue() == targetCountry;
                    },
                    out int matchCount,
                    out int lastResultIndex
                );

                Action<StateHistory, Country> redoAction = null;
                Action<StateHistory, Country> undoAction = null;

                // Если подходящих эффектов не было
                if (lastResultIndex == -1)
                {
                    redoAction = (stateHistory, country) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.HasAny(
                            (block) =>
                                block.ScriptBlockInfo.GetBlockName() == "remove_claim_by" &&
                                block.GetValue() == country)
                        )
                            return;

                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "remove_claim_by", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.RemoveLastIf(
                            (block) =>
                                block.ScriptBlockInfo.GetBlockName() == "remove_claim_by" &&
                                block.GetValue() == targetCountry
                        );
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                }
                else if (
                    blocks[lastResultIndex].ScriptBlockInfo.GetBlockName() == "add_claim_by" &&
                    blocks[lastResultIndex].GetValue() == targetCountry)
                {
                    redoAction = (stateHistory, country) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.Count <= lastResultIndex)
                            return;

                        var block = stateHistory.DynamicScriptBlocks[lastResultIndex];
                        if (block.ScriptBlockInfo.GetBlockName() != "add_claim_by" ||
                            block.GetValue() != country)
                            return;

                        stateHistory.DynamicScriptBlocks.RemoveAt(lastResultIndex);
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "add_claim_by", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false);
                    };
                }

                if (redoAction == null || undoAction == null)
                    return false;

                MapManager.ActionsBatch.AddWithExecute(
                    () => redoAction(province.State.CurrentHistory, targetCountry),
                    () => undoAction(province.State.CurrentHistory, targetCountry)
                );
            }
            */

            return true;
        }
    }
}
