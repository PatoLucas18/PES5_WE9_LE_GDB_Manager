using System;
using System.Collections.Generic;
using System.IO;
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
            string optionFileLocation = "C:\\Users\\marco\\Documents\\KONAMI\\Pro Evolution Soccer 5\\save\\folder1\\KONAMI-WIN32PES5OPT";
            OptionFile optionFile = new OptionFile(optionFileLocation);
            List<Player> players = new List<Player>();
            for (uint i = 1; i < 5000; i++) 
            {
                players.Add(new Player(optionFile, i));
            }
            for (uint i = 32768; i < 32768 + 184; i++)
            {
                players.Add(new Player(optionFile, i));
            }
            foreach (Player player in players)
            {
                Console.WriteLine(player.name);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
