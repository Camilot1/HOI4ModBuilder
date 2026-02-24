namespace HOI4ModBuilder.src.forms.recoveryForms
{
    partial class StrategicRegionsDataRecoveryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StrategicRegionsDataRecoveryForm));
            this.Button_ChooseOldRegionsDirectory = new System.Windows.Forms.Button();
            this.Label_OldDirectoryPath = new System.Windows.Forms.Label();
            this.ProgressBar_LoadingProcess = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Label_FilteredOldRegionsThatDontExistCount = new System.Windows.Forms.Label();
            this.Label_FilteredOldRegionsCount = new System.Windows.Forms.Label();
            this.Button_FilterHelp = new System.Windows.Forms.Button();
            this.Button_FilterById = new System.Windows.Forms.Button();
            this.TextBox_IdsFilter = new System.Windows.Forms.TextBox();
            this.Label_FoundOldRegionsCount = new System.Windows.Forms.Label();
            this.Label_OldRegionsIds = new System.Windows.Forms.Label();
            this.Button_LoadOldRegionsDirectory = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ProgressBar_RecoveryProcess = new System.Windows.Forms.ProgressBar();
            this.Button_ExecuteRecovery = new System.Windows.Forms.Button();
            this.CheckBox_TransferFilesIfRegionIdNotFound = new System.Windows.Forms.CheckBox();
            this.CheckedListBox_ParamsToRecover = new System.Windows.Forms.CheckedListBox();
            this.TextBox_OldRegionsIds = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_ChooseOldRegionsDirectory
            // 
            resources.ApplyResources(this.Button_ChooseOldRegionsDirectory, "Button_ChooseOldRegionsDirectory");
            this.Button_ChooseOldRegionsDirectory.Name = "Button_ChooseOldRegionsDirectory";
            this.Button_ChooseOldRegionsDirectory.UseVisualStyleBackColor = true;
            this.Button_ChooseOldRegionsDirectory.Click += new System.EventHandler(this.Button_ChooseOldRegionsDirectory_Click);
            // 
            // Label_OldDirectoryPath
            // 
            resources.ApplyResources(this.Label_OldDirectoryPath, "Label_OldDirectoryPath");
            this.Label_OldDirectoryPath.Name = "Label_OldDirectoryPath";
            // 
            // ProgressBar_LoadingProcess
            // 
            resources.ApplyResources(this.ProgressBar_LoadingProcess, "ProgressBar_LoadingProcess");
            this.ProgressBar_LoadingProcess.Name = "ProgressBar_LoadingProcess";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.Label_FilteredOldRegionsThatDontExistCount);
            this.groupBox1.Controls.Add(this.Label_FilteredOldRegionsCount);
            this.groupBox1.Controls.Add(this.Button_FilterHelp);
            this.groupBox1.Controls.Add(this.Button_FilterById);
            this.groupBox1.Controls.Add(this.TextBox_IdsFilter);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // Label_FilteredOldRegionsThatDontExistCount
            // 
            resources.ApplyResources(this.Label_FilteredOldRegionsThatDontExistCount, "Label_FilteredOldRegionsThatDontExistCount");
            this.Label_FilteredOldRegionsThatDontExistCount.Name = "Label_FilteredOldRegionsThatDontExistCount";
            // 
            // Label_FilteredOldRegionsCount
            // 
            resources.ApplyResources(this.Label_FilteredOldRegionsCount, "Label_FilteredOldRegionsCount");
            this.Label_FilteredOldRegionsCount.Name = "Label_FilteredOldRegionsCount";
            // 
            // Button_FilterHelp
            // 
            resources.ApplyResources(this.Button_FilterHelp, "Button_FilterHelp");
            this.Button_FilterHelp.Name = "Button_FilterHelp";
            this.Button_FilterHelp.UseVisualStyleBackColor = true;
            this.Button_FilterHelp.Click += new System.EventHandler(this.Button_FilterHelp_Click);
            // 
            // Button_FilterById
            // 
            resources.ApplyResources(this.Button_FilterById, "Button_FilterById");
            this.Button_FilterById.Name = "Button_FilterById";
            this.Button_FilterById.UseVisualStyleBackColor = true;
            this.Button_FilterById.Click += new System.EventHandler(this.Button_FilterById_Click);
            // 
            // TextBox_IdsFilter
            // 
            resources.ApplyResources(this.TextBox_IdsFilter, "TextBox_IdsFilter");
            this.TextBox_IdsFilter.Name = "TextBox_IdsFilter";
            this.TextBox_IdsFilter.TextChanged += new System.EventHandler(this.TextBox_IdsFilter_TextChanged);
            // 
            // Label_FoundOldRegionsCount
            // 
            resources.ApplyResources(this.Label_FoundOldRegionsCount, "Label_FoundOldRegionsCount");
            this.Label_FoundOldRegionsCount.Name = "Label_FoundOldRegionsCount";
            // 
            // Label_OldRegionsIds
            // 
            resources.ApplyResources(this.Label_OldRegionsIds, "Label_OldRegionsIds");
            this.Label_OldRegionsIds.Name = "Label_OldRegionsIds";
            // 
            // Button_LoadOldRegionsDirectory
            // 
            resources.ApplyResources(this.Button_LoadOldRegionsDirectory, "Button_LoadOldRegionsDirectory");
            this.Button_LoadOldRegionsDirectory.Name = "Button_LoadOldRegionsDirectory";
            this.Button_LoadOldRegionsDirectory.UseVisualStyleBackColor = true;
            this.Button_LoadOldRegionsDirectory.Click += new System.EventHandler(this.Button_LoadOldRegionsDirectory_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.ProgressBar_RecoveryProcess);
            this.groupBox2.Controls.Add(this.Button_ExecuteRecovery);
            this.groupBox2.Controls.Add(this.CheckBox_TransferFilesIfRegionIdNotFound);
            this.groupBox2.Controls.Add(this.CheckedListBox_ParamsToRecover);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // ProgressBar_RecoveryProcess
            // 
            resources.ApplyResources(this.ProgressBar_RecoveryProcess, "ProgressBar_RecoveryProcess");
            this.ProgressBar_RecoveryProcess.Name = "ProgressBar_RecoveryProcess";
            // 
            // Button_ExecuteRecovery
            // 
            resources.ApplyResources(this.Button_ExecuteRecovery, "Button_ExecuteRecovery");
            this.Button_ExecuteRecovery.Name = "Button_ExecuteRecovery";
            this.Button_ExecuteRecovery.UseVisualStyleBackColor = true;
            this.Button_ExecuteRecovery.Click += new System.EventHandler(this.Button_ExecuteRecovery_Click);
            // 
            // CheckBox_TransferFilesIfRegionIdNotFound
            // 
            resources.ApplyResources(this.CheckBox_TransferFilesIfRegionIdNotFound, "CheckBox_TransferFilesIfRegionIdNotFound");
            this.CheckBox_TransferFilesIfRegionIdNotFound.Name = "CheckBox_TransferFilesIfRegionIdNotFound";
            this.CheckBox_TransferFilesIfRegionIdNotFound.UseVisualStyleBackColor = true;
            this.CheckBox_TransferFilesIfRegionIdNotFound.Click += new System.EventHandler(this.CheckBox_TransferFilesIfRegionIdNotFound_Click);
            // 
            // CheckedListBox_ParamsToRecover
            // 
            this.CheckedListBox_ParamsToRecover.FormattingEnabled = true;
            this.CheckedListBox_ParamsToRecover.Items.AddRange(new object[] {
            resources.GetString("CheckedListBox_ParamsToRecover.Items"),
            resources.GetString("CheckedListBox_ParamsToRecover.Items1"),
            resources.GetString("CheckedListBox_ParamsToRecover.Items2"),
            resources.GetString("CheckedListBox_ParamsToRecover.Items3"),
            resources.GetString("CheckedListBox_ParamsToRecover.Items4")});
            resources.ApplyResources(this.CheckedListBox_ParamsToRecover, "CheckedListBox_ParamsToRecover");
            this.CheckedListBox_ParamsToRecover.Name = "CheckedListBox_ParamsToRecover";
            this.CheckedListBox_ParamsToRecover.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CheckedListBox_ParamsToRecover_MouseUp);
            // 
            // TextBox_OldRegionsIds
            // 
            resources.ApplyResources(this.TextBox_OldRegionsIds, "TextBox_OldRegionsIds");
            this.TextBox_OldRegionsIds.Name = "TextBox_OldRegionsIds";
            // 
            // StrategicRegionsDataRecoveryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextBox_OldRegionsIds);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Button_LoadOldRegionsDirectory);
            this.Controls.Add(this.Label_OldRegionsIds);
            this.Controls.Add(this.Label_FoundOldRegionsCount);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ProgressBar_LoadingProcess);
            this.Controls.Add(this.Label_OldDirectoryPath);
            this.Controls.Add(this.Button_ChooseOldRegionsDirectory);
            this.Name = "StrategicRegionsDataRecoveryForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_ChooseOldRegionsDirectory;
        private System.Windows.Forms.Label Label_OldDirectoryPath;
        private System.Windows.Forms.ProgressBar ProgressBar_LoadingProcess;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Label_FilteredOldRegionsCount;
        private System.Windows.Forms.Button Button_FilterHelp;
        private System.Windows.Forms.Button Button_FilterById;
        private System.Windows.Forms.TextBox TextBox_IdsFilter;
        private System.Windows.Forms.Label Label_FoundOldRegionsCount;
        private System.Windows.Forms.Label Label_OldRegionsIds;
        private System.Windows.Forms.Button Button_LoadOldRegionsDirectory;
        private System.Windows.Forms.Label Label_FilteredOldRegionsThatDontExistCount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ProgressBar ProgressBar_RecoveryProcess;
        private System.Windows.Forms.Button Button_ExecuteRecovery;
        private System.Windows.Forms.CheckBox CheckBox_TransferFilesIfRegionIdNotFound;
        private System.Windows.Forms.CheckedListBox CheckedListBox_ParamsToRecover;
        private System.Windows.Forms.TextBox TextBox_OldRegionsIds;
    }
}