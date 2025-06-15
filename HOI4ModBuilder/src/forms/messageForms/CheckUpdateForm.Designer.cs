namespace HOI4ModBuilder.src.forms.messageForms
{
    partial class CheckUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckUpdateForm));
            this.Button_GoToRelease = new System.Windows.Forms.Button();
            this.Button_IgnoreRelease = new System.Windows.Forms.Button();
            this.Button_Close = new System.Windows.Forms.Button();
            this.Label_Status = new System.Windows.Forms.Label();
            this.RichTexBox_Description = new System.Windows.Forms.RichTextBox();
            this.Label_VersionID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button_GoToRelease
            // 
            resources.ApplyResources(this.Button_GoToRelease, "Button_GoToRelease");
            this.Button_GoToRelease.Name = "Button_GoToRelease";
            this.Button_GoToRelease.UseVisualStyleBackColor = true;
            this.Button_GoToRelease.Click += new System.EventHandler(this.Button_GoToRelease_Click);
            // 
            // Button_IgnoreRelease
            // 
            resources.ApplyResources(this.Button_IgnoreRelease, "Button_IgnoreRelease");
            this.Button_IgnoreRelease.Name = "Button_IgnoreRelease";
            this.Button_IgnoreRelease.UseVisualStyleBackColor = true;
            this.Button_IgnoreRelease.Click += new System.EventHandler(this.Button_IgnoreRelease_Click);
            // 
            // Button_Close
            // 
            resources.ApplyResources(this.Button_Close, "Button_Close");
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.UseVisualStyleBackColor = true;
            this.Button_Close.Click += new System.EventHandler(this.Button_Close_Click);
            // 
            // Label_Status
            // 
            resources.ApplyResources(this.Label_Status, "Label_Status");
            this.Label_Status.Name = "Label_Status";
            // 
            // RichTexBox_Description
            // 
            resources.ApplyResources(this.RichTexBox_Description, "RichTexBox_Description");
            this.RichTexBox_Description.Name = "RichTexBox_Description";
            this.RichTexBox_Description.ReadOnly = true;
            // 
            // Label_VersionID
            // 
            resources.ApplyResources(this.Label_VersionID, "Label_VersionID");
            this.Label_VersionID.Name = "Label_VersionID";
            // 
            // CheckUpdateForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label_VersionID);
            this.Controls.Add(this.RichTexBox_Description);
            this.Controls.Add(this.Label_Status);
            this.Controls.Add(this.Button_Close);
            this.Controls.Add(this.Button_IgnoreRelease);
            this.Controls.Add(this.Button_GoToRelease);
            this.Name = "CheckUpdateForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_GoToRelease;
        private System.Windows.Forms.Button Button_IgnoreRelease;
        private System.Windows.Forms.Button Button_Close;
        private System.Windows.Forms.Label Label_Status;
        private System.Windows.Forms.RichTextBox RichTexBox_Description;
        private System.Windows.Forms.Label Label_VersionID;
    }
}