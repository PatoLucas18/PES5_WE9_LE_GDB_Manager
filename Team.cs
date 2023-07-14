using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Team
    {
        public int idx;
        public string name;
        private readonly uint exeBaseOffset = 0;
        private readonly uint ofBaseOffset = 0;
        public Team(int id, string exeFilePath)
        {
            idx = id;
            name = GetNameFromExe(exeFilePath);
        }
        public Team(int id, OptionFile of)
        {
            idx = id;
            name = GetNameFromOF(of);
        }
        private string GetNameFromExe(string exeFilePath)
        {
            return null;
        }
        private string GetNameFromOF(OptionFile of)
        {
            return null;
        }
    }
}
