using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.newParser;
using System.Collections;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    public class MapToolStateCoreOf : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_CORE_OF;

        public MapToolStateCoreOf(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool isHandlingMouseMove() => true;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.STATES
        };
        public override Func<ICollection> GetParametersProvider()
            => () => CountryManager.GetTagsSorted();
        public override Func<ICollection> GetValuesProvider() => null;

        // TODO: Refactor. Was made in a hurry
        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            CountryManager.TryGet(parameter, out var targetCountry);

            if (!ProvinceManager.TryGet(MapManager.GetColor(pos), out Province province))
                return false;

            if (province.State == null)
                return false;

            if (province.State.CurrentHistory == null)
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                //Если уже есть core_of на текущий момент
                if (province.State.CurrentCoresOf.Contains(targetCountry))
                    return false;

                // Удаляем все лишние add_core_of или remove_core_of
                var blocks = province.State.CurrentHistory.DynamicScriptBlocks;
                blocks.RemoveAllExceptLast(
                    (block) => // Ищем эффекты, связанные с выбранной страной
                    {
                        var blockName = block.ScriptBlockInfo.GetBlockName();
                        if (blockName != "add_core_of" && blockName != "remove_core_of")
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
                                block.ScriptBlockInfo.GetBlockName() == "add_core_of" &&
                                block.GetValue() == country)
                        )
                            return;

                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "add_core_of", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.RemoveLastIf(
                            (block) =>
                                block.ScriptBlockInfo.GetBlockName() == "add_core_of" &&
                                block.GetValue() == targetCountry
                        );
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                }
                else if (
                    blocks[lastResultIndex].ScriptBlockInfo.GetBlockName() == "remove_core_of" &&
                    blocks[lastResultIndex].GetValue() == targetCountry)
                {
                    redoAction = (stateHistory, country) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.Count <= lastResultIndex)
                            return;

                        var block = stateHistory.DynamicScriptBlocks[lastResultIndex];
                        if (block.ScriptBlockInfo.GetBlockName() != "remove_core_of" ||
                            block.GetValue() != country)
                            return;

                        stateHistory.DynamicScriptBlocks.RemoveAt(lastResultIndex);
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "remove_core_of", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
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
                if (!province.State.CurrentCoresOf.Contains(targetCountry))
                    return false;

                // Удаляем все лишние add_core_of или remove_core_of
                var blocks = province.State.CurrentHistory.DynamicScriptBlocks;
                blocks.RemoveAllExceptLast(
                    (block) => // Ищем эффекты, связанные с выбранной страной
                    {
                        var blockName = block.ScriptBlockInfo.GetBlockName();
                        if (blockName != "add_core_of" && blockName != "remove_core_of")
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
                                block.ScriptBlockInfo.GetBlockName() == "remove_core_of" &&
                                block.GetValue() == country)
                        )
                            return;

                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "remove_core_of", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.RemoveLastIf(
                            (block) =>
                                block.ScriptBlockInfo.GetBlockName() == "remove_core_of" &&
                                block.GetValue() == targetCountry
                        );
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                }
                else if (
                    blocks[lastResultIndex].ScriptBlockInfo.GetBlockName() == "add_core_of" &&
                    blocks[lastResultIndex].GetValue() == targetCountry)
                {
                    redoAction = (stateHistory, country) =>
                    {
                        if (stateHistory.DynamicScriptBlocks.Count <= lastResultIndex)
                            return;

                        var block = stateHistory.DynamicScriptBlocks[lastResultIndex];
                        if (block.ScriptBlockInfo.GetBlockName() != "add_core_of" ||
                            block.GetValue() != country)
                            return;

                        stateHistory.DynamicScriptBlocks.RemoveAt(lastResultIndex);
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                    undoAction = (stateHistory, country) =>
                    {
                        stateHistory.DynamicScriptBlocks.Add(ParserUtils.GetScriptBlockParseObject(
                            stateHistory, "add_core_of", country
                        ));
                        province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                    };
                }

                if (redoAction == null || undoAction == null)
                    return false;

                MapManager.ActionsBatch.AddWithExecute(
                    () => redoAction(province.State.CurrentHistory, targetCountry),
                    () => undoAction(province.State.CurrentHistory, targetCountry)
                );
            }

            return true;
        }
    }
}
