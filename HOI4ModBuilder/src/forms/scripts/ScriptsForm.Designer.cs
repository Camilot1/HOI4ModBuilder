namespace HOI4ModBuilder.src.forms.scripts
{
    partial class ScriptsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptsForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.GroupBox_Script = new System.Windows.Forms.GroupBox();
            this.RichTextBox_Script = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.GroupBox_Debug = new System.Windows.Forms.GroupBox();
            this.Button_Debug_Terminate = new System.Windows.Forms.Button();
            this.RichTextBox_Debug = new System.Windows.Forms.RichTextBox();
            this.Button_Debug_NextStep = new System.Windows.Forms.Button();
            this.Button_Debug_Flip = new System.Windows.Forms.Button();
            this.RichTextBox_Console = new System.Windows.Forms.RichTextBox();
            this.Button_ChooseFile = new System.Windows.Forms.Button();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Button_Load = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Button_Execute = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GroupBox_Script.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.GroupBox_Debug.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.GroupBox_Script);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // GroupBox_Script
            // 
            resources.ApplyResources(this.GroupBox_Script, "GroupBox_Script");
            this.GroupBox_Script.Controls.Add(this.RichTextBox_Script);
            this.GroupBox_Script.Name = "GroupBox_Script";
            this.GroupBox_Script.TabStop = false;
            this.GroupBox_Script.Resize += new System.EventHandler(this.GroupBox_Script_Resize);
            // 
            // RichTextBox_Script
            // 
            this.RichTextBox_Script.AcceptsTab = true;
            resources.ApplyResources(this.RichTextBox_Script, "RichTextBox_Script");
            this.RichTextBox_Script.Name = "RichTextBox_Script";
            this.RichTextBox_Script.TextChanged += new System.EventHandler(this.RichTextBox_Script_TextChanged);
            this.RichTextBox_Script.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox_Script_KeyDown);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.GroupBox_Debug);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.RichTextBox_Console);
            // 
            // GroupBox_Debug
            // 
            resources.ApplyResources(this.GroupBox_Debug, "GroupBox_Debug");
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_Terminate);
            this.GroupBox_Debug.Controls.Add(this.RichTextBox_Debug);
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_NextStep);
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_Flip);
            this.GroupBox_Debug.Name = "GroupBox_Debug";
            this.GroupBox_Debug.TabStop = false;
            // 
            // Button_Debug_Terminate
            // 
            resources.ApplyResources(this.Button_Debug_Terminate, "Button_Debug_Terminate");
            this.Button_Debug_Terminate.Name = "Button_Debug_Terminate";
            this.Button_Debug_Terminate.UseVisualStyleBackColor = true;
            this.Button_Debug_Terminate.Click += new System.EventHandler(this.Button_Debug_Terminate_Click);
            // 
            // RichTextBox_Debug
            // 
            resources.ApplyResources(this.RichTextBox_Debug, "RichTextBox_Debug");
            this.RichTextBox_Debug.Name = "RichTextBox_Debug";
            // 
            // Button_Debug_NextStep
            // 
            resources.ApplyResources(this.Button_Debug_NextStep, "Button_Debug_NextStep");
            this.Button_Debug_NextStep.Name = "Button_Debug_NextStep";
            this.Button_Debug_NextStep.UseVisualStyleBackColor = true;
            this.Button_Debug_NextStep.Click += new System.EventHandler(this.Button_Debug_NextStep_Click);
            // 
            // Button_Debug_Flip
            // 
            resources.ApplyResources(this.Button_Debug_Flip, "Button_Debug_Flip");
            this.Button_Debug_Flip.Name = "Button_Debug_Flip";
            this.Button_Debug_Flip.UseVisualStyleBackColor = true;
            this.Button_Debug_Flip.Click += new System.EventHandler(this.Button_Debug_Flip_Click);
            // 
            // RichTextBox_Console
            // 
            resources.ApplyResources(this.RichTextBox_Console, "RichTextBox_Console");
            this.RichTextBox_Console.Name = "RichTextBox_Console";
            // 
            // Button_ChooseFile
            // 
            resources.ApplyResources(this.Button_ChooseFile, "Button_ChooseFile");
            this.Button_ChooseFile.Name = "Button_ChooseFile";
            this.Button_ChooseFile.UseVisualStyleBackColor = true;
            this.Button_ChooseFile.Click += new System.EventHandler(this.Button_ChooseFile_Click);
            // 
            // Button_Save
            // 
            resources.ApplyResources(this.Button_Save, "Button_Save");
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // Button_Load
            // 
            resources.ApplyResources(this.Button_Load, "Button_Load");
            this.Button_Load.Name = "Button_Load";
            this.Button_Load.UseVisualStyleBackColor = true;
            this.Button_Load.Click += new System.EventHandler(this.Button_Load_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // Button_Execute
            // 
            resources.ApplyResources(this.Button_Execute, "Button_Execute");
            this.Button_Execute.Name = "Button_Execute";
            this.Button_Execute.UseVisualStyleBackColor = true;
            this.Button_Execute.Click += new System.EventHandler(this.Button_Execute_Click);
            // 
            // ScriptsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.Button_Execute);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Button_Load);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.Button_ChooseFile);
            this.Name = "ScriptsForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GroupBox_Script.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.GroupBox_Debug.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_ChooseFile;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Button Button_Load;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Button_Execute;
        private System.Windows.Forms.GroupBox GroupBox_Script;
        private System.Windows.Forms.RichTextBox RichTextBox_Script;
        private System.Windows.Forms.GroupBox GroupBox_Debug;
        private System.Windows.Forms.RichTextBox RichTextBox_Debug;
        private System.Windows.Forms.Button Button_Debug_NextStep;
        private System.Windows.Forms.Button Button_Debug_Flip;
        private System.Windows.Forms.Button Button_Debug_Terminate;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox RichTextBox_Console;
    }
}