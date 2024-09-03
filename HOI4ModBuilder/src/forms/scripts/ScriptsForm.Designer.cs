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
            this.Button_ChooseFile = new System.Windows.Forms.Button();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Button_Load = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Button_Execute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RichTextBox_Script = new System.Windows.Forms.RichTextBox();
            this.GroupBox_Debug = new System.Windows.Forms.GroupBox();
            this.Button_Debug_Terminate = new System.Windows.Forms.Button();
            this.RichTextBox_Debug = new System.Windows.Forms.RichTextBox();
            this.Button_Debug_NextStep = new System.Windows.Forms.Button();
            this.Button_Debug_Flip = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1.SuspendLayout();
            this.GroupBox_Debug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_ChooseFile
            // 
            this.Button_ChooseFile.Location = new System.Drawing.Point(5, 5);
            this.Button_ChooseFile.Name = "Button_ChooseFile";
            this.Button_ChooseFile.Size = new System.Drawing.Size(90, 23);
            this.Button_ChooseFile.TabIndex = 0;
            this.Button_ChooseFile.Text = "Выбрать файл";
            this.Button_ChooseFile.UseVisualStyleBackColor = true;
            this.Button_ChooseFile.Click += new System.EventHandler(this.Button_ChooseFile_Click);
            // 
            // Button_Save
            // 
            this.Button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Save.Location = new System.Drawing.Point(522, 5);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(90, 23);
            this.Button_Save.TabIndex = 1;
            this.Button_Save.Text = "Сохранить";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // Button_Load
            // 
            this.Button_Load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Load.Location = new System.Drawing.Point(618, 5);
            this.Button_Load.Name = "Button_Load";
            this.Button_Load.Size = new System.Drawing.Size(90, 23);
            this.Button_Load.TabIndex = 2;
            this.Button_Load.Text = "Загрузить";
            this.Button_Load.UseVisualStyleBackColor = true;
            this.Button_Load.Click += new System.EventHandler(this.Button_Load_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(101, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(415, 20);
            this.textBox1.TabIndex = 3;
            // 
            // Button_Execute
            // 
            this.Button_Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Execute.Location = new System.Drawing.Point(714, 5);
            this.Button_Execute.Name = "Button_Execute";
            this.Button_Execute.Size = new System.Drawing.Size(90, 23);
            this.Button_Execute.TabIndex = 4;
            this.Button_Execute.Text = "Выполнить";
            this.Button_Execute.UseVisualStyleBackColor = true;
            this.Button_Execute.Click += new System.EventHandler(this.Button_Execute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.RichTextBox_Script);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 522);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Файл не выбран";
            // 
            // RichTextBox_Script
            // 
            this.RichTextBox_Script.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RichTextBox_Script.Location = new System.Drawing.Point(7, 19);
            this.RichTextBox_Script.Name = "RichTextBox_Script";
            this.RichTextBox_Script.Size = new System.Drawing.Size(369, 497);
            this.RichTextBox_Script.TabIndex = 0;
            this.RichTextBox_Script.Text = "";
            this.RichTextBox_Script.TextChanged += new System.EventHandler(this.RichTextBox_Script_TextChanged);
            // 
            // GroupBox_Debug
            // 
            this.GroupBox_Debug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_Terminate);
            this.GroupBox_Debug.Controls.Add(this.RichTextBox_Debug);
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_NextStep);
            this.GroupBox_Debug.Controls.Add(this.Button_Debug_Flip);
            this.GroupBox_Debug.Location = new System.Drawing.Point(3, 3);
            this.GroupBox_Debug.Name = "GroupBox_Debug";
            this.GroupBox_Debug.Size = new System.Drawing.Size(401, 522);
            this.GroupBox_Debug.TabIndex = 6;
            this.GroupBox_Debug.TabStop = false;
            this.GroupBox_Debug.Text = "Дебаг: ";
            // 
            // Button_Debug_Terminate
            // 
            this.Button_Debug_Terminate.Location = new System.Drawing.Point(192, 19);
            this.Button_Debug_Terminate.Name = "Button_Debug_Terminate";
            this.Button_Debug_Terminate.Size = new System.Drawing.Size(90, 23);
            this.Button_Debug_Terminate.TabIndex = 3;
            this.Button_Debug_Terminate.Text = "Остановить";
            this.Button_Debug_Terminate.UseVisualStyleBackColor = true;
            this.Button_Debug_Terminate.Click += new System.EventHandler(this.Button_Debug_Terminate_Click);
            // 
            // RichTextBox_Debug
            // 
            this.RichTextBox_Debug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RichTextBox_Debug.Location = new System.Drawing.Point(6, 48);
            this.RichTextBox_Debug.Name = "RichTextBox_Debug";
            this.RichTextBox_Debug.Size = new System.Drawing.Size(389, 468);
            this.RichTextBox_Debug.TabIndex = 2;
            this.RichTextBox_Debug.Text = "";
            // 
            // Button_Debug_NextStep
            // 
            this.Button_Debug_NextStep.Location = new System.Drawing.Point(96, 19);
            this.Button_Debug_NextStep.Name = "Button_Debug_NextStep";
            this.Button_Debug_NextStep.Size = new System.Drawing.Size(90, 23);
            this.Button_Debug_NextStep.TabIndex = 1;
            this.Button_Debug_NextStep.Text = "След. шаг";
            this.Button_Debug_NextStep.UseVisualStyleBackColor = true;
            this.Button_Debug_NextStep.Click += new System.EventHandler(this.Button_Debug_NextStep_Click);
            // 
            // Button_Debug_Flip
            // 
            this.Button_Debug_Flip.Location = new System.Drawing.Point(0, 19);
            this.Button_Debug_Flip.Name = "Button_Debug_Flip";
            this.Button_Debug_Flip.Size = new System.Drawing.Size(90, 23);
            this.Button_Debug_Flip.TabIndex = 0;
            this.Button_Debug_Flip.Text = "Переключить";
            this.Button_Debug_Flip.UseVisualStyleBackColor = true;
            this.Button_Debug_Flip.Click += new System.EventHandler(this.Button_Debug_Flip_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(5, 34);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.GroupBox_Debug);
            this.splitContainer1.Size = new System.Drawing.Size(799, 528);
            this.splitContainer1.SplitterDistance = 388;
            this.splitContainer1.TabIndex = 7;
            // 
            // ScriptsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 565);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.Button_Execute);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Button_Load);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.Button_ChooseFile);
            this.MinimumSize = new System.Drawing.Size(595, 450);
            this.Name = "ScriptsForm";
            this.Text = "Скрипты";
            this.groupBox1.ResumeLayout(false);
            this.GroupBox_Debug.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_ChooseFile;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Button Button_Load;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Button_Execute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox RichTextBox_Script;
        private System.Windows.Forms.GroupBox GroupBox_Debug;
        private System.Windows.Forms.RichTextBox RichTextBox_Debug;
        private System.Windows.Forms.Button Button_Debug_NextStep;
        private System.Windows.Forms.Button Button_Debug_Flip;
        private System.Windows.Forms.Button Button_Debug_Terminate;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}