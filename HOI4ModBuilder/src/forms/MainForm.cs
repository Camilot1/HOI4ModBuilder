using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.forms;
using HOI4ModBuilder.src.forms.messageForms;
using HOI4ModBuilder.src.forms.recoveryForms;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.tools.map.advanced;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder
{
    partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        public GLControl glControl;
        public static bool firstLoad = false;
        public static bool errorsOrExceptionsDuringLoading = false;
        public static bool[] isLoadingOrSaving = new bool[1];
        public static bool updateGLControl = true;
        public static bool isMapMainLayerChangeEnabled = false;

        private Color brushFirstColor, brushSecondColor;
        private ViewportInfo viewportInfo;
        public EnumMainLayer enumMainLayer = EnumMainLayer.PROVINCES_MAP;
        private EnumTool enumTool = EnumTool.CURSOR;
        private EnumEditLayer enumEditLayer = EnumEditLayer.PROVINCES;
        private EnumBordersType enumBordersType = EnumBordersType.PROVINCES_BLACK;
        public EnumBordersType EnumBordersType => enumBordersType;

        // Сохраняем делегат в статическом поле для избежания ошибки "CallbackOnCollectedDelegate"
        private static DebugProc debugProc;

        private static readonly Dictionary<Keys, List<KeyEventHandler>> _globaldPressButtonsEvents = new Dictionary<Keys, List<KeyEventHandler>>();
        private static readonly Dictionary<TabPage, Dictionary<Keys, List<KeyEventHandler>>> _tabRelatedPressButtonsEvents = new Dictionary<TabPage, Dictionary<Keys, List<KeyEventHandler>>>();

        private static readonly List<Action> _guiReinitActions = new List<Action>();

        public MainForm()
        {
            Init();
            AfterFirstInit();
        }

        public void Reinit()
        {
            Controls.Clear();
            Init();
            foreach (var action in _guiReinitActions) action();
        }

        private void Init()
        {
            InitializeComponent();
            Text += $" [{Logger.version}]";

            if (glControl == null)
            {
                glControl = new GLControl();
                glControl.Dock = DockStyle.Fill;
                glControl.Load += GLControl_Load;
                glControl.Resize += GLControl_Resize;
                glControl.Paint += GLControl_Paint;
                glControl.MouseWheel += new MouseEventHandler(Panel1_MouseWheel);
                glControl.MouseDown += new MouseEventHandler(Panel1_MouseDown);
                glControl.MouseUp += new MouseEventHandler(Panel1_MouseUp);
                glControl.MouseMove += new MouseEventHandler(Panel1_MouseMove);
            }
            Panel_Map.Controls.Add(glControl);


            СomboBox_MapMainLayer.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumMainLayer)))
                СomboBox_MapMainLayer.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            СomboBox_MapMainLayer.SelectedIndex = 0;

            ComboBox_EditLayer.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumEditLayer)))
                ComboBox_EditLayer.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            ComboBox_EditLayer.SelectedIndex = 0;

            ComboBox_Tool.Items.Clear();
            foreach (EnumTool type in Enum.GetValues(typeof(EnumTool)))
            {
                string hotKey = "";
                if (MapToolsManager.TryGetMapTool(type, out MapTool mapTool))
                {
                    if (mapTool.HotKey != null) hotKey = " " + mapTool.HotKey.ToString();
                }
                ComboBox_Tool.Items.Add(GuiLocManager.GetLoc(type.ToString()) + hotKey);
            }
            ComboBox_Tool.SelectedIndex = 0;

            ComboBox_BordersType.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumBordersType)))
                ComboBox_BordersType.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            ComboBox_BordersType.SelectedIndex = 0;

            CheckedListBox_MapAdditionalLayers.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumAdditionalLayers)))
                CheckedListBox_MapAdditionalLayers.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            CheckedListBox_MapAdditionalLayers.Height = 15 * CheckedListBox_MapAdditionalLayers.Items.Count + 10;

            ToolStripComboBox_Map_Railway_Level.SelectedIndex = 0;
            ComboBox_GenerateColor_Type.SelectedIndex = 0;

            ToolStripComboBox_Map_Adjacency_Type.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumAdjaciencyType)))
                ToolStripComboBox_Map_Adjacency_Type.Items.Add(type.ToString());

            ToolStripComboBox_Map_Adjacency_Type.SelectedIndex = 0;

            switch (GuiLocManager.GetCurrentParentLanguageName)
            {
                case "ru":
                    ToolStripMenuItem_Language.Text = ToolStripMenuItem_Language_RU.Text;
                    ToolStripMenuItem_Language.Image = ToolStripMenuItem_Language_RU.Image;
                    break;
                case "en":
                    ToolStripMenuItem_Language.Text = ToolStripMenuItem_Language_EN.Text;
                    ToolStripMenuItem_Language.Image = ToolStripMenuItem_Language_EN.Image;
                    break;
            }

            MenuStrip1.Refresh();
        }

        private void AfterFirstInit()
        {
            Instance = this;
            Logger.TryOrLog(() =>
            {
                GuiLocManager.formsReinitEvents.Add(this, () => Reinit());

                SettingsManager.Init();
                MapManager.Init();

                ComboBox_Tool.Items.Clear();
                foreach (EnumTool type in Enum.GetValues(typeof(EnumTool)))
                {
                    string hotKey = "";
                    if (MapToolsManager.TryGetMapTool(type, out MapTool mapTool))
                    {
                        if (mapTool.HotKey != null) hotKey = " " + mapTool.HotKey.ToString();
                    }
                    ComboBox_Tool.Items.Add(GuiLocManager.GetLoc(type.ToString()) + hotKey);
                }
                ComboBox_Tool.SelectedIndex = 0;

                SubscribeGlobalKeyEvent(Keys.S, (sender, e) =>
                {
                    if (e.Modifiers == Keys.Control) SaveAll();
                });
                SubscribeGlobalKeyEvent(Keys.L, (sender, e) =>
                {
                    if (e.Modifiers == Keys.Control) LoadAll();
                });
                SubscribeGlobalKeyEvent(Keys.U, (sender, e) =>
                {
                    if (e.Modifiers == Keys.Control) UpdateAll();
                });
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            TextureManager.DisposeAllTextures();
        }

        public void InvokeAction(Action action)
            => Invoke((MethodInvoker)delegate { action(); });

        public void TryInvokeActionOrLog(Action tryAction, Action<Exception> catchAction)
            => Invoke((MethodInvoker)delegate { Logger.TryOrCatch(tryAction, catchAction); });

        private void SaveAll()
        {
            Logger.TryOrLog(() =>
            {
                //TODO Добавить это для обновления (CTLR+U)

                if (!firstLoad)
                {
                    Logger.LogSingleMessage(EnumLocKey.CANT_SAVE_BECAUSE_NO_DATA_WAS_LOADED);
                    return;
                }
                else if (errorsOrExceptionsDuringLoading)
                {
                    Logger.LogSingleMessage(EnumLocKey.CANT_SAVE_BECAUSE_OF_LOADING_ERRORS_OR_EXCEPTIONS);
                    return;
                }
                else if (isLoadingOrSaving[0])
                {
                    Logger.LogSingleMessage(EnumLocKey.CANT_SAVE_BECAUSE_ALREADY_SAVING_OR_LOADING);
                    return;
                }
                else if (!SettingsManager.settings.IsModDirectorySelected())
                {
                    Logger.LogSingleMessage(EnumLocKey.CANT_SAVE_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS);
                    return;
                }

                var settings = SettingsManager.settings;

                isLoadingOrSaving[0] = true;

                Logger.Log("Saving...");

                LocalModDataManager.SaveLocalSettings(settings);
                TextureManager.SaveAllMaps(settings);
                ProvinceManager.SaveProvinces(settings);
                AdjacenciesManager.Save(settings);
                SupplyManager.SaveAll(settings);
                StateManager.Save(settings);
                StrategicRegionManager.Save(settings);

                AiAreaManager.Save(settings);

                Utils.CleanUpMemory();
                isLoadingOrSaving[0] = false;
            },
            () =>
            {
                if (Logger.ExceptionsCount == 0)
                {
                    DisplayProgress(
                        EnumLocKey.PROGRESSBAR_SAVED,
                        new Dictionary<string, string>
                        {
                            { "{time}", $"{DateTime.Now.ToLongTimeString()}" },
                            { "{warningsCount}", $"{Logger.WarningsCount}" },
                            { "{errorsCount}", $"{Logger.ErrorsCount}" },
                            { "{exceptionsCount}", $"{Logger.ExceptionsCount}" },
                        },
                        0
                    );

                    if (Logger.ErrorsCount > 0) GroupBox_Progress.BackColor = Color.OrangeRed;
                    else if (Logger.WarningsCount > 0) GroupBox_Progress.BackColor = Color.Yellow;
                    else GroupBox_Progress.BackColor = Color.White;
                }
                else
                {
                    DisplayProgress(
                        EnumLocKey.PROGRESSBAR_SAVING_FAILED,
                        new Dictionary<string, string>
                        {
                            { "{time}", $"{DateTime.Now.ToLongTimeString()}" },
                            { "{warningsCount}", $"{Logger.WarningsCount}" },
                            { "{errorsCount}", $"{Logger.ErrorsCount}" },
                            { "{exceptionsCount}", $"{Logger.ExceptionsCount}" },
                        },
                        0
                    );
                    GroupBox_Progress.BackColor = Color.Red;
                }

                MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);

                Logger.DisplayWarnings();
                Logger.DisplayErrors();
                Logger.DisplayExceptions();
                Utils.CleanUpMemory();
                isLoadingOrSaving[0] = false;
            });

        }

        private void LoadAll()
        {
            if (isLoadingOrSaving[0])
            {
                Logger.LogSingleMessage(EnumLocKey.CANT_LOAD_BECAUSE_ALREADY_SAVING_OR_LOADING);
                return;
            }
            else if (!SettingsManager.settings.IsModDirectorySelected())
            {
                Logger.LogSingleMessage(EnumLocKey.CANT_LOAD_BECAUSE_MOD_DIRECTORY_ISNT_SELECTED_OR_DOESNT_EXISTS);
                return;
            }

            firstLoad = true;
            isLoadingOrSaving[0] = true;

            Logger.Log("Loading...");

            if (glControl.Context == null)
            {
                Logger.LogSingleMessage("Can't load data. glControl.Context == null");
                isLoadingOrSaving[0] = false;
                return;
            }

            PauseGLControl();
            GroupBox_Progress.BackColor = Color.White;


            Task.Run(() =>
            {
                Logger.TryOrLog(
                    () =>
                    {
                        var context = new GraphicsContext(GraphicsMode.Default, glControl.WindowInfo);
                        context.MakeCurrent(glControl.WindowInfo);

                        Logger.CloseAllTextBoxMessageForms();

                        LoadAllData(SettingsManager.settings);
                        context.MakeCurrent(null);
                    },
                    () => TryInvokeActionOrLog(
                        () =>
                        {
                            if (Logger.ExceptionsCount == 0)
                            {
                                DisplayProgress(
                                    EnumLocKey.PROGRESSBAR_LOADED,
                                    new Dictionary<string, string>
                                    {
                                        { "{warningsCount}", $"{Logger.WarningsCount}" },
                                        { "{errorsCount}", $"{Logger.ErrorsCount}" },
                                        { "{exceptionsCount}", $"{Logger.ExceptionsCount}" },
                                    },
                                    0
                                );

                                if (Logger.ErrorsCount > 0) GroupBox_Progress.BackColor = Color.OrangeRed;
                                else if (Logger.WarningsCount > 0) GroupBox_Progress.BackColor = Color.Yellow;
                                else GroupBox_Progress.BackColor = Color.White;
                            }
                            else
                            {
                                DisplayProgress(
                                    EnumLocKey.PROGRESSBAR_LOADING_FAILED,
                                    new Dictionary<string, string>
                                    {
                                        { "{warningsCount}", $"{Logger.WarningsCount}" },
                                        { "{errorsCount}", $"{Logger.ErrorsCount}" },
                                        { "{exceptionsCount}", $"{Logger.ExceptionsCount}" },
                                    },
                                    0
                                );
                                GroupBox_Progress.BackColor = Color.Red;
                            }

                            ResumeGLControl();
                            if (ToolStripComboBox_Data_Bookmark.Items.Count > 0) ToolStripComboBox_Data_Bookmark.SelectedIndex = 0;
                            UpdateSelectedTool();
                            UpdateBordersType();
                            MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                            //MapManager.LoadSDFThings();

                            if (Logger.ErrorsCount > 0 || Logger.ExceptionsCount > 0) errorsOrExceptionsDuringLoading = true;
                            else errorsOrExceptionsDuringLoading = false;

                            Logger.DisplayWarnings();
                            Logger.DisplayErrors();
                            Logger.DisplayExceptions();
                            Utils.CleanUpMemory();
                            isLoadingOrSaving[0] = false;
                        },
                        (ex) =>
                        {
                            Logger.LogException(ex);

                            if (Logger.ErrorsCount > 0 || Logger.ExceptionsCount > 0) errorsOrExceptionsDuringLoading = true;
                            else errorsOrExceptionsDuringLoading = false;

                            Logger.DisplayWarnings();
                            Logger.DisplayErrors();
                            Logger.DisplayExceptions();
                            Utils.CleanUpMemory();
                            isLoadingOrSaving[0] = false;
                        }
                    )
                );
            });
        }

        public static void PauseGLControl()
        {
            updateGLControl = false;
            Instance.glControl.Context.MakeCurrent(null);
        }

        public static void ResumeGLControl()
        {
            Instance.glControl.Context.MakeCurrent(Instance.glControl.WindowInfo);
            Instance.glControl.Invalidate();
            updateGLControl = true;
            Instance.GLControl_Resize(null, null);
        }

        private void LoadAllData(Settings settings)
        {
            LocalModDataManager.Load(settings);

            isMapMainLayerChangeEnabled = false;

            MapManager.ActionHistory.Clear();

            //Logger.Log($"Выполняю загрузку шрифтов");
            //FontManager.LoadFonts();

            Logger.Log($"Loading mod directory: {SettingsManager.settings.modDirectory}");
            DataManager.Load(SettingsManager.settings);
            MapManager.Load(SettingsManager.settings);

            ErrorManager.Init(settings);

            isMapMainLayerChangeEnabled = true;
        }

        public static void ExecuteActions(LocalizedAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                Logger.Log($"Perform action {actions[i].LocKey} ({i + 1}/{actions.Length})...");
                DisplayProgress(actions[i].LocKey, null, $"({i + 1}/{actions.Length})", (i + 1) / (float)actions.Length);
                actions[i].Action();
            }
        }

        public static void DisplayProgress(EnumLocKey enumLocKey, float progress)
            => DisplayProgress(enumLocKey, null, null, progress);
        public static void DisplayProgress(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, float progress)
            => DisplayProgress(enumLocKey, replaceValues, null, progress);

        public static void DisplayProgress(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, string additionalText, float progress)
        {
            Instance.TryInvokeActionOrLog(
            () =>
            {
                Instance.GroupBox_Progress.Text = GuiLocManager.GetLoc(enumLocKey, replaceValues, additionalText);
                Instance.GroupBox_Progress.Update();
                if (progress > 1) Instance.ProgressBar1.Value = 100;
                else if (progress < 0) Instance.ProgressBar1.Value = 0;
                else Instance.ProgressBar1.Value = (int)Math.Round(progress * 100);
                Instance.ProgressBar1.Update();
            },
                (ex) => Logger.LogException(ex)
            );
        }

        private void GLControl_Load(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.Enable(EnableCap.DebugOutput);

                debugProc = OnDebugMessage;
                GL.DebugMessageCallback(debugProc, IntPtr.Zero);

                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
            });
        }

        private void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"OpenGL debug message: {msg}");
        }

        private void GLControl_Resize(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                viewportInfo.width = glControl.Width;
                viewportInfo.height = glControl.Height;

                viewportInfo.max = viewportInfo.width > viewportInfo.height ? viewportInfo.width : viewportInfo.height;

                viewportInfo.x = (viewportInfo.width - viewportInfo.max) / 2;
                viewportInfo.y = (viewportInfo.height - viewportInfo.max) / 2;
                GL.Viewport(viewportInfo.x, viewportInfo.y, viewportInfo.max, viewportInfo.max);
                GL.ClearColor(Color4.LightBlue);
            });
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            if (!updateGLControl) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color.LightBlue);

            GL.MatrixMode(MatrixMode.Projection);

            MapManager.Draw();

            glControl.Invalidate();
            glControl.SwapBuffers();
        }

        public void SetBrushFirstColor(Color color)
        {
            brushFirstColor = color;
            Panel_FirstColor.BackColor = color;
        }

        public Color GetBrushFirstColor() => brushFirstColor;
        public void SetBrushSecondColor(Color color)
        {
            brushSecondColor = color;
            Panel_SecondColor.BackColor = color;
        }

        public Color GetBrushSecondColor() => brushSecondColor;
        private void Panel1_MouseWheel(object sender, MouseEventArgs e) => MapManager.HandleMouseWheel(e, viewportInfo);

        private void CheckedListBox_MapAdditionalLayers_MouseUp(object sender, MouseEventArgs e)
        {
            var checkedItems = CheckedListBox_MapAdditionalLayers.CheckedIndices;

            for (int i = 0; i < Enum.GetValues(typeof(EnumAdditionalLayers)).Length; i++)
            {
                MapManager.displayLayers[i] = checkedItems.Contains(i);
            }
        }

        private void Button_TextureAdd_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var fd = new OpenFileDialog();
                Utils.PrepareFileDialog(fd, Application.StartupPath + @"\data\textures\map", "BMP files (*.bmp)|*.bmp");

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = fd.FileName;
                    Utils.GetFileNameAndFormat(filePath, out string fileName, out string fileFormat);

                    if (fileName == null)
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_TEXTURE_FILE_HAS_NO_NAME,
                            new Dictionary<string, string> { { "{filePath}", filePath } }
                        ));

                    var fileScale = Utils.GetFileScaleFromName(fileName);
                    var texturePlane = MapManager.LoadAdditionalMapTexture(filePath, fileName);
                    if (texturePlane == null) return;

                    double scaleFactor = 1;
                    if (fileScale.Item1 == EnumFileScale.HEIGHT)
                    {
                        scaleFactor = fileScale.Item2 /
                            (SettingsManager.settings.MAP_VIEWPORT_HEIGHT *
                            SettingsManager.settings.GetMapScalePixelToKM() * 1000d);
                    }

                    texturePlane.Scale((float)scaleFactor);
                    texturePlane.MoveTo(-MapManager.mapDifX - texturePlane.size.x / 2d, MapManager.MapSize.y + texturePlane.size.y / 2d - MapManager.mapDifY);

                    ListBox_Textures.Items.Clear();
                    foreach (var name in MapManager.GetAdditionalMapTexturesNames())
                        ListBox_Textures.Items.Add(name);
                }
            });
        }

        private void Button_TextureMoveUp_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                int selectIndex = ListBox_Textures.SelectedIndex;

                MapManager.MoveUpAdditionalMapTexture(selectIndex);
                ListBox_Textures.Items.Clear();
                foreach (var name in MapManager.GetAdditionalMapTexturesNames())
                    ListBox_Textures.Items.Add(name);

                ListBox_Textures.SelectedIndex = selectIndex <= 0 ? 0 : selectIndex - 1;
            });
        }

        private void Button_TextureMoveDown_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                int selectIndex = ListBox_Textures.SelectedIndex;

                MapManager.MoveDownAdditionalMapTexture(selectIndex);
                ListBox_Textures.Items.Clear();
                foreach (var name in MapManager.GetAdditionalMapTexturesNames())
                    ListBox_Textures.Items.Add(name);

                ListBox_Textures.SelectedIndex = selectIndex < ListBox_Textures.Items.Count - 1 ? selectIndex + 1 : selectIndex;
            });
        }

        private void Button_TextureRemove_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                MapManager.RemoveAdditionalMapTexture(ListBox_Textures.SelectedIndex);
                ListBox_Textures.Items.Clear();
                foreach (var name in MapManager.GetAdditionalMapTexturesNames())
                    ListBox_Textures.Items.Add(name);
            });
        }

        private void Button_TexturesSave_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var fd = new SaveFileDialog();
                Utils.PrepareFileDialog(fd, Application.StartupPath + @"\data\textures\map\data", "JSON files (*.json)|*.json");

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = fd.FileName;
                    string json = JsonConvert.SerializeObject(MapManager.additionalMapTextures, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
            });
        }

        private void Button_TexturesLoad_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var fd = new OpenFileDialog();
                Utils.PrepareFileDialog(fd, Application.StartupPath + @"\data\textures\map\data", "JSON files (*.json)|*.json");

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    MapManager.ClearAdditionalMapTextures();
                    ListBox_Textures.Items.Clear();
                    string filePath = fd.FileName;
                    var tempDictionary = JsonConvert.DeserializeObject<Dictionary<string, TextureInfo>>(File.ReadAllText(filePath));

                    foreach (string key in tempDictionary.Keys)
                    {
                        try
                        {
                            var info = tempDictionary[key];
                            var plane = MapManager.LoadAdditionalMapTexture(info.filePath, key);
                            plane.LoadTextureInfo(info);
                            ListBox_Textures.Items.Add(key);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(
                                EnumLocKey.EXCEPTION_WHILE_TEXTURE_LOADING,
                                new Dictionary<string, string>
                                {
                                    { "{name}", key },
                                    { "{exceptionMessage}", ex.Message }
                                },
                                ex
                            );
                        }
                    }
                }
            });
        }

        public static void SetBrushColor(int type, Color color)
        {
            if (type == 0)
            {
                Instance.brushFirstColor = color;
                Instance.Panel_FirstColor.BackColor = color;
            }
            else if (type == 1)
            {
                Instance.brushFirstColor = color;
                Instance.Panel_SecondColor.BackColor = color;
            }
        }

        private void СomboBox_MapMainLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                enumMainLayer = (EnumMainLayer)СomboBox_MapMainLayer.SelectedIndex;
                if (enumMainLayer == EnumMainLayer.BUILDINGS && !BuildingManager.HasBuilding(ComboBox_Tool_Parameter.Text))
                    UpdateToolParameterComboBox(BuildingManager.GetBuildingNames().GetEnumerator());
                MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
            });
        }

        private void ComboBox_Tool_SelectedIndexChanged(object sender, EventArgs e) => UpdateSelectedTool();

        private void UpdateSelectedTool()
        {
            Logger.TryOrLog(() =>
            {
                enumTool = (EnumTool)ComboBox_Tool.SelectedIndex;
                switch (enumTool)
                {
                    case EnumTool.CURSOR:
                    case EnumTool.RECTANGLE:
                    case EnumTool.ELLIPSE:
                    case EnumTool.MAGIC_WAND:
                    case EnumTool.BRUSH:
                    case EnumTool.FILL:
                    case EnumTool.ERASER:
                    case EnumTool.PIPETTE:
                    case EnumTool.PROVINCE_COASTAL:
                        UpdateToolParameterComboBox(null);
                        break;

                    case EnumTool.PROVINCE_TYPE:
                        UpdateToolParameterComboBox(ToolStripComboBox_Map_Province_Type.Items.GetEnumerator());
                        break;
                    case EnumTool.TERRAIN:
                        UpdateToolParameterComboBox(ToolStripComboBox_Map_Province_Terrain.Items.GetEnumerator());
                        break;
                    case EnumTool.PROVINCE_CONTINENT:
                        UpdateToolParameterComboBox(ToolStripComboBox_Map_Province_Continent.Items.GetEnumerator());
                        break;
                    case EnumTool.PROVINCE_STATE:
                        var stateIds = new List<ushort>(StateManager.GetStatesIds());
                        stateIds.Sort();
                        UpdateToolParameterComboBox(stateIds.GetEnumerator());
                        break;
                    case EnumTool.PROVINCE_REGION:
                        var regionsIds = new List<ushort>(StrategicRegionManager.GetRegionsIds());
                        regionsIds.Sort();
                        UpdateToolParameterComboBox(regionsIds.GetEnumerator());
                        break;
                    case EnumTool.STATE_CATEGORY:
                        var stateCategories = new List<string>(StateCategoryManager.GetStateCategoriesNames());
                        UpdateToolParameterComboBox(stateCategories.GetEnumerator());
                        break;
                    case EnumTool.BUILDINGS:
                        if (!BuildingManager.HasBuilding(ComboBox_Tool_Parameter.Text))
                            UpdateToolParameterComboBox(BuildingManager.GetBuildingNames().GetEnumerator());
                        break;
                }

                if (enumTool == EnumTool.CURSOR) Panel_Map.ContextMenuStrip = ContextMenuStrip_Map;
                else Panel_Map.ContextMenuStrip = null;
            });
        }

        private void UpdateToolParameterComboBox(IEnumerator items)
        {
            if (items != null)
            {
                ComboBox_Tool_Parameter.Items.Clear();
                while (items.MoveNext()) ComboBox_Tool_Parameter.Items.Add(items.Current);
                if (ComboBox_Tool_Parameter.Items.Count > 0) ComboBox_Tool_Parameter.SelectedIndex = 0;
            }
            else
            {
                ComboBox_Tool_Parameter.Items.Clear();
                ComboBox_Tool_Parameter.Text = "";
            }

            int itemHeight = ComboBox_Tool_Parameter.GetItemHeight(0);
            int visibleItems = Math.Min(ComboBox_Tool_Parameter.Items.Count, 30);
            int dropDownHeight = itemHeight * visibleItems + SystemInformation.BorderSize.Height * 2;
            ComboBox_Tool_Parameter.DropDownHeight = dropDownHeight;

            ComboBox_Tool_Parameter.Refresh();
        }

        private void ComboBox_EditLayer_SelectedIndexChanged(object sender, EventArgs e)
            => enumEditLayer = (EnumEditLayer)ComboBox_EditLayer.SelectedIndex;
        private void Panel1_MouseDown(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseDown(e, viewportInfo, enumEditLayer, enumTool, ComboBox_Tool_Parameter.Text));
        private void Panel1_MouseUp(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseUp(e, viewportInfo, enumTool, enumEditLayer));
        private void Panel1_MouseMove(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseMoved(e, viewportInfo, enumTool, enumEditLayer, ComboBox_Tool_Parameter.Text));
        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
            => Application.Exit();
        private void ToolStripMenuItem_Save_Maps_Provinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveProvincesMap());
        private void ToolStripMenuItem_Save_Maps_Rivers_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveRiversMap(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Maps_All_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveAllMaps(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Definition_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => ProvinceManager.SaveProvinces(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Adjacencies_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.Save(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Supply_All_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveAll(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Supply_Railways_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveRailways(SettingsManager.settings.modDirectory + @"map\"));
        private void ToolStripMenuItem_Save_Supply_Hubs_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveSupplyNodes(SettingsManager.settings.modDirectory + @"map\"));
        private void ToolStripMenuItem_LoadAll_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => LoadAll());
        private void ToolStripMenuItem_Help_About_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AboutProgramForm.CreateTasked());


        public static void SubscribeGuiReinitAction(Action action) => _guiReinitActions.Add(action);

        public static void SubscribeTabKeyEvent(TabPage tabPage, Keys key, KeyEventHandler eventHandler)
        {
            if (!_tabRelatedPressButtonsEvents.TryGetValue(tabPage, out var keysEvents))
            {
                keysEvents = new Dictionary<Keys, List<KeyEventHandler>>();
                _tabRelatedPressButtonsEvents.Add(tabPage, keysEvents);
            }

            if (!keysEvents.TryGetValue(key, out var eventHandlers))
            {
                eventHandlers = new List<KeyEventHandler>();
                keysEvents.Add(key, eventHandlers);
            }

            eventHandlers.Add(eventHandler);
        }

        public static void SubscribeGlobalKeyEvent(Keys key, KeyEventHandler eventHandler)
        {
            if (!_globaldPressButtonsEvents.TryGetValue(key, out var eventHandlers))
            {
                eventHandlers = new List<KeyEventHandler>();
                _globaldPressButtonsEvents.Add(key, eventHandlers);
            }

            eventHandlers.Add(eventHandler);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled) return;

            Logger.TryOrLog(() =>
            {
                if (
                    _tabRelatedPressButtonsEvents.TryGetValue(TabControl_Main.SelectedTab, out var keysEvents) &&
                    keysEvents.TryGetValue(e.KeyCode, out var tabRelatedKeyEventHandlers)
                )
                {
                    foreach (var handler in tabRelatedKeyEventHandlers) handler.Invoke(this, e);
                }

                if (_globaldPressButtonsEvents.TryGetValue(e.KeyCode, out var globalKeyEventHandlers))
                {
                    foreach (var handler in globalKeyEventHandlers) handler.Invoke(this, e);
                }

                e.Handled = true;
            });
        }

        public EnumTool SelectedTool => (EnumTool)ComboBox_Tool.SelectedIndex;

        public void SetSelectedTool(EnumTool enumTool)
        {
            Logger.TryOrLog(() =>
            {
                ComboBox_Tool.SelectedIndex = (int)enumTool;
                ComboBox_Tool.Refresh();
            });
        }

        private void ToolStripMenuItem_Settings_Click(object sender, EventArgs e) => Logger.TryOrLog(() => new SettingsForm().ShowDialog());

        private void ToolStripMenuItem_Map_SupplyHub_Create_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyNodeTool.AddNode(SupplyNodeTool.CreateNode(1, ProvinceManager.RMBProvince)));

        private void ToolStripMenuItem_Map_SupplyHub_Remove_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyNodeTool.RemoveNode(ProvinceManager.RMBProvince?.SupplyNode));

        public byte SelectedRailwayLevel
        {
            get => (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1);
            set => ToolStripComboBox_Map_Railway_Level.SelectedIndex = value - 1;
        }

        private void ToolStripMenuItem_Map_Railway_Create_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                RailwayTool.CreateRailway(
                    (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1),
                    ProvinceManager.SelectedProvince,
                    ProvinceManager.RMBProvince
                );
            });
        }

        private void ToolStripMenuItem_Map_Railway_Remove_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwayTool.RemoveRailway(SupplyManager.SelectedRailway));

        private void ToolStripComboBox_Map_Railway_Level_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                RailwayTool.ChangeRailwayLevel(
                    SupplyManager.SelectedRailway,
                    (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1)
                );
            });
        }

        private void ToolStripMenuItem_Map_Province_DropDownOpened(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                isMapMainLayerChangeEnabled = false;

                ToolStripComboBox_Map_Province_Type.Text = "";
                ToolStripMenuItem_Map_Province_IsCoastal.Checked = false;
                ToolStripComboBox_Map_Province_Terrain.Text = "";
                ToolStripComboBox_Map_Province_Continent.Text = "";

                Province p = ProvinceManager.RMBProvince;
                if (p != null)
                {
                    ToolStripComboBox_Map_Province_Type.Text = p.GetTypeString();
                    ToolStripMenuItem_Map_Province_IsCoastal.Checked = p.IsCoastal;
                    ToolStripComboBox_Map_Province_Terrain.Text = p.Terrain == null ? "unknown" : p.Terrain.name;
                    ToolStripComboBox_Map_Province_Continent.Text = ContinentManager.GetContinentById(p.ContinentId);
                }

                ToolStripMenuItem_Map_Province_OpenStateFile.Enabled = p != null && p.State != null;
                //TODO
                //ToolStripMenuItem_Map_Province_OpenRegionFile.Enabled = p != null && p.Region != null;

                isMapMainLayerChangeEnabled = true;
            });
        }

        private void ToolStripComboBox_Map_Province_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && isMapMainLayerChangeEnabled)
            {
                var prevType = ProvinceManager.RMBProvince.Type;
                Enum.TryParse(ToolStripComboBox_Map_Province_Type.Text.ToUpper(), out EnumProvinceType newType);

                void action(EnumProvinceType type)
                {
                    ProvinceManager.RMBProvince.Type = type;
                    MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newType), () => action(prevType));
            }
        }

        private void ToolStripMenuItem_Map_Province_IsCoastal_Click(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && isMapMainLayerChangeEnabled)
            {
                bool prevIsCoastal = ProvinceManager.RMBProvince.IsCoastal;
                bool newIsCoastal = !prevIsCoastal;

                void action(bool isCoastal)
                {
                    ToolStripMenuItem_Map_Province_IsCoastal.Checked = isCoastal;
                    ProvinceManager.RMBProvince.IsCoastal = isCoastal;
                    MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newIsCoastal), () => action(prevIsCoastal));
            }
        }

        private void ToolStripComboBox_Map_Province_Terrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && isMapMainLayerChangeEnabled)
            {
                var prevTerrain = ProvinceManager.RMBProvince.Terrain;
                TerrainManager.TryGetProvincialTerrain(ToolStripComboBox_Map_Province_Terrain.Text, out ProvincialTerrain newTerrain);

                void action(ProvincialTerrain terrain)
                {
                    ProvinceManager.RMBProvince.Terrain = terrain;
                    MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newTerrain), () => action(prevTerrain));
            }
        }

        private void ToolStripComboBox_Map_Province_Continent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && isMapMainLayerChangeEnabled)
            {
                byte prevContinentId = ProvinceManager.RMBProvince.ContinentId;
                byte newContinentId = (byte)ContinentManager.GetContinentId(ToolStripComboBox_Map_Province_Continent.Text);

                void action(byte continentId)
                {
                    ProvinceManager.RMBProvince.ContinentId = continentId;
                    MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newContinentId), () => action(prevContinentId));
            }
        }

        private void ToolStripMenuItem_Map_Search_Province_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                string value = ToolStripTextBox_Map_Search_Input.Text;
                if (ushort.TryParse(value, out ushort id))
                {
                    ProvinceManager.TryGetProvince(id, out Province p);
                    if (p != null)
                    {
                        ProvinceManager.SelectedProvince = p;
                        MapManager.FocusOn(p.center);
                    }
                }
            });
        }

        private void ToolStripMenuItem_Edit_Undo_Click(object sender, EventArgs e)
        {
            if (TabControl_Main.SelectedTab == TabPage_Map) MapManager.ActionHistory.Undo();
        }

        private void ToolStripMenuItem_Edit_Redo_Click(object sender, EventArgs e)
        {
            if (TabControl_Main.SelectedTab == TabPage_Map) MapManager.ActionHistory.Redo();
        }

        private void ToolStripMenuItem_SaveAll_Click(object sender, EventArgs e) => Logger.TryOrLog(() => SaveAll());

        private void ToolStripMenuItem_Map_Railway_DropDownOpened(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (
                    ProvinceManager.SelectedProvince == null || ProvinceManager.RMBProvince == null ||
                    ProvinceManager.SelectedProvince.Type != EnumProvinceType.LAND || ProvinceManager.RMBProvince.Type != EnumProvinceType.LAND
                    )
                    ToolStripMenuItem_Map_Railway_Create.Enabled = false;
                else if (
                    ProvinceManager.SelectedProvince.HasBorderWith(ProvinceManager.RMBProvince) ||
                    ProvinceManager.SelectedProvince.HasSeaConnectionWith(ProvinceManager.RMBProvince)
                    )
                    ToolStripMenuItem_Map_Railway_Create.Enabled = true;
                else
                    ToolStripMenuItem_Map_Railway_Create.Enabled = false;

                if (SupplyManager.SelectedRailway != null)
                {
                    ToolStripComboBox_Map_Railway_Level.SelectedIndex = SupplyManager.SelectedRailway.Level - 1;

                    ToolStripMenuItem_Map_Railway_AddProvince.Enabled = SupplyManager.SelectedRailway.CanAddProvince(ProvinceManager.RMBProvince);
                    ToolStripMenuItem_Map_Railway_RemoveProvince.Enabled = SupplyManager.SelectedRailway.CanRemoveProvince(ProvinceManager.RMBProvince);

                    ToolStripMenuItem_Map_Railway_Split.Enabled = SupplyManager.SelectedRailway.CanSplitAtProvince(ProvinceManager.RMBProvince, out _);
                    ToolStripMenuItem_Map_Railway_Join.Enabled = SupplyManager.SelectedRailway.CanJoin(SupplyManager.RMBRailway, out _, out _, out _);

                    ToolStripMenuItem_Map_Railway_Remove.Enabled = true;
                }
                else
                {
                    ToolStripMenuItem_Map_Railway_AddProvince.Enabled = false;
                    ToolStripMenuItem_Map_Railway_RemoveProvince.Enabled = false;
                    ToolStripMenuItem_Map_Railway_Split.Enabled = false;
                    ToolStripMenuItem_Map_Railway_Join.Enabled = false;

                    ToolStripMenuItem_Map_Railway_Remove.Enabled = false;
                }
            });
        }

        private void ToolStripMenuItem_Map_Search_Position_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                string[] values = ToolStripTextBox_Map_Search_Input.Text.Split(',');
                if (values.Length != 2) values = ToolStripTextBox_Map_Search_Input.Text.Split(';');
                if (values.Length != 2) values = ToolStripTextBox_Map_Search_Input.Text.Split(' ');
                if (values.Length != 2)
                {
                    Logger.LogSingleMessage(
                        EnumLocKey.SINGLE_MESSAGE_SEARCH_POSITION_INCORRECT_SEPARATOR,
                        new Dictionary<string, string> { { "{allowedSeparators}", "1) ',' 2) ';' 3) ' '" } }
                    );
                    return;
                }

                try
                {
                    float x = float.Parse(values[0].Trim());
                    float y = float.Parse(values[1].Trim());
                    MapManager.FocusOn(x, y);
                }
                catch (Exception _)
                {
                    Logger.LogSingleMessage(EnumLocKey.SINGLE_MESSAGE_SEARCH_POSITION_INCORRECT_COORDS);
                    return;
                }
            });
        }

        private void ToolStripMenuItem_Map_Search_State_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                string value = ToolStripTextBox_Map_Search_Input.Text;
                if (ushort.TryParse(value, out ushort id))
                {
                    if (StateManager.TryGetState(id, out State state))
                    {
                        MapManager.FocusOn(state.center);
                    }
                }
            });
        }

        private void ToolStripMenuItem_Map_Search_Region_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                string value = ToolStripTextBox_Map_Search_Input.Text;
                if (ushort.TryParse(value, out ushort id))
                {
                    StrategicRegionManager.TryGetRegion(id, out StrategicRegion region);
                    if (region != null)
                    {
                        MapManager.FocusOn(region.center);
                    }
                }
            });
        }

        private void ToolStripMenuItem_Map_SupplyHub_DropDownOpened(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince != null)
                {
                    ToolStripMenuItem_Map_SupplyHub_Create.Enabled = ProvinceManager.RMBProvince.SupplyNode == null && ProvinceManager.RMBProvince.Type == EnumProvinceType.LAND;
                    ToolStripMenuItem_Map_SupplyHub_Remove.Enabled = ProvinceManager.RMBProvince.SupplyNode != null;
                }
            });

        }

        private void ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                ProvinceManager.AutoToolIsCoastal();
                MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
            });
        }

        private void ToolStripComboBox_Data_Bookmark_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                string[] value = ToolStripComboBox_Data_Bookmark.Text.Split(']');
                if (value.Length > 1)
                {
                    string dateTimeString = value[0].Replace('[', ' ').Trim();

                    try
                    {
                        if (Utils.TryParseDateTimeStamp(dateTimeString, out DateTime dateTime))
                        {
                            DataManager.currentDateStamp = new DateTime[] { dateTime };
                            CountryManager.UpdateByDateTimeStamp(dateTime);
                            StateManager.UpdateByDateTimeStamp(dateTime);

                            MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
                        }
                        else throw new Exception(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_BOOKMARK_NOT_FOUND,
                                new Dictionary<string, string> { { "{bookmark}", dateTimeString } }
                            ));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_WHILE_BOOKMARK_LOADING,
                                new Dictionary<string, string> { { "{bookmark}", dateTimeString } }
                            ),
                            ex
                        );
                    }
                }
            });
        }

        private void ToolStripMenuItem_Save_States_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StateManager.Save(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Regions_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StrategicRegionManager.Save(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Maps_Heights_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveHeightMap(SettingsManager.settings));
        private void ToolStripMenuItem_Save_Maps_Terrain_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveTerrainMap());
        private void ToolStripMenuItem_Save_Maps_Trees_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveTreesMap());
        private void ToolStripMenuItem_Save_Maps_Cities_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveCitiesMap());

        private void ComboBox_BordersType_SelectedIndexChanged(object sender, EventArgs e) => UpdateBordersType();

        private void UpdateBordersType()
        {
            Logger.TryOrLog(() =>
            {
                enumBordersType = (EnumBordersType)ComboBox_BordersType.SelectedIndex;
                MapManager.UpdateDisplayBorders();
            });
        }

        private void ComboBox_Tool_Parameter_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text));
        private void ToolStripMenuItem_Edit_AutoTools_StatesValidation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StateManager.ValidateAllStates());
        private void ToolStripMenuItem_Edit_AutoTools_RegionsValidation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StrategicRegionManager.ValidateAllRegions());
        private void ToolStripMenuItem_Data_Provinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new ProvinceListForm().ShowDialog()));

        private void ToolStripMenuItem_Data_States_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (StateListForm.instance == null) Task.Run(() => new StateListForm().ShowDialog());
            });
        }

        private void ToolStripMenuItem_UpdateAll_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => UpdateAll());
        private void UpdateAll()
            => Logger.TryOrLog(() => MapManager.UpdateMapInfo());

        private void Button_GenerateColor_MouseDown(object sender, MouseEventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                Color color;
                switch (ComboBox_GenerateColor_Type.SelectedIndex)
                {
                    case 0: color = Color3B.GetRandowColor().ToColor(); break;
                    case 1: color = ProvinceManager.GetNewLandColor().ToColor(); break;
                    case 2: color = ProvinceManager.GetNewSeaColor().ToColor(); break;
                    case 3: color = ProvinceManager.GetNewLakeColor().ToColor(); break;
                    default: return;
                }

                if (e.Button == MouseButtons.Left) SetBrushFirstColor(color);
                else if (e.Button == MouseButtons.Right) SetBrushSecondColor(color);
            });
        }

        private void ToolStripMenuItem_Map_Actions_DropDownOpened(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (ProvinceManager.SelectedProvince == null || ProvinceManager.RMBProvince == null ||
                    ProvinceManager.SelectedProvince == ProvinceManager.RMBProvince)
                {
                    ToolStripMenuItem_Map_Actions_Merge.Enabled = false;
                }
                else ToolStripMenuItem_Map_Actions_Merge.Enabled = true;
            });
        }

        private void ToolStripMenuItem_Map_Actions_Merge_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (enumMainLayer == EnumMainLayer.PROVINCES_MAP)
                    MergeProvincesTool.MergeProvinces(ProvinceManager.SelectedProvince, ProvinceManager.RMBProvince);
            });
        }

        private void ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                ProvinceManager.AutoToolRemoveSeaAndLakesContinents();
                MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
            });
        }

        private void ToolStripMenuItem_Map_Adjacency_DropDownOpened(object sender, EventArgs e)
        {
            void fillInAction(Adjacency adj)
            {
                if (adj != null)
                {
                    adj.GetDisplayData(out EnumAdjaciencyType enumType, out int startId, out int endId, out int throughId, out Value2I startPos, out Value2I endPos, out string adjacencyRuleName, out string comment);

                    ToolStripComboBox_Map_Adjacency_Type.Text = enumType.ToString();

                    ToolStripTextBox_Map_Adjacency_FirstProvinceId.Text = "" + startId;
                    ToolStripTextBox_Map_Adjacency_SecondProvinceId.Text = "" + endId;
                    ToolStripTextBox_Map_Adjacency_ThroughProvinceId.Text = "" + throughId;

                    ToolStripTextBox_Map_Adjacency_FirstPointPos.Text = startPos.ToPositionString();
                    ToolStripTextBox_Map_Adjacency_SecondPointPos.Text = endPos.ToPositionString();

                    ToolStripComboBox_Map_Adjacency_Rule.Text = adjacencyRuleName;
                    ToolStripTextBox_Map_Adjacency_Comment.Text = comment;
                }
                else
                {
                    ToolStripComboBox_Map_Adjacency_Type.Text = EnumAdjaciencyType.SEA.ToString();

                    ToolStripTextBox_Map_Adjacency_FirstProvinceId.Text = "";
                    ToolStripTextBox_Map_Adjacency_SecondProvinceId.Text = "";
                    ToolStripTextBox_Map_Adjacency_ThroughProvinceId.Text = "";

                    ToolStripTextBox_Map_Adjacency_FirstPointPos.Text = "";
                    ToolStripTextBox_Map_Adjacency_SecondPointPos.Text = "";

                    ToolStripComboBox_Map_Adjacency_Rule.Text = "";
                    ToolStripTextBox_Map_Adjacency_Comment.Text = "";
                }
            }

            void changeStatusAction(Adjacency adj, ToolStripTextBox toolStripTextBox_Map_Adjacency_Comment)
            {
                bool isAdjSelected = adj != null;
                var firstProvince = ProvinceManager.SelectedProvince;
                var secondProvince = ProvinceManager.RMBProvince;
                bool selectedTwoProvinces = firstProvince != null && secondProvince != null;
                bool sameProvinces = selectedTwoProvinces && firstProvince.Id == secondProvince.Id;
                bool provincesHasAdjacency = selectedTwoProvinces && firstProvince.HasSeaConnectionWith(secondProvince);

                ToolStripMenuItem_Map_Adjacency_Create.Enabled = !isAdjSelected && selectedTwoProvinces && !provincesHasAdjacency && !sameProvinces;
                ToolStripMenuItem_Map_Adjacency_Remove.Enabled = isAdjSelected;

                ToolStripMenuItem_Map_Adjacency_SetFirstProvince.Enabled = isAdjSelected;
                ToolStripTextBox_Map_Adjacency_FirstProvinceId.Enabled = isAdjSelected;
                ToolStripMenuItem_Map_Adjacency_SetFirstPoint.Enabled = isAdjSelected;
                ToolStripTextBox_Map_Adjacency_FirstPointPos.Enabled = isAdjSelected;
                ToolStripMenuItem_Map_Adjacency_ResetFirstPoint.Enabled = isAdjSelected;

                ToolStripMenuItem_Map_Adjacency_SetSecondProvince.Enabled = isAdjSelected;
                ToolStripTextBox_Map_Adjacency_SecondProvinceId.Enabled = isAdjSelected;
                ToolStripMenuItem_Map_Adjacency_SetSecondPoint.Enabled = isAdjSelected;
                ToolStripTextBox_Map_Adjacency_SecondPointPos.Enabled = isAdjSelected;
                ToolStripMenuItem_Map_Adjacency_ResetSecondPoint.Enabled = isAdjSelected;

                ToolStripMenuItem_Map_Adjacency_SetThroughProvince.Enabled = isAdjSelected;
                ToolStripTextBox_Map_Adjacency_ThroughProvinceId.Enabled = isAdjSelected;

                ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince.Enabled = adj != null && !adj.HasRuleRequiredProvince(secondProvince);
                ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince.Enabled = isAdjSelected && adj.HasRuleRequiredProvince(secondProvince);
                toolStripTextBox_Map_Adjacency_Comment.Enabled = isAdjSelected;
            }

            Logger.TryOrLog(() =>
            {
                var selectedAdjacency = AdjacenciesManager.GetSelectedSeaCross();
                fillInAction(selectedAdjacency);
                changeStatusAction(selectedAdjacency, ToolStripTextBox_Map_Adjacency_Comment);
            });
        }

        private void ToolStripMenuItem_Map_Adjacency_Create_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() => AdjacenciesManager.CreateAdjacency(
                ProvinceManager.SelectedProvince,
                ProvinceManager.RMBProvince,
                (EnumAdjaciencyType)ToolStripComboBox_Map_Adjacency_Type.SelectedIndex
            ));
        }

        private void ToolStripMenuItem_Map_Adjacency_Remove_Click(object sender, EventArgs e) => Logger.TryOrLog(()
            => AdjacenciesManager.RemoveAdjacency(AdjacenciesManager.GetSelectedSeaCross()));
        private void ToolStripMenuItem_Map_Adjacency_SetFirstProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetStartProvince(ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Adjacency_SetFirstPoint_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetStartPos(textBox_HOI4PixelPos.Text));
        private void ToolStripMenuItem_Map_Adjacency_SetSecondProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetEndProvince(ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Adjacency_SetSecondPoint_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetEndPos(textBox_HOI4PixelPos.Text));
        private void ToolStripMenuItem_Map_Adjacency_SetThroughProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetThroughProvince(ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Adjacency_ResetFirstPoint_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetStartPos(-1, -1));
        private void ToolStripMenuItem_Map_Adjacency_ResetSecondPoint_Click(object sender, EventArgs e) =>
            Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetEndPos(-1, -1));

        public void SetMapTextures(List<TextureInfo> mapTextures)
        {
            ListBox_Textures.Items.Clear();
            foreach (var texture in mapTextures) ListBox_Textures.Items.Add(texture.fileName);
        }

        private void Button_OpenSearchErrorsSettings_Click(object sender, EventArgs e) =>
            Logger.TryOrLog(() => Task.Run(() => new SearchErrorsSettingsForm().ShowDialog()));
        private void ToolStripTextBox_Map_Adjacency_Comment_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.SetComment(ToolStripTextBox_Map_Adjacency_Comment.Text));
        private void ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.AddRuleRequiredProvince(ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.GetSelectedSeaCross()?.RemoveRuleRequiredProvince(ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Language_RU_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SettingsManager.ChangeLanguage("ru"));
        private void ToolStripMenuItem_Language_EN_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SettingsManager.ChangeLanguage("en"));
        private void ToolStripMenuItem_Edit_AutoTools_FindMapChanges_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => MapManager.FindMapChanges());

        private void ToolStripMenuItem_Help_Documentation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                using (Process.Start(AboutProgramForm.DiscordDocumentationURL)) { }
            });

        private void ToolStripMenuItem_Discord_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                using (Process.Start(AboutProgramForm.DiscordServerURL)) { }
            });

        private void ToolStripMenuItem_Map_Railway_Split_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwayTool.SplitRailwayAtProvince(SupplyManager.SelectedRailway ?? SupplyManager.RMBRailway, ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Railway_Join_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwayTool.JoinRailways(SupplyManager.SelectedRailway, SupplyManager.RMBRailway));
        private void ToolStripMenuItem_Data_Recovery_Regions_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new StrategicRegionsDataRecoveryForm().ShowDialog()));
        private void ToolStripMenuItem_Map_Railway_AddProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwayTool.AddProvinceToRailway(SupplyManager.SelectedRailway, ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Railway_RemoveProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwayTool.RemoveProvinceFromRailway(SupplyManager.SelectedRailway, ProvinceManager.RMBProvince));

        private void ToolStripMenuItem_Map_Province_OpenStateFile_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.State == null) return;

                StateListForm stateListForm;
                if (StateListForm.instance == null)
                {
                    stateListForm = new StateListForm();
                    stateListForm.Show();
                }
                else stateListForm = StateListForm.instance;

                stateListForm.Focus();
                stateListForm.FindState(ProvinceManager.RMBProvince.State.Id);
            });
        }

        private void ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                ProvinceManager.AutoToolRemoveSeaProvincesFromStates();
                MapManager.HandleMapMainLayerChange(enumMainLayer, ComboBox_Tool_Parameter.Text);
            });
        }

        public void SetAdjacencyRules(Dictionary<string, AdjacencyRule>.KeyCollection rules)
        {
            Logger.TryOrLog(() =>
            {
                InvokeAction(() =>
                {
                    ToolStripComboBox_Map_Adjacency_Rule.Items.Clear();
                    ToolStripComboBox_Map_Adjacency_Rule.Items.Add("");
                    foreach (string name in rules) ToolStripComboBox_Map_Adjacency_Rule.Items.Add(name);
                });
            });
        }
    }
}
