namespace Wfm.WindowsService
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scheduleTaskServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.scheduleTaskServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // scheduleTaskServiceProcessInstaller
            // 
            this.scheduleTaskServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.scheduleTaskServiceProcessInstaller.Password = null;
            this.scheduleTaskServiceProcessInstaller.Username = null;
            // 
            // scheduleTaskServiceInstaller
            // 
            this.scheduleTaskServiceInstaller.Description = "Wfm Scheduled Task Service";
            this.scheduleTaskServiceInstaller.DisplayName = "Wfm Scheduled Task Service";
            this.scheduleTaskServiceInstaller.ServiceName = "Wfm Scheduled Task Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.scheduleTaskServiceProcessInstaller,
            this.scheduleTaskServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller scheduleTaskServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller scheduleTaskServiceInstaller;
    }
}