using System;
using System.Collections.Generic;
using System.IO;

namespace PES5_WE9_LE_GDB_Manager
{
    public class Executable
    {
        private string FileLocation;
        public byte[] Data;
        GameVersion GameVersion;
        public uint NamesOffset;
        public uint BaseAddress;
        public uint NationalityOffset;
        public static readonly uint NameRecordSize = 16;
        public static readonly uint NationalityRecordSize = 8;
        public List<Nationality> Nationalities = new List<Nationality>();
        private readonly uint[] Offsets = new uint[3];
        private readonly uint[][] GamesOffsets = new uint[][]
{
        new uint[] { 0x400000, 0x6e2a70, 0x70a630},
        new uint[] { 0x401a00, 0x6e07a8, 0x707068},
};
        public Executable(string fileLocation) 
        {
            FileLocation = fileLocation;
            ReadExecutable();
            ReadNationalities();
        }

        private void ReadNationalities()
        {
            Nationalities.Clear();
            for (uint i = 0; i < Nationality.TotalNationalities; i++)
            {
                Nationalities.Add(new Nationality(this, i));
            }
        }

        private void ReadExecutable()
        {
            Data = File.ReadAllBytes(FileLocation);
            GameVersion = GetGameVersion();

            Array.Copy(GamesOffsets[(int)GameVersion], Offsets, GamesOffsets[(int)GameVersion].Length);

            NamesOffset = Offsets[(int)ExecutableConstants.NAMES_OFFSETS];
            BaseAddress = Offsets[(int)ExecutableConstants.BASE_ADDRESS];
            NationalityOffset = Offsets[(int)ExecutableConstants.NATIONALITIES_OFFSETS];
        }

        private GameVersion GetGameVersion()
        {
            switch (Data.Length)
            {
                case 10174464:
                    return GameVersion.PES5WE9;
                case 22793412:
                    return GameVersion.WE9LEK;
                default:
                    throw new Exception($"Invalid game executable with size {Data.Length}");
            }
        }
    }

    enum ExecutableConstants
    {
        BASE_ADDRESS,
        NAMES_OFFSETS,
        NATIONALITIES_OFFSETS,
    }
    enum GameVersion
    {
        PES5WE9,
        WE9LEK
    }
}
