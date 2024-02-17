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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.src.openTK.text;
using System.Drawing.Imaging;
using System.Drawing;
using YamlDotNet.Core.Tokens;

namespace HOI4ModBuilder.managers
{
    class MapManager
    {
        private static bool _isMapDragged = false;
        public static Value2I MapSize { get; private set; }
        private static Point2D _mousePrevPoint;
        private static EnumMouseState _mouseState = EnumMouseState.NONE;
        public static Bounds4US bounds;

        public static int[] ProvincesPixels { get; set; } //RGBA
        public static byte[] HeightsPixels { get; set; }

        private static TexturedPlane _mapMainLayer, _bordersMapPlane, _riversMapPlane;
        public static List<TextureInfo> additionalMapTextures = new List<TextureInfo>();
        public static TexturedPlane selectedTexturedPlane;
        public static SDFTextBundle sdfTextBundle;
        public static Shader sdfTextShader;
        public static bool showSelectZone;
        public static bool blackBorders;
        public static bool showMainLayer, showCenters, showBorders, showRivers, showRailways, showSupplyHubs, showSeaCrosses, showImpassibleZones;
        public static bool showErrors;
        public static bool showTexturedPlanes;

        public static double zoomFactor = 0.0004f;
        public static double mapDifX, mapDifY;

        public static List<ActionPair> actionPairs = null;

        public static bool isHandlingMapMainLayerChange = false;

        public static ActionHistoryManager ActionHistory { get; private set; }

        public static void Init()
        {
            ActionHistory = new ActionHistoryManager(MainForm.Instance.TabPage_Map);

            MapToolsManager.Init();
            SupplyManager.Init();
            AdjacenciesManager.Init();
            ProvinceManager.Init();
            StateManager.Init();
            StrategicRegionManager.Init();

            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            TextureManager.LoadTextures(settings);

            Tuple<EnumLocKey, Action>[] actions =
            {
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS, () => LoadTextureMaps()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_ADJACENCIES_DEFINITION, () => AdjacenciesManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_SUPPLIES, () => SupplyManager.Load(settings))
            };

            MainForm.ExecuteActions(actions);

            ProvinceBorderManager.Init(ProvincesPixels, (ushort)MapSize.x, (ushort)MapSize.y);

            showMainLayer = true;
        }

        private static void LoadTextureMaps()
        {
            MapSize = TextureManager.provinces.texture.GetSize();
            _mapMainLayer = new TexturedPlane(TextureManager.provinces.texture, MapSize.x, MapSize.y);
            _bordersMapPlane = new TexturedPlane(TextureManager.provincesBorders.texture, MapSize.x, MapSize.y);
            _riversMapPlane = new TexturedPlane(TextureManager.rivers.texture, MapSize.x, MapSize.y);

            if (mapDifX == 0 && mapDifY == 0)
            {
                mapDifX = -_mapMainLayer.size.x / 2f;
                mapDifY = _mapMainLayer.size.y / 2f;
            }
        }

        /*
        public static void LoadSDFThings()
        {
            sdfTextBundle?.Dispose();
            sdfTextBundle = new SDFTextBundle();
            sdfTextBundle.AddText(new SDFText("aAбб123", new Color3B(255, 0, 0), new Value2F(), new Value2F(1f, 1f), FontManager.fonts["test"]));

            if (sdfTextShader == null)
            {
                sdfTextShader = new Shader(@"shaders\sdf_text_shader.vert", @"shaders\sdf_text_shader.frag");
            }
        }
        */

        public static void FocusOn(Point2F point) => FocusOn(point.x, point.y);

        public static void FocusOn(float x, float y)
        {
            mapDifX = -x;
            mapDifY = MapSize.y - y;
        }

