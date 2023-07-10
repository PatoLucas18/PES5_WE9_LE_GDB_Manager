using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PES5_WE9_LE_GDB_Manager
{
    public static class Utils
    {
        public static string GetFilename()
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            Console.WriteLine(fileName);
            return fileName;
        }

        public static string GetFolder()
        {
            // Crear una instancia del cuadro de diálogo para seleccionar carpeta
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            string selectedPath = "";
            // Verificar si se seleccionó una carpeta
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Obtener la ruta de la carpeta seleccionada
                selectedPath = folderBrowserDialog.SelectedPath;
            }
            Console.WriteLine(selectedPath);
            return selectedPath;
        }
    }
}
