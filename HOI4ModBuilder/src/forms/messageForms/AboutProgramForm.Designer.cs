namespace HOI4ModBuilder.src.forms.messageForms
{
    partial class AboutProgramForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutProgramForm));
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Button_Releases = new System.Windows.Forms.Button();
            this.Button_Discord = new System.Windows.Forms.Button();
            this.Button_GitHubRepo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RichTextBox1
            // 
            resources.ApplyResources(this.RichTextBox1, "RichTextBox1");
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.ReadOnly = true;
            this.RichTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RichTextBox1_LinkClicked);
            // 
            // Button_Releases
            // 
            resources.ApplyResources(this.Button_Releases, "Button_Releases");
            this.Button_Releases.Name = "Button_Releases";
            this.Button_Releases.UseVisualStyleBackColor = true;
            this.Button_Releases.Click += new System.EventHandler(this.Button_GitHubReleases_Click);
            // 
            // Button_Discord
            // 
            resources.ApplyResources(this.Button_Discord, "Button_Discord");
            this.Button_Discord.Name = "Button_Discord";
            this.Button_Discord.UseVisualStyleBackColor = true;
            this.Button_Discord.Click += new System.EventHandler(this.Button_Discord_Click);
            // 
            // Button_GitHubRepo
            // 
            resources.ApplyResources(this.Button_GitHubRepo, "Button_GitHubRepo");
            this.Button_GitHubRepo.Name = "Button_GitHubRepo";
            this.Button_GitHubRepo.UseVisualStyleBackColor = true;
            this.Button_GitHubRepo.Click += new System.EventHandler(this.Button_GitHubRepo_Click);
            // 
            // AboutProgramForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_GitHubRepo);
            this.Controls.Add(this.Button_Discord);
            this.Controls.Add(this.Button_Releases);
            this.Controls.Add(this.RichTextBox1);
            this.Name = "AboutProgramForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RichTextBox1;
        private System.Windows.Forms.Button Button_Releases;
        private System.Windows.Forms.Button Button_Discord;
        private System.Windows.Forms.Button Button_GitHubRepo;
    }
}