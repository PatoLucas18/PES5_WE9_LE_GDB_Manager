using System;
using System.Collections.Generic;
using System.Text;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Team
    {
        public uint Id;
        public string Name;
        private readonly uint OfBaseOffset = 751472;
        private readonly uint OfPlayersInTeamOffset = 664372;
        public static readonly uint TotalNations = 64;
        public static readonly uint TotalClassicTeams = 7;
        public static readonly uint TotalClubs = 140;
        public static readonly uint ExtraTeams = 17;
        public static readonly uint TotalTeams = TotalNations + TotalClubs + ExtraTeams;
        private readonly uint OfNameRecordSize = 49;
        public uint PlayersInTeam 
        {
            get 
            { 
                return (uint)(IsNation() ? 23 : 32);
            }
        }
        private readonly uint OfRecordSize = 88;
        public List<Player> Players = new List<Player>();
        public string KitsPath = "";
        public Ball HomeBall;
        public string HomeStadium = "";
        public string ChantsFolder = "";
        public string BannerFolder = "";
        public SupporterColour supporterColour;
        public string FlagPath = "";
        public string SmallFlagPath = "";
        public string SupporterFlagPath = "";
        public string CallnameNormal = "";
        public string CallnameVs = "";
        public string CallnameLoud = "";
        

        public Team(Executable executable, uint id)
        {
            Id = id;
            Name = GetNameFromExe(executable);
        }
        public Team(OptionFile of, uint id)
        {
            Id = id;
            Name = GetNameFromOF(of);
        }
        public bool IsNation()
        {
            return Id < TotalNations;
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
            uint offset = executable.NamesOffset + Id * Executable.NameRecordSize;
            uint nameOffset = BitConverter.ToUInt32(executable.Data, (int)offset);
            return nameOffset - executable.BaseAddress;
        }
        private uint GetOFOffset()
        {
            return OfBaseOffset + (Id - TotalNations) * OfRecordSize;
        }
        private string GetNameFromOF(OptionFile of)
        {
            uint offset = GetOFOffset();

            string name = Utils.BytesUTF8ToString(of.Data, offset, OfNameRecordSize);
            if (string.IsNullOrEmpty(name))
            {
                name = Utils.BytesUTF8ToString(of.Data, offset + OfNameRecordSize, 0x18);
            }
            return name;
        }
        public uint TeamPlayersOffset()
        {
            uint shift = IsNation() ? 0 : 46 + Player.TotalEditedPlayers * 2;
            uint baseOffset = IsNation() ? OfPlayersInTeamOffset : OfPlayersInTeamOffset + TotalNations * 23 * 2;
            uint id = IsNation() ? Id : Id - TotalNations;
            uint offset = baseOffset + id * PlayersInTeam * 2 + shift;
            if (Id == TotalTeams - 1) 
            {
                return baseOffset + TotalClubs * PlayersInTeam * 2 + shift;
            }
            if (Id > TotalNations + TotalClubs - 1) return 0xffffffff;
            return offset;
        }
        public override string ToString()
        {
            return Name;
        }

    }
}
