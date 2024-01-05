namespace HOI4ModBuilder.src.forms
{
    partial class SearchErrorsSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchErrorsSettingsForm));
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Button_EnableAll = new System.Windows.Forms.Button();
            this.Button_DisableAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            resources.ApplyResources(this.checkedListBox1, "checkedListBox1");
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // Button_Save
            // 
            resources.ApplyResources(this.Button_Save, "Button_Save");
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // Button_EnableAll
            // 
            resources.ApplyResources(this.Button_EnableAll, "Button_EnableAll");
            this.Button_EnableAll.Name = "Button_EnableAll";
            this.Button_EnableAll.UseVisualStyleBackColor = true;
            this.Button_EnableAll.Click += new System.EventHandler(this.Button_EnableAll_Click);
            // 
            // Button_DisableAll
            // 
            resources.ApplyResources(this.Button_DisableAll, "Button_DisableAll");
            this.Button_DisableAll.Name = "Button_DisableAll";
            this.Button_DisableAll.UseVisualStyleBackColor = true;
            this.Button_DisableAll.Click += new System.EventHandler(this.Button_DisableAll_Click);
            // 
            // SearchErrorsSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_DisableAll);
            this.Controls.Add(this.Button_EnableAll);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.checkedListBox1);
            this.Name = "SearchErrorsSettingsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Button Button_EnableAll;
        private System.Windows.Forms.Button Button_DisableAll;
    }
}