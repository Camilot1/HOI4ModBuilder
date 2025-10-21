namespace HOI4ModBuilder.src.forms
{
    partial class StateListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateListForm));
            this.DataGridView_States = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnlocalizedName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Manpower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Owner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Provinces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pixels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TextBox_Id = new System.Windows.Forms.TextBox();
            this.Button_Find = new System.Windows.Forms.Button();
            this.Button_Create = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TextBox_FileInfo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RichTextBox_StateInfo = new System.Windows.Forms.RichTextBox();
            this.Button_Load = new System.Windows.Forms.Button();
            this.Button_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_States)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView_States
            // 
            resources.ApplyResources(this.DataGridView_States, "DataGridView_States");
            this.DataGridView_States.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_States.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.UnlocalizedName,
            this.Manpower,
            this.Category,
            this.Owner,
            this.Provinces,
            this.Pixels});
            this.DataGridView_States.Name = "DataGridView_States";
            // 
            // ID
            // 
            resources.ApplyResources(this.ID, "ID");
            this.ID.Name = "ID";
            // 
            // UnlocalizedName
            // 
            resources.ApplyResources(this.UnlocalizedName, "UnlocalizedName");
            this.UnlocalizedName.Name = "UnlocalizedName";
            // 
            // Manpower
            // 
            resources.ApplyResources(this.Manpower, "Manpower");
            this.Manpower.Name = "Manpower";
            // 
            // Category
            // 
            resources.ApplyResources(this.Category, "Category");
            this.Category.Name = "Category";
            // 
            // Owner
            // 
            resources.ApplyResources(this.Owner, "Owner");
            this.Owner.Name = "Owner";
            // 
            // Provinces
            // 
            resources.ApplyResources(this.Provinces, "Provinces");
            this.Provinces.Name = "Provinces";
            this.Provinces.ReadOnly = true;
            // 
            // Pixels
            // 
            resources.ApplyResources(this.Pixels, "Pixels");
            this.Pixels.Name = "Pixels";
            this.Pixels.ReadOnly = true;
            // 
            // TextBox_Id
            // 
            resources.ApplyResources(this.TextBox_Id, "TextBox_Id");
            this.TextBox_Id.Name = "TextBox_Id";
            // 
            // Button_Find
            // 
            resources.ApplyResources(this.Button_Find, "Button_Find");
            this.Button_Find.Name = "Button_Find";
            this.Button_Find.UseVisualStyleBackColor = true;
            this.Button_Find.Click += new System.EventHandler(this.Button_Find_Click);
            // 
            // Button_Create
            // 
            resources.ApplyResources(this.Button_Create, "Button_Create");
            this.Button_Create.Name = "Button_Create";
            this.Button_Create.UseVisualStyleBackColor = true;
            this.Button_Create.Click += new System.EventHandler(this.Button_Create_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.TextBox_FileInfo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.RichTextBox_StateInfo);
            this.groupBox1.Controls.Add(this.Button_Load);
            this.groupBox1.Controls.Add(this.Button_Save);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // TextBox_FileInfo
            // 
            resources.ApplyResources(this.TextBox_FileInfo, "TextBox_FileInfo");
            this.TextBox_FileInfo.Name = "TextBox_FileInfo";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // RichTextBox_StateInfo
            // 
            this.RichTextBox_StateInfo.AcceptsTab = true;
            resources.ApplyResources(this.RichTextBox_StateInfo, "RichTextBox_StateInfo");
            this.RichTextBox_StateInfo.Name = "RichTextBox_StateInfo";
            // 
            // Button_Load
            // 
            resources.ApplyResources(this.Button_Load, "Button_Load");
            this.Button_Load.Name = "Button_Load";
            this.Button_Load.UseVisualStyleBackColor = true;
            this.Button_Load.Click += new System.EventHandler(this.Button_Load_Click);
            // 
            // Button_Save
            // 
            resources.ApplyResources(this.Button_Save, "Button_Save");
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // StateListForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Button_Create);
            this.Controls.Add(this.Button_Find);
            this.Controls.Add(this.TextBox_Id);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataGridView_States);
            this.Name = "StateListForm";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_States)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView_States;
        private System.Windows.Forms.TextBox TextBox_Id;
        private System.Windows.Forms.Button Button_Find;
        private System.Windows.Forms.Button Button_Create;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox RichTextBox_StateInfo;
        private System.Windows.Forms.Button Button_Load;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.TextBox TextBox_FileInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnlocalizedName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Manpower;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn Owner;
        private System.Windows.Forms.DataGridViewTextBoxColumn Provinces;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pixels;
    }
}