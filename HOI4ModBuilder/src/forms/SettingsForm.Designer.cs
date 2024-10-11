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
            this.TextBox_MAP_SCALE_PIXEL_TO_KM_Default = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TextBox_ActionHistorySize = new System.Windows.Forms.TextBox();
            this.TextBox_Textures_Opacity = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ComboBox_MaxAdditionalTextureSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TextBox_WATER_HEIGHT_Default = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TextBox_NormalMapStrength_Default = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_SaveSettings_Default = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Button_CreateModSettings = new System.Windows.Forms.Button();
            this.ComboBox_UsingSettingsType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage_DefaultModSettings = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_Wips_Default = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TabPage_CurrentModSettings = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_Wips_Current = new System.Windows.Forms.CheckedListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.TextBox_MAP_SCALE_PIXEL_TO_KM_Current = new System.Windows.Forms.TextBox();
            this.TextBox_WATER_HEIGHT_Current = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.CheckedListBox_SaveSettings_Current = new System.Windows.Forms.CheckedListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.TextBox_NormalMapStrength_Current = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabPage_DefaultModSettings.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.TabPage_CurrentModSettings.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            // TextBox_MAP_SCALE_PIXEL_TO_KM_Default
            // 
            resources.ApplyResources(this.TextBox_MAP_SCALE_PIXEL_TO_KM_Default, "TextBox_MAP_SCALE_PIXEL_TO_KM_Default");
            this.TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Name = "TextBox_MAP_SCALE_PIXEL_TO_KM_Default";
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
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items3"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items4"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items5"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items6"),
            resources.GetString("ComboBox_MaxAdditionalTextureSize.Items7")});
            this.ComboBox_MaxAdditionalTextureSize.Name = "ComboBox_MaxAdditionalTextureSize";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // TextBox_WATER_HEIGHT_Default
            // 
            resources.ApplyResources(this.TextBox_WATER_HEIGHT_Default, "TextBox_WATER_HEIGHT_Default");
            this.TextBox_WATER_HEIGHT_Default.Name = "TextBox_WATER_HEIGHT_Default";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // TextBox_NormalMapStrength_Default
            // 
            resources.ApplyResources(this.TextBox_NormalMapStrength_Default, "TextBox_NormalMapStrength_Default");
            this.TextBox_NormalMapStrength_Default.Name = "TextBox_NormalMapStrength_Default";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.CheckedListBox_SaveSettings_Default);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.TextBox_NormalMapStrength_Default);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // CheckedListBox_SaveSettings_Default
            // 
            resources.ApplyResources(this.CheckedListBox_SaveSettings_Default, "CheckedListBox_SaveSettings_Default");
            this.CheckedListBox_SaveSettings_Default.CheckOnClick = true;
            this.CheckedListBox_SaveSettings_Default.FormattingEnabled = true;
            this.CheckedListBox_SaveSettings_Default.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_SaveSettings_Default.Items"),
            resources.GetString("CheckedListBox_SaveSettings_Default.Items1")});
            this.CheckedListBox_SaveSettings_Default.Name = "CheckedListBox_SaveSettings_Default";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.Button_CreateModSettings);
            this.groupBox2.Controls.Add(this.ComboBox_UsingSettingsType);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
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
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.TabPage_DefaultModSettings);
            this.tabControl1.Controls.Add(this.TabPage_CurrentModSettings);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // TabPage_DefaultModSettings
            // 
            resources.ApplyResources(this.TabPage_DefaultModSettings, "TabPage_DefaultModSettings");
            this.TabPage_DefaultModSettings.Controls.Add(this.groupBox6);
            this.TabPage_DefaultModSettings.Controls.Add(this.groupBox3);
            this.TabPage_DefaultModSettings.Controls.Add(this.groupBox1);
            this.TabPage_DefaultModSettings.Name = "TabPage_DefaultModSettings";
            this.TabPage_DefaultModSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.CheckedListBox_Wips_Default);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // CheckedListBox_Wips_Default
            // 
            resources.ApplyResources(this.CheckedListBox_Wips_Default, "CheckedListBox_Wips_Default");
            this.CheckedListBox_Wips_Default.CheckOnClick = true;
            this.CheckedListBox_Wips_Default.FormattingEnabled = true;
            this.CheckedListBox_Wips_Default.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_Wips_Default.Items"),
            resources.GetString("CheckedListBox_Wips_Default.Items1"),
            resources.GetString("CheckedListBox_Wips_Default.Items2"),
            resources.GetString("CheckedListBox_Wips_Default.Items3")});
            this.CheckedListBox_Wips_Default.Name = "CheckedListBox_Wips_Default";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.TextBox_MAP_SCALE_PIXEL_TO_KM_Default);
            this.groupBox3.Controls.Add(this.TextBox_WATER_HEIGHT_Default);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // TabPage_CurrentModSettings
            // 
            resources.ApplyResources(this.TabPage_CurrentModSettings, "TabPage_CurrentModSettings");
            this.TabPage_CurrentModSettings.Controls.Add(this.groupBox7);
            this.TabPage_CurrentModSettings.Controls.Add(this.groupBox4);
            this.TabPage_CurrentModSettings.Controls.Add(this.groupBox5);
            this.TabPage_CurrentModSettings.Name = "TabPage_CurrentModSettings";
            this.TabPage_CurrentModSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.CheckedListBox_Wips_Current);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // CheckedListBox_Wips_Current
            // 
            resources.ApplyResources(this.CheckedListBox_Wips_Current, "CheckedListBox_Wips_Current");
            this.CheckedListBox_Wips_Current.CheckOnClick = true;
            this.CheckedListBox_Wips_Current.FormattingEnabled = true;
            this.CheckedListBox_Wips_Current.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_Wips_Current.Items"),
            resources.GetString("CheckedListBox_Wips_Current.Items1"),
            resources.GetString("CheckedListBox_Wips_Current.Items2"),
            resources.GetString("CheckedListBox_Wips_Current.Items3")});
            this.CheckedListBox_Wips_Current.Name = "CheckedListBox_Wips_Current";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.TextBox_MAP_SCALE_PIXEL_TO_KM_Current);
            this.groupBox4.Controls.Add(this.TextBox_WATER_HEIGHT_Current);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // TextBox_MAP_SCALE_PIXEL_TO_KM_Current
            // 
            resources.ApplyResources(this.TextBox_MAP_SCALE_PIXEL_TO_KM_Current, "TextBox_MAP_SCALE_PIXEL_TO_KM_Current");
            this.TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Name = "TextBox_MAP_SCALE_PIXEL_TO_KM_Current";
            // 
            // TextBox_WATER_HEIGHT_Current
            // 
            resources.ApplyResources(this.TextBox_WATER_HEIGHT_Current, "TextBox_WATER_HEIGHT_Current");
            this.TextBox_WATER_HEIGHT_Current.Name = "TextBox_WATER_HEIGHT_Current";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.CheckedListBox_SaveSettings_Current);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.TextBox_NormalMapStrength_Current);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // CheckedListBox_SaveSettings_Current
            // 
            resources.ApplyResources(this.CheckedListBox_SaveSettings_Current, "CheckedListBox_SaveSettings_Current");
            this.CheckedListBox_SaveSettings_Current.CheckOnClick = true;
            this.CheckedListBox_SaveSettings_Current.FormattingEnabled = true;
            this.CheckedListBox_SaveSettings_Current.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_SaveSettings_Current.Items"),
            resources.GetString("CheckedListBox_SaveSettings_Current.Items1")});
            this.CheckedListBox_SaveSettings_Current.Name = "CheckedListBox_SaveSettings_Current";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // TextBox_NormalMapStrength_Current
            // 
            resources.ApplyResources(this.TextBox_NormalMapStrength_Current, "TextBox_NormalMapStrength_Current");
            this.TextBox_NormalMapStrength_Current.Name = "TextBox_NormalMapStrength_Current";
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.TabPage_DefaultModSettings.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.TabPage_CurrentModSettings.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.TextBox TextBox_MAP_SCALE_PIXEL_TO_KM_Default;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBox_ActionHistorySize;
        private System.Windows.Forms.TextBox TextBox_Textures_Opacity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ComboBox_MaxAdditionalTextureSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBox_WATER_HEIGHT_Default;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TextBox_NormalMapStrength_Default;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox CheckedListBox_SaveSettings_Default;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Button_CreateModSettings;
        private System.Windows.Forms.ComboBox ComboBox_UsingSettingsType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPage_DefaultModSettings;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabPage TabPage_CurrentModSettings;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TextBox_MAP_SCALE_PIXEL_TO_KM_Current;
        private System.Windows.Forms.TextBox TextBox_WATER_HEIGHT_Current;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckedListBox CheckedListBox_SaveSettings_Current;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TextBox_NormalMapStrength_Current;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckedListBox CheckedListBox_Wips_Default;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckedListBox CheckedListBox_Wips_Current;
    }
}