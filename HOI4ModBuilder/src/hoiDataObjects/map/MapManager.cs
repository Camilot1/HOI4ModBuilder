using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.src.managers.ActionHistoryManager;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.tools.brushes;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using HOI4ModBuilder.src.scripts;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System.Collections.Concurrent;
using System.Linq;
using HOI4ModBuilder.src.hoiDataObjects.map.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using OpenTK;
using System.Numerics;

namespace HOI4ModBuilder.managers
{
    class MapManager
    {
        private static bool _isMapDragged = false;
        public static Value2I MapSize { get; private set; }
        private static Point2D _mousePrevPoint;
        private static Point2D _mapSizeFactor;
        public static Point2D MapSizeFactor { get => _mapSizeFactor; private set => _mapSizeFactor = value; }
        private static Point2D _pointerSize = new Point2D { x = 1, y = 1 };
        private static EnumMouseState _mouseState = EnumMouseState.NONE;
        public static Bounds4US bounds;

        public static int[] ProvincesPixels { get; set; } //RGBA
        public static byte[] HeightsPixels { get; set; }

        public static TexturedPlane MapMainLayer, BordersMapPlane, RiversMapPlane;
        public static List<TextureInfo> additionalMapTextures = new List<TextureInfo>();
        public static TexturedPlane selectedTexturedPlane;
        public static Shader sdfTextShader;
        public static bool showSelectZone;
        public static bool blackBorders;
        public static bool showMainLayer;

        public static bool[] displayLayers = new bool[Enum.GetValues(typeof(EnumAdditionalLayers)).Length];

        public static double zoomFactor = 0.0004f;
        public static double mapDifX, mapDifY;

        public static bool[] isHandlingMapMainLayerChange = new bool[1];
        public static bool[] isWaitingHandlingMapMainLayerChange = new bool[1];

        public static ActionHistoryManager ActionHistory { get; private set; }
        public static ActionsBatch ActionsBatch { get; private set; }

