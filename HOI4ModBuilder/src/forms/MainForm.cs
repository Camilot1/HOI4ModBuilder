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
using HOI4ModBuilder.src.tools.auto;
using HOI4ModBuilder.src.forms.scripts;
using HOI4ModBuilder.src.scripts;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.tools.brushes;
using System.Linq;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using static HOI4ModBuilder.utils.Structs;
using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.src.forms.actionForms;

namespace HOI4ModBuilder
{
    partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        public GLControl GLControl { get; private set; }
        public static bool IsFirstLoaded { get; set; } = false;
        public static bool ErrorsOrExceptionsDuringLoading { get; set; } = false;
        public static bool IsLoadingOrSaving { get; set; } = false;
        public static bool UpdateGLControl { get; set; } = true;
        public static bool IsMapMainLayerChangeEnabled { get; set; } = false;

        private Color brushFirstColor = Color.White, brushSecondColor = Color.White;
        public ViewportInfo ViewportInfo { get; private set; } = new ViewportInfo();

        private EnumMainLayer _selectedMainLayer = EnumMainLayer.PROVINCES_MAP;
        public EnumMainLayer SelectedMainLayer
        {
            get => _selectedMainLayer;
            set
            {
                _prevMainLayer = _selectedMainLayer;
                _selectedMainLayer = value;
            }
        }
        public void SetSelectedMainLayerWithRefresh(EnumMainLayer layer)
        {
            SelectedMainLayer = layer;
            СomboBox_MapMainLayer.SelectedIndex = (int)layer;
            СomboBox_MapMainLayer.Refresh();
        }
        private EnumMainLayer _prevMainLayer;
        public EnumMainLayer PrevMainLayer { get => _prevMainLayer; }


        private EnumTool _selectedTool = EnumTool.CURSOR;
        public EnumTool SelectedTool
        {
            get => _selectedTool;
            set
            {
                _prevTool = _selectedTool;
                _selectedTool = value;

                ComboBox_Tool.SelectedIndex = (int)_selectedTool;
                ComboBox_Tool.Refresh();
            }
        }
        public void SetSelectedToolWithRefresh(EnumTool tool)
        {
            SelectedTool = tool;
            ComboBox_Tool.SelectedIndex = (int)tool;
            ComboBox_Tool.Refresh();
        }
        public EnumTool _prevTool;
        public EnumTool PrevTool { get => _prevTool; }


        private EnumEditLayer _selectedEditLayer = EnumEditLayer.PROVINCES;
        public EnumEditLayer SelectedEditLayer
        {
            get => _selectedEditLayer;
            set
            {
                _prevEditLayer = _selectedEditLayer;
                _selectedEditLayer = value;
            }
        }
        public EnumEditLayer _prevEditLayer;
        public EnumEditLayer PrevEditLayer { get => _prevEditLayer; }

        public EnumBordersType SelectedBordersType { get; private set; } = EnumBordersType.PROVINCES_BLACK;

        // Сохраняем делегат в статическом поле для избежания ошибки "CallbackOnCollectedDelegate"
        private static DebugProc debugProc;

        private static readonly Dictionary<Keys, List<KeyEventHandler>> _globaldPressButtonsEvents = new Dictionary<Keys, List<KeyEventHandler>>();
        private static readonly Dictionary<int, Dictionary<Keys, List<KeyEventHandler>>> _tabRelatedPressButtonsEvents = new Dictionary<int, Dictionary<Keys, List<KeyEventHandler>>>();

        private static readonly List<Action> _guiReinitActions = new List<Action>();

        public MainForm()
        {
            AddMainLayerHotKey(EnumMainLayer.PROVINCES_MAP, new[] { Keys.Alt, Keys.P });
            AddMainLayerHotKey(EnumMainLayer.STATES, new[] { Keys.Alt, Keys.S });
            AddMainLayerHotKey(EnumMainLayer.STRATEGIC_REGIONS, new[] { Keys.Alt, Keys.R });
            AddMainLayerHotKey(EnumMainLayer.AI_AREAS, new[] { Keys.Alt, Keys.A });
            AddMainLayerHotKey(EnumMainLayer.COUNTRIES, new[] { Keys.Control, Keys.Alt, Keys.C });
            AddMainLayerHotKey(EnumMainLayer.PROVINCES_TERRAINS, new[] { Keys.Alt, Keys.T });
            AddMainLayerHotKey(EnumMainLayer.CONTINENTS, new[] { Keys.Alt, Keys.C });
            AddMainLayerHotKey(EnumMainLayer.MANPOWER, new[] { Keys.Alt, Keys.M });
            AddMainLayerHotKey(EnumMainLayer.VICTORY_POINTS, new[] { Keys.Alt, Keys.V });
            AddMainLayerHotKey(EnumMainLayer.RESOURCES, new[] { Keys.Control, Keys.Alt, Keys.R });
            AddMainLayerHotKey(EnumMainLayer.BUILDINGS, new[] { Keys.Alt, Keys.B });

            AfterFirstInit();
        }

        public void Reinit()
        {
            Controls.Clear();
            Init();
            foreach (var action in _guiReinitActions)
                action();
        }

