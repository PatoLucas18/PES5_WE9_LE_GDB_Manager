using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Player
    {
        public uint idx;
        public string name = "";
        public uint nationalTeamId;
        public uint clubTeamId;
        public string facePath = "";
        public string hairPath = "";
        public string bootPath = "";
        public string skinPath = "";
        public string glovesPath = "";
        private OptionFile optionFile;
        private static readonly uint recordSize = 124;
        private static readonly uint baseOffset = 36872;
        private static readonly uint editedBaseOffset = 14044;
        private static readonly uint nameSize = 32;
        private static readonly uint firstEditedIdx = 32768;

        public Player(OptionFile of, uint id) 
        {
            optionFile = of;
            idx = id;
            GetName();
        }
        private bool IsEdited()
        {
            return idx >= firstEditedIdx;
        }
        private uint GetOffset()
        {
            return IsEdited() ? editedBaseOffset + (idx - firstEditedIdx) * recordSize : baseOffset + idx * recordSize;
        }
        private void GetName()
        {
            uint offset = GetOffset();
            byte[] nameBytes = new byte[nameSize];
            Array.Copy(optionFile.data, offset, nameBytes, 0, nameSize);
            bool end = false;
            int len = 0;
            for (int j = 0; !end && j < nameBytes.Length - 1; j = j + 2)
            {
                if (nameBytes[j] == 0 && nameBytes[j + 1] == 0)
                {
                    end = true;
                    len = j;
                }
            }
            name = Encoding.Unicode.GetString(nameBytes,0, len);
            if (name.Length == 0)
            {
                name = $"<Player ID {idx}>";
            }
        }
    }
}
