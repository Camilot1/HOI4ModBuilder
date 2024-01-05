namespace HOI4ModBuilder.src.forms
{
    partial class ProvinceListForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProvinceListForm));
            this.DataGridView_Provinces = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Red = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Green = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Blue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsCoastal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Terrain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Continent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StateID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegionID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pixels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Borders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Button_Find = new System.Windows.Forms.Button();
            this.TextBox_Id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Panel_Color = new System.Windows.Forms.Panel();
            this.Button_ReplaceColor = new System.Windows.Forms.Button();
            this.Button_Remove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBox_Blue = new System.Windows.Forms.TextBox();
            this.TextBox_Green = new System.Windows.Forms.TextBox();
            this.TextBox_Red = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Button_GenerateColor_Lake = new System.Windows.Forms.Button();
            this.Button_GenerateColor_Sea = new System.Windows.Forms.Button();
            this.Button_GenerateColor_Land = new System.Windows.Forms.Button();
            this.Button_GenerateColor_Random = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Button_SetSecondColor = new System.Windows.Forms.Button();
            this.Button_SetFirstColor = new System.Windows.Forms.Button();
            this.provinceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Provinces)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.provinceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridView_Provinces
            // 
            resources.ApplyResources(this.DataGridView_Provinces, "DataGridView_Provinces");
            this.DataGridView_Provinces.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_Provinces.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Red,
            this.Green,
            this.Blue,
            this.Type,
            this.IsCoastal,
            this.Terrain,
            this.Continent,
            this.StateID,
            this.RegionID,
            this.Pixels,
            this.Borders});
            this.DataGridView_Provinces.Name = "DataGridView_Provinces";
            // 
            // ID
            // 
            resources.ApplyResources(this.ID, "ID");
            this.ID.Name = "ID";
            // 
            // Red
            // 
            resources.ApplyResources(this.Red, "Red");
            this.Red.Name = "Red";
            // 
            // Green
            // 
            resources.ApplyResources(this.Green, "Green");
            this.Green.Name = "Green";
            // 
            // Blue
            // 
            resources.ApplyResources(this.Blue, "Blue");
            this.Blue.Name = "Blue";
            // 
            // Type
            // 
            resources.ApplyResources(this.Type, "Type");
            this.Type.Name = "Type";
            // 
            // IsCoastal
            // 
            resources.ApplyResources(this.IsCoastal, "IsCoastal");
            this.IsCoastal.Name = "IsCoastal";
            // 
            // Terrain
            // 
            resources.ApplyResources(this.Terrain, "Terrain");
            this.Terrain.Name = "Terrain";
            // 
            // Continent
            // 
            resources.ApplyResources(this.Continent, "Continent");
            this.Continent.Name = "Continent";
            // 
            // StateID
            // 
            resources.ApplyResources(this.StateID, "StateID");
            this.StateID.Name = "StateID";
            this.StateID.ReadOnly = true;
            // 
            // RegionID
            // 
            resources.ApplyResources(this.RegionID, "RegionID");
            this.RegionID.Name = "RegionID";
            this.RegionID.ReadOnly = true;
            // 
            // Pixels
            // 
            resources.ApplyResources(this.Pixels, "Pixels");
            this.Pixels.Name = "Pixels";
            this.Pixels.ReadOnly = true;
            // 
            // Borders
            // 
            resources.ApplyResources(this.Borders, "Borders");
            this.Borders.Name = "Borders";
            this.Borders.ReadOnly = true;
            // 
            // Button_Find
            // 
            resources.ApplyResources(this.Button_Find, "Button_Find");
            this.Button_Find.Name = "Button_Find";
            this.Button_Find.UseVisualStyleBackColor = true;
            this.Button_Find.Click += new System.EventHandler(this.Button_Find_Click);
            // 
            // TextBox_Id
            // 
            resources.ApplyResources(this.TextBox_Id, "TextBox_Id");
            this.TextBox_Id.Name = "TextBox_Id";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Panel_Color
            // 
            resources.ApplyResources(this.Panel_Color, "Panel_Color");
            this.Panel_Color.BackColor = System.Drawing.Color.Transparent;
            this.Panel_Color.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_Color.Name = "Panel_Color";
            // 
            // Button_ReplaceColor
            // 
            resources.ApplyResources(this.Button_ReplaceColor, "Button_ReplaceColor");
            this.Button_ReplaceColor.Name = "Button_ReplaceColor";
            this.Button_ReplaceColor.UseVisualStyleBackColor = true;
            // 
            // Button_Remove
            // 
            resources.ApplyResources(this.Button_Remove, "Button_Remove");
            this.Button_Remove.Name = "Button_Remove";
            this.Button_Remove.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TextBox_Blue
            // 
            resources.ApplyResources(this.TextBox_Blue, "TextBox_Blue");
            this.TextBox_Blue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.TextBox_Blue.Name = "TextBox_Blue";
            // 
            // TextBox_Green
            // 
            resources.ApplyResources(this.TextBox_Green, "TextBox_Green");
            this.TextBox_Green.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.TextBox_Green.Name = "TextBox_Green";
            // 
            // TextBox_Red
            // 
            resources.ApplyResources(this.TextBox_Red, "TextBox_Red");
            this.TextBox_Red.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.TextBox_Red.Name = "TextBox_Red";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.Button_GenerateColor_Lake);
            this.groupBox1.Controls.Add(this.Button_GenerateColor_Sea);
            this.groupBox1.Controls.Add(this.Button_GenerateColor_Land);
            this.groupBox1.Controls.Add(this.Button_GenerateColor_Random);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // Button_GenerateColor_Lake
            // 
            resources.ApplyResources(this.Button_GenerateColor_Lake, "Button_GenerateColor_Lake");
            this.Button_GenerateColor_Lake.Name = "Button_GenerateColor_Lake";
            this.Button_GenerateColor_Lake.UseVisualStyleBackColor = true;
            this.Button_GenerateColor_Lake.Click += new System.EventHandler(this.Button_GenerateColor_Lake_Click);
            // 
            // Button_GenerateColor_Sea
            // 
            resources.ApplyResources(this.Button_GenerateColor_Sea, "Button_GenerateColor_Sea");
            this.Button_GenerateColor_Sea.Name = "Button_GenerateColor_Sea";
            this.Button_GenerateColor_Sea.UseVisualStyleBackColor = true;
            this.Button_GenerateColor_Sea.Click += new System.EventHandler(this.Button_GenerateColor_Sea_Click);
            // 
            // Button_GenerateColor_Land
            // 
            resources.ApplyResources(this.Button_GenerateColor_Land, "Button_GenerateColor_Land");
            this.Button_GenerateColor_Land.Name = "Button_GenerateColor_Land";
            this.Button_GenerateColor_Land.UseVisualStyleBackColor = true;
            this.Button_GenerateColor_Land.Click += new System.EventHandler(this.Button_GenerateColor_Land_Click);
            // 
            // Button_GenerateColor_Random
            // 
            resources.ApplyResources(this.Button_GenerateColor_Random, "Button_GenerateColor_Random");
            this.Button_GenerateColor_Random.Name = "Button_GenerateColor_Random";
            this.Button_GenerateColor_Random.UseVisualStyleBackColor = true;
            this.Button_GenerateColor_Random.Click += new System.EventHandler(this.Button_GenerateColor_Random_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.Button_SetSecondColor);
            this.groupBox2.Controls.Add(this.Button_SetFirstColor);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // Button_SetSecondColor
            // 
            resources.ApplyResources(this.Button_SetSecondColor, "Button_SetSecondColor");
            this.Button_SetSecondColor.Name = "Button_SetSecondColor";
            this.Button_SetSecondColor.UseVisualStyleBackColor = true;
            this.Button_SetSecondColor.Click += new System.EventHandler(this.Button_SetSecondColor_Click);
            // 
            // Button_SetFirstColor
            // 
            resources.ApplyResources(this.Button_SetFirstColor, "Button_SetFirstColor");
            this.Button_SetFirstColor.Name = "Button_SetFirstColor";
            this.Button_SetFirstColor.UseVisualStyleBackColor = true;
            this.Button_SetFirstColor.Click += new System.EventHandler(this.Button_SetFirstColor_Click);
            // 
            // provinceBindingSource
            // 
            this.provinceBindingSource.DataSource = typeof(HOI4ModBuilder.hoiDataObjects.map.Province);
            // 
            // ProvinceListForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TextBox_Red);
            this.Controls.Add(this.TextBox_Green);
            this.Controls.Add(this.TextBox_Blue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_Remove);
            this.Controls.Add(this.Button_ReplaceColor);
            this.Controls.Add(this.Panel_Color);
            this.Controls.Add(this.Button_Find);
            this.Controls.Add(this.TextBox_Id);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataGridView_Provinces);
            this.Name = "ProvinceListForm";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Provinces)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.provinceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView_Provinces;
        private System.Windows.Forms.BindingSource provinceBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Red;
        private System.Windows.Forms.DataGridViewTextBoxColumn Green;
        private System.Windows.Forms.DataGridViewTextBoxColumn Blue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsCoastal;
        private System.Windows.Forms.DataGridViewTextBoxColumn Terrain;
        private System.Windows.Forms.DataGridViewTextBoxColumn Continent;
        private System.Windows.Forms.DataGridViewTextBoxColumn StateID;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegionID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pixels;
        private System.Windows.Forms.DataGridViewTextBoxColumn Borders;
        private System.Windows.Forms.Button Button_Find;
        private System.Windows.Forms.TextBox TextBox_Id;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Panel Panel_Color;
        private System.Windows.Forms.Button Button_ReplaceColor;
        private System.Windows.Forms.Button Button_Remove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBox_Blue;
        private System.Windows.Forms.TextBox TextBox_Green;
        private System.Windows.Forms.TextBox TextBox_Red;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Button_GenerateColor_Land;
        private System.Windows.Forms.Button Button_GenerateColor_Random;
        private System.Windows.Forms.Button Button_GenerateColor_Sea;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Button_SetSecondColor;
        private System.Windows.Forms.Button Button_SetFirstColor;
        private System.Windows.Forms.Button Button_GenerateColor_Lake;
    }
}