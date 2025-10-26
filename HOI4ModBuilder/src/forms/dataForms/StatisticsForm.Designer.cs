namespace HOI4ModBuilder.src.forms.dataForms
{
    partial class StatisticsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsForm));
            this.Button_Update = new System.Windows.Forms.Button();
            this.GroupBox_Filters = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.TextBox_Filters_Countries = new System.Windows.Forms.TextBox();
            this.Button_Filters_Clear = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TextBox_Filters_Regions = new System.Windows.Forms.TextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TextBox_Filters_States = new System.Windows.Forms.TextBox();
            this.RichTextBox_Statistics = new System.Windows.Forms.RichTextBox();
            this.GroupBox_Filters.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_Update
            // 
            resources.ApplyResources(this.Button_Update, "Button_Update");
            this.Button_Update.Name = "Button_Update";
            this.Button_Update.UseVisualStyleBackColor = true;
            this.Button_Update.Click += new System.EventHandler(this.Button_Update_Click);
            // 
            // GroupBox_Filters
            // 
            resources.ApplyResources(this.GroupBox_Filters, "GroupBox_Filters");
            this.GroupBox_Filters.Controls.Add(this.groupBox4);
            this.GroupBox_Filters.Controls.Add(this.Button_Filters_Clear);
            this.GroupBox_Filters.Controls.Add(this.Button_Update);
            this.GroupBox_Filters.Controls.Add(this.groupBox3);
            this.GroupBox_Filters.Controls.Add(this.richTextBox2);
            this.GroupBox_Filters.Controls.Add(this.groupBox2);
            this.GroupBox_Filters.Name = "GroupBox_Filters";
            this.GroupBox_Filters.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.TextBox_Filters_Countries);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // TextBox_Filters_Countries
            // 
            resources.ApplyResources(this.TextBox_Filters_Countries, "TextBox_Filters_Countries");
            this.TextBox_Filters_Countries.Name = "TextBox_Filters_Countries";
            this.TextBox_Filters_Countries.TextChanged += new System.EventHandler(this.TextBox_Filters_Countries_TextChanged);
            // 
            // Button_Filters_Clear
            // 
            resources.ApplyResources(this.Button_Filters_Clear, "Button_Filters_Clear");
            this.Button_Filters_Clear.Name = "Button_Filters_Clear";
            this.Button_Filters_Clear.UseVisualStyleBackColor = true;
            this.Button_Filters_Clear.Click += new System.EventHandler(this.Button_Filters_Clear_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TextBox_Filters_Regions);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // TextBox_Filters_Regions
            // 
            resources.ApplyResources(this.TextBox_Filters_Regions, "TextBox_Filters_Regions");
            this.TextBox_Filters_Regions.Name = "TextBox_Filters_Regions";
            this.TextBox_Filters_Regions.TextChanged += new System.EventHandler(this.TextBox_Filters_Regions_TextChanged);
            // 
            // richTextBox2
            // 
            resources.ApplyResources(this.richTextBox2, "richTextBox2");
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TextBox_Filters_States);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // TextBox_Filters_States
            // 
            resources.ApplyResources(this.TextBox_Filters_States, "TextBox_Filters_States");
            this.TextBox_Filters_States.Name = "TextBox_Filters_States";
            this.TextBox_Filters_States.TextChanged += new System.EventHandler(this.TextBox_Filters_States_TextChanged);
            // 
            // RichTextBox_Statistics
            // 
            resources.ApplyResources(this.RichTextBox_Statistics, "RichTextBox_Statistics");
            this.RichTextBox_Statistics.Name = "RichTextBox_Statistics";
            this.RichTextBox_Statistics.ReadOnly = true;
            // 
            // StatisticsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RichTextBox_Statistics);
            this.Controls.Add(this.GroupBox_Filters);
            this.Name = "StatisticsForm";
            this.GroupBox_Filters.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Button_Update;
        private System.Windows.Forms.GroupBox GroupBox_Filters;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TextBox_Filters_Regions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TextBox_Filters_States;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox TextBox_Filters_Countries;
        private System.Windows.Forms.RichTextBox RichTextBox_Statistics;
        private System.Windows.Forms.Button Button_Filters_Clear;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}