        public static void Init()
        {
            ActionHistory = new ActionHistoryManager(EnumTabPage.MAP);
            ActionsBatch = ActionHistory.CreateNewActionBatch();

            MapToolsManager.Init();
            SupplyManager.Init();
            AdjacenciesManager.Init();
            ProvinceManager.Init();
            StateManager.Init();
            StrategicRegionManager.Init();

            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            TextureManager.LoadTextures(settings);

            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS, () => LoadTextureMaps()),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_ADJACENCIES_DEFINITION, () => AdjacenciesManager.Load(settings)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_SUPPLIES, () => SupplyManager.Load(settings))
            };

            MainForm.ExecuteActions(actions);

            ProvinceBorderManager.Init(ProvincesPixels, (short)MapSize.x, (short)MapSize.y);

            showMainLayer = true;
        }

        private static void LoadTextureMaps()
        {
            MapSize = TextureManager.provinces.texture.Size;
            MapMainLayer = new TexturedPlane(TextureManager.provinces.texture, MapSize.x, MapSize.y);
            BordersMapPlane = new TexturedPlane(TextureManager.provincesBorders.texture, MapSize.x, MapSize.y);
            RiversMapPlane = new TexturedPlane(TextureManager.rivers.texture, MapSize.x, MapSize.y);

            if (mapDifX == 0 && mapDifY == 0)
            {
                mapDifX = -MapMainLayer.size.x / 2f;
                mapDifY = MapMainLayer.size.y / 2f;
            }
        }

        public static void FocusOn(Point2F point) => FocusOn(point.x, point.y);

        public static void FocusOn(float x, float y)
        {
            mapDifX = -x;
            mapDifY = MapSize.y - y;
        }

        public static void Draw()
        {
            if (MapMainLayer == null) return;

            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(zoomFactor, zoomFactor, zoomFactor);
            GL.Translate(mapDifX, -mapDifY, 0); // перемещение

            if (showMainLayer) MapMainLayer.Draw();

            if (displayLayers[(int)EnumAdditionalLayers.BORDERS])
            {
                GL.Translate(0.5f, -0.5f, 0f);
                if (blackBorders) GL.Color3(0f, 0f, 0f);
                BordersMapPlane.Draw();
                GL.Color3(1f, 1f, 1f);
                GL.Translate(-0.5f, 0.5f, 0f);
            }

            if (displayLayers[(int)EnumAdditionalLayers.RIVERS])
                RiversMapPlane.Draw();

            GL.Scale(1f, -1f, 1f);
            GL.Translate(0, -MapMainLayer.size.y, 0);

            ProvinceManager.Draw(displayLayers[(int)EnumAdditionalLayers.CENTERS] & (MainForm.Instance.enumMainLayer == EnumMainLayer.PROVINCES_MAP));
            StateManager.Draw(displayLayers[(int)EnumAdditionalLayers.CENTERS] & (MainForm.Instance.enumMainLayer == EnumMainLayer.STATES));
            StrategicRegionManager.Draw(displayLayers[(int)EnumAdditionalLayers.CENTERS] & (MainForm.Instance.enumMainLayer == EnumMainLayer.STRATEGIC_REGIONS));
            AdjacenciesManager.Draw(displayLayers[(int)EnumAdditionalLayers.ADJACENCIES], displayLayers[(int)EnumAdditionalLayers.ADJACENCIES]);
            SupplyManager.Draw(displayLayers[(int)EnumAdditionalLayers.RAILWAYS], displayLayers[(int)EnumAdditionalLayers.SUPPLY_HUBS]);

            DrawPointer();
            if (bounds.HasSpace())
                DrawBounds();

            GL.Scale(1f, -1f, 1f);
            GL.Color3(1f, 1f, 1f);
            GL.PointSize(5f);
            if (displayLayers[(int)EnumAdditionalLayers.OVERLAY_TEXTURES])
            {
                foreach (TextureInfo info in additionalMapTextures)
                {
                    SegmentedTexturedPlane plane = info.plane;
                    GL.Translate(plane.pos.x, -plane.pos.y, 0f);
                    plane.Draw();
                    GL.Translate(-plane.pos.x, plane.pos.y, 0f);
                }
            }

            GL.Scale(1f, -1f, 1f);
            if (displayLayers[(int)EnumAdditionalLayers.WARNINGS])
                WarningsManager.Instance.Draw();
            if (displayLayers[(int)EnumAdditionalLayers.ERRORS])
                ErrorManager.Instance.Draw();
            //DrawTest();

            MapPositionsManager.Draw();

            var _projection = Matrix4.CreateOrthographicOffCenter(
                MainForm.Instance.viewportInfo.x,
                -MainForm.Instance.viewportInfo.x + MainForm.Instance.viewportInfo.width,
                MainForm.Instance.viewportInfo.y,
                -MainForm.Instance.viewportInfo.y + MainForm.Instance.viewportInfo.height,
                -1f, 1f
            );


            var scale = 0.1f;
            var scaleSq = scale / 2f;
            float factor = (float)(zoomFactor) * MainForm.Instance.viewportInfo.max;

            var viewMatrix =
                Matrix4.CreateScale(scaleSq, scaleSq, scaleSq) *
                Matrix4.CreateScale(factor, factor, factor) *
                Matrix4.CreateTranslation(
                    MainForm.Instance.viewportInfo.width / 2f + (float)(mapDifX * factor / 2f),
                    MainForm.Instance.viewportInfo.height / 2f + (float)(-mapDifY * factor / 2f),
                    0f
                );

            TextManager.Instance.Render(viewMatrix * _projection, scale);

            GL.PopMatrix();

            GL.Color3(1f, 1f, 1f);
        }
        public static void DrawPointer()
        {
            double rawMapX = _mousePrevPoint.x / _mapSizeFactor.x;
            double rawMapY = _mousePrevPoint.y / _mapSizeFactor.y;

            GL.LineWidth(1f);
            GL.Color3(0.0f, 0.0f, 0.0f);

            bool isBrushTool = (MainForm.Instance.enumTool == EnumTool.BRUSH ||
                                MainForm.Instance.enumTool == EnumTool.ERASER);

            if (isBrushTool &&
                BrushManager.TryGetBrush(
                    SettingsManager.Settings,
                    MainForm.Instance.ComboBox_Tool_Parameter.Text,
                    out var brush
                )
            )
            {
                brush.ForEachLineStrip(
                    MainForm.Instance.ComboBox_Tool_Parameter_Value.Text, rawMapX, rawMapY, _mapSizeFactor.x, _mapSizeFactor.y,
                    (line, xOffset, yOffset) =>
                {
                    if (line == null || line.Count < 2)
                        return;

                    GL.Begin(PrimitiveType.LineLoop);
                    foreach (var pixel in line)
                        GL.Vertex2(pixel.x / _mapSizeFactor.x + xOffset, pixel.y / _mapSizeFactor.y + yOffset);
                    GL.End();
                });
            }
            else
            {
                Point2D size = new Point2D
                {
                    x = _pointerSize.x / _mapSizeFactor.x,
                    y = _pointerSize.y / _mapSizeFactor.y,
                };

                double snappedTopLeftX = Math.Floor(rawMapX * _mapSizeFactor.x) / _mapSizeFactor.x;
                double snappedTopLeftY = Math.Floor(rawMapY * _mapSizeFactor.y) / _mapSizeFactor.y;

                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(snappedTopLeftX, snappedTopLeftY);
                GL.Vertex2(snappedTopLeftX + size.x, snappedTopLeftY);
                GL.Vertex2(snappedTopLeftX + size.x, snappedTopLeftY + size.y);
                GL.Vertex2(snappedTopLeftX, snappedTopLeftY + size.y);
                GL.End();
            }
        }

        private static void DrawBounds()
        {
            ushort left, right, top, bottom;

            if (bounds.left < bounds.right)
            {
                left = bounds.left;
                right = bounds.right;
            }
            else
            {
                right = bounds.left;
                left = bounds.right;
            }

            if (bounds.top < bounds.bottom)
            {
                top = bounds.top;
                bottom = bounds.bottom;
            }
            else
            {
                bottom = bounds.top;
                top = bounds.bottom;
            }

            GL.LineWidth(2f);
            GL.Color3(0f, 0f, 0f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(left, top);
            GL.Vertex2(right, top);
            GL.Vertex2(right, bottom);
            GL.Vertex2(left, bottom);
            GL.End();
        }

        public static void HandleMapMainLayerChange(EnumMainLayer enumMainLayer, string parameter)
        {
            if (ProvincesPixels == null)
                return;

            if (!MainForm.firstLoad)
                return;

            Func<Province, int> func = (p) => Utils.ArgbToInt(255, 0, 0, 0);
            Func<Province, int, int> customFunc = null;
            LogScaleData logScaleData = default;

            switch (enumMainLayer)
            {
                case EnumMainLayer.PROVINCES_MAP:
                    func = null;
                    break;
                case EnumMainLayer.TERRAIN_MAP:
                    MapMainLayer.Texture = TextureManager.terrain.texture;
                    return;
                case EnumMainLayer.TREES_MAP:
                    MapMainLayer.Texture = TextureManager.trees.texture;
                    return;
                case EnumMainLayer.CITIES_MAP:
                    MapMainLayer.Texture = TextureManager.cities.texture;
                    return;
                case EnumMainLayer.HEIGHT_MAP:
                    MapMainLayer.Texture = TextureManager.height.texture;
                    return;
                case EnumMainLayer.NORMAL_MAP:
                    MapMainLayer.Texture = TextureManager.normal.texture;
                    return;
                case EnumMainLayer.NONE:
                    MapMainLayer.Texture = TextureManager.none.texture;
                    return;

                case EnumMainLayer.STATES:
                    func = (p) =>
                    {
                        if (p.State == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.State.Color;
                    };
                    break;
                case EnumMainLayer.STRATEGIC_REGIONS:
                    func = (p) =>
                    {
                        if (p.Region == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.Region.color;
                    };
                    break;
                case EnumMainLayer.AI_AREAS:
                    if (!AiAreaManager.TryGetAiArea(parameter, out AiArea aiArea))
                    {
                        func = (p) => Utils.ArgbToInt(255, 0, 0, 0);
                        break;
                    }

                    func = (p) =>
                    {
                        bool continentFlag = aiArea.HasContinents && aiArea.HasContinentId(p.ContinentId);
                        bool regionFlag = aiArea.HasRegions && aiArea.HasRegion(p.Region);
                        return Utils.ArgbToInt(255, 0, continentFlag ? (byte)127 : (byte)0, regionFlag ? (byte)127 : (byte)0);
                    };

                    break;
                case EnumMainLayer.COUNTRIES:
                    func = (p) =>
                    {
                        var type = p.Type;
                        //Проверка на sea провинции
                        if (type == EnumProvinceType.SEA)
                        {
                            if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 255);
                            else
                                return Utils.ArgbToInt(255, 0, 0, 0);
                        }
                        else if (type == EnumProvinceType.LAKE)
                            return Utils.ArgbToInt(255, 0, 255, 255);
                        else if (p.State == null || (p.State.owner == null && p.State.controller == null))
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.controller != null)
                            return p.State.controller.color;
                        else return p.State.owner.color;
                    };
                    break;
                case EnumMainLayer.CORES_OF:
                    CountryManager.TryGetCountry(parameter, out var targetCoreOfCountry);
                    func = (p) =>
                    {
                        var type = p.Type;
                        //Проверка на sea провинции
                        if (type == EnumProvinceType.SEA)
                        {
                            if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 255);
                            else
                                return Utils.ArgbToInt(255, 0, 0, 0);
                        }
                        else if (type == EnumProvinceType.LAKE)
                            return Utils.ArgbToInt(255, 0, 255, 255);
                        else if (p.State == null || targetCoreOfCountry == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.CurrentCoresOf.Contains(targetCoreOfCountry))
                            return targetCoreOfCountry.color;
                        else return Utils.ArgbToInt(255, 0, 0, 0);
                    };
                    break;
                case EnumMainLayer.CLAIMS_BY:
                    CountryManager.TryGetCountry(parameter, out var targetClaimByCountry);
                    func = (p) =>
                    {
                        var type = p.Type;
                        //Проверка на sea провинции
                        if (type == EnumProvinceType.SEA)
                        {
                            if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 255);
                            else
                                return Utils.ArgbToInt(255, 0, 0, 0);
                        }
                        else if (type == EnumProvinceType.LAKE)
                            return Utils.ArgbToInt(255, 0, 255, 255);
                        else if (p.State == null || targetClaimByCountry == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.CurrentClaimsBy.Contains(targetClaimByCountry))
                            return targetClaimByCountry.color;
                        else return Utils.ArgbToInt(255, 0, 0, 0);
                    };
                    break;
                case EnumMainLayer.PROVINCES_TYPES:
                    func = (p) =>
                    {
                        var type = p.Type;
                        bool isCoastal = p.IsCoastal;
                        if (type == EnumProvinceType.LAND)
                        {
                            if (isCoastal)
                                return Utils.ArgbToInt(255, 127, 127, 0);
                            else
                                return Utils.ArgbToInt(255, 0, 127, 0);
                        }
                        else if (type == EnumProvinceType.SEA)
                        {
                            if (isCoastal)
                                return Utils.ArgbToInt(255, 127, 0, 127);
                            else
                                return Utils.ArgbToInt(255, 0, 0, 127);
                        }
                        else if (type == EnumProvinceType.LAKE)
                            return Utils.ArgbToInt(255, 127, 255, 255);
                        else
                            return Utils.ArgbToInt(255, 0, 0, 0);
                    };
                    break;
                case EnumMainLayer.PROVINCES_TERRAINS:
                    func = (p) =>
                    {
                        if (p.Terrain == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else
                            return p.Terrain.color;
                    };
                    break;
                case EnumMainLayer.PROVINCES_SIZES:
                    ProvinceManager.GetMinMaxMapProvinceSizes(out int minPixelsCount, out int maxPixelsCount);
                    logScaleData = new LogScaleData(minPixelsCount, maxPixelsCount);
                    func = (p) =>
                    {
                        var value = (byte)logScaleData.CalculateInverted(p.pixelsCount, 255d);
                        return Utils.ArgbToInt(255, value, value, value);
                    };
                    break;
                case EnumMainLayer.REGIONS_TERRAINS:
                    func = (p) =>
                    {
                        if (p.Region == null || p.Region.Terrain == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else
                            return p.Region.Terrain.color;
                    };
                    break;
                case EnumMainLayer.CONTINENTS:
                    func = (p) => ContinentManager.GetColorById(p.ContinentId);
                    break;
                case EnumMainLayer.MANPOWER:
                    StateManager.GetMinMaxWeightedManpower(out double manpowerWeightedMin, out double manpowerWeightedMax);
                    logScaleData = new LogScaleData(manpowerWeightedMin, manpowerWeightedMax);

                    func = (p) =>
                    {
                        var type = p.Type;
                        //Проверка на sea провинции
                        if (type == EnumProvinceType.SEA)
                        {
                            if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 255);
                            else
                                return Utils.ArgbToInt(255, 255, 0, 255);
                        }
                        else if (type == EnumProvinceType.LAKE)
                            return Utils.ArgbToInt(255, 127, 255, 255);
                        else if (p.State == null)
                            return Utils.ArgbToInt(255, 255, 0, 0);
                        else if (p.State.CurrentManpower < 1)
                            return Utils.ArgbToInt(255, 255, 106, 0);

                        var valueFactor = p.State.CurrentManpower / (double)p.State.pixelsCount;
                        var value = (byte)logScaleData.CalculateInverted(valueFactor, 255d);
                        return Utils.ArgbToInt(255, value, value, value);
                    };
                    break;
                case EnumMainLayer.VICTORY_POINTS:
                    ProvinceManager.GetMinMaxVictoryPoints(out uint victoryPointsMin, out uint victoryPointsMax);
                    logScaleData = new LogScaleData(victoryPointsMin, victoryPointsMax);

                    func = (p) =>
                    {
                        var type = p.Type;
                        //Проверка на sea провинции
                        if (type == EnumProvinceType.SEA)
                        {
                            if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 255);
                            else
                                return Utils.ArgbToInt(255, 255, 0, 255);
                        }

                        byte value = (byte)logScaleData.CalculateInverted(p.victoryPoints, 255d);
                        return Utils.ArgbToInt(255, value, value, value);
                    };
                    break;
                case EnumMainLayer.STATES_CATEGORIES:
                    func = (p) =>
                    {
                        if (p.State == null)
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.StateCategory.GetValue() == null)
                            return Utils.ArgbToInt(255, 255, 0, 0);
                        else
                            return p.State.StateCategory.GetValue().color;
                    };
                    break;
                case EnumMainLayer.BUILDINGS:
                    if (!BuildingManager.TryGetBuilding(parameter, out Building building))
                    {
                        func = (p) =>
                        {
                            var type = p.Type;
                            if (type == EnumProvinceType.SEA)
                                return Utils.ArgbToInt(255, 0, 0, 127);
                            else if (type == EnumProvinceType.LAKE)
                                return Utils.ArgbToInt(255, 127, 255, 255);
                            else
                                return Utils.ArgbToInt(255, 0, 0, 0);
                        };
                        break;
                    }

                    var buildingLevelCap = building.LevelCap.GetValue();
                    var buildingSlotCategory = buildingLevelCap.GetSlotCategory();

                    if (buildingSlotCategory == EnumBuildingSlotCategory.PROVINCIAL)
                    {
                        float maxLevel = buildingLevelCap.GetProvinceMaxCount();

                        func = (p) =>
                        {
                            var type = p.Type;
                            if (type == EnumProvinceType.SEA)
                                return Utils.ArgbToInt(255, 0, 0, 127);
                            else if (type == EnumProvinceType.LAKE)
                                return Utils.ArgbToInt(255, 127, 255, 255);

                            if (!p.TryGetBuildingCount(building, out uint count))
                                return Utils.ArgbToInt(255, 0, 0, 0);

                            float factor = count / maxLevel;
                            if (factor > 1)
                                return Utils.ArgbToInt(255, 255, 0, 0);
                            else
                            {
                                byte value = (byte)(255 * factor);
                                return Utils.ArgbToInt(255, 0, value, 0);
                            }
                        };
                    }
                    else if (buildingSlotCategory == EnumBuildingSlotCategory.SHARED)
                    {
                        uint maxCount = 0;
                        foreach (State state in StateManager.GetStates())
                        {
                            if (state.stateBuildings.TryGetValue(building, out uint max))
                                if (max > maxCount) maxCount = max;
                        }

                        func = (p) =>
                        {
                            var type = p.Type;
                            if (type == EnumProvinceType.SEA)
                                return Utils.ArgbToInt(255, 0, 0, 127);
                            else if (type == EnumProvinceType.LAKE)
                                return Utils.ArgbToInt(255, 127, 255, 255);
                            else if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 0);
                            else
                            {
                                p.State.stateBuildings.TryGetValue(building, out uint count);

                                float factor = count / (float)maxCount;
                                if (factor > 1)
                                    return Utils.ArgbToInt(255, 255, 0, 0);
                                else
                                {
                                    byte value = (byte)(255 * factor);
                                    return Utils.ArgbToInt(255, 0, value, 0);
                                }
                            }
                        };
                    }
                    else //NON_SHARED
                    {
                        float maxCount = buildingLevelCap.GetStateMaxCount();
                        func = (p) =>
                        {
                            var type = p.Type;
                            if (type == EnumProvinceType.SEA)
                                return Utils.ArgbToInt(255, 0, 0, 127);
                            else if (type == EnumProvinceType.LAKE)
                                return Utils.ArgbToInt(255, 127, 255, 255);
                            else if (p.State == null)
                                return Utils.ArgbToInt(255, 0, 0, 0);
                            else
                            {
                                p.State.stateBuildings.TryGetValue(building, out uint count);

                                float factor = count / maxCount;
                                if (factor > 1)
                                    return Utils.ArgbToInt(255, 255, 0, 0);
                                else
                                {
                                    byte value = (byte)(255 * factor);
                                    return Utils.ArgbToInt(255, 0, value, 0);
                                }
                            }
                        };
                    }
                    break;

                case EnumMainLayer.CUSTOM_SCRIPT:
                    if (ScriptParser.MapMainLayerCustomScriptName == null)
                    {
                        func = null;
                        return;
                    }

                    ScriptParser.IsDebug = false;

                    customFunc = (p, idx) =>
                    {
                        if (idx >= ScriptParser.MapMainLayerCustomScriptTasks)
                        {
                            return Utils.ArgbToInt(255, 0, 0, 0);
                        }

                        var action = ScriptParser.MapMainLayerCustomScriptActions[idx];
                        var varsScope = ScriptParser.MapMainLayerCustomScriptMainVarsScopes[idx];

                        varsScope.PutLocalVariable("province_id", new IntObject(p.Id));
                        varsScope.PutLocalVariable("red", new IntObject());
                        varsScope.PutLocalVariable("green", new IntObject());
                        varsScope.PutLocalVariable("blue", new IntObject());
                        action();
                        byte r = 0, g = 0, b = 0;

                        if (varsScope.TryGetLocalValue("red", out var variable) && variable is INumberObject redObj)
                            r = Convert.ToByte(redObj.GetValue());
                        if (varsScope.TryGetLocalValue("green", out variable) && variable is INumberObject greenObj)
                            g = Convert.ToByte(greenObj.GetValue());
                        if (varsScope.TryGetLocalValue("blue", out variable) && variable is INumberObject blueObj)
                            b = Convert.ToByte(blueObj.GetValue());

                        varsScope.ClearLocalVars();

                        return Utils.ArgbToInt(255, r, g, b);
                    };
                    break;
            }

            if (isHandlingMapMainLayerChange[0])
                isWaitingHandlingMapMainLayerChange[0] = true;
            else
                UpdateTask();

            void UpdateTask()
            {
                isHandlingMapMainLayerChange[0] = true;
                var assembleTask = new Task<byte[]>(
                    () =>
                    customFunc != null ?
                        AssembleBitmapBytesByCustomScript(customFunc) :
                        func != null ?
                            AssembleBitmapBytes(func) :
                            AssembleBitmapBytes()
                );

                assembleTask.ContinueWith(
                    task =>
                    {
                        MapMainLayer.Texture = TextureManager.provinces.texture;
                        TextureManager.provinces.texture.Update(TextureManager._24bppRgb, 0, 0, MapSize.x, MapSize.y, task.Result);
                        isHandlingMapMainLayerChange[0] = false;

                        if (isWaitingHandlingMapMainLayerChange[0])
                        {
                            isWaitingHandlingMapMainLayerChange[0] = false;
                            UpdateTask();
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
                assembleTask.Start();
            }
        }

        public static byte[] AssembleBitmapBytes()
        {
            byte[] values = new byte[ProvincesPixels.Length * 3];

            Parallel.For(0, MapSize.y, (row) =>
            {
                int color = 0;

                int start = row * MapSize.x;
                int end = start + MapSize.x;

                int byteIndex = start * 3;

                for (int i = start; i < end; i++)
                {
                    color = ProvincesPixels[i];

                    values[byteIndex] = (byte)color;
                    values[byteIndex + 1] = (byte)(color >> 8);
                    values[byteIndex + 2] = (byte)(color >> 16);

                    byteIndex += 3;
                }
            });

            return values;
        }

        public static byte[] AssembleBitmapBytesByCustomScript(Func<Province, int, int> func)
        {
            byte[] values = new byte[ProvincesPixels.Length * 3];

            var freeIndices = new ConcurrentQueue<int>(Enumerable.Range(0, ScriptParser.MapMainLayerCustomScriptTasks));
            var options = new ParallelOptions { MaxDegreeOfParallelism = ScriptParser.MapMainLayerCustomScriptTasks - 1 };

            Parallel.For(
                0,
                MapSize.y,
                options,
                () =>
                {
                    int idx;
                    while (!freeIndices.TryDequeue(out idx))
                        Thread.SpinWait(1);          // ждём, пока освободится индекс
                    return idx;
                },
                (row, loopState, idx) =>
                {
                    int color = 0;
                    int newColor = 0;

                    int start = (int)(row * MapSize.x);
                    int end = start + MapSize.x;

                    int byteIndex = start * 3;

                    for (int i = start; i < end; i++)
                    {
                        if (color != ProvincesPixels[i])
                        {
                            color = ProvincesPixels[i];
                            ProvinceManager.TryGetProvince(color, out Province province);
                            if (province != null)
                                newColor = func(province, idx);
                            else
                                newColor = 0;
                        }

                        values[byteIndex] = (byte)newColor;
                        values[byteIndex + 1] = (byte)(newColor >> 8);
                        values[byteIndex + 2] = (byte)(newColor >> 16);

                        byteIndex += 3;
                    }

                    return idx;
                },
                idx => freeIndices.Enqueue(idx)
            );

            return values;
        }

        public static byte[] AssembleBitmapBytes(Func<Province, int> func)
        {
            byte[] values = new byte[ProvincesPixels.Length * 3];

            Parallel.For(0, MapSize.y, (row) =>
            {
                int color = 0;
                int newColor = 0;

                int start = row * MapSize.x;
                int end = start + MapSize.x;

                int byteIndex = start * 3;

                for (int i = start; i < end; i++)
                {
                    if (color != ProvincesPixels[i])
                    {
                        color = ProvincesPixels[i];
                        ProvinceManager.TryGetProvince(color, out Province province);
                        if (province != null)
                            newColor = func(province);
                        else
                            newColor = 0;
                    }

                    values[byteIndex] = (byte)newColor;
                    values[byteIndex + 1] = (byte)(newColor >> 8);
                    values[byteIndex + 2] = (byte)(newColor >> 16);

                    byteIndex += 3;
                }
            });

            return values;
        }

        public static void UpdateMapInfo()
        {
            MainForm.PauseGLControl();

            Task.Run(() =>
            {
                Logger.TryOrLog(
                    () =>
                    {
                        var context = new GraphicsContext(GraphicsMode.Default, MainForm.Instance.glControl.WindowInfo);
                        context.MakeCurrent(MainForm.Instance.glControl.WindowInfo);

                        TextureManager.LoadBorders();
                        ProvinceManager.ProcessProvincesPixels(ProvincesPixels, MapSize.x, MapSize.y);

                        foreach (var p in ProvinceManager.GetProvinces())
                        {
                            p.ClearBorders();
                        }

                        ProvinceBorderManager.Init(ProvincesPixels, (short)MapSize.x, (short)MapSize.y);
                        WarningsManager.Init();
                        ErrorManager.Init();

                        context.MakeCurrent(null);
                    },
                    () =>
                    {
                        MainForm.Instance.Invoke((MethodInvoker)delegate
                        {
                            Logger.TryOrLog(() =>
                            {
                                UpdateDisplayBorders();
                                MainForm.DisplayProgress(EnumLocKey.PROGRESSBAR_UPDATED, 0);
                                MainForm.ResumeGLControl();
                                Utils.CleanUpMemory();
                            });
                        });
                    }
                );
            });

        }

        public static void HandleMouseWheel(MouseEventArgs e, ViewportInfo viewportInfo)
        {
            if (selectedTexturedPlane != null)
            {
                if (e.Delta > 0)
                    selectedTexturedPlane.Scale(1.001f);
                else if (e.Delta < 0)
                    selectedTexturedPlane.Scale(0.999f);
            }
            else if (/*MainForm.Instance.IsParameterValueVisible() &&*/ MainForm.Instance.IsShiftPressed())
            {
                if (e.Delta > 0)
                    MainForm.Instance.IncreaseParameterValue();
                else if (e.Delta < 0)
                    MainForm.Instance.DecreaseParameterValue();
            }
            else
            {
                if (e.Delta > 0 && zoomFactor < 0.1f)
                    zoomFactor *= 1.2f;
                else if (e.Delta < 0 && zoomFactor > 0.0004f)
                    zoomFactor *= 0.8f;
            }
            _mousePrevPoint = CalculateMapPos(e.X, e.Y, viewportInfo);
        }

        public static Point2D CalculateMapPos(int eX, int eY, ViewportInfo viewportInfo)
        {
            Point2D point;
            Point2D mapSize = new Point2D();

            if (MapMainLayer != null)
            {
                mapSize.y = MapMainLayer == null ? 0 : MapMainLayer.size.y;
                mapSize.x = MapMainLayer == null ? 0 : MapMainLayer.size.x;
                _mapSizeFactor.x = MapMainLayer.Texture.Size.x / mapSize.x;
                _mapSizeFactor.y = MapMainLayer.Texture.Size.y / mapSize.y;
            }

            point.x = ((2 * eX - viewportInfo.width) / (viewportInfo.max * zoomFactor) - mapDifX) * _mapSizeFactor.x;
            point.y = (mapSize.y + (2 * eY - viewportInfo.height) / (viewportInfo.max * zoomFactor) - mapDifY) * _mapSizeFactor.y;
            return point;
        }


        public static void HandleMouseDown(MouseEventArgs e, ViewportInfo viewportInfo, EnumEditLayer enumEditLayer, EnumTool enumTool, string parameter, string value)
        {
            Point2D pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            _mousePrevPoint = pos;
            _mouseState = EnumMouseState.DOWN;

            if (enumTool != EnumTool.CURSOR)
                ActionsBatch.Enabled = true;

            MapToolsManager.HandleTool(e, _mouseState, pos, _mapSizeFactor, enumEditLayer, enumTool, bounds, parameter, value);

            if (e.Button == MouseButtons.Middle)
                _isMapDragged = true;

            _mouseState = EnumMouseState.NONE;
        }

        public static void HandleMouseUp(MouseEventArgs e, ViewportInfo viewportInfo, EnumTool enumTool, EnumEditLayer enumEditLayer)
        {
            Point2D pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            var button = e.Button;
            _mouseState = EnumMouseState.UP;

            ActionsBatch.Execute();
            ActionsBatch.Enabled = false;

            if (enumTool == EnumTool.CURSOR)
                selectedTexturedPlane = null;
            if (button == MouseButtons.Middle)
                _isMapDragged = false;
            _mouseState = EnumMouseState.NONE;
        }

        public static void HandleMouseMoved(MouseEventArgs e, ViewportInfo viewportInfo, EnumTool enumTool, EnumEditLayer enumEditLayer, string parameter, string value)
        {
            var pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            _mouseState = EnumMouseState.MOVE;

            if (_isMapDragged)
            {
                mapDifX += (pos.x - _mousePrevPoint.x) / _mapSizeFactor.x;
                mapDifY += (pos.y - _mousePrevPoint.y) / _mapSizeFactor.y;
            }
            else
            {
                if (selectedTexturedPlane != null)
                {
                    selectedTexturedPlane.Move(pos.x - _mousePrevPoint.x, pos.y - _mousePrevPoint.y);
                }
                else if (ActionsBatch.Enabled && (_mousePrevPoint.x != pos.x || _mousePrevPoint.y != pos.y))
                {
                    if (enumTool != EnumTool.BUILDINGS)
                        MapToolsManager.HandleTool(e, _mouseState, pos, _mapSizeFactor, enumEditLayer, enumTool, bounds, parameter, value);
                }
                _mousePrevPoint = pos;
            }
            _mouseState = EnumMouseState.NONE;
        }

        public static int GetColor(Point2D point) => ProvincesPixels[(int)point.x + (int)point.y * MapSize.x];
        public static int GetColor(double x, double y) => ProvincesPixels[(int)x + (int)y * MapSize.x];

        public static void ClearAdditionalMapTextures()
        {
            foreach (TextureInfo info in additionalMapTextures) info.plane.Dispose();
            additionalMapTextures = new List<TextureInfo>();
        }

        public static SegmentedTexturedPlane LoadAdditionalMapTexture(string filePath, string fileName)
        {
            var textures = new List<Texture2D>(0);
            TextureManager.LoadSegmentedTextures(filePath, SettingsManager.Settings, textures, out float imageWidth, out float imageHeight);
            if (textures.Count == 0) return null;

            var texturedPlane = new SegmentedTexturedPlane(textures, imageWidth, imageHeight);
            foreach (var texture in additionalMapTextures)
            {
                if (texture.fileName == fileName)
                {
                    additionalMapTextures.Remove(texture);
                    break;
                }
            }

            additionalMapTextures.Add(new TextureInfo(filePath, fileName, texturedPlane));
            return texturedPlane;
        }

        public static void MoveUpAdditionalMapTexture(int index)
        {
            if (index > 0)
            {
                var texture = additionalMapTextures[index];
                additionalMapTextures.RemoveAt(index);
                additionalMapTextures.Insert(index - 1, texture);
            }
        }

        public static void MoveDownAdditionalMapTexture(int index)
        {
            if (index < additionalMapTextures.Count - 1)
            {
                var texture = additionalMapTextures[index];
                additionalMapTextures.RemoveAt(index);
                additionalMapTextures.Insert(index + 1, texture);
            }
        }

        public static void RemoveAdditionalMapTexture(int index)
        {
            if (index < 0 || index >= additionalMapTextures.Count) return;

            additionalMapTextures[index].plane.Dispose();
            additionalMapTextures.RemoveAt(index);
        }

        public static List<string> GetAdditionalMapTexturesNames()
        {
            List<string> names = new List<string>(additionalMapTextures.Count);
            foreach (var info in additionalMapTextures) names.Add(info.fileName);
            return names;
        }

        public static void FindMapChanges()
        {
            if (ProvincesPixels == null) return;

            string bmpFilePath, definitionFilePath;
            var fd = new OpenFileDialog();
            var dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "mapChanges" });
            Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_PROVINCES_BMP_FILE), dialogPath, "BMP files (*.bmp)|*.bmp");
            if (fd.ShowDialog() == DialogResult.OK) bmpFilePath = fd.FileName;
            else return;

            fd = new OpenFileDialog();
            Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_DEFINITION_CSV_FILE), dialogPath, "CSV files (*.csv)|*.csv");
            if (fd.ShowDialog() == DialogResult.OK) definitionFilePath = fd.FileName;
            else return;

            string[] provinceData = File.ReadAllLines(definitionFilePath);
            Dictionary<int, ushort> provincesByColor = new Dictionary<int, ushort>(provinceData.Length);
            foreach (string provinceString in provinceData)
            {
                if (provinceString.Trim().Length == 0) continue;

                string[] data = provinceString.Split(';');
                ushort provinceId = ushort.Parse(data[0]);
                byte r = byte.Parse(data[1]);
                byte g = byte.Parse(data[2]);
                byte b = byte.Parse(data[3]);
                provincesByColor[Utils.ArgbToInt(255, r, g, b)] = provinceId;
            }

            using (var provincesBmp = new Bitmap(bmpFilePath))
            {
                int width = provincesBmp.Width;
                int height = provincesBmp.Height;
                int pixelCount = width * height;

                if (provincesBmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_24BPP));
                else if (pixelCount != ProvincesPixels.Length)
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_SAME_SIZE));

                int[] oldPixels = TextureManager.BrgToArgb(Utils.BitmapToArray(provincesBmp, ImageLockMode.ReadOnly, TextureManager._24bppRgb), 255);

                byte[] resultPixels = new byte[pixelCount * 4];
                int i4;
                Province oldProvince, newProvince;
                ushort oldProvinceId;

                for (int i = 0; i < pixelCount; i++)
                {
                    i4 = i * 4;

                    try //TODO Доработать доп. цвета
                    {
                        if ( //Если провинции с новым и старым цветом всё ещё существуют
                            provincesByColor.TryGetValue(oldPixels[i], out oldProvinceId) &&
                            ProvinceManager.TryGetProvince(oldProvinceId, out oldProvince) &&
                            ProvinceManager.TryGetProvince(ProvincesPixels[i], out newProvince)
                        )
                        {
                            //Если у старой провинции изменился цвет
                            if (oldProvince.Color != oldPixels[i])
                            { //Ставим фиолетовый цвет
                                resultPixels[i4] = 255;
                                resultPixels[i4 + 1] = 0;
                                resultPixels[i4 + 2] = 255;
                                resultPixels[i4 + 3] = 255;
                            }
                            else if (newProvince.Color != oldPixels[i])
                            {
                                resultPixels[i4] = 0;
                                resultPixels[i4 + 1] = 255;
                                resultPixels[i4 + 2] = 0;
                                resultPixels[i4 + 3] = 255;
                            }
                        }
                        //Если старой или новой провинции с этим цветом нет
                        else
                        { //Ставим фиолетовый цвет
                            resultPixels[i4] = 255;
                            resultPixels[i4 + 1] = 0;
                            resultPixels[i4 + 2] = 255;
                            resultPixels[i4 + 3] = 255;
                        }
                    }
                    catch (Exception _)
                    {
                        resultPixels[i4] = 255;
                        resultPixels[i4 + 1] = 0;
                        resultPixels[i4 + 2] = 255;
                        resultPixels[i4 + 3] = 255;
                    }
                }

                Bitmap resultBitmap = new Bitmap(width, height, TextureManager._32bppArgb.imagePixelFormat);
                Utils.ArrayToBitmap(resultPixels, resultBitmap, ImageLockMode.WriteOnly, width, height, TextureManager._32bppArgb);
                var directoryPath = $"{Application.StartupPath}\\data\\mapChanges\\output\\";
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                var filePath = directoryPath + DateTime.Now.ToString().Replace('.', '-').Replace(':', '-') + ".png";
                resultBitmap.Save(filePath, ImageFormat.Png);

                Task.Run(() => MessageBox.Show(
                    GuiLocManager.GetLoc(
                        EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TEXT,
                        new Dictionary<string, string> { { "{filePath}", filePath } }
                    ),
                    GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TITLE),
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification)
                );


                Utils.CleanUpMemory();
            }
        }

        public static void HandleEscape()
        {
            selectedTexturedPlane = null;
            MainForm.Instance.textBox_SelectedObjectId.Text = "";
            MainForm.Instance.textBox_PixelPos.Text = "";
            MainForm.Instance.textBox_HOI4PixelPos.Text = "";
            bounds.Set(0, 0, 0, 0);
        }

        public static void HandleDelete()
        {
            if (selectedTexturedPlane != null)
            {
                foreach (var info in additionalMapTextures)
                {
                    if (info.plane.Equals(selectedTexturedPlane))
                    {
                        info.plane.Dispose();
                        additionalMapTextures.Remove(info);
                        break;
                    }
                }
                selectedTexturedPlane = null;
                MainForm.Instance.SetMapTextures(additionalMapTextures);
                return;
            }
        }

        public static void UpdateDisplayBorders()
        {
            if (MainForm.Instance == null) return;
            switch (MainForm.Instance.EnumBordersType)
            {
                case EnumBordersType.PROVINCES_BLACK:
                    if (TextureManager.provincesBorders.texture != null) BordersMapPlane.Texture = TextureManager.provincesBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.PROVINCES_WHITE:
                    if (TextureManager.provincesBorders.texture != null) BordersMapPlane.Texture = TextureManager.provincesBorders.texture;
                    blackBorders = false;
                    break;
                case EnumBordersType.STATES_BLACK:
                    if (TextureManager.statesBorders.texture != null) BordersMapPlane.Texture = TextureManager.statesBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.STATES_WHITE:
                    if (TextureManager.statesBorders.texture != null) BordersMapPlane.Texture = TextureManager.statesBorders.texture;
                    blackBorders = false;
                    break;
                case EnumBordersType.STRATEGIC_REGIONS_BLACK:
                    if (TextureManager.regionsBorders.texture != null) BordersMapPlane.Texture = TextureManager.regionsBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.STRATEGIC_REGIONS_WHITE:
                    if (TextureManager.regionsBorders.texture != null) BordersMapPlane.Texture = TextureManager.regionsBorders.texture;
                    blackBorders = false;
                    break;
            }
        }
    }

    class TextureInfo
    {
        public string filePath;
        [JsonIgnore]
        public string fileName;
        [JsonProperty("texture")]
        public SegmentedTexturedPlane plane;

        public TextureInfo(string filePath, string fileName, SegmentedTexturedPlane plane)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.plane = plane;
        }

        public override bool Equals(object obj)
        {
            return obj is TextureInfo info &&
                   filePath == info.filePath &&
                   fileName == info.fileName &&
                   EqualityComparer<SegmentedTexturedPlane>.Default.Equals(plane, info.plane);
        }

        public override int GetHashCode()
        {
            int hashCode = -2056841599;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(filePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<SegmentedTexturedPlane>.Default.GetHashCode(plane);
            return hashCode;
        }
    }
}
