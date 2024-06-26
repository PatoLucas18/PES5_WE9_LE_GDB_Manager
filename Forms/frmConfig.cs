using System;
using System.Windows.Forms;

namespace PES5_WE9_LE_GDB_Manager
{
    public partial class frmConfig : Form
    {
        public GDBManagerConfig config;
        public frmConfig()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnAccept_Click(object sender, EventArgs e)
        {
            string ofPath = txtOFPath.Text;
            string exePath = txtExePath.Text;
            string gdbFolderPath = txtGDBFolder.Text;
            if (string.IsNullOrEmpty(ofPath) || string.IsNullOrEmpty(exePath) || string.IsNullOrEmpty(gdbFolderPath))
            {
                Utils.ShowError("All items need to be selected before continue");
                return;
            }
            config = new GDBManagerConfig();
            config.ExePath = exePath;
            config.GDBFolderPath = gdbFolderPath;
            config.OFPath = ofPath;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOFPath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Option File Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                txtOFPath.Text = ofd.FileName;

            }
        }

        private void btnExePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Executable Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                txtExePath.Text = ofd.FileName;
            }
        }

        private void btnGDBFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK) return;

                txtGDBFolder.Text = fbd.SelectedPath;
            }
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.Cancel;
            if (config == null) return;
            txtExePath.Text = config.ExePath;
            txtOFPath.Text = config.OFPath;
            txtGDBFolder.Text = config.GDBFolderPath;
        }
    }
}
