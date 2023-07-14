using System;
using System.Windows.Forms;

namespace PES5_WE9_LE_GDB_Manager
{
    public static class Utils
    {
        public static uint ReadUInt32FromByteArray(byte[] data, uint startIndex)
        {
            uint value = (uint)(data[startIndex] | (data[startIndex + 1] << 8) | (data[startIndex + 2] << 16) | (data[startIndex + 3] << 24));
            return value;
        }
        public static uint ZeroFillRightShift(uint val, int n)
        {
            return (uint)((val % 0x100000000) >> n);
        }

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
