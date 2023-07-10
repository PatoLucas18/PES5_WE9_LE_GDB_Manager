using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace PES5_WE9_LE_GDB_Manager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario secundario
            ConfigForm newForm = new ConfigForm();

            // Mostrar el formulario secundario
            newForm.Show();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void teamKitsSelectButton_Click(object sender, EventArgs e)
        {
            Utils.GetFolder();
        }
    }
}