        private void Init()
        {
            InitializeComponent();
            Text += $" [{Logger.version}]";

            if (GLControl == null)
            {
                GLControl = new GLControl();
                GLControl.Dock = DockStyle.Fill;
                GLControl.Load += GLControl_Load;
                GLControl.Resize += GLControl_Resize;
                GLControl.Paint += GLControl_Paint;
                GLControl.MouseWheel += new MouseEventHandler(Panel1_MouseWheel);
                GLControl.MouseDown += new MouseEventHandler(Panel1_MouseDown);
                GLControl.MouseUp += new MouseEventHandler(Panel1_MouseUp);
                GLControl.MouseMove += new MouseEventHandler(Panel1_MouseMove);
            }
            Panel_Map.Controls.Add(GLControl);

            InitComboBoxMapMainLayerItems();

            InterfaceUtils.ResizeComboBox(GroupBox_Main_Layer, СomboBox_MapMainLayer);

            ComboBox_EditLayer.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumEditLayer)))
                ComboBox_EditLayer.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            ComboBox_EditLayer.SelectedIndex = 0;

            InterfaceUtils.ResizeComboBox(GroupBox_Edit_Layer, ComboBox_EditLayer);

            InitComboBoxToolItems();

            InterfaceUtils.ResizeComboBox(GroupBox_Tool, ComboBox_Tool);

            ComboBox_BordersType.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumBordersType)))
                ComboBox_BordersType.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            ComboBox_BordersType.SelectedIndex = 0;

            CheckedListBox_MapAdditionalLayers.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(EnumAdditionalLayers)))
                CheckedListBox_MapAdditionalLayers.Items.Add(GuiLocManager.GetLoc(type.ToString()));
            CheckedListBox_MapAdditionalLayers.Height = 15 * CheckedListBox_MapAdditionalLayers.Items.Count + 10;

            CheckedListBox_MapAdditionalLayers.SetItemChecked((int)EnumAdditionalLayers.TEXT, true);
            MapManager.displayLayers[(int)EnumAdditionalLayers.TEXT] = true;

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
            ScriptParser.Wake();
            BrushManager.Load();
        }

        private static readonly Dictionary<EnumMainLayer, HotKey> _mapMainLayerMapHotKeys = new Dictionary<EnumMainLayer, HotKey>
        { };
        private static void AddMainLayerHotKey(EnumMainLayer layer, Keys[] keys)
        {
            if (keys == null || keys.Length == 0)
                return;
            _mapMainLayerMapHotKeys[layer] = new HotKey
            {
                control = Utils.Contains(keys, Keys.Control),
                shift = Utils.Contains(keys, Keys.Shift),
                alt = Utils.Contains(keys, Keys.Alt),
                key = Utils.GetFirst(keys, (k) => k != Keys.Control && k != Keys.Shift && k != Keys.Alt, Keys.NoName),
                hotKeyEvent = (e) => Instance.SetSelectedMainLayerWithRefresh(layer)
            };
        }
        private void InitComboBoxMapMainLayerItems()
        {
            СomboBox_MapMainLayer.Items.Clear();
            foreach (EnumMainLayer type in Enum.GetValues(typeof(EnumMainLayer)))
            {
                string hotKeyStr = "";
                if (_mapMainLayerMapHotKeys.TryGetValue(type, out var hotkey))
                    hotKeyStr = " " + hotkey.ToString();
                СomboBox_MapMainLayer.Items.Add(GuiLocManager.GetLoc(type.ToString()) + hotKeyStr);
            }
            СomboBox_MapMainLayer.SelectedIndex = 0;
        }

        private void InitComboBoxToolItems()
        {
            ComboBox_Tool.Items.Clear();
            foreach (EnumTool type in Enum.GetValues(typeof(EnumTool)))
            {
                string hotKeyStr = "";
                if (MapToolsManager.TryGetMapTool(type, out MapTool mapTool) && mapTool.HotKey != null)
                    hotKeyStr = " " + mapTool.HotKey.ToString();
                ComboBox_Tool.Items.Add(GuiLocManager.GetLoc(type.ToString()) + hotKeyStr);
            }
            ComboBox_Tool.SelectedIndex = 0;
        }

        private void AfterFirstInit()
        {
            Instance = this;
            Logger.TryOrLog(() =>
            {
                GuiLocManager.formsReinitEvents.Add(this, () => Reinit());

                SettingsManager.Init();
                MapManager.Init();

                InitComboBoxMapMainLayerItems();
                InitComboBoxToolItems();

                new HotKey { control = true, key = Keys.S, hotKeyEvent = (e) => DataManager.SaveAll() }.SubscribeGlobalKeyEvent();
                new HotKey { control = true, key = Keys.L, hotKeyEvent = (e) => DataManager.LoadAll() }.SubscribeGlobalKeyEvent();
                new HotKey { control = true, key = Keys.U, hotKeyEvent = (e) => DataManager.UpdateAll() }.SubscribeGlobalKeyEvent();

                foreach (var hotkey in _mapMainLayerMapHotKeys.Values)
                    hotkey.SubscribeTabKeyEvent(EnumTabPage.MAP);

                NetworkManager.SyncGithubInfo();
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            TextureManager.DisposeAllTextures();
            TextRenderManager.Instance.Dispose();
        }

        public void InvokeAction(Action action)
            => Invoke((MethodInvoker)delegate { action(); });

        public void TryInvokeActionOrLog(Action tryAction, Action<Exception> catchAction)
            => Invoke((MethodInvoker)delegate { Logger.TryOrCatch(tryAction, catchAction); });

        public bool IsParameterValueVisible()
            => GroupBox_Tool_Parameter_Value.Visible;
        public string IncreaseParameterValue()
        {
            var maxIndex = ComboBox_Tool_Parameter_Value.Items.Count - 1;
            if (ComboBox_Tool_Parameter_Value.SelectedIndex < maxIndex)
                ComboBox_Tool_Parameter_Value.SelectedIndex++;
            ComboBox_Tool_Parameter_Value.Refresh();

            return ComboBox_Tool_Parameter_Value.Text;
        }

        public string DecreaseParameterValue()
        {
            if (ComboBox_Tool_Parameter_Value.SelectedIndex > 0)
                ComboBox_Tool_Parameter_Value.SelectedIndex--;

            return ComboBox_Tool_Parameter_Value.Text;
        }

        public static void PauseGLControl()
        {
            UpdateGLControl = false;
            Instance.GLControl.Context.MakeCurrent(null);
        }

        public static void ResumeGLControl()
        {
            Instance.GLControl.Context.MakeCurrent(Instance.GLControl.WindowInfo);
            Instance.GLControl.Invalidate();
            UpdateGLControl = true;
            Instance.GLControl_Resize(null, null);
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
                if (progress > 1)
                    Instance.ProgressBar1.Value = 100;
                else if (progress < 0)
                    Instance.ProgressBar1.Value = 0;
                else
                    Instance.ProgressBar1.Value = (int)Math.Round(progress * 100);
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

                TextRenderManager.Instance.OnLoad();
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
                ViewportInfo.width = GLControl.Width;
                ViewportInfo.height = GLControl.Height;

                ViewportInfo.max = ViewportInfo.width > ViewportInfo.height ? ViewportInfo.width : ViewportInfo.height;

                ViewportInfo.x = (ViewportInfo.width - ViewportInfo.max) / 2;
                ViewportInfo.y = (ViewportInfo.height - ViewportInfo.max) / 2;
                GL.Viewport(ViewportInfo.x, ViewportInfo.y, ViewportInfo.max, ViewportInfo.max);
                GL.ClearColor(Color4.LightBlue);
            });
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            if (!UpdateGLControl)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color.LightBlue);

            GL.MatrixMode(MatrixMode.Projection);

            MapManager.Draw();
            if (Panel_ColorPicker.Visible)
                ElementHost_ColorPicker.Invalidate();

            if (MapManager.IsMapDragged)
                Panel_Map.Cursor = Cursors.SizeAll;
            else
                Panel_Map.Cursor = Cursors.Default;

            GLControl.Invalidate();
            GLControl.SwapBuffers();
        }


        private void Panel_FirstColor_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => FlipColorPickerVisibility());
        private void Panel_SecondColor_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => FlipColorPickerVisibility());
        private void Panel_ColorPicker_Button_Close_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => FlipColorPickerVisibility());

        private void FlipColorPickerVisibility()
        {
            Panel_ColorPicker.Visible = !Panel_ColorPicker.Visible;
            if (Panel_ColorPicker.Visible)
                UpdateColorPickerColors(brushFirstColor, brushSecondColor);
        }

        private void Panel_ColorPicker_Button_Save_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var firstColor = Color.FromArgb(Utils.ArgbToInt(
                    255,
                    standardColorPicker1.SelectedColor.R,
                    standardColorPicker1.SelectedColor.G,
                    standardColorPicker1.SelectedColor.B
                ));
                var secondColor = Color.FromArgb(Utils.ArgbToInt(
                    255,
                    standardColorPicker1.SecondaryColor.R,
                    standardColorPicker1.SecondaryColor.G,
                    standardColorPicker1.SecondaryColor.B
                ));

                SetBrushFirstColor(firstColor);
                SetBrushSecondColor(secondColor);
            });
        }

        public void SetBrushFirstColor(Color color)
        {
            if (brushFirstColor == color)
                return;

            brushFirstColor = color;
            Panel_FirstColor.BackColor = color;

            UpdateColorPickerColors(brushFirstColor, brushSecondColor);

            if (SelectedEditLayer != EnumEditLayer.RIVERS)
                PushColorToColorsHistory(color);
        }

        public Color GetBrushFirstColor() => brushFirstColor;
        public void SetBrushSecondColor(Color color)
        {
            if (brushSecondColor == color)
                return;

            brushSecondColor = color;
            Panel_SecondColor.BackColor = color;

            UpdateColorPickerColors(brushFirstColor, brushSecondColor);

            if (SelectedEditLayer != EnumEditLayer.RIVERS)
                PushColorToColorsHistory(color);
        }

        public void UpdateColorPickerColors(Color m, Color s)
        {
            standardColorPicker1.SelectedColor = System.Windows.Media.Color.FromArgb(m.A, m.R, m.G, m.B);
            standardColorPicker1.SecondaryColor = System.Windows.Media.Color.FromArgb(s.A, s.R, s.G, s.B);
        }

        public Color GetBrushSecondColor() => brushSecondColor;
        public void SwitchBrushColors()
        {
            (brushFirstColor, brushSecondColor) = (brushSecondColor, brushFirstColor);
            Panel_FirstColor.BackColor = brushFirstColor;
            Panel_SecondColor.BackColor = brushSecondColor;
        }

        private static readonly int _colorsHistoryPalleteMaxCount = 10;
        private static List<Color> _colorsHistoryPallete = new List<Color>(_colorsHistoryPalleteMaxCount + 1);

        public static void PushColorToColorsHistory(Color color)
        {
            _colorsHistoryPallete.Remove(color);
            _colorsHistoryPallete.Insert(0, color);
            while (_colorsHistoryPallete.Count > _colorsHistoryPalleteMaxCount)
                _colorsHistoryPallete.RemoveAt(_colorsHistoryPallete.Count - 1);
            InitColorsHistoryPallete();
        }

        public static void InitColorsPallete()
        {
            if (Instance.SelectedEditLayer == EnumEditLayer.RIVERS)
                InitRiversPallete();
            else
                InitColorsHistoryPallete();
        }

        public static void InitColorsHistoryPallete()
            => InitColorsPallete(_colorsHistoryPallete);
        public static void InitRiversPallete()
            => InitColorsPallete(TextureManager.RiverColors);

        public static void InitColorsPallete(ICollection<Color> colors)
        {
            Instance.InvokeAction(() =>
            {
                int index = 0;
                foreach (var color in colors)
                {
                    if (index < Instance.FlowLayoutPanel_Color.Controls.Count)
                    {
                        Instance.FlowLayoutPanel_Color.Controls[index].BackColor = color;
                        index++;
                        continue;
                    }

                    var panel = new Panel
                    {
                        BackColor = color,
                        BorderStyle = BorderStyle.FixedSingle,
                        Size = new Size(21, 21),
                        Margin = new Padding(1, 0, 1, 0),
                        Padding = new Padding(1, 0, 1, 0),
                        Cursor = Cursors.Hand,
                    };
                    panel.MouseDown += new MouseEventHandler(PalleteColorMouseDown);
                    index++;
                    Instance.FlowLayoutPanel_Color.Controls.Add(panel);
                }

                int count = Instance.FlowLayoutPanel_Color.Controls.Count;
                while (count > 0 && count > index)
                {
                    Instance.FlowLayoutPanel_Color.Controls.RemoveAt(count - 1);
                    count = Instance.FlowLayoutPanel_Color.Controls.Count;
                }
            });
        }
        public static void PalleteColorMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetBrushColor(0, ((Panel)sender).BackColor);
            else if (e.Button == MouseButtons.Right)
                SetBrushColor(1, ((Panel)sender).BackColor);
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
            => MapManager.HandleMouseWheel(e, ViewportInfo);

        private void CheckedListBox_MapAdditionalLayers_MouseUp(object sender, MouseEventArgs e)
        {
            var checkedItems = CheckedListBox_MapAdditionalLayers.CheckedIndices;

            var prevTextEnabled = MapManager.displayLayers[(int)EnumAdditionalLayers.TEXT];
            for (int i = 0; i < Enum.GetValues(typeof(EnumAdditionalLayers)).Length; i++)
                MapManager.displayLayers[i] = checkedItems.Contains(i);

            if (!prevTextEnabled && MapManager.displayLayers[(int)EnumAdditionalLayers.TEXT])
                MapManager.HandleMapMainLayerChange(true, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
        }

        private void Button_TextureAdd_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var fd = new OpenFileDialog();
                string dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "textures", "map" });
                Utils.PrepareFileDialog(fd, dialogPath, "BMP files (*.bmp)|*.bmp");

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
                            (SettingsManager.Settings.MAP_VIEWPORT_HEIGHT *
                            SettingsManager.Settings.GetMapScalePixelToKM() * 1000d);
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
                string dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "textures", "map", "data" });
                Utils.PrepareFileDialog(fd, dialogPath, "JSON files (*.json)|*.json");

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
                string dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "textures", "map", "data" });
                Utils.PrepareFileDialog(fd, dialogPath, "JSON files (*.json)|*.json");

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
                Instance.SetBrushFirstColor(color);
            else if (type == 1)
                Instance.SetBrushSecondColor(color);
        }

        private static readonly Dictionary<EnumMainLayer, LayerActions> _mainLayerActions = new Dictionary<EnumMainLayer, LayerActions>
        {
            { EnumMainLayer.BUILDINGS, new LayerActions {
                isValidChecker = (s) => BuildingManager.HasBuilding(s),
                parameterProvider = () => BuildingManager.GetBuildingNames(),
                onSelect = () => Instance.SetSelectedToolWithRefresh(EnumTool.BUILDINGS)
            } },
            { EnumMainLayer.RESOURCES, new LayerActions {
                isValidChecker = (s) => ResourceManager.HasResource(s),
                parameterProvider = () => ResourceManager.GetResourcesTags(),
                onSelect = () => Instance.SetSelectedToolWithRefresh(EnumTool.RESOURCES)
            } },
            { EnumMainLayer.AI_AREAS, new LayerActions {
                isValidChecker = (s) => AiAreaManager.HasAiArea(s),
                parameterProvider = () => AiAreaManager.GetAiAreasNames(),
                onSelect = () => Instance.SetSelectedToolWithRefresh(EnumTool.AI_AREAS)
            } },
            { EnumMainLayer.CORES_OF, new LayerActions {
                isValidChecker = (s) => CountryManager.HasCountry(s),
                parameterProvider = () => CountryManager.GetCountryTagsSorted(),
                onSelect = () => Instance.SetSelectedToolWithRefresh(EnumTool.STATE_CORE_OF)
            } },
            { EnumMainLayer.CLAIMS_BY, new LayerActions {
                isValidChecker = (s) => CountryManager.HasCountry(s),
                parameterProvider = () => CountryManager.GetCountryTagsSorted(),
                onSelect = () => Instance.SetSelectedToolWithRefresh(EnumTool.STATE_CLAIM_BY)
            } },
        };

        public class LayerActions
        {
            public Func<string, bool> isValidChecker;
            public Func<ICollection> parameterProvider;
            public Action onSelect;
        }

        private bool IsUpdatingMainLayerAndTool { get; set; }
        public void UpdateSelectedMainLayerAndTool(bool mainLayerChange, bool toolChange)
        {
            Logger.TryOrLog(() =>
            {
                if (IsUpdatingMainLayerAndTool)
                    return;

                IsUpdatingMainLayerAndTool = true;

                MapToolsManager.TryGetMapToolParametersProvider(SelectedTool, out var toolParametersProvider);
                MapToolsManager.TryGetMapToolValuesProvider(SelectedTool, out var valuesProvider);

                Func<ICollection> parametersProvider = null;

                _mainLayerActions.TryGetValue(SelectedMainLayer, out var actions);
                if (mainLayerChange && actions != null && actions.onSelect != null)
                {
                    actions.onSelect.Invoke();
                    toolParametersProvider = actions.parameterProvider;
                }

                if (toolParametersProvider != null)
                    parametersProvider = toolParametersProvider;
                else if (actions != null)
                    parametersProvider = actions.parameterProvider;

                UpdateToolParameterComboBox(SelectedTool, parametersProvider, valuesProvider);

                Panel_Map.ContextMenuStrip = (SelectedTool != EnumTool.CURSOR) ? null : ContextMenuStrip_Map;

                var shouldRecalculateAll = mainLayerChange;
                if (!(mainLayerChange || toolChange))
                    shouldRecalculateAll = MapToolsManager.ShouldRecalculateAllText(SelectedMainLayer, SelectedTool);

                if (actions == null || actions.isValidChecker == null || actions.isValidChecker.Invoke(GetParameter()))
                    MapManager.HandleMapMainLayerChange(shouldRecalculateAll, SelectedMainLayer, GetParameter());

                IsUpdatingMainLayerAndTool = false;
            });
        }

        private static readonly string[] preferedParameter = new string[Enum.GetValues(typeof(EnumTool)).Length];
        private static readonly string[] preferedParameterValue = new string[Enum.GetValues(typeof(EnumTool)).Length];

        private void UpdateToolParameterComboBox(EnumTool tool, Func<ICollection> parametersProvider, Func<ICollection> valuesProvider)
        {
            var parameterItems = parametersProvider?.Invoke();
            var parameterValueItems = valuesProvider?.Invoke();

            InterfaceUtils.UpdateComboBoxToolValues(GroupBox_Tool_Parameter, ComboBox_Tool_Parameter, parameterItems, preferedParameter, tool);

            if (parameterValueItems == null && (tool == EnumTool.BRUSH || tool == EnumTool.ERASER))
            {
                var brushName = ComboBox_Tool_Parameter.Text;
                if (BrushManager.TryGetBrush(SettingsManager.Settings, brushName, out var brush))
                    parameterValueItems = brush.SortedVariantKeys;
            }

            InterfaceUtils.UpdateComboBoxToolValues(GroupBox_Tool_Parameter_Value, ComboBox_Tool_Parameter_Value, parameterValueItems, preferedParameterValue, tool);
        }

        private void UpdateMainLayerParameterButton(EnumMainLayer mainLayer)
        {
            if (mainLayer == EnumMainLayer.CUSTOM_SCRIPT)
            {
                Button_MapMainLayer_Parameter.Text =
                    ScriptParser.MapMainLayerCustomScriptActions == null ?
                    GuiLocManager.GetLoc(EnumLocKey.NOT_SELECTED) :
                    ScriptParser.MapMainLayerCustomScriptName;

                GroupBox_Main_Layer_Parameter.Visible = true;
                InterfaceUtils.ResizeButton(GroupBox_Main_Layer_Parameter, Button_MapMainLayer_Parameter, 150);
            }
            else GroupBox_Main_Layer_Parameter.Visible = false;
        }
        private void СomboBox_MapMainLayer_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                SelectedMainLayer = (EnumMainLayer)СomboBox_MapMainLayer.SelectedIndex;
                UpdateMainLayerParameterButton(SelectedMainLayer);
                UpdateSelectedMainLayerAndTool(true, false);
            });

        private void Button_MapMainLayer_Parameter_Click(object sender, EventArgs e)
                => Logger.TryOrLog(() =>
                {
                    string filePath;
                    var fd = new OpenFileDialog();
                    var dialogPath = FileManager.AssembleFolderPath(new[] { Application.StartupPath, "data", "scripts" });
                    Utils.PrepareFileDialog(fd, GuiLocManager.GetLoc(EnumLocKey.SCRIPTS_CHOOSE_FILE), dialogPath, "MAP MODE TXT files (*.mm.txt)|*.mm.txt");
                    if (fd.ShowDialog() == DialogResult.OK)
                        filePath = fd.FileName;
                    else
                        return;

                    ScriptParser.CompileMapMainLayerCustomScript(filePath);
                    UpdateMainLayerParameterButton(SelectedMainLayer);
                    MapManager.HandleMapMainLayerChange(true, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                });

        public bool IsControlPressed() => (ModifierKeys & Keys.Control) != 0;
        public bool IsShiftPressed() => (ModifierKeys & Keys.Shift) != 0;
        public bool IsAltPressed() => (ModifierKeys & Keys.Alt) != 0;

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseDown(e, ViewportInfo, SelectedEditLayer, SelectedTool, ComboBox_Tool_Parameter.Text, ComboBox_Tool_Parameter_Value.Text));
        private void Panel1_MouseUp(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseUp(e, ViewportInfo, SelectedTool, SelectedEditLayer));
        private void Panel1_MouseMove(object sender, MouseEventArgs e)
            => Logger.TryOrLog(() => MapManager.HandleMouseMoved(e, ViewportInfo, SelectedTool, SelectedEditLayer, ComboBox_Tool_Parameter.Text, ComboBox_Tool_Parameter_Value.Text));
        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
            => Application.Exit();
        private void ToolStripMenuItem_Save_Maps_Provinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveProvincesMap());
        private void ToolStripMenuItem_Save_Maps_Rivers_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveRiversMap(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Maps_All_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveAllMaps(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Definition_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => ProvinceManager.Save(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Adjacencies_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AdjacenciesManager.Save(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Supply_All_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveAll(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Supply_Railways_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveRailways(FileManager.AssembleFolderPath(new[] { SettingsManager.Settings.modDirectory, "map" })));
        private void ToolStripMenuItem_Save_Supply_Hubs_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyManager.SaveSupplyNodes(FileManager.AssembleFolderPath(new[] { SettingsManager.Settings.modDirectory, "map" })));
        private void ToolStripMenuItem_LoadAll_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => DataManager.LoadAll());
        private void ToolStripMenuItem_Help_About_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AboutProgramForm.CreateTasked());

        public static void SubscribeGuiReinitAction(Action action) => _guiReinitActions.Add(action);
        public static void SubscribeTabKeyEvent(EnumTabPage tabPage, Keys key, KeyEventHandler eventHandler)
        {
            if (!_tabRelatedPressButtonsEvents.TryGetValue((int)tabPage, out var keysEvents))
            {
                keysEvents = new Dictionary<Keys, List<KeyEventHandler>>();
                _tabRelatedPressButtonsEvents.Add((int)tabPage, keysEvents);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_SYSKEYDOWN = 0x0104;

            if ((keyData & Keys.Alt) != 0) //Отключаем мнемоники в WinForms
            {
                if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
                {
                    bool isRepeat = (((long)msg.LParam >> 30) & 1) != 0;
                    if (!isRepeat)
                    {
                        var e = new KeyEventArgs(keyData);
                        MainForm_KeyDown(this, e);
                    }
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            Logger.TryOrLog(() =>
            {
                if (
                    _tabRelatedPressButtonsEvents.TryGetValue(TabControl_Main.SelectedIndex, out var keysEvents) &&
                    keysEvents.TryGetValue(e.KeyCode, out var tabRelatedKeyEventHandlers)
                )
                {
                    foreach (var handler in tabRelatedKeyEventHandlers)
                        handler.Invoke(this, e);
                }

                if (_globaldPressButtonsEvents.TryGetValue(e.KeyCode, out var globalKeyEventHandlers))
                {
                    foreach (var handler in globalKeyEventHandlers)
                        handler.Invoke(this, e);
                }

                e.Handled = true;
            });
        }

        private void ToolStripMenuItem_Settings_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => new SettingsForm().ShowDialog());
        private void ToolStripMenuItem_Map_SupplyHub_Create_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyHubsTool.AddNode(SupplyHubsTool.CreateNode(1, ProvinceManager.RMBProvince)));
        private void ToolStripMenuItem_Map_SupplyHub_Remove_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SupplyHubsTool.RemoveNode(ProvinceManager.RMBProvince?.SupplyNode));

        public byte SelectedRailwayLevel
        {
            get => (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1);
            set => ToolStripComboBox_Map_Railway_Level.SelectedIndex = value - 1;
        }

        private void ToolStripMenuItem_Map_Railway_Create_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                byte level = (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1);
                if (ProvinceManager.GroupSelectedProvinces.Count > 1)
                    RailwaysTool.CreateRailway(level, ProvinceManager.GroupSelectedProvinces);
                else
                    RailwaysTool.CreateRailway(level, ProvinceManager.SelectedProvince, ProvinceManager.RMBProvince);
            });

        private void ToolStripMenuItem_Map_Railway_Remove_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.RemoveRailway(SupplyManager.SelectedRailway));

        private void ToolStripComboBox_Map_Railway_Level_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.ChangeRailwayLevel(
                SupplyManager.SelectedRailway,
                (byte)(ToolStripComboBox_Map_Railway_Level.SelectedIndex + 1))
            );

        private void ToolStripMenuItem_Map_Province_DropDownOpened(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                IsMapMainLayerChangeEnabled = false;
                Update_ToolStripMenuItem_Map_Province_Items(ProvinceManager.RMBProvince);
                IsMapMainLayerChangeEnabled = true;
            });

        private void Update_ToolStripMenuItem_Map_Province_Items(Province p)
        {
            if (p == null)
                return;

            ToolStripComboBox_Map_Province_Type.Text = p.GetTypeString();
            ToolStripMenuItem_Map_Province_IsCoastal.Checked = p.IsCoastal;
            ToolStripComboBox_Map_Province_Terrain.Text = p.Terrain == null ? "unknown" : p.Terrain.name;
            ToolStripComboBox_Map_Province_Continent.Text = ContinentManager.GetContinentById(p.ContinentId);

            ToolStripMenuItem_Map_Province_VictoryPoints_Info.Text =
                GuiLocManager.GetLoc(EnumLocKey.VICTORY_POINTS) + ": " + p.victoryPoints;
        }

        private void ToolStripComboBox_Map_Province_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && IsMapMainLayerChangeEnabled)
            {
                var prevType = ProvinceManager.RMBProvince.Type;
                Enum.TryParse(ToolStripComboBox_Map_Province_Type.Text.ToUpper(), out EnumProvinceType newType);

                void action(EnumProvinceType type)
                {
                    ProvinceManager.RMBProvince.Type = type;
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newType), () => action(prevType));
            }
        }

        private void ToolStripMenuItem_Map_Province_IsCoastal_Click(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && IsMapMainLayerChangeEnabled)
            {
                bool prevIsCoastal = ProvinceManager.RMBProvince.IsCoastal;
                bool newIsCoastal = !prevIsCoastal;

                void action(bool isCoastal)
                {
                    ToolStripMenuItem_Map_Province_IsCoastal.Checked = isCoastal;
                    ProvinceManager.RMBProvince.IsCoastal = isCoastal;
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newIsCoastal), () => action(prevIsCoastal));
            }
        }

        private void ToolStripComboBox_Map_Province_Terrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && IsMapMainLayerChangeEnabled)
            {
                var prevTerrain = ProvinceManager.RMBProvince.Terrain;
                TerrainManager.TryGetProvincialTerrain(ToolStripComboBox_Map_Province_Terrain.Text, out ProvincialTerrain newTerrain);

                void action(ProvincialTerrain terrain)
                {
                    ProvinceManager.RMBProvince.Terrain = terrain;
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newTerrain), () => action(prevTerrain));
            }
        }

        private void ToolStripComboBox_Map_Province_Continent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProvinceManager.RMBProvince != null && IsMapMainLayerChangeEnabled)
            {
                int prevContinentId = ProvinceManager.RMBProvince.ContinentId;
                int newContinentId = ContinentManager.GetContinentId(ToolStripComboBox_Map_Province_Continent.Text);

                void action(int continentId)
                {
                    ProvinceManager.RMBProvince.ContinentId = continentId;
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                MapManager.ActionHistory.Add(() => action(newContinentId), () => action(prevContinentId));
            }
        }

        private void ToolStripMenuItem_Map_Search_Province_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                ProvinceManager.SelectProvinces(Utils.ToIdArray(ToolStripTextBox_Map_Search_Input.Text, ' '));
                MapManager.FocusOn(ProvinceManager.GetGroupSelectedProvincesCenter());
            });

        private void ToolStripMenuItem_Edit_Undo_Click(object sender, EventArgs e)
        {
            if (TabControl_Main.SelectedTab == TabPage_Map)
                MapManager.ActionHistory.Undo();
        }

        private void ToolStripMenuItem_Edit_Redo_Click(object sender, EventArgs e)
        {
            if (TabControl_Main.SelectedTab == TabPage_Map)
                MapManager.ActionHistory.Redo();
        }

        private void ToolStripMenuItem_SaveAll_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => DataManager.SaveAll());

        private void ToolStripMenuItem_Map_Railway_DropDownOpened(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (
                    !RailwaysTool.CanCreateRailway(ProvinceManager.GroupSelectedProvinces) && (
                    ProvinceManager.SelectedProvince == null || ProvinceManager.RMBProvince == null ||
                    ProvinceManager.SelectedProvince.Id == ProvinceManager.RMBProvince.Id ||
                    ProvinceManager.SelectedProvince.Type != EnumProvinceType.LAND || ProvinceManager.RMBProvince.Type != EnumProvinceType.LAND
                ))
                    ToolStripMenuItem_Map_Railway_Create.Enabled = false;
                else
                    ToolStripMenuItem_Map_Railway_Create.Enabled = true;

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
                    Logger.LogSingleErrorMessage(
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
                    Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_SEARCH_POSITION_INCORRECT_COORDS);
                    return;
                }
            });
        }

        private void ToolStripMenuItem_Map_Search_State_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                StateManager.SelectStates(Utils.ToIdArray(ToolStripTextBox_Map_Search_Input.Text, ' '));
                MapManager.FocusOn(StateManager.GetGroupSelectedStatesCenter());
            });

        private void ToolStripMenuItem_Map_Search_Region_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                StrategicRegionManager.SelectRegions(Utils.ToIdArray(ToolStripTextBox_Map_Search_Input.Text, ' '));
                MapManager.FocusOn(StrategicRegionManager.GetGroupSelectedRegionsCenter());
            });
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
            => Logger.TryOrLog(() => AutoTools.FixProvincesCoastalType(true));
        private void ToolStripComboBox_Data_Bookmark_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => DataManager.OnBookmarkChange());
        private void ToolStripMenuItem_Save_States_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StateManager.Save(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Regions_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => StrategicRegionManager.Save(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Maps_Heights_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveHeightMap(SettingsManager.Settings));
        private void ToolStripMenuItem_Save_Maps_Terrain_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveTerrainMap());
        private void ToolStripMenuItem_Save_Maps_Trees_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveTreesMap());
        private void ToolStripMenuItem_Save_Maps_Cities_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => TextureManager.SaveCitiesMap());
        private void ComboBox_BordersType_SelectedIndexChanged(object sender, EventArgs e)
            => UpdateBordersType();

        public void UpdateBordersType()
        {
            Logger.TryOrLog(() =>
            {
                SelectedBordersType = (EnumBordersType)ComboBox_BordersType.SelectedIndex;
                MapManager.UpdateDisplayBorders();
            });
        }

        private void ToolStripMenuItem_Edit_AutoTools_StatesValidation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AutoTools.ValidateAllStates(true));
        private void ToolStripMenuItem_Edit_AutoTools_RegionsValidation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AutoTools.ValidateAllRegions(true));
        private void ToolStripMenuItem_Data_Provinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new ProvinceListForm().ShowDialog()));

        private void ToolStripMenuItem_Data_States_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (StateListForm.Instance == null)
                    Task.Run(() => new StateListForm().ShowDialog());
            });
        }

        private void ToolStripMenuItem_UpdateAll_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => DataManager.UpdateAll());

        private void Button_GenerateColor_MouseDown(object sender, MouseEventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                Color color;
                switch (ComboBox_GenerateColor_Type.SelectedIndex)
                {
                    case 0: color = ProvinceManager.GetNewRandomColor().ToColor(); break;
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

                ToolStripMenuItem_Map_Actions_Merge_All.Enabled = ProvinceManager.GroupSelectedProvinces.Count >= 2;
            });
        }

        private void ToolStripMenuItem_Map_Actions_Merge_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (SelectedMainLayer == EnumMainLayer.PROVINCES_MAP)
                    MergeProvincesTool.MergeProvinces(ProvinceManager.SelectedProvince, ProvinceManager.RMBProvince);
            });
        }

        private void ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AutoTools.RemoveSeaAndLakesContinents(true));

        private void ToolStripMenuItem_Map_Adjacency_DropDownOpened(object sender, EventArgs e)
        {
            void fillInAction(Adjacency adj)
            {
                if (adj != null)
                {
                    adj.GetDisplayData(out EnumAdjaciencyType enumType, out int startId, out int endId, out int throughId, out Value2S startPos, out Value2S endPos, out string adjacencyRuleName, out string comment);

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
            => Logger.TryOrLog(() => AdjacenciesManager.CreateAdjacency(
                ProvinceManager.SelectedProvince,
                ProvinceManager.RMBProvince,
                (EnumAdjaciencyType)ToolStripComboBox_Map_Adjacency_Type.SelectedIndex
            ));

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
            foreach (var texture in mapTextures)
                ListBox_Textures.Items.Add(texture.fileName);
        }

        private void Button_OpenSearchWarningsSettings_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new SearchSettingsForm(EnumSearchSettingsType.WARNINGS).ShowDialog()));
        private void Button_OpenSearchErrorsSettings_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new SearchSettingsForm(EnumSearchSettingsType.ERRORS).ShowDialog()));

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

        private void ToolStripMenuItem_Help_Documentation_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                using (Process.Start(NetworkManager.DocumentationURL)) { }
            });

        private void ToolStripMenuItem_Map_Railway_Split_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.SplitRailwayAtProvince(SupplyManager.SelectedRailway ?? SupplyManager.RMBRailway, ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Railway_Join_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.JoinRailways(SupplyManager.SelectedRailway, SupplyManager.RMBRailway));
        private void ToolStripMenuItem_Data_Recovery_Regions_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Task.Run(() => new StrategicRegionsDataRecoveryForm().ShowDialog()));
        private void ToolStripMenuItem_Map_Railway_AddProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.AddProvinceToRailway(SupplyManager.SelectedRailway, ProvinceManager.RMBProvince));
        private void ToolStripMenuItem_Map_Railway_RemoveProvince_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => RailwaysTool.RemoveProvinceFromRailway(SupplyManager.SelectedRailway, ProvinceManager.RMBProvince));

        private void ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AutoTools.RemoveSeaProvincesFromStates(true));

        private void ToolStripMenuItem_Edit_Scripts_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => new ScriptsForm().ShowDialog());

        private void ToolStripMenuItem_GitHub_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.GitHubRepoURL);
        private void ToolStripMenuItem_Discord_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.DiscordServerURL);
        private void ToolStripMenuItem_Telegram_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.TelegramURL);

        public void SetAdjacencyRules(Dictionary<string, AdjacencyRule>.KeyCollection rules)
        {
            Logger.TryOrLog(() =>
            {
                InvokeAction(() =>
                {
                    ToolStripComboBox_Map_Adjacency_Rule.Items.Clear();
                    ToolStripComboBox_Map_Adjacency_Rule.Items.Add("");
                    foreach (string name in rules)
                        ToolStripComboBox_Map_Adjacency_Rule.Items.Add(name);
                });
            });
        }

        private void ComboBox_EditLayer_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                SelectedEditLayer = (EnumEditLayer)ComboBox_EditLayer.SelectedIndex;
                if (IsFirstLoaded)
                    InitColorsPallete();
            });

        private void ComboBox_Tool_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                SelectedTool = (EnumTool)ComboBox_Tool.SelectedIndex;
                UpdateSelectedMainLayerAndTool(false, true);
            });

        private void ComboBox_Tool_Parameter_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                preferedParameter[(int)SelectedTool] = ComboBox_Tool_Parameter.Text;
                UpdateSelectedMainLayerAndTool(false, false);
            });
        private void ComboBox_Tool_Parameter_Value_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => preferedParameterValue[(int)SelectedTool] = ComboBox_Tool_Parameter_Value.Text);

        private void ToolStripMenuItem_Export_MainLayer_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => ExportTexturePlane(MapManager.MapMainLayer, "main_layer"));

        private void ToolStripMenuItem_Export_SelectedBorders_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => ExportTexturePlane(MapManager.BordersMapPlane, "selected_borders"));

        private void ExportTexturePlane(TexturedPlane plane, string fileNamePrefix)
        {
            if (plane == null || plane.Texture == null)
                return;

            if (!Directory.Exists(@"data\images"))
                Directory.CreateDirectory(@"data\images");

            var filePath = @"data\images\" + fileNamePrefix + " " + DateTime.Now.ToString().Replace('.', '-').Replace(':', '-') + ".bmp";
            plane.Texture.Save(filePath);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.EXPORT_TEXTURE_RESULT,
                    new Dictionary<string, string> { { "{filePath}", filePath } }
                ),
                GuiLocManager.GetLoc(EnumLocKey.EXPORT_TEXTURE_TITLE),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
        }

        private void ToolStripComboBox_Map_Adjacency_Type_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (AdjacenciesManager.GetSelectedSeaCross() == null)
                    return;
                AdjacenciesManager.GetSelectedSeaCross().EnumType = (EnumAdjaciencyType)(ToolStripComboBox_Map_Adjacency_Type.SelectedIndex);
            });

        private void ToolStripComboBox_Map_Adjacency_Rule_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (AdjacenciesManager.GetSelectedSeaCross() == null)
                    return;

                AdjacenciesManager.TryGetAdjacencyRule(ToolStripComboBox_Map_Adjacency_Rule.Text, out var rule);
                AdjacenciesManager.GetSelectedSeaCross().AdjacencyRule = rule;
            });

        private void ToolStripMenuItem_Map_Province_VictoryPoints_Info_DropDownOpened(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.State == null)
                {
                    ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Text = "0";
                    ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Enabled = false;
                    return;
                }
                else ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Enabled = true;

                ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Text = "" + ProvinceManager.RMBProvince.victoryPoints;
            });

        private void ToolStripTextBox_Map_Province_VictoryPoints_Info_Value_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.State == null)
                    return;

                var province = ProvinceManager.RMBProvince;
                uint prevCount = province.victoryPoints;

                if (!uint.TryParse(
                    ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Text.Length == 0 ?
                        "0" :
                        ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Text,
                    out var newCount)
                )
                    return;

                var history = province.State.History.GetValue();
                if (history == null)
                    return;


                Action<uint> action;

                action = (c) =>
                {
                    if (province.State.SetVictoryPoints(province, c))
                    {
                        MapManager.FontRenderController.AddEventData(EnumMapRenderEvents.VICTORY_POINTS, province);
                        Update_ToolStripMenuItem_Map_Province_Items(province);
                        MapManager.HandleMapMainLayerChange(false, GetMainLayer(), GetParameter());
                    }
                };

                MapManager.ActionHistory.Add(
                    () => action(newCount),
                    () => action(prevCount)
                );
            });

        public EnumMainLayer GetMainLayer() => SelectedMainLayer;
        public string GetParameter() => ComboBox_Tool_Parameter.Text;
        private void ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => AutoTools.RemoveGhostProvinces(true));
        private void ToolStripMenuItem_Map_Actions_Merge_All_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => MergeProvincesTool.MergeProvinces(ProvinceManager.RMBProvince, ProvinceManager.GroupSelectedProvinces));
        private void ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => MergeProvincesTool.MergeSelectedProvinces());
        private void ToolStripMenuItem_Edit_Actions_DropDownOpened(object sender, EventArgs e)
            => Logger.TryOrLog(() => ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces.Enabled = ProvinceManager.GroupSelectedProvinces.Count >= 2);

        private void ToolStripMenuItem_Edit_Actions_FindMapChanges_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => FindMapChangesTool.Execute());

        private void ContextMenuStrip_Map_Opened(object sender, EventArgs e)
            => Logger.TryOrLog(() => Update_ContextMenuStrip_Map());

        private void Update_ContextMenuStrip_Map()
        {
            ToolStripMenuItem_Map_Province.Enabled = ProvinceManager.RMBProvince != null;
            var provinceID = "" + ProvinceManager.RMBProvince?.Id;
            if (provinceID.Length == 0)
                provinceID = GuiLocManager.GetLoc(EnumLocKey.NONE);
            ToolStripMenuItem_Map_Province.Text = GuiLocManager.GetLoc(EnumLocKey.PROVINCE) + ": " + provinceID;

            ToolStripMenuItem_Map_State.Enabled = ProvinceManager.RMBProvince != null || StateManager.RMBState != null;
            var stateID = "" + ProvinceManager.RMBProvince?.State?.Id.GetValue();
            if (ProvinceManager.RMBProvince == null)
                stateID = "" + StateManager.RMBState?.Id.GetValue();
            if (stateID.Length == 0)
                stateID = GuiLocManager.GetLoc(EnumLocKey.NONE);
            ToolStripMenuItem_Map_State.Text = GuiLocManager.GetLoc(EnumLocKey.STATE) + ": " + stateID;

            ToolStripMenuItem_Map_Region.Enabled = ProvinceManager.RMBProvince != null || StrategicRegionManager.RMBRegion != null;
            var regionID = "" + ProvinceManager.RMBProvince?.Region?.Id;
            if (ProvinceManager.RMBProvince == null)
                regionID = "" + StrategicRegionManager.RMBRegion?.Id;
            if (regionID.Length == 0)
                regionID = GuiLocManager.GetLoc(EnumLocKey.NONE);
            ToolStripMenuItem_Map_Region.Text = GuiLocManager.GetLoc(EnumLocKey.REGION) + ": " + regionID;
        }

        private void ToolStripMenuItem_Map_State_DropDownOpened(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                IsMapMainLayerChangeEnabled = false;
                Update_ToolStripMenuItem_Map_State_Items();
                IsMapMainLayerChangeEnabled = true;
            });

        private void Update_ToolStripMenuItem_Map_State_Items()
        {
            ToolStripComboBox_Map_State_Id.Enabled = ProvinceManager.RMBProvince != null;
            ToolStripComboBox_Map_State_Id.Items.Clear();
            ToolStripComboBox_Map_State_Id.Items.Add("");

            ushort[] ids = StateManager.GetStatesIds().OrderBy(x => x).ToArray();
            object[] objIds = new object[ids.Length];

            ushort? currentId = null;
            if (ProvinceManager.RMBProvince != null)
                currentId = ProvinceManager.RMBProvince.State?.Id.GetValue();
            else
                currentId = StateManager.RMBState?.Id.GetValue();

            int? index = null;

            for (int i = 0; i < ids.Length; i++)
            {
                objIds[i] = ids[i];
                if (ids[i] == currentId)
                    index = i + 1;
            }

            ToolStripComboBox_Map_State_Id.Items.AddRange(objIds);
            if (index != null)
                ToolStripComboBox_Map_State_Id.SelectedIndex = (int)index;

            ToolStripMenuItem_Map_State_CreateAndSet.Enabled = ProvinceManager.RMBProvince != null;

            ToolStripMenuItem_Map_State_OpenFileInEditor.Enabled = index != null;
            ToolStripMenuItem_Map_State_OpenFileInExplorer.Enabled = index != null;
        }

        private void ToolStripMenuItem_Map_Region_DropDownOpened(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                IsMapMainLayerChangeEnabled = false;
                Update_ToolStripMenuItem_Map_Region_Items();
                IsMapMainLayerChangeEnabled = true;
            });

        private void Update_ToolStripMenuItem_Map_Region_Items()
        {
            ToolStripComboBox_Map_Region_Id.Enabled = ProvinceManager.RMBProvince != null;
            ToolStripComboBox_Map_Region_Id.Items.Clear();
            ToolStripComboBox_Map_Region_Id.Items.Add("");

            ushort[] ids = StrategicRegionManager.GetRegionsIds().OrderBy(x => x).ToArray();
            object[] objIds = new object[ids.Length];

            ushort? currentId = null;
            if (ProvinceManager.RMBProvince != null)
                currentId = ProvinceManager.RMBProvince.Region?.Id;
            else
                currentId = StrategicRegionManager.RMBRegion?.Id;

            int? index = null;

            for (int i = 0; i < ids.Length; i++)
            {
                objIds[i] = ids[i];
                if (ids[i] == currentId)
                    index = i + 1;
            }

            ToolStripComboBox_Map_Region_Id.Items.AddRange(objIds);
            if (index != null)
                ToolStripComboBox_Map_Region_Id.SelectedIndex = (int)index;

            ToolStripMenuItem_Map_Region_CreateAndSet.Enabled = ProvinceManager.RMBProvince != null;

            //ToolStripMenuItem_Map_Region_OpenFileInEditor.Enabled = index != null;
            ToolStripMenuItem_Map_Region_OpenFileInExplorer.Enabled = index != null;
        }

        private void ToolStripMenuItem_Map_State_CreateAndSet_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                var province = ProvinceManager.RMBProvince;
                if (province == null)
                    return;

                var prevState = province.State;
                CreateObjectForm.CreateTasked(
                    EnumCreateObjectType.STATE, true,
                    (id) => //On redo
                    {
                        if (StateManager.TryGetState((ushort)id, out var newState))
                        {
                            if (prevState != null)
                                StateManager.TransferProvince(province, prevState, newState);
                            else
                                newState.AddProvince(province);
                            InvokeAction(() => MapManager.HandleMapMainLayerChange(false, GetMainLayer(), GetParameter()));
                        }
                    },
                    (id) => //On undo
                    {
                        if (StateManager.TryGetState((ushort)id, out var newState))
                        {
                            if (prevState == null)
                                newState.RemoveProvince(province);
                            else
                                StateManager.TransferProvince(province, newState, prevState);
                            InvokeAction(() => MapManager.HandleMapMainLayerChange(false, GetMainLayer(), GetParameter()));
                        }
                    }
                );
            });

        private void ToolStripMenuItem_Map_State_OpenFileInExplorer_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.State == null)
                    return;

                if (!StateManager.TryGetState(ProvinceManager.RMBProvince.State.Id.GetValue(), out var state))
                    return;

                if (!state.TryGetGameFile(out var file))
                    return;

                NetworkManager.OpenLink(file.FilePath);
            });

        private void ToolStripMenuItem_Map_State_OpenFileInEditor_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.State == null)
                    return;

                StateListForm stateListForm;
                if (StateListForm.Instance == null)
                {
                    stateListForm = new StateListForm();
                    stateListForm.Show();
                }
                else stateListForm = StateListForm.Instance;

                stateListForm.Focus();
                stateListForm.FindState(ProvinceManager.RMBProvince.State.Id.GetValue());
            });

        private void ToolStripMenuItem_Map_Region_CreateAndSet_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                var province = ProvinceManager.RMBProvince;
                if (province == null)
                    return;

                var prevRegion = province.Region;
                CreateObjectForm.CreateTasked(
                    EnumCreateObjectType.REGION, true,
                    (id) => //On redo
                    {
                        if (StrategicRegionManager.TryGetRegion((ushort)id, out var newRegion))
                        {
                            if (prevRegion != null)
                                StrategicRegionManager.TransferProvince(province, prevRegion, newRegion);
                            else
                                newRegion.AddProvince(province);
                            InvokeAction(() => MapManager.HandleMapMainLayerChange(false, GetMainLayer(), GetParameter()));
                        }
                    },
                    (id) => //On undo
                    {
                        if (StrategicRegionManager.TryGetRegion((ushort)id, out var newRegion))
                        {
                            if (prevRegion == null)
                                newRegion.RemoveProvince(province);
                            else
                                StrategicRegionManager.TransferProvince(province, newRegion, prevRegion);
                            InvokeAction(() => MapManager.HandleMapMainLayerChange(false, GetMainLayer(), GetParameter()));
                        }
                    }
                );
            });

        private void ToolStripMenuItem_Map_Region_OpenFileInExplorer_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (ProvinceManager.RMBProvince == null || ProvinceManager.RMBProvince.Region == null)
                    return;

                if (!StrategicRegionManager.TryGetRegion(ProvinceManager.RMBProvince.Region.Id, out var region))
                    return;

                if (region.FileInfo == null)
                    return;

                NetworkManager.OpenLink(region.FileInfo.filePath);
            });

        private void ToolStripMenuItem_Map_Region_OpenFileInEditor_Click(object sender, EventArgs e)
        {
            //TODO implement
        }

        private void ToolStripComboBox_Map_State_Id_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (!IsMapMainLayerChangeEnabled)
                    return;

                State newState = null;

                if (ushort.TryParse(ToolStripComboBox_Map_State_Id.Text, out var id))
                    StateManager.TryGetState(id, out newState);

                if (ProvinceManager.RMBProvince == null)
                    return;

                State currentState = ProvinceManager.RMBProvince?.State;

                if (StateManager.TransferProvince(ProvinceManager.RMBProvince, currentState, newState))
                {
                    Update_ContextMenuStrip_Map();
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                ToolStripMenuItem_Map_State_OpenFileInEditor.Enabled = newState != null;
                ToolStripMenuItem_Map_State_OpenFileInExplorer.Enabled = newState != null;
            });

        private void ToolStripComboBox_Map_Region_Id_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                if (!IsMapMainLayerChangeEnabled)
                    return;

                StrategicRegion newRegion = null;

                if (ushort.TryParse(ToolStripComboBox_Map_Region_Id.Text, out var id))
                    StrategicRegionManager.TryGetRegion(id, out newRegion);

                if (ProvinceManager.RMBProvince == null)
                    return;

                StrategicRegion currentRegion = ProvinceManager.RMBProvince?.Region;

                if (StrategicRegionManager.TransferProvince(ProvinceManager.RMBProvince, currentRegion, newRegion))
                {
                    Update_ContextMenuStrip_Map();
                    MapManager.HandleMapMainLayerChange(false, SelectedMainLayer, ComboBox_Tool_Parameter.Text);
                }

                //ToolStripMenuItem_Map_Region_OpenFileInEditor.Enabled = newRegion != null;
                ToolStripMenuItem_Map_Region_OpenFileInExplorer.Enabled = newRegion != null;
            });

        private void ToolStripMenuItem_Edit_Actions_CreateObject_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => CreateObjectForm.CreateTasked());

        public void SetGroupBoxProgressBackColor(Color color)
            => GroupBox_Progress.BackColor = color;
    }
}
