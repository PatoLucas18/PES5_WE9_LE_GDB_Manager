using System;
using System.Text;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Nationality
    {
        public uint Id;
        public string Name;
        public static readonly int TotalNationalities = 111;
        public Nationality(Executable executable, uint id)
        {
            Id = id;
            Name = GetNameFromExe(executable);
        }

        private string GetNameFromExe(Executable executable)
        {
            uint offset = GetExeOffset(executable);
            int endIndex = Array.IndexOf(executable.Data, (byte)0x00, (int)offset);

            if (endIndex == -1)
            {
                throw new ArgumentException($"Unable to read name for team id {Id} from executable.");
            }

            int length = endIndex - (int)offset;
            return Encoding.UTF8.GetString(executable.Data, (int)offset, length);
        }

        private uint GetExeOffset(Executable executable)
        {
            uint offset = executable.NationalityOffset + Id * Executable.NationalityRecordSize;
            uint nameOffset = BitConverter.ToUInt32(executable.Data, (int)offset);
            return nameOffset - executable.BaseAddress;
        }
        public override string ToString()
        {
            return Name;
        }

    }
}