        public static void Draw()
        {
            if (_mapMainLayer == null) return;

            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(zoomFactor, zoomFactor, zoomFactor);
            GL.Translate(mapDifX, -mapDifY, 0); // перемещение

            if (showMainLayer) _mapMainLayer.Draw();

            if (showBorders)
            {
                GL.Translate(0.5f, -0.5f, 0f);
                if (blackBorders) GL.Color3(0f, 0f, 0f);
                _bordersMapPlane.Draw();
                GL.Color3(1f, 1f, 1f);
                GL.Translate(-0.5f, 0.5f, 0f);
            }

            if (showRivers) _riversMapPlane.Draw();

            GL.Scale(1f, -1f, 1f);
            GL.Translate(0, -_mapMainLayer.size.y, 0);

            ProvinceManager.Draw(showCenters & (MainForm.Instance.enumMainLayer == EnumMainLayer.PROVINCES_MAP));
            StateManager.Draw(showCenters & (MainForm.Instance.enumMainLayer == EnumMainLayer.STATES));
            StrategicRegionManager.Draw(showCenters & (MainForm.Instance.enumMainLayer == EnumMainLayer.STRATEGIC_REGIONS));
            AdjacenciesManager.Draw(showSeaCrosses, showImpassibleZones);
            SupplyManager.Draw(showRailways, showSupplyHubs);

            DrawPointer();
            if (bounds.HasSpace()) DrawBounds();

            GL.Scale(1f, -1f, 1f);
            GL.Color3(1f, 1f, 1f);
            GL.PointSize(5f);
            if (showTexturedPlanes)
            {
                foreach (TextureInfo info in additionalMapTextures)
                {
                    SegmentedTexturedPlane plane = info.plane;
                    GL.Translate(plane.pos.x, -plane.pos.y, 0f);
                    plane.Draw();
                    GL.Translate(-plane.pos.x, plane.pos.y, 0f);
                }
            }

            if (showErrors)
            {
                GL.Scale(1f, -1f, 1f);
                ErrorManager.Draw();
            }
            //DrawTest();

            GL.PopMatrix();

            /*
            if (sdfTextShader != null)
            {
                GL.LoadIdentity();
                GL.PushMatrix();
                GL.Scale(zoomFactor, zoomFactor, zoomFactor);
                GL.Translate(mapDifX, -mapDifY, 0); // перемещение
                GL.Color3(1f, 1f, 0f);

                GL.PointSize(10);
                GL.Begin(PrimitiveType.Points);
                foreach (SDFText text in sdfTextBundle.GetFontInfo((FontManager.fonts.Values.ToArray())[0]).texts)
                {
                    GL.Vertex2(text.position.x, text.position.y);
                    GL.Vertex2(text.position.x + text.GetWidth(), text.position.y);
                }
                GL.End();

                // Проверка местоположения атрибутов
                int positionLocation = sdfTextShader.GetAttribProgram("inPosition");
                int colorLocation = sdfTextShader.GetAttribProgram("inColor");
                int texCoordLocation = sdfTextShader.GetAttribProgram("inTextureCoord");

                sdfTextShader.ActiveProgram();

                positionLocation = sdfTextShader.GetAttribProgram("inPosition");
                colorLocation = sdfTextShader.GetAttribProgram("inColor");
                texCoordLocation = sdfTextShader.GetAttribProgram("inTextureCoord");

                sdfTextBundle.Draw();
                sdfTextShader.DeactiveProgram();

                GL.PopMatrix();
            }
            */

            GL.Color3(1f, 1f, 1f);
        }

