namespace HOI4ModBuilder.src.forms.actionForms
{
    partial class CreateObjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateObjectForm));
            this.RichTextBox_File_Text = new System.Windows.Forms.RichTextBox();
            this.TextBox_File_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Button_Create = new System.Windows.Forms.Button();
            this.GroupBox_Pattern = new System.Windows.Forms.GroupBox();
            this.Button_Pattern_Load = new System.Windows.Forms.Button();
            this.Button_Pattern_Save = new System.Windows.Forms.Button();
            this.Label_ID = new System.Windows.Forms.Label();
            this.ComboBox_ObjectType = new System.Windows.Forms.ComboBox();
            this.GroupBox_Pattern.SuspendLayout();
            this.SuspendLayout();
            // 
            // RichTextBox_File_Text
            // 
            this.RichTextBox_File_Text.AcceptsTab = true;
            resources.ApplyResources(this.RichTextBox_File_Text, "RichTextBox_File_Text");
            this.RichTextBox_File_Text.Name = "RichTextBox_File_Text";
            this.RichTextBox_File_Text.TextChanged += new System.EventHandler(this.RichTextBox_File_Text_TextChanged);
            this.RichTextBox_File_Text.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox_File_Text_KeyDown);
            // 
            // TextBox_File_Name
            // 
            resources.ApplyResources(this.TextBox_File_Name, "TextBox_File_Name");
            this.TextBox_File_Name.Name = "TextBox_File_Name";
            this.TextBox_File_Name.TextChanged += new System.EventHandler(this.TextBox_File_Name_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Button_Create
            // 
            resources.ApplyResources(this.Button_Create, "Button_Create");
            this.Button_Create.Name = "Button_Create";
            this.Button_Create.UseVisualStyleBackColor = true;
            this.Button_Create.Click += new System.EventHandler(this.Button_Create_Click);
            // 
            // GroupBox_Pattern
            // 
            resources.ApplyResources(this.GroupBox_Pattern, "GroupBox_Pattern");
            this.GroupBox_Pattern.Controls.Add(this.Button_Pattern_Load);
            this.GroupBox_Pattern.Controls.Add(this.Button_Pattern_Save);
            this.GroupBox_Pattern.Name = "GroupBox_Pattern";
            this.GroupBox_Pattern.TabStop = false;
            // 
            // Button_Pattern_Load
            // 
            resources.ApplyResources(this.Button_Pattern_Load, "Button_Pattern_Load");
            this.Button_Pattern_Load.Name = "Button_Pattern_Load";
            this.Button_Pattern_Load.UseVisualStyleBackColor = true;
            this.Button_Pattern_Load.Click += new System.EventHandler(this.Button_Pattern_Load_Click);
            // 
            // Button_Pattern_Save
            // 
            resources.ApplyResources(this.Button_Pattern_Save, "Button_Pattern_Save");
            this.Button_Pattern_Save.Name = "Button_Pattern_Save";
            this.Button_Pattern_Save.UseVisualStyleBackColor = true;
            this.Button_Pattern_Save.Click += new System.EventHandler(this.Button_Pattern_Save_Click);
            // 
            // Label_ID
            // 
            resources.ApplyResources(this.Label_ID, "Label_ID");
            this.Label_ID.Name = "Label_ID";
            // 
            // ComboBox_ObjectType
            // 
            resources.ApplyResources(this.ComboBox_ObjectType, "ComboBox_ObjectType");
            this.ComboBox_ObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_ObjectType.FormattingEnabled = true;
            this.ComboBox_ObjectType.Name = "ComboBox_ObjectType";
            this.ComboBox_ObjectType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_ObjectType_SelectedIndexChanged);
            // 
            // CreateObjectForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ComboBox_ObjectType);
            this.Controls.Add(this.GroupBox_Pattern);
            this.Controls.Add(this.Button_Create);
            this.Controls.Add(this.Label_ID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_File_Name);
            this.Controls.Add(this.RichTextBox_File_Text);
            this.Name = "CreateObjectForm";
            this.GroupBox_Pattern.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RichTextBox_File_Text;
        private System.Windows.Forms.TextBox TextBox_File_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Button_Create;
        private System.Windows.Forms.GroupBox GroupBox_Pattern;
        private System.Windows.Forms.Button Button_Pattern_Load;
        private System.Windows.Forms.Button Button_Pattern_Save;
        private System.Windows.Forms.Label Label_ID;
        private System.Windows.Forms.ComboBox ComboBox_ObjectType;
    }
}