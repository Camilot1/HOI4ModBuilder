namespace HOI4ModBuilder.src.forms
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.ComboBox_Language = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_GameDirectory = new System.Windows.Forms.TextBox();
            this.Button_GameDirectory = new System.Windows.Forms.Button();
            this.Button_GameTempDirectory = new System.Windows.Forms.Button();
            this.TextBox_GameTempDirectory = new System.Windows.Forms.TextBox();
            this.Button_ModDirectory = new System.Windows.Forms.Button();
            this.TextBox_ModDirectory = new System.Windows.Forms.TextBox();
            this.Button_Save = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_MAP_VIEWPORT_HEIGHT = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_MAP_SCALE_PIXEL_TO_KM = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TextBox_ActionHistorySize = new System.Windows.Forms.TextBox();
            this.TextBox_Textures_Opacity = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ComboBox_MaxAdditionalTextureSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TextBox_WATER_HEIGHT = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TextBox_NormalMapStrength = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckBox_UseCustomSavePatterns = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.TextBox_NormalMapBlur = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.CheckedListBox_SaveSettings = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DataGridView_ColorGenerationPatterns = new System.Windows.Forms.DataGridView();
            this.Column_ParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_MinValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_MaxValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComboBox_SelectedColorGenerationPattern = new System.Windows.Forms.ComboBox();
            this.Button_ModSettings_ApplyChanges = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_Wips = new System.Windows.Forms.CheckedListBox();
            this.Button_CreateModSettings = new System.Windows.Forms.Button();
            this.ComboBox_UsingSettingsType = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TextBox_WATER_HEIGHT_max_water_offset = new System.Windows.Forms.TextBox();
            this.TextBox_WATER_HEIGHT_min_land_offset = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.CheckBox_IgnoreUpdateVersion = new System.Windows.Forms.CheckBox();
            this.TextBox_IgnoreUpdateVersion = new System.Windows.Forms.TextBox();
            this.Button_Directory_AutoDetect = new System.Windows.Forms.Button();
            this.Button_CreateDesktopShortcut = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_ColorGenerationPatterns)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ComboBox_Language
            // 
            resources.ApplyResources(this.ComboBox_Language, "ComboBox_Language");
            this.ComboBox_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Language.FormattingEnabled = true;
            this.ComboBox_Language.Items.AddRange(new object[] {
            resources.GetString("ComboBox_Language.Items"),
            resources.GetString("ComboBox_Language.Items1")});
            this.ComboBox_Language.Name = "ComboBox_Language";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TextBox_GameDirectory
            // 
            resources.ApplyResources(this.TextBox_GameDirectory, "TextBox_GameDirectory");
            this.TextBox_GameDirectory.Name = "TextBox_GameDirectory";
            // 
            // Button_GameDirectory
            // 
            resources.ApplyResources(this.Button_GameDirectory, "Button_GameDirectory");
            this.Button_GameDirectory.Name = "Button_GameDirectory";
            this.Button_GameDirectory.UseVisualStyleBackColor = true;
            this.Button_GameDirectory.Click += new System.EventHandler(this.Button_GameDirectory_Click);
            // 
            // Button_GameTempDirectory
            // 
            resources.ApplyResources(this.Button_GameTempDirectory, "Button_GameTempDirectory");
            this.Button_GameTempDirectory.Name = "Button_GameTempDirectory";
            this.Button_GameTempDirectory.UseVisualStyleBackColor = true;
            this.Button_GameTempDirectory.Click += new System.EventHandler(this.Button_GameTempDirectory_Click);
            // 
            // TextBox_GameTempDirectory
            // 
            resources.ApplyResources(this.TextBox_GameTempDirectory, "TextBox_GameTempDirectory");
            this.TextBox_GameTempDirectory.Name = "TextBox_GameTempDirectory";
            // 
            // Button_ModDirectory
            // 
            resources.ApplyResources(this.Button_ModDirectory, "Button_ModDirectory");
            this.Button_ModDirectory.Name = "Button_ModDirectory";
            this.Button_ModDirectory.UseVisualStyleBackColor = true;
            this.Button_ModDirectory.Click += new System.EventHandler(this.Button_ModDirectory_Click);
            // 
            // TextBox_ModDirectory
            // 
            resources.ApplyResources(this.TextBox_ModDirectory, "TextBox_ModDirectory");
            this.TextBox_ModDirectory.Name = "TextBox_ModDirectory";
            // 
            // Button_Save
            // 
            resources.ApplyResources(this.Button_Save, "Button_Save");
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TextBox_MAP_VIEWPORT_HEIGHT
            // 
            resources.ApplyResources(this.TextBox_MAP_VIEWPORT_HEIGHT, "TextBox_MAP_VIEWPORT_HEIGHT");
            this.TextBox_MAP_VIEWPORT_HEIGHT.Name = "TextBox_MAP_VIEWPORT_HEIGHT";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // TextBox_MAP_SCALE_PIXEL_TO_KM
            // 
            resources.ApplyResources(this.TextBox_MAP_SCALE_PIXEL_TO_KM, "TextBox_MAP_SCALE_PIXEL_TO_KM");
            this.TextBox_MAP_SCALE_PIXEL_TO_KM.Name = "TextBox_MAP_SCALE_PIXEL_TO_KM";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // TextBox_ActionHistorySize
            // 
            resources.ApplyResources(this.TextBox_ActionHistorySize, "TextBox_ActionHistorySize");
            this.TextBox_ActionHistorySize.Name = "TextBox_ActionHistorySize";
            // 
            // TextBox_Textures_Opacity
            // 
            resources.ApplyResources(this.TextBox_Textures_Opacity, "TextBox_Textures_Opacity");
            this.TextBox_Textures_Opacity.Name = "TextBox_Textures_Opacity";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // ComboBox_MaxAdditionalTextureSize
            // 
            resources.ApplyResources(this.ComboBox_MaxAdditionalTextureSize, "ComboBox_MaxAdditionalTextureSize");
            this.ComboBox_MaxAdditionalTextureSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MaxAdditionalTextureSize.FormattingEnabled = true;
            this.ComboBox_MaxAdditionalTextureSize.Items.AddRange(new object[] {
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items1"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items2"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items3")});
            this.ComboBox_MaxAdditionalTextureSize.Name = "ComboBox_MaxAdditionalTextureSize";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // TextBox_WATER_HEIGHT
            // 
            resources.ApplyResources(this.TextBox_WATER_HEIGHT, "TextBox_WATER_HEIGHT");
            this.TextBox_WATER_HEIGHT.Name = "TextBox_WATER_HEIGHT";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // TextBox_NormalMapStrength
            // 
            resources.ApplyResources(this.TextBox_NormalMapStrength, "TextBox_NormalMapStrength");
            this.TextBox_NormalMapStrength.Name = "TextBox_NormalMapStrength";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.CheckBox_UseCustomSavePatterns);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.TextBox_NormalMapBlur);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.CheckedListBox_SaveSettings);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.TextBox_NormalMapStrength);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // CheckBox_UseCustomSavePatterns
            // 
            resources.ApplyResources(this.CheckBox_UseCustomSavePatterns, "CheckBox_UseCustomSavePatterns");
            this.CheckBox_UseCustomSavePatterns.Name = "CheckBox_UseCustomSavePatterns";
            this.CheckBox_UseCustomSavePatterns.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // TextBox_NormalMapBlur
            // 
            resources.ApplyResources(this.TextBox_NormalMapBlur, "TextBox_NormalMapBlur");
            this.TextBox_NormalMapBlur.Name = "TextBox_NormalMapBlur";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // CheckedListBox_SaveSettings
            // 
            resources.ApplyResources(this.CheckedListBox_SaveSettings, "CheckedListBox_SaveSettings");
            this.CheckedListBox_SaveSettings.CheckOnClick = true;
            this.CheckedListBox_SaveSettings.FormattingEnabled = true;
            this.CheckedListBox_SaveSettings.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_SaveSettings.Items"),
            resources.GetString("CheckedListBox_SaveSettings.Items1")});
            this.CheckedListBox_SaveSettings.Name = "CheckedListBox_SaveSettings";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.Button_ModSettings_ApplyChanges);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.Button_CreateModSettings);
            this.groupBox2.Controls.Add(this.ComboBox_UsingSettingsType);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.DataGridView_ColorGenerationPatterns);
            this.groupBox4.Controls.Add(this.ComboBox_SelectedColorGenerationPattern);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // DataGridView_ColorGenerationPatterns
            // 
            resources.ApplyResources(this.DataGridView_ColorGenerationPatterns, "DataGridView_ColorGenerationPatterns");
            this.DataGridView_ColorGenerationPatterns.AllowUserToAddRows = false;
            this.DataGridView_ColorGenerationPatterns.AllowUserToDeleteRows = false;
            this.DataGridView_ColorGenerationPatterns.AllowUserToResizeColumns = false;
            this.DataGridView_ColorGenerationPatterns.AllowUserToResizeRows = false;
            this.DataGridView_ColorGenerationPatterns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DataGridView_ColorGenerationPatterns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_ParameterName,
            this.Column_MinValue,
            this.Column_MaxValue});
            this.DataGridView_ColorGenerationPatterns.MultiSelect = false;
            this.DataGridView_ColorGenerationPatterns.Name = "DataGridView_ColorGenerationPatterns";
            this.DataGridView_ColorGenerationPatterns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // Column_ParameterName
            // 
            this.Column_ParameterName.FillWeight = 110F;
            this.Column_ParameterName.Frozen = true;
            resources.ApplyResources(this.Column_ParameterName, "Column_ParameterName");
            this.Column_ParameterName.Name = "Column_ParameterName";
            this.Column_ParameterName.ReadOnly = true;
            // 
            // Column_MinValue
            // 
            this.Column_MinValue.FillWeight = 80F;
            this.Column_MinValue.Frozen = true;
            resources.ApplyResources(this.Column_MinValue, "Column_MinValue");
            this.Column_MinValue.Name = "Column_MinValue";
            // 
            // Column_MaxValue
            // 
            this.Column_MaxValue.FillWeight = 80F;
            this.Column_MaxValue.Frozen = true;
            resources.ApplyResources(this.Column_MaxValue, "Column_MaxValue");
            this.Column_MaxValue.Name = "Column_MaxValue";
            // 
            // ComboBox_SelectedColorGenerationPattern
            // 
            resources.ApplyResources(this.ComboBox_SelectedColorGenerationPattern, "ComboBox_SelectedColorGenerationPattern");
            this.ComboBox_SelectedColorGenerationPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_SelectedColorGenerationPattern.FormattingEnabled = true;
            this.ComboBox_SelectedColorGenerationPattern.Name = "ComboBox_SelectedColorGenerationPattern";
            this.ComboBox_SelectedColorGenerationPattern.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectedColorGenerationPattern_SelectedIndexChanged);
            // 
            // Button_ModSettings_ApplyChanges
            // 
            resources.ApplyResources(this.Button_ModSettings_ApplyChanges, "Button_ModSettings_ApplyChanges");
            this.Button_ModSettings_ApplyChanges.Name = "Button_ModSettings_ApplyChanges";
            this.Button_ModSettings_ApplyChanges.UseVisualStyleBackColor = true;
            this.Button_ModSettings_ApplyChanges.Click += new System.EventHandler(this.Button_ModSettings_ApplyChanges_Click);
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.CheckedListBox_Wips);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // CheckedListBox_Wips
            // 
            resources.ApplyResources(this.CheckedListBox_Wips, "CheckedListBox_Wips");
            this.CheckedListBox_Wips.CheckOnClick = true;
            this.CheckedListBox_Wips.FormattingEnabled = true;
            this.CheckedListBox_Wips.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_Wips.Items"),
            resources.GetString("CheckedListBox_Wips.Items1"),
            resources.GetString("CheckedListBox_Wips.Items2"),
            resources.GetString("CheckedListBox_Wips.Items3"),
            resources.GetString("CheckedListBox_Wips.Items4")});
            this.CheckedListBox_Wips.Name = "CheckedListBox_Wips";
            // 
            // Button_CreateModSettings
            // 
            resources.ApplyResources(this.Button_CreateModSettings, "Button_CreateModSettings");
            this.Button_CreateModSettings.Name = "Button_CreateModSettings";
            this.Button_CreateModSettings.UseVisualStyleBackColor = true;
            this.Button_CreateModSettings.Click += new System.EventHandler(this.Button_CreateModSettings_Click);
            // 
            // ComboBox_UsingSettingsType
            // 
            resources.ApplyResources(this.ComboBox_UsingSettingsType, "ComboBox_UsingSettingsType");
            this.ComboBox_UsingSettingsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_UsingSettingsType.FormattingEnabled = true;
            this.ComboBox_UsingSettingsType.Items.AddRange(new object[] {
            resources.GetString("ComboBox_UsingSettingsType.Items"),
            resources.GetString("ComboBox_UsingSettingsType.Items1")});
            this.ComboBox_UsingSettingsType.Name = "ComboBox_UsingSettingsType";
            this.ComboBox_UsingSettingsType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_UsingSettingsType_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.TextBox_WATER_HEIGHT_max_water_offset);
            this.groupBox3.Controls.Add(this.TextBox_WATER_HEIGHT_min_land_offset);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.TextBox_MAP_SCALE_PIXEL_TO_KM);
            this.groupBox3.Controls.Add(this.TextBox_WATER_HEIGHT);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // TextBox_WATER_HEIGHT_max_water_offset
            // 
            resources.ApplyResources(this.TextBox_WATER_HEIGHT_max_water_offset, "TextBox_WATER_HEIGHT_max_water_offset");
            this.TextBox_WATER_HEIGHT_max_water_offset.Name = "TextBox_WATER_HEIGHT_max_water_offset";
            // 
            // TextBox_WATER_HEIGHT_min_land_offset
            // 
            resources.ApplyResources(this.TextBox_WATER_HEIGHT_min_land_offset, "TextBox_WATER_HEIGHT_min_land_offset");
            this.TextBox_WATER_HEIGHT_min_land_offset.Name = "TextBox_WATER_HEIGHT_min_land_offset";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // CheckBox_IgnoreUpdateVersion
            // 
            resources.ApplyResources(this.CheckBox_IgnoreUpdateVersion, "CheckBox_IgnoreUpdateVersion");
            this.CheckBox_IgnoreUpdateVersion.Name = "CheckBox_IgnoreUpdateVersion";
            this.CheckBox_IgnoreUpdateVersion.UseVisualStyleBackColor = true;
            // 
            // TextBox_IgnoreUpdateVersion
            // 
            resources.ApplyResources(this.TextBox_IgnoreUpdateVersion, "TextBox_IgnoreUpdateVersion");
            this.TextBox_IgnoreUpdateVersion.Name = "TextBox_IgnoreUpdateVersion";
            // 
            // Button_Directory_AutoDetect
            // 
            resources.ApplyResources(this.Button_Directory_AutoDetect, "Button_Directory_AutoDetect");
            this.Button_Directory_AutoDetect.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Button_Directory_AutoDetect.Name = "Button_Directory_AutoDetect";
            this.Button_Directory_AutoDetect.UseVisualStyleBackColor = false;
            this.Button_Directory_AutoDetect.Click += new System.EventHandler(this.Button_Directory_AutoDetect_Click);
            // 
            // Button_CreateDesktopShortcut
            // 
            resources.ApplyResources(this.Button_CreateDesktopShortcut, "Button_CreateDesktopShortcut");
            this.Button_CreateDesktopShortcut.Name = "Button_CreateDesktopShortcut";
            this.Button_CreateDesktopShortcut.UseVisualStyleBackColor = true;
            this.Button_CreateDesktopShortcut.Click += new System.EventHandler(this.Button_CreateDesktopShortcut_Click);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_CreateDesktopShortcut);
            this.Controls.Add(this.Button_Directory_AutoDetect);
            this.Controls.Add(this.TextBox_IgnoreUpdateVersion);
            this.Controls.Add(this.CheckBox_IgnoreUpdateVersion);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ComboBox_MaxAdditionalTextureSize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextBox_Textures_Opacity);
            this.Controls.Add(this.TextBox_ActionHistorySize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TextBox_MAP_VIEWPORT_HEIGHT);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.Button_ModDirectory);
            this.Controls.Add(this.TextBox_ModDirectory);
            this.Controls.Add(this.Button_GameTempDirectory);
            this.Controls.Add(this.TextBox_GameTempDirectory);
            this.Controls.Add(this.Button_GameDirectory);
            this.Controls.Add(this.TextBox_GameDirectory);
            this.Controls.Add(this.ComboBox_Language);
            this.Controls.Add(this.label1);
            this.Name = "SettingsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_ColorGenerationPatterns)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboBox_Language;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_GameDirectory;
        private System.Windows.Forms.Button Button_GameDirectory;
        private System.Windows.Forms.Button Button_GameTempDirectory;
        private System.Windows.Forms.TextBox TextBox_GameTempDirectory;
        private System.Windows.Forms.Button Button_ModDirectory;
        private System.Windows.Forms.TextBox TextBox_ModDirectory;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBox_MAP_VIEWPORT_HEIGHT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_MAP_SCALE_PIXEL_TO_KM;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBox_ActionHistorySize;
        private System.Windows.Forms.TextBox TextBox_Textures_Opacity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ComboBox_MaxAdditionalTextureSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBox_WATER_HEIGHT;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TextBox_NormalMapStrength;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox CheckedListBox_SaveSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Button_CreateModSettings;
        private System.Windows.Forms.ComboBox ComboBox_UsingSettingsType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckedListBox CheckedListBox_Wips;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TextBox_NormalMapBlur;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox CheckBox_UseCustomSavePatterns;
        private System.Windows.Forms.CheckBox CheckBox_IgnoreUpdateVersion;
        private System.Windows.Forms.TextBox TextBox_IgnoreUpdateVersion;
        private System.Windows.Forms.Button Button_Directory_AutoDetect;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TextBox_WATER_HEIGHT_max_water_offset;
        private System.Windows.Forms.TextBox TextBox_WATER_HEIGHT_min_land_offset;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button Button_ModSettings_ApplyChanges;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox ComboBox_SelectedColorGenerationPattern;
        private System.Windows.Forms.DataGridView DataGridView_ColorGenerationPatterns;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ParameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_MinValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_MaxValue;
        private System.Windows.Forms.Button Button_CreateDesktopShortcut;
    }
}