        private static void DrawPointer()
        {
            GL.LineWidth(1f);
            GL.Color3(0f, 0f, 0f);
            GL.Begin(PrimitiveType.LineLoop);
            int x = (int)_mousePrevPoint.x;
            int y = (int)_mousePrevPoint.y;
            GL.Vertex2(x, y);
            GL.Vertex2(x + 1, y);
            GL.Vertex2(x + 1, y + 1);
            GL.Vertex2(x, y + 1);
            GL.End();
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

        private static void DrawTest()
        {
            float[][] data = new float[][]
            {
                new float[] { 5923f, 0f, 4482.00f, 10.30f, 1266.00f, 0.00f, 0.12f },
                new float[] { 5923f, 1f, 4481.50f, 10.30f, 1265.00f, -0.46f, 0.12f },
                new float[] { 5923f, 2f, 4482.50f, 10.30f, 1264.50f, 0.32f, 0.12f },
                new float[] { 5923f, 3f, 4483.50f, 10.30f, 1266.00f, 1.57f, 0.12f },
                new float[] { 5923f, 4f, 4481.00f, 10.30f, 1267.00f, 3.92f, 0.12f },
                new float[] { 5923f, 5f, 4483.00f, 10.30f, 1267.50f, 2.55f, 0.12f },
            };
            /*
            float[][] data = new float[][]
            {
                new float[] { 15389f, 0f, 4453.00f, 10.30f, 1256.00f, 0.00f, 0.08f },
                new float[] { 15389f, 1f, 4452.50f, 10.30f, 1255.00f, -0.46f, 0.08f },
                new float[] { 15389f, 2f, 4454.00f, 10.30f, 1255.00f, 0.78f, 0.08f },
                new float[] { 15389f, 3f, 4452.00f, 10.30f, 1256.00f, 4.71f, 0.08f },
                new float[] { 15389f, 4f, 4455.00f, 10.30f, 1256.00f, 1.57f, 0.08f },
                new float[] { 15389f, 5f, 4452.50f, 10.30f, 1257.00f, 3.60f, 0.08f },
                new float[] { 15389f, 6f, 4454.00f, 10.30f, 1256.50f, 2.03f, 0.08f },
            };
            */

            GL.LineWidth(2f);
            GL.Translate(0.5f, -MapSize.y, 0f);
            ProvinceManager.TryGetProvince((ushort)data[0][0], out Province p);
            foreach (float[] arr in data)
            {
                if (p != null)
                {
                    byte r = 0;
                    byte g = (byte)(40 + 40 * arr[1]);
                    byte b = (byte)(215 - 40 * arr[1]);
                    float x = arr[2];
                    float y = arr[4];

                    GL.Color3(r, g, b);
                    GL.Begin(PrimitiveType.Points);
                    GL.Vertex2(x, y);
                    GL.End();

                    //double angle = (180 / Math.PI) * arr[5];
                    if (arr[1] != 0)
                    {
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex2(x, y);
                        GL.Vertex2(x + 5 * Math.Sin(arr[5]), y - 5 * Math.Cos(arr[5]));
                        GL.End();
                    }
                }
            }
            GL.Translate(-0.5f, MapSize.y, 0f);
            GL.Translate(0f, -MapSize.y, 0f);

            if (p != null)
            {
                foreach (ProvinceBorder b in p.borders)
                {
                    int count = b.pixels.Count();
                    if (count > 1)
                    {
                        GL.Color3(1f, 0f, 0f);
                        GL.Begin(PrimitiveType.LineStrip);
                        Value2F medium = new Value2F();
                        foreach (Value2US pos in b.pixels)
                        {
                            medium.x += pos.x;
                            medium.y += MapSize.y - pos.y;
                            GL.Vertex2(pos.x, MapSize.y - pos.y);
                        }
                        medium.x /= count;
                        medium.y /= count;
                        GL.End();

                        GL.Begin(PrimitiveType.Points);
                        GL.Vertex2(medium.x, medium.y);
                        GL.End();
                    }
                }
            }
            GL.Translate(0f, MapSize.y, 0f);
        }

        public static void HandleMapMainLayerChange(EnumMainLayer enumMainLayer, string parameter)
        {
            if (ProvincesPixels == null) return;

            Func<Province, int> func = (p) => Utils.ArgbToInt(255, 0, 0, 0);

            switch (enumMainLayer)
            {
                case EnumMainLayer.PROVINCES_MAP:
                    _mapMainLayer.Texture = TextureManager.provinces.texture;
                    TextureManager.provinces.texture.Update(TextureManager._24bppRgb, 0, 0, MapSize.x, MapSize.y, ProvincesPixels);
                    return;
                case EnumMainLayer.TERRAIN_MAP:
                    _mapMainLayer.Texture = TextureManager.terrain.texture;
                    return;
                case EnumMainLayer.TREES_MAP:
                    _mapMainLayer.Texture = TextureManager.trees.texture;
                    return;
                case EnumMainLayer.CITIES_MAP:
                    _mapMainLayer.Texture = TextureManager.cities.texture;
                    return;
                case EnumMainLayer.HEIGHT_MAP:
                    _mapMainLayer.Texture = TextureManager.height.texture;
                    return;
                case EnumMainLayer.NORMAL_MAP:
                    _mapMainLayer.Texture = TextureManager.normal.texture;
                    return;
                case EnumMainLayer.NONE:
                    _mapMainLayer.Texture = TextureManager.none.texture;
                    return;

                case EnumMainLayer.STATES:
                    func = (p) =>
                    {
                        if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.State.color;
                    };
                    break;
                case EnumMainLayer.STRATEGIC_REGIONS:
                    func = (p) =>
                    {
                        if (p.Region == null) return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.Region.color;
                    };
                    break;
                case EnumMainLayer.COUNTRIES:
                    func = (p) =>
                    {
                        byte typeId = p.TypeId;
                        //Проверка на sea провинции
                        if (typeId == 1)
                        {
                            if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 255);
                            else return Utils.ArgbToInt(255, 0, 0, 0);
                        }
                        else if (typeId == 2) return Utils.ArgbToInt(255, 0, 255, 255);
                        else if (p.State == null || (p.State.owner == null && p.State.controller == null)) return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.controller != null) return p.State.controller.color;
                        else return p.State.owner.color;
                    };
                    break;
                case EnumMainLayer.PROVINCE_TYPES:
                    func = (p) =>
                    {
                        byte typeId = p.TypeId;
                        bool isCoastal = p.IsCoastal;
                        if (typeId == 0)
                        {
                            if (isCoastal) return Utils.ArgbToInt(255, 127, 127, 0);
                            else return Utils.ArgbToInt(255, 0, 127, 0);
                        }
                        else if (typeId == 1)
                        {
                            if (isCoastal) return Utils.ArgbToInt(255, 127, 0, 127);
                            else return Utils.ArgbToInt(255, 0, 0, 127);
                        }
                        else if (typeId == 2) return Utils.ArgbToInt(255, 127, 255, 255);
                        else return Utils.ArgbToInt(255, 0, 0, 0);
                    };
                    break;
                case EnumMainLayer.PROVINCES_TERRAIN:
                    func = (p) =>
                    {
                        if (p.Terrain == null) return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.Terrain.color;
                    };
                    break;
                case EnumMainLayer.REGIONS_TERRAIN:
                    func = (p) =>
                    {
                        if (p.Region == null || p.Region.Terrain == null) return Utils.ArgbToInt(255, 0, 0, 0);
                        else return p.Region.Terrain.color;
                    };
                    break;
                case EnumMainLayer.CONTINENTS:
                    func = (p) => ContinentManager.GetColorById(p.ContinentId);
                    break;
                case EnumMainLayer.MANPOWER:
                    StateManager.GetMinMaxManpower(out int manpowerMin, out int manpowerMax);
                    double maxManpower = manpowerMax;

                    func = (p) =>
                    {
                        byte typeId = p.TypeId;
                        //Проверка на sea провинции
                        if (typeId == 1)
                        {
                            if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 255);
                            else return Utils.ArgbToInt(255, 255, 0, 255);
                        }
                        else if (typeId == 2) return Utils.ArgbToInt(255, 127, 255, 255);
                        else if (p.State == null) return Utils.ArgbToInt(255, 255, 0, 0);
                        else if (p.State.manpower < 1) return Utils.ArgbToInt(255, 255, 106, 0);

                        byte value = (byte)(255 * p.State.manpower / maxManpower);
                        return Utils.ArgbToInt(255, value, value, value);
                    };
                    break;
                case EnumMainLayer.VICTORY_POINTS:
                    ProvinceManager.GetMinMaxVictoryPoints(out uint victoryPointsMin, out uint victoryPointsMax);
                    double maxVictoryPoints = victoryPointsMax;

