namespace HOI4ModBuilder.src.forms
{
    partial class TextBoxMessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextBoxMessageForm));
            this.RichTextBox_Main = new System.Windows.Forms.RichTextBox();
            this.Label_MainText = new System.Windows.Forms.Label();
            this.Button_Close = new System.Windows.Forms.Button();
            this.Button_CopyToClipboard = new System.Windows.Forms.Button();
            this.Button_OpenLogsDirectory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RichTextBox_Main
            // 
            resources.ApplyResources(this.RichTextBox_Main, "RichTextBox_Main");
            this.RichTextBox_Main.Name = "RichTextBox_Main";
            // 
            // Label_MainText
            // 
            resources.ApplyResources(this.Label_MainText, "Label_MainText");
            this.Label_MainText.Name = "Label_MainText";
            // 
            // Button_Close
            // 
            resources.ApplyResources(this.Button_Close, "Button_Close");
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.UseVisualStyleBackColor = true;
            this.Button_Close.Click += new System.EventHandler(this.Button_Close_Click);
            // 
            // Button_CopyToClipboard
            // 
            resources.ApplyResources(this.Button_CopyToClipboard, "Button_CopyToClipboard");
            this.Button_CopyToClipboard.Name = "Button_CopyToClipboard";
            this.Button_CopyToClipboard.UseVisualStyleBackColor = true;
            this.Button_CopyToClipboard.Click += new System.EventHandler(this.Button_CopyToClipboard_Click);
            // 
            // Button_OpenLogsDirectory
            // 
            resources.ApplyResources(this.Button_OpenLogsDirectory, "Button_OpenLogsDirectory");
            this.Button_OpenLogsDirectory.Name = "Button_OpenLogsDirectory";
            this.Button_OpenLogsDirectory.UseVisualStyleBackColor = true;
            this.Button_OpenLogsDirectory.Click += new System.EventHandler(this.Button_OpenLogsDirectory_Click);
            // 
            // TextBoxMessageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_OpenLogsDirectory);
            this.Controls.Add(this.Button_CopyToClipboard);
            this.Controls.Add(this.Button_Close);
            this.Controls.Add(this.Label_MainText);
            this.Controls.Add(this.RichTextBox_Main);
            this.Name = "TextBoxMessageForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RichTextBox_Main;
        private System.Windows.Forms.Label Label_MainText;
        private System.Windows.Forms.Button Button_Close;
        private System.Windows.Forms.Button Button_CopyToClipboard;
        private System.Windows.Forms.Button Button_OpenLogsDirectory;
    }
}