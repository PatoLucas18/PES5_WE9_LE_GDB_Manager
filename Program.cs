using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PES5_WE9_LE_GDB_Manager
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Utils.CheckForUpdates();

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PES5_WE9_LE_GDB_Manager.config");
            if (File.Exists(configPath))
            {
                GDBManagerConfig config = Utils.LoadConfiguration(configPath);
                if (File.Exists(config.ExePath) && File.Exists(config.OFPath) && Directory.Exists(config.GDBFolderPath))
                {
                    frmMain mainForm = new frmMain();
                    mainForm.config = config;
                    Application.Run(mainForm);
                }
            }
            else
            {
                frmConfig configForm = new frmConfig();
                DialogResult result = configForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    frmMain mainForm = new frmMain();
                    mainForm.config = configForm.config;
                    Application.Run(mainForm);
                }
            }
        }
    }
}