                    func = (p) =>
                    {
                        byte typeId = p.TypeId;
                        //Проверка на sea провинции
                        if (typeId == 1)
                        {
                            if (p.State == null) Utils.ArgbToInt(255, 0, 0, 255);
                            else return Utils.ArgbToInt(255, 255, 0, 255);
                        }

                        byte value = (byte)(255 * p.victoryPoints / maxVictoryPoints);
                        return Utils.ArgbToInt(255, value, value, value);
                    };
                    break;
                case EnumMainLayer.STATE_CATEGORIES:
                    func = (p) =>
                    {
                        if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 0);
                        else if (p.State.startStateCategory == null) return Utils.ArgbToInt(255, 255, 0, 0);
                        else return p.State.startStateCategory.color;
                    };
                    break;
                case EnumMainLayer.BUILDINGS:
                    if (BuildingManager.TryGetBuilding(parameter, out Building building))
                    {
                        uint count = 0;

                        if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.PROVINCIAL)
                        {
                            func = (p) =>
                            {
                                byte typeId = p.TypeId;
                                if (typeId == 1) Utils.ArgbToInt(255, 0, 0, 127);
                                else if (typeId == 2) Utils.ArgbToInt(255, 127, 255, 255);

                                if (!p.TryGetBuildingCount(building, out count))
                                    return Utils.ArgbToInt(255, 0, 0, 0);

                                float factor = count / (float)building.maxLevel;
                                if (factor > 1) return Utils.ArgbToInt(255, 255, 0, 0);
                                else
                                {
                                    byte value = (byte)(255 * factor);
                                    return Utils.ArgbToInt(255, 0, value, 0);
                                }
                            };
                        }
                        else if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.SHARED)
                        {
                            uint maxCount = 0;
                            foreach (State state in StateManager.GetStates())
                            {
                                if (state.stateBuildings.TryGetValue(building, out uint max))
                                {
                                    if (max > maxCount) maxCount = max;
                                }
                            }

                            func = (p) =>
                            {
                                byte typeId = p.TypeId;
                                if (typeId == 1) return Utils.ArgbToInt(255, 0, 0, 127);
                                else if (typeId == 2) return Utils.ArgbToInt(255, 127, 255, 255);
                                else if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 0);
                                else
                                {
                                    p.State.stateBuildings.TryGetValue(building, out count);

                                    float factor = count / (float)maxCount;
                                    if (factor > 1) return Utils.ArgbToInt(255, 255, 0, 0);
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
                            func = (p) =>
                            {
                                byte typeId = p.TypeId;
                                if (typeId == 1) return Utils.ArgbToInt(255, 0, 0, 127);
                                else if (typeId == 2) return Utils.ArgbToInt(255, 127, 255, 255);
                                else if (p.State == null) return Utils.ArgbToInt(255, 0, 0, 0);
                                else
                                {
                                    p.State.stateBuildings.TryGetValue(building, out count);

                                    float factor = count / (float)building.maxLevel;
                                    if (factor > 1) return Utils.ArgbToInt(255, 255, 0, 0);
                                    else
                                    {
                                        byte value = (byte)(255 * factor);
                                        return Utils.ArgbToInt(255, 0, value, 0);
                                    }
                                }
                            };
                        }
                    }
                    else func = (p) =>
                    {
                        byte typeId = p.TypeId;
                        if (typeId == 1) return Utils.ArgbToInt(255, 0, 0, 127);
                        else if (typeId == 2) return Utils.ArgbToInt(255, 127, 255, 255);
                        else return Utils.ArgbToInt(255, 0, 0, 0);
                    };
                    break;

            }

            if (!isHandlingMapMainLayerChange)
            {
                var assembleTask = new Task<int[]>(
                    () =>
                    {
                        isHandlingMapMainLayerChange = true;
                        return AssembleBitmap(func);
                    }
                );

                assembleTask.ContinueWith(
                    task =>
                    {
                        _mapMainLayer.Texture = TextureManager.provinces.texture;
                        TextureManager.provinces.texture.Update(TextureManager._24bppRgb, 0, 0, MapSize.x, MapSize.y, task.Result);
                        isHandlingMapMainLayerChange = false;
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
                assembleTask.Start();
            }
        }

        public static int[] AssembleBitmap(Func<Province, int> func)
        {
            int[] pixels = new int[ProvincesPixels.Length];

            Parallel.For(0, MapSize.y, (row) =>
            {
                int color = 0;
                int newColor = 0;

                int start = row * MapSize.x;
                int end = start + MapSize.x;

                for (int i = start; i < end; i++)
                {
                    if (color != ProvincesPixels[i])
                    {
                        color = ProvincesPixels[i];
                        ProvinceManager.TryGetProvince(color, out Province province);
                        if (province != null) newColor = func(province);
                        else newColor = 0;
                    }

                    pixels[i] = newColor;
                }
            });

            return pixels;
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

                        ProvinceBorderManager.Init(ProvincesPixels, (ushort)MapSize.x, (ushort)MapSize.y);
                        ErrorManager.Init(SettingsManager.settings);

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
                if (e.Delta > 0) selectedTexturedPlane.Scale(1.001f);
                else if (e.Delta < 0) selectedTexturedPlane.Scale(0.999f);
            }
            else
            {
                if (e.Delta > 0 && zoomFactor < 0.1f) zoomFactor *= 1.2f;
                else if (e.Delta < 0 && zoomFactor > 0.0004f) zoomFactor *= 0.8f;
            }
            _mousePrevPoint = CalculateMapPos(e.X, e.Y, viewportInfo);
        }

        public static Point2D CalculateMapPos(int eX, int eY, ViewportInfo viewportInfo)
        {
            Point2D point;
            double mapSizeY = _mapMainLayer == null ? 0 : _mapMainLayer.size.y;

            point.x = ((2 * eX - viewportInfo.width) / (viewportInfo.max * zoomFactor) - mapDifX);
            point.y = (mapSizeY + (2 * eY - viewportInfo.height) / (viewportInfo.max * zoomFactor) - mapDifY);
            return point;
        }


        public static void HandleMouseDown(MouseEventArgs e, ViewportInfo viewportInfo, EnumEditLayer enumEditLayer, EnumTool enumTool, string toolParameter)
        {
            Point2D pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            var buttons = e.Button;
            _mousePrevPoint = pos;
            _mouseState = EnumMouseState.DOWN;

            if (enumTool != EnumTool.CURSOR) actionPairs = new List<ActionPair>();

            MapToolsManager.HandleTool(buttons, _mouseState, pos, enumEditLayer, enumTool, bounds, toolParameter);

            if (buttons == MouseButtons.Middle) _isMapDragged = true;

            _mouseState = EnumMouseState.NONE;
        }

        public static void HandleMouseUp(MouseEventArgs e, ViewportInfo viewportInfo, EnumTool enumTool, EnumEditLayer enumEditLayer)
        {
            Point2D pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            var button = e.Button;
            _mouseState = EnumMouseState.UP;

            if (actionPairs != null && actionPairs.Count > 0)
            {
                var list = new List<ActionPair>(actionPairs);
                ActionHistory.Add(
                    () => list.ForEach(pair => pair.undo()),
                    () => list.ForEach(pair => pair.redo())
                );
            }
            actionPairs = null;

            if (enumTool == EnumTool.CURSOR) selectedTexturedPlane = null;
            if (button == MouseButtons.Middle) _isMapDragged = false;
            _mouseState = EnumMouseState.NONE;
        }

        public static void HandleMouseMoved(MouseEventArgs e, ViewportInfo viewportInfo, EnumTool enumTool, EnumEditLayer enumEditLayer, string toolParameter)
        {
            var pos = CalculateMapPos(e.X, e.Y, viewportInfo);
            _mouseState = EnumMouseState.MOVE;

            if (_isMapDragged)
            {
                mapDifX += pos.x - _mousePrevPoint.x;
                mapDifY += pos.y - _mousePrevPoint.y;
            }
            else
            {
                if (selectedTexturedPlane != null)
                {
                    selectedTexturedPlane.Move(pos.x - _mousePrevPoint.x, pos.y - _mousePrevPoint.y);
                }
                else if (actionPairs != null && ((int)_mousePrevPoint.x != (int)pos.x || (int)_mousePrevPoint.y != (int)pos.y))
                {
                    if (enumTool != EnumTool.BUILDINGS)
                        MapToolsManager.HandleTool(e.Button, _mouseState, pos, enumEditLayer, enumTool, bounds, toolParameter);
                }
                _mousePrevPoint = pos;
            }
            _mouseState = EnumMouseState.NONE;
        }

        public static int GetColor(Point2D point) => ProvincesPixels[(int)point.x + (int)point.y * MapSize.x];

        public static void ClearAdditionalMapTextures()
        {
            foreach (TextureInfo info in additionalMapTextures) info.plane.Dispose();
            additionalMapTextures = new List<TextureInfo>();
        }

        public static SegmentedTexturedPlane LoadAdditionalMapTexture(string filePath, string fileName)
        {
            var textures = new List<Texture2D>(0);
            TextureManager.LoadSegmentedTextures(filePath, SettingsManager.settings, textures, out float imageWidth, out float imageHeight);
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
            Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_PROVINCES_BMP_FILE), Application.StartupPath + @"\data\mapChanges", "BMP files (*.bmp)|*.bmp");
            if (fd.ShowDialog() == DialogResult.OK) bmpFilePath = fd.FileName;
            else return;

            fd = new OpenFileDialog();
            Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_DEFINITION_CSV_FILE), Application.StartupPath + @"\data\mapChanges", "CSV files (*.csv)|*.csv");
            if (fd.ShowDialog() == DialogResult.OK) definitionFilePath = fd.FileName;
            else return;

            string[] provinceData = File.ReadAllLines(definitionFilePath);
            Dictionary<int, ushort> provincesByColor = new Dictionary<int, ushort>(provinceData.Length);
            foreach (string provinceString in provinceData)
            {
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
                    if (TextureManager.provincesBorders.texture != null) _bordersMapPlane.Texture = TextureManager.provincesBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.PROVINCES_WHITE:
                    if (TextureManager.provincesBorders.texture != null) _bordersMapPlane.Texture = TextureManager.provincesBorders.texture;
                    blackBorders = false;
                    break;
                case EnumBordersType.STATES_BLACK:
                    if (TextureManager.statesBorders.texture != null) _bordersMapPlane.Texture = TextureManager.statesBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.STATES_WHITE:
                    if (TextureManager.statesBorders.texture != null) _bordersMapPlane.Texture = TextureManager.statesBorders.texture;
                    blackBorders = false;
                    break;
                case EnumBordersType.STRATEGIC_REGIONS_BLACK:
                    if (TextureManager.regionsBorders.texture != null) _bordersMapPlane.Texture = TextureManager.regionsBorders.texture;
                    blackBorders = true;
                    break;
                case EnumBordersType.STRATEGIC_REGIONS_WHITE:
                    if (TextureManager.regionsBorders.texture != null) _bordersMapPlane.Texture = TextureManager.regionsBorders.texture;
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
