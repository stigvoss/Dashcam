namespace Agent
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
            this.DashcamLoaderAgentProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DashcamLoaderAgentInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // DashcamLoaderAgentProcessInstaller
            // 
            this.DashcamLoaderAgentProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DashcamLoaderAgentProcessInstaller.Password = null;
            this.DashcamLoaderAgentProcessInstaller.Username = null;
            // 
            // DashcamLoaderAgentInstaller
            // 
            this.DashcamLoaderAgentInstaller.Description = "Autoload dashcam application when detected on a USB drive.";
            this.DashcamLoaderAgentInstaller.DisplayName = "Dashcam Agent";
            this.DashcamLoaderAgentInstaller.ServiceName = "DashcamLoaderService";
            this.DashcamLoaderAgentInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DashcamLoaderAgentProcessInstaller,
            this.DashcamLoaderAgentInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DashcamLoaderAgentProcessInstaller;
        private System.ServiceProcess.ServiceInstaller DashcamLoaderAgentInstaller;
    }
}