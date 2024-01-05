namespace HOI4ModBuilder.gui
{
    partial class GuiResource
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_Remove = new System.Windows.Forms.Button();
            this.button_AddAfter = new System.Windows.Forms.Button();
            this.button_MoveDown = new System.Windows.Forms.Button();
            this.button_MoveUp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_FactoryPerUnit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_ConvoysPerUnit = new System.Windows.Forms.TextBox();
            this.textBox_Tag = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_IconFrame = new System.Windows.Forms.ComboBox();
            this.textBox_Localization = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button_Remove
            // 
            this.button_Remove.Location = new System.Drawing.Point(474, 5);
            this.button_Remove.Name = "button_Remove";
            this.button_Remove.Size = new System.Drawing.Size(35, 23);
            this.button_Remove.TabIndex = 1;
            this.button_Remove.Text = "X";
            this.button_Remove.UseVisualStyleBackColor = true;
            this.button_Remove.Click += new System.EventHandler(this.button_Remove_Click);
            // 
            // button_AddAfter
            // 
            this.button_AddAfter.Location = new System.Drawing.Point(438, 5);
            this.button_AddAfter.Name = "button_AddAfter";
            this.button_AddAfter.Size = new System.Drawing.Size(35, 23);
            this.button_AddAfter.TabIndex = 2;
            this.button_AddAfter.Text = "+";
            this.button_AddAfter.UseVisualStyleBackColor = true;
            this.button_AddAfter.Click += new System.EventHandler(this.button_AddAfter_Click);
            // 
            // button_MoveDown
            // 
            this.button_MoveDown.Location = new System.Drawing.Point(474, 28);
            this.button_MoveDown.Name = "button_MoveDown";
            this.button_MoveDown.Size = new System.Drawing.Size(35, 23);
            this.button_MoveDown.TabIndex = 4;
            this.button_MoveDown.Text = "↓";
            this.button_MoveDown.UseVisualStyleBackColor = true;
            this.button_MoveDown.Click += new System.EventHandler(this.button_MoveDown_Click);
            // 
            // button_MoveUp
            // 
            this.button_MoveUp.Location = new System.Drawing.Point(438, 28);
            this.button_MoveUp.Name = "button_MoveUp";
            this.button_MoveUp.Size = new System.Drawing.Size(35, 23);
            this.button_MoveUp.TabIndex = 3;
            this.button_MoveUp.Text = "↑";
            this.button_MoveUp.UseVisualStyleBackColor = true;
            this.button_MoveUp.Click += new System.EventHandler(this.button_MoveUp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Цена (фабрик):";
            // 
            // textBox_FactoryPerUnit
            // 
            this.textBox_FactoryPerUnit.Location = new System.Drawing.Point(255, 30);
            this.textBox_FactoryPerUnit.Name = "textBox_FactoryPerUnit";
            this.textBox_FactoryPerUnit.Size = new System.Drawing.Size(60, 20);
            this.textBox_FactoryPerUnit.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Конвоев:";
            // 
            // textBox_ConvoysPerUnit
            // 
            this.textBox_ConvoysPerUnit.Location = new System.Drawing.Point(375, 30);
            this.textBox_ConvoysPerUnit.Name = "textBox_ConvoysPerUnit";
            this.textBox_ConvoysPerUnit.Size = new System.Drawing.Size(60, 20);
            this.textBox_ConvoysPerUnit.TabIndex = 10;
            // 
            // textBox_Tag
            // 
            this.textBox_Tag.Location = new System.Drawing.Point(87, 6);
            this.textBox_Tag.Name = "textBox_Tag";
            this.textBox_Tag.Size = new System.Drawing.Size(86, 20);
            this.textBox_Tag.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Тег:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(175, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Локализация:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Иконка:";
            // 
            // comboBox_IconFrame
            // 
            this.comboBox_IconFrame.FormattingEnabled = true;
            this.comboBox_IconFrame.Location = new System.Drawing.Point(103, 30);
            this.comboBox_IconFrame.Name = "comboBox_IconFrame";
            this.comboBox_IconFrame.Size = new System.Drawing.Size(70, 21);
            this.comboBox_IconFrame.TabIndex = 15;
            // 
            // textBox_Localization
            // 
            this.textBox_Localization.Location = new System.Drawing.Point(255, 6);
            this.textBox_Localization.Name = "textBox_Localization";
            this.textBox_Localization.Size = new System.Drawing.Size(180, 20);
            this.textBox_Localization.TabIndex = 16;
            // 
            // GuiResource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.textBox_Localization);
            this.Controls.Add(this.comboBox_IconFrame);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_Tag);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_ConvoysPerUnit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_FactoryPerUnit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_MoveDown);
            this.Controls.Add(this.button_MoveUp);
            this.Controls.Add(this.button_AddAfter);
            this.Controls.Add(this.button_Remove);
            this.Controls.Add(this.pictureBox1);
            this.Name = "GuiResource";
            this.Size = new System.Drawing.Size(512, 54);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button_Remove;
        private System.Windows.Forms.Button button_AddAfter;
        private System.Windows.Forms.Button button_MoveDown;
        private System.Windows.Forms.Button button_MoveUp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_FactoryPerUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ConvoysPerUnit;
        private System.Windows.Forms.TextBox textBox_Tag;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_IconFrame;
        private System.Windows.Forms.TextBox textBox_Localization;
    }
}
