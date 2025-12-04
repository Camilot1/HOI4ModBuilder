namespace HOI4ModBuilder
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TabControl_Main = new System.Windows.Forms.TabControl();
            this.TabPage_Map = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FlowLayoutPanel_ToolBar = new System.Windows.Forms.FlowLayoutPanel();
            this.GroupBox_Main_Layer = new System.Windows.Forms.GroupBox();
            this.СomboBox_MapMainLayer = new System.Windows.Forms.ComboBox();
            this.GroupBox_Main_Layer_Parameter = new System.Windows.Forms.GroupBox();
            this.Button_MapMainLayer_Parameter = new System.Windows.Forms.Button();
            this.GroupBox_Edit_Layer = new System.Windows.Forms.GroupBox();
            this.ComboBox_EditLayer = new System.Windows.Forms.ComboBox();
            this.GroupBox_Tool = new System.Windows.Forms.GroupBox();
            this.ComboBox_Tool = new System.Windows.Forms.ComboBox();
            this.GroupBox_Tool_Parameter = new System.Windows.Forms.GroupBox();
            this.ComboBox_Tool_Parameter = new System.Windows.Forms.ComboBox();
            this.GroupBox_Tool_Parameter_Value = new System.Windows.Forms.GroupBox();
            this.ComboBox_Tool_Parameter_Value = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Panel_SecondColor = new System.Windows.Forms.Panel();
            this.Panel_FirstColor = new System.Windows.Forms.Panel();
            this.GroupBox_GenerateColor = new System.Windows.Forms.GroupBox();
            this.Button_GenerateColor_OpenSettings = new System.Windows.Forms.Button();
            this.Button_GenerateColor = new System.Windows.Forms.Button();
            this.ComboBox_GenerateColor_Type = new System.Windows.Forms.ComboBox();
            this.GroupBox_Palette = new System.Windows.Forms.GroupBox();
            this.FlowLayoutPanel_Color = new System.Windows.Forms.FlowLayoutPanel();
            this.GroupBox_Progress = new System.Windows.Forms.GroupBox();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_MapAdditionalLayers = new System.Windows.Forms.CheckedListBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.ComboBox_BordersType = new System.Windows.Forms.ComboBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.Button_OpenSearchWarningsSettings = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Button_OpenSearchErrorsSettings = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Button_TextureMoveDown = new System.Windows.Forms.Button();
            this.Button_TextureMoveUp = new System.Windows.Forms.Button();
            this.Button_TexturesLoad = new System.Windows.Forms.Button();
            this.Button_TexturesSave = new System.Windows.Forms.Button();
            this.Button_TextureAdd = new System.Windows.Forms.Button();
            this.ListBox_Textures = new System.Windows.Forms.ListBox();
            this.Button_TextureRemove = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.textBox_HOI4PixelPos = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_PixelPos = new System.Windows.Forms.TextBox();
            this.textBox_SelectedObjectId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Panel_Map = new System.Windows.Forms.Panel();
            this.ContextMenuStrip_Map = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Map_Search = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Search_Input = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Search_Position = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Search_Province = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Search_State = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Search_Region = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Province = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_Province_Type = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripMenuItem_Map_Province_IsCoastal = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_Province_Terrain = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripComboBox_Map_Province_Continent = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Province_VictoryPoints_Info = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_State = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_State_Id = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_State_CreateAndSet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_State_OpenFileInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_State_OpenFileInEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Region = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_Region_Id = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Region_CreateAndSet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Region_OpenFileInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Region_OpenFileInEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Adjacency = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Adjacency_Create = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_Adjacency_Type = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripMenuItem_Map_Adjacency_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Adjacency_FirstProvinceId = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Adjacency_FirstPointPos = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Adjacency_SecondProvinceId = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Adjacency_SecondPointPos = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripComboBox_Map_Adjacency_Rule = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripTextBox_Map_Adjacency_Comment = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripMenuItem_Map_Railway = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Railway_Create = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Map_Railway_Level = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Railway_AddProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Railway_RemoveProvince = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Railway_Join = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Railway_Split = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Railway_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_SupplyHub = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_SupplyHub_Create = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_SupplyHub_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Map_Actions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Actions_Merge = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Map_Actions_Merge_All = new System.Windows.Forms.ToolStripMenuItem();
            this.Panel_ColorPicker = new System.Windows.Forms.Panel();
            this.Panel_ColorPicker_Button_Close = new System.Windows.Forms.Button();
            this.Panel_ColorPicker_Button_Save = new System.Windows.Forms.Button();
            this.ElementHost_ColorPicker = new System.Windows.Forms.Integration.ElementHost();
            this.standardColorPicker1 = new ColorPicker.StandardColorPicker();
            this.TabPage_Buildings = new System.Windows.Forms.TabPage();
            this.TabPage_Resources = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.TabPage_Localization = new System.Windows.Forms.TabPage();
            this.TabPage_Home = new System.Windows.Forms.TabPage();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_All = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Save_Maps_Provinces = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_Rivers = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_Terrain = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_Trees = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_Cities = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Maps_Heights = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Definition = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Adjacencies = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Supply = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Supply_All = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Save_Supply_Railways = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Supply_Hubs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Save_States = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Save_Regions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_SaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_All = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Load_Maps_Provinces = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_Rivers = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_Terrain = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_Trees = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_Cities = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Maps_Heights = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Definitions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Adjacencies = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Supply = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Supply_All = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Load_Supply_Railways = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Load_Supply_Hubs = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_LoadAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Update = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_UpdateAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Export_MainLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Export_SelectedBorders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Edit_Actions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Actions_CreateObject = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Actions_FindMapChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesValidation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_StatesValidation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_Scripts = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Data = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripComboBox_Data_Bookmark = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Data_Provinces = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Data_States = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Statistics = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Data_Recovery = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Data_Recovery_Regions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Help_Documentation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Help_About = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language_RU = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language_EN = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_GitHub = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Discord = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Telegram = new System.Windows.Forms.ToolStripMenuItem();
            this.TabControl_Main.SuspendLayout();
            this.TabPage_Map.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.FlowLayoutPanel_ToolBar.SuspendLayout();
            this.GroupBox_Main_Layer.SuspendLayout();
            this.GroupBox_Main_Layer_Parameter.SuspendLayout();
            this.GroupBox_Edit_Layer.SuspendLayout();
            this.GroupBox_Tool.SuspendLayout();
            this.GroupBox_Tool_Parameter.SuspendLayout();
            this.GroupBox_Tool_Parameter_Value.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.GroupBox_GenerateColor.SuspendLayout();
            this.GroupBox_Palette.SuspendLayout();
            this.GroupBox_Progress.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.Panel_Map.SuspendLayout();
            this.ContextMenuStrip_Map.SuspendLayout();
            this.Panel_ColorPicker.SuspendLayout();
            this.TabPage_Resources.SuspendLayout();
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl_Main
            // 
            this.TabControl_Main.Controls.Add(this.TabPage_Map);
            this.TabControl_Main.Controls.Add(this.TabPage_Buildings);
            this.TabControl_Main.Controls.Add(this.TabPage_Resources);
            this.TabControl_Main.Controls.Add(this.TabPage_Localization);
            this.TabControl_Main.Controls.Add(this.TabPage_Home);
            resources.ApplyResources(this.TabControl_Main, "TabControl_Main");
            this.TabControl_Main.Name = "TabControl_Main";
            this.TabControl_Main.SelectedIndex = 0;
            // 
            // TabPage_Map
            // 
            this.TabPage_Map.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.TabPage_Map, "TabPage_Map");
            this.TabPage_Map.Name = "TabPage_Map";
            this.TabPage_Map.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.FlowLayoutPanel_ToolBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Panel_Map, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // FlowLayoutPanel_ToolBar
            // 
            resources.ApplyResources(this.FlowLayoutPanel_ToolBar, "FlowLayoutPanel_ToolBar");
            this.tableLayoutPanel1.SetColumnSpan(this.FlowLayoutPanel_ToolBar, 2);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Main_Layer);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Main_Layer_Parameter);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Edit_Layer);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Tool);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Tool_Parameter);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Tool_Parameter_Value);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.groupBox3);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_GenerateColor);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Palette);
            this.FlowLayoutPanel_ToolBar.Controls.Add(this.GroupBox_Progress);
            this.FlowLayoutPanel_ToolBar.Name = "FlowLayoutPanel_ToolBar";
            // 
            // GroupBox_Main_Layer
            // 
            resources.ApplyResources(this.GroupBox_Main_Layer, "GroupBox_Main_Layer");
            this.GroupBox_Main_Layer.Controls.Add(this.СomboBox_MapMainLayer);
            this.GroupBox_Main_Layer.Name = "GroupBox_Main_Layer";
            this.GroupBox_Main_Layer.TabStop = false;
            // 
            // СomboBox_MapMainLayer
            // 
            this.СomboBox_MapMainLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.СomboBox_MapMainLayer.FormattingEnabled = true;
            resources.ApplyResources(this.СomboBox_MapMainLayer, "СomboBox_MapMainLayer");
            this.СomboBox_MapMainLayer.Name = "СomboBox_MapMainLayer";
            this.СomboBox_MapMainLayer.SelectedIndexChanged += new System.EventHandler(this.СomboBox_MapMainLayer_SelectedIndexChanged);
            // 
            // GroupBox_Main_Layer_Parameter
            // 
            resources.ApplyResources(this.GroupBox_Main_Layer_Parameter, "GroupBox_Main_Layer_Parameter");
            this.GroupBox_Main_Layer_Parameter.Controls.Add(this.Button_MapMainLayer_Parameter);
            this.GroupBox_Main_Layer_Parameter.Name = "GroupBox_Main_Layer_Parameter";
            this.GroupBox_Main_Layer_Parameter.TabStop = false;
            // 
            // Button_MapMainLayer_Parameter
            // 
            resources.ApplyResources(this.Button_MapMainLayer_Parameter, "Button_MapMainLayer_Parameter");
            this.Button_MapMainLayer_Parameter.Name = "Button_MapMainLayer_Parameter";
            this.Button_MapMainLayer_Parameter.UseVisualStyleBackColor = true;
            this.Button_MapMainLayer_Parameter.Click += new System.EventHandler(this.Button_MapMainLayer_Parameter_Click);
            // 
            // GroupBox_Edit_Layer
            // 
            resources.ApplyResources(this.GroupBox_Edit_Layer, "GroupBox_Edit_Layer");
            this.GroupBox_Edit_Layer.Controls.Add(this.ComboBox_EditLayer);
            this.GroupBox_Edit_Layer.Name = "GroupBox_Edit_Layer";
            this.GroupBox_Edit_Layer.TabStop = false;
            // 
            // ComboBox_EditLayer
            // 
            this.ComboBox_EditLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_EditLayer.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_EditLayer, "ComboBox_EditLayer");
            this.ComboBox_EditLayer.Name = "ComboBox_EditLayer";
            this.ComboBox_EditLayer.SelectedIndexChanged += new System.EventHandler(this.ComboBox_EditLayer_SelectedIndexChanged);
            // 
            // GroupBox_Tool
            // 
            resources.ApplyResources(this.GroupBox_Tool, "GroupBox_Tool");
            this.GroupBox_Tool.Controls.Add(this.ComboBox_Tool);
            this.GroupBox_Tool.Name = "GroupBox_Tool";
            this.GroupBox_Tool.TabStop = false;
            // 
            // ComboBox_Tool
            // 
            this.ComboBox_Tool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Tool.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_Tool, "ComboBox_Tool");
            this.ComboBox_Tool.Name = "ComboBox_Tool";
            this.ComboBox_Tool.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Tool_SelectedIndexChanged);
            // 
            // GroupBox_Tool_Parameter
            // 
            resources.ApplyResources(this.GroupBox_Tool_Parameter, "GroupBox_Tool_Parameter");
            this.GroupBox_Tool_Parameter.Controls.Add(this.ComboBox_Tool_Parameter);
            this.GroupBox_Tool_Parameter.Name = "GroupBox_Tool_Parameter";
            this.GroupBox_Tool_Parameter.TabStop = false;
            // 
            // ComboBox_Tool_Parameter
            // 
            this.ComboBox_Tool_Parameter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Tool_Parameter.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_Tool_Parameter, "ComboBox_Tool_Parameter");
            this.ComboBox_Tool_Parameter.Name = "ComboBox_Tool_Parameter";
            this.ComboBox_Tool_Parameter.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Tool_Parameter_SelectedIndexChanged);
            // 
            // GroupBox_Tool_Parameter_Value
            // 
            resources.ApplyResources(this.GroupBox_Tool_Parameter_Value, "GroupBox_Tool_Parameter_Value");
            this.GroupBox_Tool_Parameter_Value.Controls.Add(this.ComboBox_Tool_Parameter_Value);
            this.GroupBox_Tool_Parameter_Value.Name = "GroupBox_Tool_Parameter_Value";
            this.GroupBox_Tool_Parameter_Value.TabStop = false;
            // 
            // ComboBox_Tool_Parameter_Value
            // 
            this.ComboBox_Tool_Parameter_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Tool_Parameter_Value.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_Tool_Parameter_Value, "ComboBox_Tool_Parameter_Value");
            this.ComboBox_Tool_Parameter_Value.Name = "ComboBox_Tool_Parameter_Value";
            this.ComboBox_Tool_Parameter_Value.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Tool_Parameter_Value_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Panel_SecondColor);
            this.groupBox3.Controls.Add(this.Panel_FirstColor);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // Panel_SecondColor
            // 
            this.Panel_SecondColor.BackColor = System.Drawing.Color.Transparent;
            this.Panel_SecondColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_SecondColor.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.Panel_SecondColor, "Panel_SecondColor");
            this.Panel_SecondColor.Name = "Panel_SecondColor";
            this.Panel_SecondColor.Click += new System.EventHandler(this.Panel_SecondColor_Click);
            // 
            // Panel_FirstColor
            // 
            this.Panel_FirstColor.BackColor = System.Drawing.Color.Transparent;
            this.Panel_FirstColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_FirstColor.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.Panel_FirstColor, "Panel_FirstColor");
            this.Panel_FirstColor.Name = "Panel_FirstColor";
            this.Panel_FirstColor.Click += new System.EventHandler(this.Panel_FirstColor_Click);
            // 
            // GroupBox_GenerateColor
            // 
            this.GroupBox_GenerateColor.Controls.Add(this.Button_GenerateColor_OpenSettings);
            this.GroupBox_GenerateColor.Controls.Add(this.Button_GenerateColor);
            this.GroupBox_GenerateColor.Controls.Add(this.ComboBox_GenerateColor_Type);
            resources.ApplyResources(this.GroupBox_GenerateColor, "GroupBox_GenerateColor");
            this.GroupBox_GenerateColor.Name = "GroupBox_GenerateColor";
            this.GroupBox_GenerateColor.TabStop = false;
            // 
            // Button_GenerateColor_OpenSettings
            // 
            resources.ApplyResources(this.Button_GenerateColor_OpenSettings, "Button_GenerateColor_OpenSettings");
            this.Button_GenerateColor_OpenSettings.Name = "Button_GenerateColor_OpenSettings";
            this.Button_GenerateColor_OpenSettings.UseVisualStyleBackColor = true;
            this.Button_GenerateColor_OpenSettings.Click += new System.EventHandler(this.Button_GenerateColor_OpenSettings_Click);
            // 
            // Button_GenerateColor
            // 
            resources.ApplyResources(this.Button_GenerateColor, "Button_GenerateColor");
            this.Button_GenerateColor.Name = "Button_GenerateColor";
            this.Button_GenerateColor.UseVisualStyleBackColor = true;
            this.Button_GenerateColor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_GenerateColor_MouseDown);
            // 
            // ComboBox_GenerateColor_Type
            // 
            this.ComboBox_GenerateColor_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_GenerateColor_Type.FormattingEnabled = true;
            this.ComboBox_GenerateColor_Type.Items.AddRange(new object[] {
            resources.GetString("ComboBox_GenerateColor_Type.Items"),
            resources.GetString("ComboBox_GenerateColor_Type.Items1"),
            resources.GetString("ComboBox_GenerateColor_Type.Items2"),
            resources.GetString("ComboBox_GenerateColor_Type.Items3")});
            resources.ApplyResources(this.ComboBox_GenerateColor_Type, "ComboBox_GenerateColor_Type");
            this.ComboBox_GenerateColor_Type.Name = "ComboBox_GenerateColor_Type";
            // 
            // GroupBox_Palette
            // 
            resources.ApplyResources(this.GroupBox_Palette, "GroupBox_Palette");
            this.GroupBox_Palette.Controls.Add(this.FlowLayoutPanel_Color);
            this.GroupBox_Palette.Name = "GroupBox_Palette";
            this.GroupBox_Palette.TabStop = false;
            // 
            // FlowLayoutPanel_Color
            // 
            resources.ApplyResources(this.FlowLayoutPanel_Color, "FlowLayoutPanel_Color");
            this.FlowLayoutPanel_Color.Name = "FlowLayoutPanel_Color";
            // 
            // GroupBox_Progress
            // 
            resources.ApplyResources(this.GroupBox_Progress, "GroupBox_Progress");
            this.GroupBox_Progress.Controls.Add(this.ProgressBar1);
            this.GroupBox_Progress.Name = "GroupBox_Progress";
            this.GroupBox_Progress.TabStop = false;
            // 
            // ProgressBar1
            // 
            resources.ApplyResources(this.ProgressBar1, "ProgressBar1");
            this.ProgressBar1.Name = "ProgressBar1";
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.groupBox7);
            this.flowLayoutPanel2.Controls.Add(this.groupBox8);
            this.flowLayoutPanel2.Controls.Add(this.groupBox10);
            this.flowLayoutPanel2.Controls.Add(this.groupBox4);
            this.flowLayoutPanel2.Controls.Add(this.groupBox5);
            this.flowLayoutPanel2.Controls.Add(this.groupBox11);
            this.flowLayoutPanel2.Controls.Add(this.groupBox9);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.CheckedListBox_MapAdditionalLayers);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // CheckedListBox_MapAdditionalLayers
            // 
            this.CheckedListBox_MapAdditionalLayers.CheckOnClick = true;
            this.CheckedListBox_MapAdditionalLayers.FormattingEnabled = true;
            resources.ApplyResources(this.CheckedListBox_MapAdditionalLayers, "CheckedListBox_MapAdditionalLayers");
            this.CheckedListBox_MapAdditionalLayers.Name = "CheckedListBox_MapAdditionalLayers";
            this.CheckedListBox_MapAdditionalLayers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CheckedListBox_MapAdditionalLayers_MouseUp);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.ComboBox_BordersType);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // ComboBox_BordersType
            // 
            this.ComboBox_BordersType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_BordersType.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox_BordersType, "ComboBox_BordersType");
            this.ComboBox_BordersType.Name = "ComboBox_BordersType";
            this.ComboBox_BordersType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_BordersType_SelectedIndexChanged);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.Button_OpenSearchWarningsSettings);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // Button_OpenSearchWarningsSettings
            // 
            resources.ApplyResources(this.Button_OpenSearchWarningsSettings, "Button_OpenSearchWarningsSettings");
            this.Button_OpenSearchWarningsSettings.Name = "Button_OpenSearchWarningsSettings";
            this.Button_OpenSearchWarningsSettings.UseVisualStyleBackColor = true;
            this.Button_OpenSearchWarningsSettings.Click += new System.EventHandler(this.Button_OpenSearchWarningsSettings_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Button_OpenSearchErrorsSettings);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // Button_OpenSearchErrorsSettings
            // 
            resources.ApplyResources(this.Button_OpenSearchErrorsSettings, "Button_OpenSearchErrorsSettings");
            this.Button_OpenSearchErrorsSettings.Name = "Button_OpenSearchErrorsSettings";
            this.Button_OpenSearchErrorsSettings.UseVisualStyleBackColor = true;
            this.Button_OpenSearchErrorsSettings.Click += new System.EventHandler(this.Button_OpenSearchErrorsSettings_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Button_TextureMoveDown);
            this.groupBox5.Controls.Add(this.Button_TextureMoveUp);
            this.groupBox5.Controls.Add(this.Button_TexturesLoad);
            this.groupBox5.Controls.Add(this.Button_TexturesSave);
            this.groupBox5.Controls.Add(this.Button_TextureAdd);
            this.groupBox5.Controls.Add(this.ListBox_Textures);
            this.groupBox5.Controls.Add(this.Button_TextureRemove);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // Button_TextureMoveDown
            // 
            resources.ApplyResources(this.Button_TextureMoveDown, "Button_TextureMoveDown");
            this.Button_TextureMoveDown.Name = "Button_TextureMoveDown";
            this.Button_TextureMoveDown.UseVisualStyleBackColor = true;
            this.Button_TextureMoveDown.Click += new System.EventHandler(this.Button_TextureMoveDown_Click);
            // 
            // Button_TextureMoveUp
            // 
            resources.ApplyResources(this.Button_TextureMoveUp, "Button_TextureMoveUp");
            this.Button_TextureMoveUp.Name = "Button_TextureMoveUp";
            this.Button_TextureMoveUp.UseVisualStyleBackColor = true;
            this.Button_TextureMoveUp.Click += new System.EventHandler(this.Button_TextureMoveUp_Click);
            // 
            // Button_TexturesLoad
            // 
            resources.ApplyResources(this.Button_TexturesLoad, "Button_TexturesLoad");
            this.Button_TexturesLoad.Name = "Button_TexturesLoad";
            this.Button_TexturesLoad.UseVisualStyleBackColor = true;
            this.Button_TexturesLoad.Click += new System.EventHandler(this.Button_TexturesLoad_Click);
            // 
            // Button_TexturesSave
            // 
            resources.ApplyResources(this.Button_TexturesSave, "Button_TexturesSave");
            this.Button_TexturesSave.Name = "Button_TexturesSave";
            this.Button_TexturesSave.UseVisualStyleBackColor = true;
            this.Button_TexturesSave.Click += new System.EventHandler(this.Button_TexturesSave_Click);
            // 
            // Button_TextureAdd
            // 
            resources.ApplyResources(this.Button_TextureAdd, "Button_TextureAdd");
            this.Button_TextureAdd.Name = "Button_TextureAdd";
            this.Button_TextureAdd.UseVisualStyleBackColor = true;
            this.Button_TextureAdd.Click += new System.EventHandler(this.Button_TextureAdd_Click);
            // 
            // ListBox_Textures
            // 
            resources.ApplyResources(this.ListBox_Textures, "ListBox_Textures");
            this.ListBox_Textures.FormattingEnabled = true;
            this.ListBox_Textures.Name = "ListBox_Textures";
            // 
            // Button_TextureRemove
            // 
            resources.ApplyResources(this.Button_TextureRemove, "Button_TextureRemove");
            this.Button_TextureRemove.Name = "Button_TextureRemove";
            this.Button_TextureRemove.UseVisualStyleBackColor = true;
            this.Button_TextureRemove.Click += new System.EventHandler(this.Button_TextureRemove_Click);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.textBox_HOI4PixelPos);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // textBox_HOI4PixelPos
            // 
            resources.ApplyResources(this.textBox_HOI4PixelPos, "textBox_HOI4PixelPos");
            this.textBox_HOI4PixelPos.Name = "textBox_HOI4PixelPos";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label3);
            this.groupBox9.Controls.Add(this.textBox_PixelPos);
            this.groupBox9.Controls.Add(this.textBox_SelectedObjectId);
            this.groupBox9.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBox_PixelPos
            // 
            resources.ApplyResources(this.textBox_PixelPos, "textBox_PixelPos");
            this.textBox_PixelPos.Name = "textBox_PixelPos";
            // 
            // textBox_SelectedObjectId
            // 
            resources.ApplyResources(this.textBox_SelectedObjectId, "textBox_SelectedObjectId");
            this.textBox_SelectedObjectId.Name = "textBox_SelectedObjectId";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Panel_Map
            // 
            resources.ApplyResources(this.Panel_Map, "Panel_Map");
            this.Panel_Map.ContextMenuStrip = this.ContextMenuStrip_Map;
            this.Panel_Map.Controls.Add(this.Panel_ColorPicker);
            this.Panel_Map.Name = "Panel_Map";
            // 
            // ContextMenuStrip_Map
            // 
            this.ContextMenuStrip_Map.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Map_Search,
            this.toolStripSeparator25,
            this.ToolStripMenuItem_Map_Province,
            this.ToolStripMenuItem_Map_State,
            this.ToolStripMenuItem_Map_Region,
            this.toolStripSeparator7,
            this.ToolStripMenuItem_Map_Adjacency,
            this.ToolStripMenuItem_Map_Railway,
            this.ToolStripMenuItem_Map_SupplyHub,
            this.toolStripSeparator10,
            this.ToolStripMenuItem_Map_Actions});
            this.ContextMenuStrip_Map.Name = "ContextMenuStrip_Plane_Map";
            resources.ApplyResources(this.ContextMenuStrip_Map, "ContextMenuStrip_Map");
            this.ContextMenuStrip_Map.Opened += new System.EventHandler(this.ContextMenuStrip_Map_Opened);
            // 
            // ToolStripMenuItem_Map_Search
            // 
            this.ToolStripMenuItem_Map_Search.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripTextBox_Map_Search_Input,
            this.ToolStripMenuItem_Map_Search_Position,
            this.ToolStripMenuItem_Map_Search_Province,
            this.ToolStripMenuItem_Map_Search_State,
            this.ToolStripMenuItem_Map_Search_Region});
            this.ToolStripMenuItem_Map_Search.Name = "ToolStripMenuItem_Map_Search";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Search, "ToolStripMenuItem_Map_Search");
            // 
            // ToolStripTextBox_Map_Search_Input
            // 
            this.ToolStripTextBox_Map_Search_Input.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripTextBox_Map_Search_Input.Name = "ToolStripTextBox_Map_Search_Input";
            resources.ApplyResources(this.ToolStripTextBox_Map_Search_Input, "ToolStripTextBox_Map_Search_Input");
            // 
            // ToolStripMenuItem_Map_Search_Position
            // 
            this.ToolStripMenuItem_Map_Search_Position.Name = "ToolStripMenuItem_Map_Search_Position";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Search_Position, "ToolStripMenuItem_Map_Search_Position");
            this.ToolStripMenuItem_Map_Search_Position.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Search_Position_Click);
            // 
            // ToolStripMenuItem_Map_Search_Province
            // 
            this.ToolStripMenuItem_Map_Search_Province.Name = "ToolStripMenuItem_Map_Search_Province";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Search_Province, "ToolStripMenuItem_Map_Search_Province");
            this.ToolStripMenuItem_Map_Search_Province.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Search_Province_Click);
            // 
            // ToolStripMenuItem_Map_Search_State
            // 
            this.ToolStripMenuItem_Map_Search_State.Name = "ToolStripMenuItem_Map_Search_State";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Search_State, "ToolStripMenuItem_Map_Search_State");
            this.ToolStripMenuItem_Map_Search_State.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Search_State_Click);
            // 
            // ToolStripMenuItem_Map_Search_Region
            // 
            this.ToolStripMenuItem_Map_Search_Region.Name = "ToolStripMenuItem_Map_Search_Region";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Search_Region, "ToolStripMenuItem_Map_Search_Region");
            this.ToolStripMenuItem_Map_Search_Region.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Search_Region_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // ToolStripMenuItem_Map_Province
            // 
            this.ToolStripMenuItem_Map_Province.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripComboBox_Map_Province_Type,
            this.ToolStripMenuItem_Map_Province_IsCoastal,
            this.ToolStripComboBox_Map_Province_Terrain,
            this.ToolStripComboBox_Map_Province_Continent,
            this.toolStripSeparator24,
            this.ToolStripMenuItem_Map_Province_VictoryPoints_Info});
            this.ToolStripMenuItem_Map_Province.Name = "ToolStripMenuItem_Map_Province";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Province, "ToolStripMenuItem_Map_Province");
            this.ToolStripMenuItem_Map_Province.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Province_DropDownOpened);
            // 
            // ToolStripComboBox_Map_Province_Type
            // 
            this.ToolStripComboBox_Map_Province_Type.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripComboBox_Map_Province_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Province_Type.Items.AddRange(new object[] {
            resources.GetString("ToolStripComboBox_Map_Province_Type.Items"),
            resources.GetString("ToolStripComboBox_Map_Province_Type.Items1"),
            resources.GetString("ToolStripComboBox_Map_Province_Type.Items2")});
            this.ToolStripComboBox_Map_Province_Type.Name = "ToolStripComboBox_Map_Province_Type";
            resources.ApplyResources(this.ToolStripComboBox_Map_Province_Type, "ToolStripComboBox_Map_Province_Type");
            this.ToolStripComboBox_Map_Province_Type.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Province_Type_SelectedIndexChanged);
            // 
            // ToolStripMenuItem_Map_Province_IsCoastal
            // 
            this.ToolStripMenuItem_Map_Province_IsCoastal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ToolStripMenuItem_Map_Province_IsCoastal.Name = "ToolStripMenuItem_Map_Province_IsCoastal";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Province_IsCoastal, "ToolStripMenuItem_Map_Province_IsCoastal");
            this.ToolStripMenuItem_Map_Province_IsCoastal.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Province_IsCoastal_Click);
            // 
            // ToolStripComboBox_Map_Province_Terrain
            // 
            this.ToolStripComboBox_Map_Province_Terrain.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripComboBox_Map_Province_Terrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Province_Terrain.Name = "ToolStripComboBox_Map_Province_Terrain";
            resources.ApplyResources(this.ToolStripComboBox_Map_Province_Terrain, "ToolStripComboBox_Map_Province_Terrain");
            this.ToolStripComboBox_Map_Province_Terrain.Tag = "test";
            this.ToolStripComboBox_Map_Province_Terrain.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Province_Terrain_SelectedIndexChanged);
            // 
            // ToolStripComboBox_Map_Province_Continent
            // 
            this.ToolStripComboBox_Map_Province_Continent.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripComboBox_Map_Province_Continent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Province_Continent.Name = "ToolStripComboBox_Map_Province_Continent";
            resources.ApplyResources(this.ToolStripComboBox_Map_Province_Continent, "ToolStripComboBox_Map_Province_Continent");
            this.ToolStripComboBox_Map_Province_Continent.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Province_Continent_SelectedIndexChanged);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // ToolStripMenuItem_Map_Province_VictoryPoints_Info
            // 
            this.ToolStripMenuItem_Map_Province_VictoryPoints_Info.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value});
            this.ToolStripMenuItem_Map_Province_VictoryPoints_Info.Name = "ToolStripMenuItem_Map_Province_VictoryPoints_Info";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Province_VictoryPoints_Info, "ToolStripMenuItem_Map_Province_VictoryPoints_Info");
            this.ToolStripMenuItem_Map_Province_VictoryPoints_Info.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Province_VictoryPoints_Info_DropDownOpened);
            // 
            // ToolStripTextBox_Map_Province_VictoryPoints_Info_Value
            // 
            this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.Name = "ToolStripTextBox_Map_Province_VictoryPoints_Info_Value";
            resources.ApplyResources(this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value, "ToolStripTextBox_Map_Province_VictoryPoints_Info_Value");
            this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value.TextChanged += new System.EventHandler(this.ToolStripTextBox_Map_Province_VictoryPoints_Info_Value_TextChanged);
            // 
            // ToolStripMenuItem_Map_State
            // 
            this.ToolStripMenuItem_Map_State.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripComboBox_Map_State_Id,
            this.toolStripSeparator18,
            this.ToolStripMenuItem_Map_State_CreateAndSet,
            this.toolStripSeparator27,
            this.ToolStripMenuItem_Map_State_OpenFileInExplorer,
            this.ToolStripMenuItem_Map_State_OpenFileInEditor});
            this.ToolStripMenuItem_Map_State.Name = "ToolStripMenuItem_Map_State";
            resources.ApplyResources(this.ToolStripMenuItem_Map_State, "ToolStripMenuItem_Map_State");
            this.ToolStripMenuItem_Map_State.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_State_DropDownOpened);
            // 
            // ToolStripComboBox_Map_State_Id
            // 
            this.ToolStripComboBox_Map_State_Id.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_State_Id.Name = "ToolStripComboBox_Map_State_Id";
            resources.ApplyResources(this.ToolStripComboBox_Map_State_Id, "ToolStripComboBox_Map_State_Id");
            this.ToolStripComboBox_Map_State_Id.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_State_Id_SelectedIndexChanged);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // ToolStripMenuItem_Map_State_CreateAndSet
            // 
            this.ToolStripMenuItem_Map_State_CreateAndSet.Name = "ToolStripMenuItem_Map_State_CreateAndSet";
            resources.ApplyResources(this.ToolStripMenuItem_Map_State_CreateAndSet, "ToolStripMenuItem_Map_State_CreateAndSet");
            this.ToolStripMenuItem_Map_State_CreateAndSet.Click += new System.EventHandler(this.ToolStripMenuItem_Map_State_CreateAndSet_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(this.toolStripSeparator27, "toolStripSeparator27");
            // 
            // ToolStripMenuItem_Map_State_OpenFileInExplorer
            // 
            this.ToolStripMenuItem_Map_State_OpenFileInExplorer.Name = "ToolStripMenuItem_Map_State_OpenFileInExplorer";
            resources.ApplyResources(this.ToolStripMenuItem_Map_State_OpenFileInExplorer, "ToolStripMenuItem_Map_State_OpenFileInExplorer");
            this.ToolStripMenuItem_Map_State_OpenFileInExplorer.Click += new System.EventHandler(this.ToolStripMenuItem_Map_State_OpenFileInExplorer_Click);
            // 
            // ToolStripMenuItem_Map_State_OpenFileInEditor
            // 
            this.ToolStripMenuItem_Map_State_OpenFileInEditor.Name = "ToolStripMenuItem_Map_State_OpenFileInEditor";
            resources.ApplyResources(this.ToolStripMenuItem_Map_State_OpenFileInEditor, "ToolStripMenuItem_Map_State_OpenFileInEditor");
            this.ToolStripMenuItem_Map_State_OpenFileInEditor.Click += new System.EventHandler(this.ToolStripMenuItem_Map_State_OpenFileInEditor_Click);
            // 
            // ToolStripMenuItem_Map_Region
            // 
            this.ToolStripMenuItem_Map_Region.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripComboBox_Map_Region_Id,
            this.toolStripSeparator28,
            this.ToolStripMenuItem_Map_Region_CreateAndSet,
            this.toolStripSeparator29,
            this.ToolStripMenuItem_Map_Region_OpenFileInExplorer,
            this.ToolStripMenuItem_Map_Region_OpenFileInEditor});
            this.ToolStripMenuItem_Map_Region.Name = "ToolStripMenuItem_Map_Region";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Region, "ToolStripMenuItem_Map_Region");
            this.ToolStripMenuItem_Map_Region.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Region_DropDownOpened);
            // 
            // ToolStripComboBox_Map_Region_Id
            // 
            this.ToolStripComboBox_Map_Region_Id.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Region_Id.Name = "ToolStripComboBox_Map_Region_Id";
            resources.ApplyResources(this.ToolStripComboBox_Map_Region_Id, "ToolStripComboBox_Map_Region_Id");
            this.ToolStripComboBox_Map_Region_Id.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Region_Id_SelectedIndexChanged);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            resources.ApplyResources(this.toolStripSeparator28, "toolStripSeparator28");
            // 
            // ToolStripMenuItem_Map_Region_CreateAndSet
            // 
            this.ToolStripMenuItem_Map_Region_CreateAndSet.Name = "ToolStripMenuItem_Map_Region_CreateAndSet";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Region_CreateAndSet, "ToolStripMenuItem_Map_Region_CreateAndSet");
            this.ToolStripMenuItem_Map_Region_CreateAndSet.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Region_CreateAndSet_Click);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            resources.ApplyResources(this.toolStripSeparator29, "toolStripSeparator29");
            // 
            // ToolStripMenuItem_Map_Region_OpenFileInExplorer
            // 
            this.ToolStripMenuItem_Map_Region_OpenFileInExplorer.Name = "ToolStripMenuItem_Map_Region_OpenFileInExplorer";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Region_OpenFileInExplorer, "ToolStripMenuItem_Map_Region_OpenFileInExplorer");
            this.ToolStripMenuItem_Map_Region_OpenFileInExplorer.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Region_OpenFileInExplorer_Click);
            // 
            // ToolStripMenuItem_Map_Region_OpenFileInEditor
            // 
            resources.ApplyResources(this.ToolStripMenuItem_Map_Region_OpenFileInEditor, "ToolStripMenuItem_Map_Region_OpenFileInEditor");
            this.ToolStripMenuItem_Map_Region_OpenFileInEditor.Name = "ToolStripMenuItem_Map_Region_OpenFileInEditor";
            this.ToolStripMenuItem_Map_Region_OpenFileInEditor.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Region_OpenFileInEditor_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // ToolStripMenuItem_Map_Adjacency
            // 
            this.ToolStripMenuItem_Map_Adjacency.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Map_Adjacency_Create,
            this.ToolStripComboBox_Map_Adjacency_Type,
            this.ToolStripMenuItem_Map_Adjacency_Remove,
            this.toolStripSeparator16,
            this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince,
            this.ToolStripTextBox_Map_Adjacency_FirstProvinceId,
            this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint,
            this.ToolStripTextBox_Map_Adjacency_FirstPointPos,
            this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint,
            this.toolStripSeparator12,
            this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince,
            this.ToolStripTextBox_Map_Adjacency_SecondProvinceId,
            this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint,
            this.ToolStripTextBox_Map_Adjacency_SecondPointPos,
            this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint,
            this.toolStripSeparator13,
            this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince,
            this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId,
            this.toolStripSeparator14,
            this.ToolStripComboBox_Map_Adjacency_Rule,
            this.toolStripSeparator15,
            this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince,
            this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince,
            this.toolStripSeparator17,
            this.ToolStripTextBox_Map_Adjacency_Comment});
            this.ToolStripMenuItem_Map_Adjacency.Name = "ToolStripMenuItem_Map_Adjacency";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency, "ToolStripMenuItem_Map_Adjacency");
            this.ToolStripMenuItem_Map_Adjacency.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_DropDownOpened);
            // 
            // ToolStripMenuItem_Map_Adjacency_Create
            // 
            this.ToolStripMenuItem_Map_Adjacency_Create.Name = "ToolStripMenuItem_Map_Adjacency_Create";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_Create, "ToolStripMenuItem_Map_Adjacency_Create");
            this.ToolStripMenuItem_Map_Adjacency_Create.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_Create_Click);
            // 
            // ToolStripComboBox_Map_Adjacency_Type
            // 
            this.ToolStripComboBox_Map_Adjacency_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Adjacency_Type.Name = "ToolStripComboBox_Map_Adjacency_Type";
            resources.ApplyResources(this.ToolStripComboBox_Map_Adjacency_Type, "ToolStripComboBox_Map_Adjacency_Type");
            this.ToolStripComboBox_Map_Adjacency_Type.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Adjacency_Type_SelectedIndexChanged);
            // 
            // ToolStripMenuItem_Map_Adjacency_Remove
            // 
            this.ToolStripMenuItem_Map_Adjacency_Remove.Name = "ToolStripMenuItem_Map_Adjacency_Remove";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_Remove, "ToolStripMenuItem_Map_Adjacency_Remove");
            this.ToolStripMenuItem_Map_Adjacency_Remove.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_Remove_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // ToolStripMenuItem_Map_Adjacency_SetFirstProvince
            // 
            this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince.Name = "ToolStripMenuItem_Map_Adjacency_SetFirstProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince, "ToolStripMenuItem_Map_Adjacency_SetFirstProvince");
            this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_SetFirstProvince_Click);
            // 
            // ToolStripTextBox_Map_Adjacency_FirstProvinceId
            // 
            this.ToolStripTextBox_Map_Adjacency_FirstProvinceId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ToolStripTextBox_Map_Adjacency_FirstProvinceId.Name = "ToolStripTextBox_Map_Adjacency_FirstProvinceId";
            this.ToolStripTextBox_Map_Adjacency_FirstProvinceId.ReadOnly = true;
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_FirstProvinceId, "ToolStripTextBox_Map_Adjacency_FirstProvinceId");
            // 
            // ToolStripMenuItem_Map_Adjacency_SetFirstPoint
            // 
            this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint.Name = "ToolStripMenuItem_Map_Adjacency_SetFirstPoint";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint, "ToolStripMenuItem_Map_Adjacency_SetFirstPoint");
            this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_SetFirstPoint_Click);
            // 
            // ToolStripTextBox_Map_Adjacency_FirstPointPos
            // 
            this.ToolStripTextBox_Map_Adjacency_FirstPointPos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ToolStripTextBox_Map_Adjacency_FirstPointPos.Name = "ToolStripTextBox_Map_Adjacency_FirstPointPos";
            this.ToolStripTextBox_Map_Adjacency_FirstPointPos.ReadOnly = true;
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_FirstPointPos, "ToolStripTextBox_Map_Adjacency_FirstPointPos");
            // 
            // ToolStripMenuItem_Map_Adjacency_ResetFirstPoint
            // 
            this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint.Name = "ToolStripMenuItem_Map_Adjacency_ResetFirstPoint";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint, "ToolStripMenuItem_Map_Adjacency_ResetFirstPoint");
            this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_ResetFirstPoint_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // ToolStripMenuItem_Map_Adjacency_SetSecondProvince
            // 
            this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince.Name = "ToolStripMenuItem_Map_Adjacency_SetSecondProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince, "ToolStripMenuItem_Map_Adjacency_SetSecondProvince");
            this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_SetSecondProvince_Click);
            // 
            // ToolStripTextBox_Map_Adjacency_SecondProvinceId
            // 
            this.ToolStripTextBox_Map_Adjacency_SecondProvinceId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ToolStripTextBox_Map_Adjacency_SecondProvinceId.Name = "ToolStripTextBox_Map_Adjacency_SecondProvinceId";
            this.ToolStripTextBox_Map_Adjacency_SecondProvinceId.ReadOnly = true;
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_SecondProvinceId, "ToolStripTextBox_Map_Adjacency_SecondProvinceId");
            // 
            // ToolStripMenuItem_Map_Adjacency_SetSecondPoint
            // 
            this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint.Name = "ToolStripMenuItem_Map_Adjacency_SetSecondPoint";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint, "ToolStripMenuItem_Map_Adjacency_SetSecondPoint");
            this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_SetSecondPoint_Click);
            // 
            // ToolStripTextBox_Map_Adjacency_SecondPointPos
            // 
            this.ToolStripTextBox_Map_Adjacency_SecondPointPos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ToolStripTextBox_Map_Adjacency_SecondPointPos.Name = "ToolStripTextBox_Map_Adjacency_SecondPointPos";
            this.ToolStripTextBox_Map_Adjacency_SecondPointPos.ReadOnly = true;
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_SecondPointPos, "ToolStripTextBox_Map_Adjacency_SecondPointPos");
            // 
            // ToolStripMenuItem_Map_Adjacency_ResetSecondPoint
            // 
            this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint.Name = "ToolStripMenuItem_Map_Adjacency_ResetSecondPoint";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint, "ToolStripMenuItem_Map_Adjacency_ResetSecondPoint");
            this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_ResetSecondPoint_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // ToolStripMenuItem_Map_Adjacency_SetThroughProvince
            // 
            this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince.Name = "ToolStripMenuItem_Map_Adjacency_SetThroughProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince, "ToolStripMenuItem_Map_Adjacency_SetThroughProvince");
            this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_SetThroughProvince_Click);
            // 
            // ToolStripTextBox_Map_Adjacency_ThroughProvinceId
            // 
            this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId.Name = "ToolStripTextBox_Map_Adjacency_ThroughProvinceId";
            this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId.ReadOnly = true;
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_ThroughProvinceId, "ToolStripTextBox_Map_Adjacency_ThroughProvinceId");
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // ToolStripComboBox_Map_Adjacency_Rule
            // 
            this.ToolStripComboBox_Map_Adjacency_Rule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Adjacency_Rule.Name = "ToolStripComboBox_Map_Adjacency_Rule";
            resources.ApplyResources(this.ToolStripComboBox_Map_Adjacency_Rule, "ToolStripComboBox_Map_Adjacency_Rule");
            this.ToolStripComboBox_Map_Adjacency_Rule.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Adjacency_Rule_SelectedIndexChanged);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince
            // 
            this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince.Name = "ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince, "ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince");
            this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince_Click);
            // 
            // ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince
            // 
            this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince.Name = "ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince, "ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince");
            this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // ToolStripTextBox_Map_Adjacency_Comment
            // 
            this.ToolStripTextBox_Map_Adjacency_Comment.Name = "ToolStripTextBox_Map_Adjacency_Comment";
            resources.ApplyResources(this.ToolStripTextBox_Map_Adjacency_Comment, "ToolStripTextBox_Map_Adjacency_Comment");
            this.ToolStripTextBox_Map_Adjacency_Comment.TextChanged += new System.EventHandler(this.ToolStripTextBox_Map_Adjacency_Comment_TextChanged);
            // 
            // ToolStripMenuItem_Map_Railway
            // 
            this.ToolStripMenuItem_Map_Railway.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Map_Railway_Create,
            this.ToolStripComboBox_Map_Railway_Level,
            this.toolStripSeparator22,
            this.ToolStripMenuItem_Map_Railway_AddProvince,
            this.ToolStripMenuItem_Map_Railway_RemoveProvince,
            this.toolStripSeparator23,
            this.ToolStripMenuItem_Map_Railway_Join,
            this.ToolStripMenuItem_Map_Railway_Split,
            this.toolStripSeparator21,
            this.ToolStripMenuItem_Map_Railway_Remove});
            this.ToolStripMenuItem_Map_Railway.Name = "ToolStripMenuItem_Map_Railway";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway, "ToolStripMenuItem_Map_Railway");
            this.ToolStripMenuItem_Map_Railway.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_DropDownOpened);
            // 
            // ToolStripMenuItem_Map_Railway_Create
            // 
            this.ToolStripMenuItem_Map_Railway_Create.Name = "ToolStripMenuItem_Map_Railway_Create";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_Create, "ToolStripMenuItem_Map_Railway_Create");
            this.ToolStripMenuItem_Map_Railway_Create.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_Create_Click);
            // 
            // ToolStripComboBox_Map_Railway_Level
            // 
            this.ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource1"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource2"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource3"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.AutoCompleteCustomSource4")});
            this.ToolStripComboBox_Map_Railway_Level.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripComboBox_Map_Railway_Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Map_Railway_Level.Items.AddRange(new object[] {
            resources.GetString("ToolStripComboBox_Map_Railway_Level.Items"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.Items1"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.Items2"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.Items3"),
            resources.GetString("ToolStripComboBox_Map_Railway_Level.Items4")});
            this.ToolStripComboBox_Map_Railway_Level.Name = "ToolStripComboBox_Map_Railway_Level";
            resources.ApplyResources(this.ToolStripComboBox_Map_Railway_Level, "ToolStripComboBox_Map_Railway_Level");
            this.ToolStripComboBox_Map_Railway_Level.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Map_Railway_Level_SelectedIndexChanged);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // ToolStripMenuItem_Map_Railway_AddProvince
            // 
            this.ToolStripMenuItem_Map_Railway_AddProvince.Name = "ToolStripMenuItem_Map_Railway_AddProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_AddProvince, "ToolStripMenuItem_Map_Railway_AddProvince");
            this.ToolStripMenuItem_Map_Railway_AddProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_AddProvince_Click);
            // 
            // ToolStripMenuItem_Map_Railway_RemoveProvince
            // 
            this.ToolStripMenuItem_Map_Railway_RemoveProvince.Name = "ToolStripMenuItem_Map_Railway_RemoveProvince";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_RemoveProvince, "ToolStripMenuItem_Map_Railway_RemoveProvince");
            this.ToolStripMenuItem_Map_Railway_RemoveProvince.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_RemoveProvince_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // ToolStripMenuItem_Map_Railway_Join
            // 
            this.ToolStripMenuItem_Map_Railway_Join.Name = "ToolStripMenuItem_Map_Railway_Join";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_Join, "ToolStripMenuItem_Map_Railway_Join");
            this.ToolStripMenuItem_Map_Railway_Join.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_Join_Click);
            // 
            // ToolStripMenuItem_Map_Railway_Split
            // 
            this.ToolStripMenuItem_Map_Railway_Split.Name = "ToolStripMenuItem_Map_Railway_Split";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_Split, "ToolStripMenuItem_Map_Railway_Split");
            this.ToolStripMenuItem_Map_Railway_Split.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_Split_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            // 
            // ToolStripMenuItem_Map_Railway_Remove
            // 
            this.ToolStripMenuItem_Map_Railway_Remove.Name = "ToolStripMenuItem_Map_Railway_Remove";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Railway_Remove, "ToolStripMenuItem_Map_Railway_Remove");
            this.ToolStripMenuItem_Map_Railway_Remove.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Railway_Remove_Click);
            // 
            // ToolStripMenuItem_Map_SupplyHub
            // 
            this.ToolStripMenuItem_Map_SupplyHub.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Map_SupplyHub_Create,
            this.ToolStripMenuItem_Map_SupplyHub_Remove});
            this.ToolStripMenuItem_Map_SupplyHub.Name = "ToolStripMenuItem_Map_SupplyHub";
            resources.ApplyResources(this.ToolStripMenuItem_Map_SupplyHub, "ToolStripMenuItem_Map_SupplyHub");
            this.ToolStripMenuItem_Map_SupplyHub.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_SupplyHub_DropDownOpened);
            // 
            // ToolStripMenuItem_Map_SupplyHub_Create
            // 
            this.ToolStripMenuItem_Map_SupplyHub_Create.Name = "ToolStripMenuItem_Map_SupplyHub_Create";
            resources.ApplyResources(this.ToolStripMenuItem_Map_SupplyHub_Create, "ToolStripMenuItem_Map_SupplyHub_Create");
            this.ToolStripMenuItem_Map_SupplyHub_Create.Click += new System.EventHandler(this.ToolStripMenuItem_Map_SupplyHub_Create_Click);
            // 
            // ToolStripMenuItem_Map_SupplyHub_Remove
            // 
            this.ToolStripMenuItem_Map_SupplyHub_Remove.Name = "ToolStripMenuItem_Map_SupplyHub_Remove";
            resources.ApplyResources(this.ToolStripMenuItem_Map_SupplyHub_Remove, "ToolStripMenuItem_Map_SupplyHub_Remove");
            this.ToolStripMenuItem_Map_SupplyHub_Remove.Click += new System.EventHandler(this.ToolStripMenuItem_Map_SupplyHub_Remove_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // ToolStripMenuItem_Map_Actions
            // 
            this.ToolStripMenuItem_Map_Actions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Map_Actions_Merge,
            this.ToolStripMenuItem_Map_Actions_Merge_All});
            this.ToolStripMenuItem_Map_Actions.Name = "ToolStripMenuItem_Map_Actions";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Actions, "ToolStripMenuItem_Map_Actions");
            this.ToolStripMenuItem_Map_Actions.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Map_Actions_DropDownOpened);
            // 
            // ToolStripMenuItem_Map_Actions_Merge
            // 
            this.ToolStripMenuItem_Map_Actions_Merge.Name = "ToolStripMenuItem_Map_Actions_Merge";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Actions_Merge, "ToolStripMenuItem_Map_Actions_Merge");
            this.ToolStripMenuItem_Map_Actions_Merge.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Actions_Merge_Click);
            // 
            // ToolStripMenuItem_Map_Actions_Merge_All
            // 
            this.ToolStripMenuItem_Map_Actions_Merge_All.Name = "ToolStripMenuItem_Map_Actions_Merge_All";
            resources.ApplyResources(this.ToolStripMenuItem_Map_Actions_Merge_All, "ToolStripMenuItem_Map_Actions_Merge_All");
            this.ToolStripMenuItem_Map_Actions_Merge_All.Click += new System.EventHandler(this.ToolStripMenuItem_Map_Actions_Merge_All_Click);
            // 
            // Panel_ColorPicker
            // 
            this.Panel_ColorPicker.Controls.Add(this.Panel_ColorPicker_Button_Close);
            this.Panel_ColorPicker.Controls.Add(this.Panel_ColorPicker_Button_Save);
            this.Panel_ColorPicker.Controls.Add(this.ElementHost_ColorPicker);
            resources.ApplyResources(this.Panel_ColorPicker, "Panel_ColorPicker");
            this.Panel_ColorPicker.Name = "Panel_ColorPicker";
            // 
            // Panel_ColorPicker_Button_Close
            // 
            resources.ApplyResources(this.Panel_ColorPicker_Button_Close, "Panel_ColorPicker_Button_Close");
            this.Panel_ColorPicker_Button_Close.Name = "Panel_ColorPicker_Button_Close";
            this.Panel_ColorPicker_Button_Close.UseVisualStyleBackColor = true;
            this.Panel_ColorPicker_Button_Close.Click += new System.EventHandler(this.Panel_ColorPicker_Button_Close_Click);
            // 
            // Panel_ColorPicker_Button_Save
            // 
            resources.ApplyResources(this.Panel_ColorPicker_Button_Save, "Panel_ColorPicker_Button_Save");
            this.Panel_ColorPicker_Button_Save.Name = "Panel_ColorPicker_Button_Save";
            this.Panel_ColorPicker_Button_Save.UseVisualStyleBackColor = true;
            this.Panel_ColorPicker_Button_Save.Click += new System.EventHandler(this.Panel_ColorPicker_Button_Save_Click);
            // 
            // ElementHost_ColorPicker
            // 
            resources.ApplyResources(this.ElementHost_ColorPicker, "ElementHost_ColorPicker");
            this.ElementHost_ColorPicker.Name = "ElementHost_ColorPicker";
            this.ElementHost_ColorPicker.Child = this.standardColorPicker1;
            // 
            // TabPage_Buildings
            // 
            resources.ApplyResources(this.TabPage_Buildings, "TabPage_Buildings");
            this.TabPage_Buildings.Name = "TabPage_Buildings";
            this.TabPage_Buildings.UseVisualStyleBackColor = true;
            // 
            // TabPage_Resources
            // 
            this.TabPage_Resources.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.TabPage_Resources, "TabPage_Resources");
            this.TabPage_Resources.Name = "TabPage_Resources";
            this.TabPage_Resources.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Gainsboro;
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // TabPage_Localization
            // 
            resources.ApplyResources(this.TabPage_Localization, "TabPage_Localization");
            this.TabPage_Localization.Name = "TabPage_Localization";
            this.TabPage_Localization.UseVisualStyleBackColor = true;
            // 
            // TabPage_Home
            // 
            resources.ApplyResources(this.TabPage_Home, "TabPage_Home");
            this.TabPage_Home.Name = "TabPage_Home";
            this.TabPage_Home.UseVisualStyleBackColor = true;
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_File,
            this.ToolStripMenuItem_Edit,
            this.ToolStripMenuItem_Data,
            this.ToolStripMenuItem_Help,
            this.ToolStripMenuItem_Language,
            this.ToolStripMenuItem_GitHub,
            this.ToolStripMenuItem_Discord,
            this.ToolStripMenuItem_Telegram});
            resources.ApplyResources(this.MenuStrip1, "MenuStrip1");
            this.MenuStrip1.Name = "MenuStrip1";
            // 
            // ToolStripMenuItem_File
            // 
            this.ToolStripMenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Save,
            this.ToolStripMenuItem_SaveAll,
            this.ToolStripMenuItem_Load,
            this.ToolStripMenuItem_LoadAll,
            this.ToolStripMenuItem_Update,
            this.ToolStripMenuItem_UpdateAll,
            this.ToolStripMenuItem_Export,
            this.toolStripSeparator2,
            this.ToolStripMenuItem_Settings,
            this.toolStripSeparator4,
            this.ToolStripMenuItem_Exit});
            this.ToolStripMenuItem_File.Name = "ToolStripMenuItem_File";
            resources.ApplyResources(this.ToolStripMenuItem_File, "ToolStripMenuItem_File");
            // 
            // ToolStripMenuItem_Save
            // 
            this.ToolStripMenuItem_Save.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Save_Maps,
            this.ToolStripMenuItem_Save_Definition,
            this.ToolStripMenuItem_Save_Adjacencies,
            this.ToolStripMenuItem_Save_Supply,
            this.toolStripSeparator9,
            this.ToolStripMenuItem_Save_States,
            this.ToolStripMenuItem_Save_Regions});
            this.ToolStripMenuItem_Save.Name = "ToolStripMenuItem_Save";
            resources.ApplyResources(this.ToolStripMenuItem_Save, "ToolStripMenuItem_Save");
            // 
            // ToolStripMenuItem_Save_Maps
            // 
            this.ToolStripMenuItem_Save_Maps.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Save_Maps_All,
            this.toolStripSeparator1,
            this.ToolStripMenuItem_Save_Maps_Provinces,
            this.ToolStripMenuItem_Save_Maps_Rivers,
            this.ToolStripMenuItem_Save_Maps_Terrain,
            this.ToolStripMenuItem_Save_Maps_Trees,
            this.ToolStripMenuItem_Save_Maps_Cities,
            this.ToolStripMenuItem_Save_Maps_Heights});
            this.ToolStripMenuItem_Save_Maps.Name = "ToolStripMenuItem_Save_Maps";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps, "ToolStripMenuItem_Save_Maps");
            // 
            // ToolStripMenuItem_Save_Maps_All
            // 
            this.ToolStripMenuItem_Save_Maps_All.Name = "ToolStripMenuItem_Save_Maps_All";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_All, "ToolStripMenuItem_Save_Maps_All");
            this.ToolStripMenuItem_Save_Maps_All.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_All_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // ToolStripMenuItem_Save_Maps_Provinces
            // 
            this.ToolStripMenuItem_Save_Maps_Provinces.Name = "ToolStripMenuItem_Save_Maps_Provinces";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Provinces, "ToolStripMenuItem_Save_Maps_Provinces");
            this.ToolStripMenuItem_Save_Maps_Provinces.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Provinces_Click);
            // 
            // ToolStripMenuItem_Save_Maps_Rivers
            // 
            this.ToolStripMenuItem_Save_Maps_Rivers.Name = "ToolStripMenuItem_Save_Maps_Rivers";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Rivers, "ToolStripMenuItem_Save_Maps_Rivers");
            this.ToolStripMenuItem_Save_Maps_Rivers.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Rivers_Click);
            // 
            // ToolStripMenuItem_Save_Maps_Terrain
            // 
            this.ToolStripMenuItem_Save_Maps_Terrain.Name = "ToolStripMenuItem_Save_Maps_Terrain";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Terrain, "ToolStripMenuItem_Save_Maps_Terrain");
            this.ToolStripMenuItem_Save_Maps_Terrain.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Terrain_Click);
            // 
            // ToolStripMenuItem_Save_Maps_Trees
            // 
            this.ToolStripMenuItem_Save_Maps_Trees.Name = "ToolStripMenuItem_Save_Maps_Trees";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Trees, "ToolStripMenuItem_Save_Maps_Trees");
            this.ToolStripMenuItem_Save_Maps_Trees.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Trees_Click);
            // 
            // ToolStripMenuItem_Save_Maps_Cities
            // 
            this.ToolStripMenuItem_Save_Maps_Cities.Name = "ToolStripMenuItem_Save_Maps_Cities";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Cities, "ToolStripMenuItem_Save_Maps_Cities");
            this.ToolStripMenuItem_Save_Maps_Cities.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Cities_Click);
            // 
            // ToolStripMenuItem_Save_Maps_Heights
            // 
            this.ToolStripMenuItem_Save_Maps_Heights.Name = "ToolStripMenuItem_Save_Maps_Heights";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Maps_Heights, "ToolStripMenuItem_Save_Maps_Heights");
            this.ToolStripMenuItem_Save_Maps_Heights.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Maps_Heights_Click);
            // 
            // ToolStripMenuItem_Save_Definition
            // 
            this.ToolStripMenuItem_Save_Definition.Name = "ToolStripMenuItem_Save_Definition";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Definition, "ToolStripMenuItem_Save_Definition");
            this.ToolStripMenuItem_Save_Definition.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Definition_Click);
            // 
            // ToolStripMenuItem_Save_Adjacencies
            // 
            resources.ApplyResources(this.ToolStripMenuItem_Save_Adjacencies, "ToolStripMenuItem_Save_Adjacencies");
            this.ToolStripMenuItem_Save_Adjacencies.Name = "ToolStripMenuItem_Save_Adjacencies";
            this.ToolStripMenuItem_Save_Adjacencies.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Adjacencies_Click);
            // 
            // ToolStripMenuItem_Save_Supply
            // 
            this.ToolStripMenuItem_Save_Supply.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Save_Supply_All,
            this.toolStripSeparator3,
            this.ToolStripMenuItem_Save_Supply_Railways,
            this.ToolStripMenuItem_Save_Supply_Hubs});
            this.ToolStripMenuItem_Save_Supply.Name = "ToolStripMenuItem_Save_Supply";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Supply, "ToolStripMenuItem_Save_Supply");
            // 
            // ToolStripMenuItem_Save_Supply_All
            // 
            this.ToolStripMenuItem_Save_Supply_All.Name = "ToolStripMenuItem_Save_Supply_All";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Supply_All, "ToolStripMenuItem_Save_Supply_All");
            this.ToolStripMenuItem_Save_Supply_All.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Supply_All_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // ToolStripMenuItem_Save_Supply_Railways
            // 
            this.ToolStripMenuItem_Save_Supply_Railways.Name = "ToolStripMenuItem_Save_Supply_Railways";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Supply_Railways, "ToolStripMenuItem_Save_Supply_Railways");
            this.ToolStripMenuItem_Save_Supply_Railways.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Supply_Railways_Click);
            // 
            // ToolStripMenuItem_Save_Supply_Hubs
            // 
            this.ToolStripMenuItem_Save_Supply_Hubs.Name = "ToolStripMenuItem_Save_Supply_Hubs";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Supply_Hubs, "ToolStripMenuItem_Save_Supply_Hubs");
            this.ToolStripMenuItem_Save_Supply_Hubs.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Supply_Hubs_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // ToolStripMenuItem_Save_States
            // 
            this.ToolStripMenuItem_Save_States.Name = "ToolStripMenuItem_Save_States";
            resources.ApplyResources(this.ToolStripMenuItem_Save_States, "ToolStripMenuItem_Save_States");
            this.ToolStripMenuItem_Save_States.Click += new System.EventHandler(this.ToolStripMenuItem_Save_States_Click);
            // 
            // ToolStripMenuItem_Save_Regions
            // 
            this.ToolStripMenuItem_Save_Regions.Name = "ToolStripMenuItem_Save_Regions";
            resources.ApplyResources(this.ToolStripMenuItem_Save_Regions, "ToolStripMenuItem_Save_Regions");
            this.ToolStripMenuItem_Save_Regions.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Regions_Click);
            // 
            // ToolStripMenuItem_SaveAll
            // 
            this.ToolStripMenuItem_SaveAll.Name = "ToolStripMenuItem_SaveAll";
            resources.ApplyResources(this.ToolStripMenuItem_SaveAll, "ToolStripMenuItem_SaveAll");
            this.ToolStripMenuItem_SaveAll.Click += new System.EventHandler(this.ToolStripMenuItem_SaveAll_Click);
            // 
            // ToolStripMenuItem_Load
            // 
            this.ToolStripMenuItem_Load.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Load_Maps,
            this.ToolStripMenuItem_Load_Definitions,
            this.ToolStripMenuItem_Load_Adjacencies,
            this.ToolStripMenuItem_Load_Supply});
            resources.ApplyResources(this.ToolStripMenuItem_Load, "ToolStripMenuItem_Load");
            this.ToolStripMenuItem_Load.Name = "ToolStripMenuItem_Load";
            // 
            // ToolStripMenuItem_Load_Maps
            // 
            this.ToolStripMenuItem_Load_Maps.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Load_Maps_All,
            this.toolStripSeparator5,
            this.ToolStripMenuItem_Load_Maps_Provinces,
            this.ToolStripMenuItem_Load_Maps_Rivers,
            this.ToolStripMenuItem_Load_Maps_Terrain,
            this.ToolStripMenuItem_Load_Maps_Trees,
            this.ToolStripMenuItem_Load_Maps_Cities,
            this.ToolStripMenuItem_Load_Maps_Heights});
            this.ToolStripMenuItem_Load_Maps.Name = "ToolStripMenuItem_Load_Maps";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps, "ToolStripMenuItem_Load_Maps");
            // 
            // ToolStripMenuItem_Load_Maps_All
            // 
            this.ToolStripMenuItem_Load_Maps_All.Name = "ToolStripMenuItem_Load_Maps_All";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_All, "ToolStripMenuItem_Load_Maps_All");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // ToolStripMenuItem_Load_Maps_Provinces
            // 
            this.ToolStripMenuItem_Load_Maps_Provinces.Name = "ToolStripMenuItem_Load_Maps_Provinces";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Provinces, "ToolStripMenuItem_Load_Maps_Provinces");
            // 
            // ToolStripMenuItem_Load_Maps_Rivers
            // 
            this.ToolStripMenuItem_Load_Maps_Rivers.Name = "ToolStripMenuItem_Load_Maps_Rivers";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Rivers, "ToolStripMenuItem_Load_Maps_Rivers");
            // 
            // ToolStripMenuItem_Load_Maps_Terrain
            // 
            this.ToolStripMenuItem_Load_Maps_Terrain.Name = "ToolStripMenuItem_Load_Maps_Terrain";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Terrain, "ToolStripMenuItem_Load_Maps_Terrain");
            // 
            // ToolStripMenuItem_Load_Maps_Trees
            // 
            this.ToolStripMenuItem_Load_Maps_Trees.Name = "ToolStripMenuItem_Load_Maps_Trees";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Trees, "ToolStripMenuItem_Load_Maps_Trees");
            // 
            // ToolStripMenuItem_Load_Maps_Cities
            // 
            this.ToolStripMenuItem_Load_Maps_Cities.Name = "ToolStripMenuItem_Load_Maps_Cities";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Cities, "ToolStripMenuItem_Load_Maps_Cities");
            // 
            // ToolStripMenuItem_Load_Maps_Heights
            // 
            this.ToolStripMenuItem_Load_Maps_Heights.Name = "ToolStripMenuItem_Load_Maps_Heights";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Maps_Heights, "ToolStripMenuItem_Load_Maps_Heights");
            // 
            // ToolStripMenuItem_Load_Definitions
            // 
            this.ToolStripMenuItem_Load_Definitions.Name = "ToolStripMenuItem_Load_Definitions";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Definitions, "ToolStripMenuItem_Load_Definitions");
            // 
            // ToolStripMenuItem_Load_Adjacencies
            // 
            this.ToolStripMenuItem_Load_Adjacencies.Name = "ToolStripMenuItem_Load_Adjacencies";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Adjacencies, "ToolStripMenuItem_Load_Adjacencies");
            // 
            // ToolStripMenuItem_Load_Supply
            // 
            this.ToolStripMenuItem_Load_Supply.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Load_Supply_All,
            this.toolStripSeparator6,
            this.ToolStripMenuItem_Load_Supply_Railways,
            this.ToolStripMenuItem_Load_Supply_Hubs});
            this.ToolStripMenuItem_Load_Supply.Name = "ToolStripMenuItem_Load_Supply";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Supply, "ToolStripMenuItem_Load_Supply");
            // 
            // ToolStripMenuItem_Load_Supply_All
            // 
            this.ToolStripMenuItem_Load_Supply_All.Name = "ToolStripMenuItem_Load_Supply_All";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Supply_All, "ToolStripMenuItem_Load_Supply_All");
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // ToolStripMenuItem_Load_Supply_Railways
            // 
            this.ToolStripMenuItem_Load_Supply_Railways.Name = "ToolStripMenuItem_Load_Supply_Railways";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Supply_Railways, "ToolStripMenuItem_Load_Supply_Railways");
            // 
            // ToolStripMenuItem_Load_Supply_Hubs
            // 
            this.ToolStripMenuItem_Load_Supply_Hubs.Name = "ToolStripMenuItem_Load_Supply_Hubs";
            resources.ApplyResources(this.ToolStripMenuItem_Load_Supply_Hubs, "ToolStripMenuItem_Load_Supply_Hubs");
            // 
            // ToolStripMenuItem_LoadAll
            // 
            this.ToolStripMenuItem_LoadAll.Name = "ToolStripMenuItem_LoadAll";
            resources.ApplyResources(this.ToolStripMenuItem_LoadAll, "ToolStripMenuItem_LoadAll");
            this.ToolStripMenuItem_LoadAll.Click += new System.EventHandler(this.ToolStripMenuItem_LoadAll_Click);
            // 
            // ToolStripMenuItem_Update
            // 
            resources.ApplyResources(this.ToolStripMenuItem_Update, "ToolStripMenuItem_Update");
            this.ToolStripMenuItem_Update.Name = "ToolStripMenuItem_Update";
            // 
            // ToolStripMenuItem_UpdateAll
            // 
            this.ToolStripMenuItem_UpdateAll.Name = "ToolStripMenuItem_UpdateAll";
            resources.ApplyResources(this.ToolStripMenuItem_UpdateAll, "ToolStripMenuItem_UpdateAll");
            this.ToolStripMenuItem_UpdateAll.Click += new System.EventHandler(this.ToolStripMenuItem_UpdateAll_Click);
            // 
            // ToolStripMenuItem_Export
            // 
            this.ToolStripMenuItem_Export.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Export_MainLayer,
            this.ToolStripMenuItem_Export_SelectedBorders});
            this.ToolStripMenuItem_Export.Name = "ToolStripMenuItem_Export";
            resources.ApplyResources(this.ToolStripMenuItem_Export, "ToolStripMenuItem_Export");
            // 
            // ToolStripMenuItem_Export_MainLayer
            // 
            this.ToolStripMenuItem_Export_MainLayer.Name = "ToolStripMenuItem_Export_MainLayer";
            resources.ApplyResources(this.ToolStripMenuItem_Export_MainLayer, "ToolStripMenuItem_Export_MainLayer");
            this.ToolStripMenuItem_Export_MainLayer.Click += new System.EventHandler(this.ToolStripMenuItem_Export_MainLayer_Click);
            // 
            // ToolStripMenuItem_Export_SelectedBorders
            // 
            this.ToolStripMenuItem_Export_SelectedBorders.Name = "ToolStripMenuItem_Export_SelectedBorders";
            resources.ApplyResources(this.ToolStripMenuItem_Export_SelectedBorders, "ToolStripMenuItem_Export_SelectedBorders");
            this.ToolStripMenuItem_Export_SelectedBorders.Click += new System.EventHandler(this.ToolStripMenuItem_Export_SelectedBorders_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // ToolStripMenuItem_Settings
            // 
            this.ToolStripMenuItem_Settings.Name = "ToolStripMenuItem_Settings";
            resources.ApplyResources(this.ToolStripMenuItem_Settings, "ToolStripMenuItem_Settings");
            this.ToolStripMenuItem_Settings.Click += new System.EventHandler(this.ToolStripMenuItem_Settings_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // ToolStripMenuItem_Exit
            // 
            this.ToolStripMenuItem_Exit.Name = "ToolStripMenuItem_Exit";
            resources.ApplyResources(this.ToolStripMenuItem_Exit, "ToolStripMenuItem_Exit");
            this.ToolStripMenuItem_Exit.Click += new System.EventHandler(this.ToolStripMenuItem_Exit_Click);
            // 
            // ToolStripMenuItem_Edit
            // 
            this.ToolStripMenuItem_Edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Edit_Undo,
            this.ToolStripMenuItem_Edit_Redo,
            this.toolStripSeparator8,
            this.ToolStripMenuItem_Edit_Actions,
            this.ToolStripMenuItem_Edit_AutoTools,
            this.ToolStripMenuItem_Edit_Scripts});
            this.ToolStripMenuItem_Edit.Name = "ToolStripMenuItem_Edit";
            resources.ApplyResources(this.ToolStripMenuItem_Edit, "ToolStripMenuItem_Edit");
            // 
            // ToolStripMenuItem_Edit_Undo
            // 
            this.ToolStripMenuItem_Edit_Undo.Name = "ToolStripMenuItem_Edit_Undo";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Undo, "ToolStripMenuItem_Edit_Undo");
            this.ToolStripMenuItem_Edit_Undo.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Undo_Click);
            // 
            // ToolStripMenuItem_Edit_Redo
            // 
            this.ToolStripMenuItem_Edit_Redo.Name = "ToolStripMenuItem_Edit_Redo";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Redo, "ToolStripMenuItem_Edit_Redo");
            this.ToolStripMenuItem_Edit_Redo.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Redo_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // ToolStripMenuItem_Edit_Actions
            // 
            this.ToolStripMenuItem_Edit_Actions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Edit_Actions_CreateObject,
            this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces,
            this.ToolStripMenuItem_Edit_Actions_FindMapChanges});
            this.ToolStripMenuItem_Edit_Actions.Name = "ToolStripMenuItem_Edit_Actions";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Actions, "ToolStripMenuItem_Edit_Actions");
            this.ToolStripMenuItem_Edit_Actions.DropDownOpened += new System.EventHandler(this.ToolStripMenuItem_Edit_Actions_DropDownOpened);
            // 
            // ToolStripMenuItem_Edit_Actions_CreateObject
            // 
            this.ToolStripMenuItem_Edit_Actions_CreateObject.Name = "ToolStripMenuItem_Edit_Actions_CreateObject";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Actions_CreateObject, "ToolStripMenuItem_Edit_Actions_CreateObject");
            this.ToolStripMenuItem_Edit_Actions_CreateObject.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Actions_CreateObject_Click);
            // 
            // ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces
            // 
            this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces.Name = "ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces, "ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces");
            this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces_Click);
            // 
            // ToolStripMenuItem_Edit_Actions_FindMapChanges
            // 
            this.ToolStripMenuItem_Edit_Actions_FindMapChanges.Name = "ToolStripMenuItem_Edit_Actions_FindMapChanges";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Actions_FindMapChanges, "ToolStripMenuItem_Edit_Actions_FindMapChanges");
            this.ToolStripMenuItem_Edit_Actions_FindMapChanges.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Actions_FindMapChanges_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools
            // 
            this.ToolStripMenuItem_Edit_AutoTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces,
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal,
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents,
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates,
            this.toolStripSeparator11,
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors,
            this.toolStripSeparator26,
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesValidation,
            this.ToolStripMenuItem_Edit_AutoTools_StatesValidation,
            this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation});
            this.ToolStripMenuItem_Edit_AutoTools.Name = "ToolStripMenuItem_Edit_AutoTools";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools, "ToolStripMenuItem_Edit_AutoTools");
            // 
            // ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces.Name = "ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces, "ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces");
            this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal
            // 
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal.Name = "ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal, "ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal");
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents.Name = "ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents, "ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents");
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates.Name = "ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates, "ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates");
            this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random,
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors,
            this.toolStripSeparator30,
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings});
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors.Name = "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors, "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors");
            // 
            // ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random.Name = "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random, "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random");
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors.Name = "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors, "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors");
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors_Click);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            resources.ApplyResources(this.toolStripSeparator30, "toolStripSeparator30");
            // 
            // ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings.Name = "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings, "ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings");
            this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(this.toolStripSeparator26, "toolStripSeparator26");
            // 
            // ToolStripMenuItem_Edit_AutoTools_ProvincesValidation
            // 
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_ProvincesValidation, "ToolStripMenuItem_Edit_AutoTools_ProvincesValidation");
            this.ToolStripMenuItem_Edit_AutoTools_ProvincesValidation.Name = "ToolStripMenuItem_Edit_AutoTools_ProvincesValidation";
            // 
            // ToolStripMenuItem_Edit_AutoTools_StatesValidation
            // 
            this.ToolStripMenuItem_Edit_AutoTools_StatesValidation.Name = "ToolStripMenuItem_Edit_AutoTools_StatesValidation";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_StatesValidation, "ToolStripMenuItem_Edit_AutoTools_StatesValidation");
            this.ToolStripMenuItem_Edit_AutoTools_StatesValidation.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_StatesValidation_Click);
            // 
            // ToolStripMenuItem_Edit_AutoTools_RegionsValidation
            // 
            this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation.Name = "ToolStripMenuItem_Edit_AutoTools_RegionsValidation";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation, "ToolStripMenuItem_Edit_AutoTools_RegionsValidation");
            this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_AutoTools_RegionsValidation_Click);
            // 
            // ToolStripMenuItem_Edit_Scripts
            // 
            this.ToolStripMenuItem_Edit_Scripts.Name = "ToolStripMenuItem_Edit_Scripts";
            resources.ApplyResources(this.ToolStripMenuItem_Edit_Scripts, "ToolStripMenuItem_Edit_Scripts");
            this.ToolStripMenuItem_Edit_Scripts.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Scripts_Click);
            // 
            // ToolStripMenuItem_Data
            // 
            this.ToolStripMenuItem_Data.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripComboBox_Data_Bookmark,
            this.toolStripSeparator20,
            this.ToolStripMenuItem_Data_Provinces,
            this.ToolStripMenuItem_Data_States,
            this.toolStripSeparator19,
            this.ToolStripMenuItem_Statistics,
            this.toolStripSeparator31,
            this.ToolStripMenuItem_Data_Recovery});
            this.ToolStripMenuItem_Data.Name = "ToolStripMenuItem_Data";
            resources.ApplyResources(this.ToolStripMenuItem_Data, "ToolStripMenuItem_Data");
            // 
            // ToolStripComboBox_Data_Bookmark
            // 
            this.ToolStripComboBox_Data_Bookmark.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripComboBox_Data_Bookmark.Name = "ToolStripComboBox_Data_Bookmark";
            resources.ApplyResources(this.ToolStripComboBox_Data_Bookmark, "ToolStripComboBox_Data_Bookmark");
            this.ToolStripComboBox_Data_Bookmark.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox_Data_Bookmark_SelectedIndexChanged);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // ToolStripMenuItem_Data_Provinces
            // 
            this.ToolStripMenuItem_Data_Provinces.Name = "ToolStripMenuItem_Data_Provinces";
            resources.ApplyResources(this.ToolStripMenuItem_Data_Provinces, "ToolStripMenuItem_Data_Provinces");
            this.ToolStripMenuItem_Data_Provinces.Click += new System.EventHandler(this.ToolStripMenuItem_Data_Provinces_Click);
            // 
            // ToolStripMenuItem_Data_States
            // 
            this.ToolStripMenuItem_Data_States.Name = "ToolStripMenuItem_Data_States";
            resources.ApplyResources(this.ToolStripMenuItem_Data_States, "ToolStripMenuItem_Data_States");
            this.ToolStripMenuItem_Data_States.Click += new System.EventHandler(this.ToolStripMenuItem_Data_States_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // ToolStripMenuItem_Statistics
            // 
            this.ToolStripMenuItem_Statistics.Name = "ToolStripMenuItem_Statistics";
            resources.ApplyResources(this.ToolStripMenuItem_Statistics, "ToolStripMenuItem_Statistics");
            this.ToolStripMenuItem_Statistics.Click += new System.EventHandler(this.ToolStripMenuItem_Statistics_Click);
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            resources.ApplyResources(this.toolStripSeparator31, "toolStripSeparator31");
            // 
            // ToolStripMenuItem_Data_Recovery
            // 
            this.ToolStripMenuItem_Data_Recovery.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Data_Recovery_Regions});
            this.ToolStripMenuItem_Data_Recovery.Name = "ToolStripMenuItem_Data_Recovery";
            resources.ApplyResources(this.ToolStripMenuItem_Data_Recovery, "ToolStripMenuItem_Data_Recovery");
            // 
            // ToolStripMenuItem_Data_Recovery_Regions
            // 
            this.ToolStripMenuItem_Data_Recovery_Regions.Name = "ToolStripMenuItem_Data_Recovery_Regions";
            resources.ApplyResources(this.ToolStripMenuItem_Data_Recovery_Regions, "ToolStripMenuItem_Data_Recovery_Regions");
            this.ToolStripMenuItem_Data_Recovery_Regions.Click += new System.EventHandler(this.ToolStripMenuItem_Data_Recovery_Regions_Click);
            // 
            // ToolStripMenuItem_Help
            // 
            this.ToolStripMenuItem_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Help_Documentation,
            this.ToolStripMenuItem_Help_About});
            this.ToolStripMenuItem_Help.Name = "ToolStripMenuItem_Help";
            resources.ApplyResources(this.ToolStripMenuItem_Help, "ToolStripMenuItem_Help");
            // 
            // ToolStripMenuItem_Help_Documentation
            // 
            this.ToolStripMenuItem_Help_Documentation.Name = "ToolStripMenuItem_Help_Documentation";
            resources.ApplyResources(this.ToolStripMenuItem_Help_Documentation, "ToolStripMenuItem_Help_Documentation");
            this.ToolStripMenuItem_Help_Documentation.Click += new System.EventHandler(this.ToolStripMenuItem_Help_Documentation_Click);
            // 
            // ToolStripMenuItem_Help_About
            // 
            this.ToolStripMenuItem_Help_About.Name = "ToolStripMenuItem_Help_About";
            resources.ApplyResources(this.ToolStripMenuItem_Help_About, "ToolStripMenuItem_Help_About");
            this.ToolStripMenuItem_Help_About.Click += new System.EventHandler(this.ToolStripMenuItem_Help_About_Click);
            // 
            // ToolStripMenuItem_Language
            // 
            this.ToolStripMenuItem_Language.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Language_RU,
            this.ToolStripMenuItem_Language_EN});
            this.ToolStripMenuItem_Language.Image = global::HOI4ModBuilder.Properties.Resources.flag_ru;
            this.ToolStripMenuItem_Language.Name = "ToolStripMenuItem_Language";
            resources.ApplyResources(this.ToolStripMenuItem_Language, "ToolStripMenuItem_Language");
            // 
            // ToolStripMenuItem_Language_RU
            // 
            this.ToolStripMenuItem_Language_RU.Image = global::HOI4ModBuilder.Properties.Resources.flag_ru;
            this.ToolStripMenuItem_Language_RU.Name = "ToolStripMenuItem_Language_RU";
            resources.ApplyResources(this.ToolStripMenuItem_Language_RU, "ToolStripMenuItem_Language_RU");
            this.ToolStripMenuItem_Language_RU.Click += new System.EventHandler(this.ToolStripMenuItem_Language_RU_Click);
            // 
            // ToolStripMenuItem_Language_EN
            // 
            this.ToolStripMenuItem_Language_EN.Image = global::HOI4ModBuilder.Properties.Resources.flag_en;
            this.ToolStripMenuItem_Language_EN.Name = "ToolStripMenuItem_Language_EN";
            resources.ApplyResources(this.ToolStripMenuItem_Language_EN, "ToolStripMenuItem_Language_EN");
            this.ToolStripMenuItem_Language_EN.Click += new System.EventHandler(this.ToolStripMenuItem_Language_EN_Click);
            // 
            // ToolStripMenuItem_GitHub
            // 
            this.ToolStripMenuItem_GitHub.Image = global::HOI4ModBuilder.Properties.Resources.icon_github;
            this.ToolStripMenuItem_GitHub.Name = "ToolStripMenuItem_GitHub";
            resources.ApplyResources(this.ToolStripMenuItem_GitHub, "ToolStripMenuItem_GitHub");
            this.ToolStripMenuItem_GitHub.Click += new System.EventHandler(this.ToolStripMenuItem_GitHub_Click);
            // 
            // ToolStripMenuItem_Discord
            // 
            this.ToolStripMenuItem_Discord.Image = global::HOI4ModBuilder.Properties.Resources.icon_discord;
            this.ToolStripMenuItem_Discord.Name = "ToolStripMenuItem_Discord";
            resources.ApplyResources(this.ToolStripMenuItem_Discord, "ToolStripMenuItem_Discord");
            this.ToolStripMenuItem_Discord.Click += new System.EventHandler(this.ToolStripMenuItem_Discord_Click);
            // 
            // ToolStripMenuItem_Telegram
            // 
            this.ToolStripMenuItem_Telegram.Image = global::HOI4ModBuilder.Properties.Resources.icon_telegram;
            this.ToolStripMenuItem_Telegram.Name = "ToolStripMenuItem_Telegram";
            resources.ApplyResources(this.ToolStripMenuItem_Telegram, "ToolStripMenuItem_Telegram");
            this.ToolStripMenuItem_Telegram.Click += new System.EventHandler(this.ToolStripMenuItem_Telegram_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TabControl_Main);
            this.Controls.Add(this.MenuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "MainForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.TabControl_Main.ResumeLayout(false);
            this.TabPage_Map.ResumeLayout(false);
            this.TabPage_Map.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.FlowLayoutPanel_ToolBar.ResumeLayout(false);
            this.FlowLayoutPanel_ToolBar.PerformLayout();
            this.GroupBox_Main_Layer.ResumeLayout(false);
            this.GroupBox_Main_Layer_Parameter.ResumeLayout(false);
            this.GroupBox_Edit_Layer.ResumeLayout(false);
            this.GroupBox_Tool.ResumeLayout(false);
            this.GroupBox_Tool_Parameter.ResumeLayout(false);
            this.GroupBox_Tool_Parameter_Value.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.GroupBox_GenerateColor.ResumeLayout(false);
            this.GroupBox_Palette.ResumeLayout(false);
            this.GroupBox_Palette.PerformLayout();
            this.GroupBox_Progress.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.Panel_Map.ResumeLayout(false);
            this.ContextMenuStrip_Map.ResumeLayout(false);
            this.Panel_ColorPicker.ResumeLayout(false);
            this.TabPage_Resources.ResumeLayout(false);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabPage TabPage_Resources;
        private System.Windows.Forms.Panel Panel_Map;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckedListBox CheckedListBox_MapAdditionalLayers;
        private System.Windows.Forms.ComboBox СomboBox_MapMainLayer;
        private System.Windows.Forms.TabPage TabPage_Localization;
        private System.Windows.Forms.Button Button_TextureRemove;
        private System.Windows.Forms.Button Button_TextureAdd;
        private System.Windows.Forms.ListBox ListBox_Textures;
        private System.Windows.Forms.ComboBox ComboBox_Tool;
        private System.Windows.Forms.GroupBox GroupBox_Tool;
        private System.Windows.Forms.GroupBox GroupBox_Main_Layer;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button Button_TexturesLoad;
        private System.Windows.Forms.Button Button_TexturesSave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.FlowLayoutPanel FlowLayoutPanel_ToolBar;
        private System.Windows.Forms.GroupBox GroupBox_Edit_Layer;
        private System.Windows.Forms.ComboBox ComboBox_EditLayer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox GroupBox_Palette;
        public System.Windows.Forms.FlowLayoutPanel FlowLayoutPanel_Color;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBox_SelectedObjectId;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox textBox_PixelPos;
        private System.Windows.Forms.GroupBox groupBox11;
        public System.Windows.Forms.TextBox textBox_HOI4PixelPos;
        private System.Windows.Forms.MenuStrip MenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Help;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SaveAll;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_LoadAll;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Help_Documentation;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Help_About;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_All;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Provinces;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Terrain;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Trees;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Cities;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Heights;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Definition;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Adjacencies;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Supply;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Exit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Maps_Rivers;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Supply_All;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Supply_Railways;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Supply_Hubs;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_All;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Provinces;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Rivers;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Terrain;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Trees;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Cities;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Maps_Heights;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Settings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Definitions;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Adjacencies;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Supply;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Supply_All;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Supply_Railways;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Load_Supply_Hubs;
        private System.Windows.Forms.TabPage TabPage_Home;
        private System.Windows.Forms.GroupBox GroupBox_Progress;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Map;
        private System.Windows.Forms.GroupBox GroupBox_Tool_Parameter;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Data;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Data_Provinces;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_Create;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Railway_Level;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_Remove;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_SupplyHub;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_SupplyHub_Create;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_SupplyHub_Remove;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Search;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Search_Input;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Search_Province;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Undo;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Redo;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Search_Position;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Search_State;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Search_Region;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_ProvincesIsCoastal;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_ProvincesValidation;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_StatesValidation;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RegionsValidation;
        private System.Windows.Forms.TabPage TabPage_Buildings;
        public System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Data_Bookmark;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_States;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save_Regions;
        public System.Windows.Forms.ComboBox ComboBox_Tool_Parameter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox ComboBox_BordersType;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Data_States;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Update;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_UpdateAll;
        private System.Windows.Forms.GroupBox GroupBox_GenerateColor;
        private System.Windows.Forms.Button Button_GenerateColor;
        private System.Windows.Forms.ComboBox ComboBox_GenerateColor_Type;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Actions;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Actions_Merge;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RemoveSeaAndLakesContinents;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_SetFirstProvince;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_FirstProvinceId;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_SetFirstPoint;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_FirstPointPos;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_SetSecondProvince;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_SecondProvinceId;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_SetSecondPoint;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_SecondPointPos;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_SetThroughProvince;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_ThroughProvinceId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Adjacency_Rule;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Adjacency_Comment;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_Create;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Adjacency_Type;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_Remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_ResetFirstPoint;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_ResetSecondPoint;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button Button_OpenSearchErrorsSettings;
        public System.Windows.Forms.TabControl TabControl_Main;
        public System.Windows.Forms.TabPage TabPage_Map;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_Rule_AddRequiredProvince;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Adjacency_Rule_RemoveRequiredProvince;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language_RU;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language_EN;
        private System.Windows.Forms.Button Button_TextureMoveDown;
        private System.Windows.Forms.Button Button_TextureMoveUp;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Discord;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_Split;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_Join;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Data_Recovery;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Data_Recovery_Regions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_AddProvince;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Railway_RemoveProvince;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Province;
        public System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Province_Type;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Province_IsCoastal;
        public System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Province_Terrain;
        public System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Province_Continent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RemoveSeaProvincesFromStates;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Scripts;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button Button_OpenSearchWarningsSettings;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_GitHub;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Telegram;
        public System.Windows.Forms.ComboBox ComboBox_Tool_Parameter_Value;
        public System.Windows.Forms.GroupBox GroupBox_Tool_Parameter_Value;
        private System.Windows.Forms.GroupBox GroupBox_Main_Layer_Parameter;
        private System.Windows.Forms.Button Button_MapMainLayer_Parameter;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Export;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Export_MainLayer;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Export_SelectedBorders;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Province_VictoryPoints_Info;
        private System.Windows.Forms.ToolStripTextBox ToolStripTextBox_Map_Province_VictoryPoints_Info_Value;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RemoveGhostProvinces;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Actions_Merge_All;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Actions;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Actions_MergeSelectedProvinces;
        private System.Windows.Forms.Panel Panel_SecondColor;
        private System.Windows.Forms.Panel Panel_FirstColor;
        private System.Windows.Forms.Integration.ElementHost ElementHost_ColorPicker;
        private ColorPicker.StandardColorPicker standardColorPicker1;
        private System.Windows.Forms.Panel Panel_ColorPicker;
        private System.Windows.Forms.Button Panel_ColorPicker_Button_Save;
        private System.Windows.Forms.Button Panel_ColorPicker_Button_Close;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Actions_FindMapChanges;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_State;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Region;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_State_Id;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_State_CreateAndSet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_State_OpenFileInExplorer;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_State_OpenFileInEditor;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBox_Map_Region_Id;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Region_CreateAndSet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Region_OpenFileInExplorer;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Map_Region_OpenFileInEditor;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_Actions_CreateObject;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_Random;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_BasedOnStatesColors;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AutoTools_RegenerateProvincesColors_OpenPatternsSettings;
        private System.Windows.Forms.Button Button_GenerateColor_OpenSettings;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Statistics;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
    }
}

