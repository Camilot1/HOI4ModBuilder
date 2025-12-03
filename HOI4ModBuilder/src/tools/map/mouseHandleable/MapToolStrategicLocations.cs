using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
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

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (!StrategicLocationManager.TryGet(value, out var targetStrategicLocation))
                return false;

            if (!ProvinceManager.TryGet(MapManager.GetColor(pos), out Province province))
                return false;

            var state = province.State;
            if (state == null)
                return false;

            if (state.CurrentHistory == null)
                return false;

            var history = state.History.GetValue();
            if (history == null) 
                return false;

            bool provinceMode = GuiLocManager.GetLoc(EnumLocKey.PROVINCE) == parameter;
            bool stateMode = GuiLocManager.GetLoc(EnumLocKey.STATE) == parameter;


            if (!provinceMode && !stateMode)
                return false;

            var dict = provinceMode ? province.State.provinceStrategicLocations : province.State.stateStrategicLocations;
            var blockTag = provinceMode ? "strategic_province_location" : "strategic_state_location";

            bool contains = dict.TryGetValue(province, out var tempList) && tempList.Contains(targetStrategicLocation);


            Action addAction = () =>
            {
                var blocks = state.CurrentHistory.DynamicScriptBlocks;
                if (!blocks.TryGetLast(b => b.GetBlockName() == blockTag, out var targetBlock))
                {
                    targetBlock = ParserUtils.GetScriptBlockParseObject(blocks, blockTag, new GameList<ScriptBlockParseObject>());
                    blocks.Add(targetBlock);
                }

                targetBlock.TryAddUniversalParams(true, new List<(string, EnumValueType, object)> {
                    (targetStrategicLocation.Name, EnumValueType.PROVINCE, province)
                });

                MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.STRATEGIC_LOCATIONS, province);

                province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                MapManager.HandleMapMainLayerChange(false);
            };

            Action removeAction = () =>
            {
                var targetParameter = new ScriptBlockParseObject[1];

                state.ForEachHistory((h) =>
                {
                    if (h.dateTime > DataManager.currentDateStamp[0])
                        return true;

                    foreach (var b in h.DynamicScriptBlocks)
                    {
                        if (b.GetBlockName() != blockTag)
                            continue;

                        var v = b.GetValue();
                        if (!(v is GameList<ScriptBlockParseObject> vList))
                            continue;

                        vList.RemoveAllIf(o => o.GetBlockName() == targetStrategicLocation.Name && o.GetValue() == province);
                    }

                    return false;
                });

                MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.STRATEGIC_LOCATIONS, province);

                province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                MapManager.HandleMapMainLayerChange(false);
            };


            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (contains)
                    return false;


                MapManager.ActionsBatch.AddWithExecute(
                    () => addAction(),
                    () => removeAction()
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                if (!contains)
                    return false;


                MapManager.ActionsBatch.AddWithExecute(
                    () => removeAction(),
                    () => addAction()
                );
            }

            return true;
        }
    }
}
