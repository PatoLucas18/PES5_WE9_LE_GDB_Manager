using System;
using System.Text;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Player
    {
        public uint Id;
        public string Name = "";
        public string FacePath = "";
        public string HairPath = "";
        public string BootPath = "";
        public string SkinPath = "";
        public string GlovesPath = "";
        public string GKLeftGlovePath = "";
        public string GKRightGlovePath = "";
        public string CallnameFolder = "";
        public OptionFile optionFile;
        public static readonly uint RecordSize = 124;
        private static readonly uint BaseOffset = 37116;
        private static readonly uint EditedBaseOffset = 14288;
        public static readonly uint NameSize = 32;
        public static readonly uint ShirtNameSize = 16;
        public static readonly uint FirstEditedIdx = 32768;
        public static readonly uint FirstUnusedIdx = 4896;
        public static readonly uint TotalPlayers = 5000;
        public static readonly uint TotalEditedPlayers = 184;
        public Stat Nationality;
        public bool IsFree = true;
        public Player(OptionFile of, uint id) 
        {
            optionFile = of;
            Id = id;
            GetName();
            Nationality = new Stat(this, 65, 0, 0x7F);
        }
        private bool IsEdited()
        {
            return Id >= FirstEditedIdx;
        }
        public uint GetOffset()
        {
            return IsEdited() ? EditedBaseOffset + (Id - FirstEditedIdx) * RecordSize : BaseOffset + Id * RecordSize;
        }
        private void GetName()
        {
            uint offset = GetOffset();
            byte[] nameBytes = new byte[NameSize];
            Array.Copy(optionFile.Data, offset, nameBytes, 0, NameSize);
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
            Name = Encoding.Unicode.GetString(nameBytes,0, len);
            if (Name.Length == 0)
            {
                string prefix = GetNamePrefixById();
                Name = $"<{prefix} {Id}>";
            }
        }
        private string GetNamePrefixById()
        {
            string prefix = "Player ID";
            if (Id >= FirstEditedIdx) 
            {
                prefix = "Edited";
            }
            else if (Id >= FirstUnusedIdx)
            {
                prefix = "Unused";
            }

            return prefix;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
