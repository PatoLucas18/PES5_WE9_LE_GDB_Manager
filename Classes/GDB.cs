using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PES5_WE9_LE_GDB_Manager
{
    public class GDB
    {
        private OptionFile optionFile;
        private Executable executable;
        private string GDBFolderPath;
        public List<Team> teams = new List<Team>();
        public List<string> stadiums = new List<string>();
        public List<Ball> balls = new List<Ball>();
        private List<string> extraFlagMapLines = new List<string>();
        private const string MAP_HEADER = "# Map created with PES5/WE9/LE GDB Manager by marqisspes5";
        public GDB(OptionFile of, Executable exe, string GDBFolder) 
        {
            optionFile = of;
            executable = exe;
            GDBFolderPath = GDBFolder;
        }

        public string GetGDBPath(string path)
        {
            return Path.Combine(GDBFolderPath, path);
        }
        public void LoadTeams()
        {
            teams.Clear();
            for (uint i = 0; i < Team.TotalNations + Team.TotalClubs + Team.ExtraTeams; i++)
            {
                if (Team.TotalNations <= i && i < Team.TotalClubs)
                {
                    teams.Add(new Team(optionFile, i));
                }
                else
                {
                    teams.Add(new Team(executable, i));
                }
            }
        }
        public void LoadStadiums()
        {
            string stadiumsPath = GetGDBPath("stadiums\\");
            if (!Directory.Exists(stadiumsPath)) return;

            string[] stadiumsInFolder = Directory.GetDirectories(stadiumsPath);
            for (int i = 0; i < stadiumsInFolder.Length; i++)
            {
                string stadiumRelativePath = Utils.GetRelativePath(stadiumsInFolder[i], stadiumsPath);
                stadiums.Add(stadiumRelativePath);
            }
        }
        public void LoadAllMaps()
        {
            ReadFaceMap();
            ReadHairMap();
            ReadBootMap();
            ReadSkinMap();
            ReadGlovesMap();
            ReadUniMap();
            ReadStadiumMap();
            ReadBallsMap();
            ReadHomeBallMap();
            ReadChantsMap();
            ReadBannersMap();
            ReadFlagsMap();
            ReadSupporterFlagsMap();
            ReadTeamCallnamesMap();
        }
        private void ReadMap(string mapFileName, Action<Player, string[]> updatePlayerAction, int expectedPartsLength)
        {
            string mapFilePath = GetGDBPath(mapFileName);
            if (!File.Exists(mapFilePath)) return;

            string[] lines = File.ReadAllLines(mapFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new[] { ',' }, expectedPartsLength);
                if (parts.Length < 2)
                    continue;

                uint id;
                if (uint.TryParse(parts[0], out id))
                {
                    Player player = optionFile.Players.FirstOrDefault(p => p.Id == id);
                    if (player != null)
                    {
                        updatePlayerAction(player, parts);
                    }
                }
            }
        }
        private void ReadMap(string mapFileName, Action<Team, string[]> updateAction, int expectedPartsLength, int minimumParts)
        {
            string mapFilePath = GetGDBPath(mapFileName);
            if (!File.Exists(mapFilePath)) return;

            string[] lines = File.ReadAllLines(mapFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new[] { ',' }, expectedPartsLength);
                if (parts.Length < minimumParts)
                    continue;

                if (uint.TryParse(parts[0], out uint id))
                {
                    Team team = teams.FirstOrDefault(t => t.Id == id);
                    if (team != null)
                    {
                        updateAction(team, parts);
                    }
                }
            }
        }
        private void ReadFaceMap()
        {
            ReadMap("faces/map.txt", (player, parts) => player.FacePath = Utils.GetStringInsideQuotes(parts[1]), 2);
        }
        private void ReadHairMap()
        {
            ReadMap("hair/map.txt", (player, parts) => player.HairPath = Utils.GetStringInsideQuotes(parts[1]), 2);
        }
        private void ReadBootMap()
        {
            ReadMap("boots/map.txt", (player, parts) => player.BootPath = Utils.GetStringInsideQuotes(parts[1]), 2);
        }
        private void ReadSkinMap()
        {
            ReadMap("skins/map.txt", (player, parts) =>
            {
                player.SkinPath = Utils.GetStringInsideQuotes(parts[1]);
                player.GlovesPath = parts.Length == 3 ? Utils.GetStringInsideQuotes(parts[2]) : "";
            }, 3);
        }
        private void ReadGlovesMap()
        {
            ReadMap("gloves/map.txt", (player, parts) =>
            {
                player.GKLeftGlovePath = Utils.GetStringInsideQuotes(parts[1]);
                player.GKRightGlovePath = parts.Length == 3 ? Utils.GetStringInsideQuotes(parts[2]) : "";
            }, 3);
        }
        private void ReadUniMap()
        {
            ReadMap("uni/map.txt", (team, parts) => team.KitsPath = Utils.GetStringInsideQuotes(parts[1]), 2, 2);
        }
        private void ReadChantsMap()
        {
            ReadMap("chants/map.txt", (team, parts) => team.ChantsFolder = Utils.GetStringInsideQuotes(parts[1]), 2, 2);
        }
        private void ReadBannersMap()
        {
            ReadMap("banners/map.txt",
               (team, parts) =>
               {
                   team.BannerFolder = Utils.GetStringInsideQuotes(parts[1]);
                   if (!string.IsNullOrEmpty(team.BannerFolder))
                   {
                       string supporterColourFilePath = Path.Combine(GetGDBPath($"banners\\{team.BannerFolder}"), "colours.txt");
                       ReadSupporterColour(team, supporterColourFilePath);
                   }
               },
               2, 
               2);
        }
        private void ReadHomeBallMap()
        {
            ReadMap("balls/home_map.txt",
                (team, parts) =>
                {
                    string ballName = Utils.GetStringInsideQuotes(parts[1]);
                    Ball ball = balls.FirstOrDefault(b => b.Name == ballName);
                    team.HomeBall = ball;
                },
                2, 
                2);
        }
        private void ReadBallsMap()
        {
            string mapFilePath = GetGDBPath("balls/map.txt");
            if (!File.Exists(mapFilePath)) return;

            string[] lines = File.ReadAllLines(mapFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new[] { ',' }, 3);
                if (parts.Length < 3)
                    continue;
                Ball ball = new Ball
                {
                    Name = Utils.GetStringInsideQuotes(parts[0]),
                    Mdl = Utils.GetStringInsideQuotes(parts[1]),
                    TexturePath = Utils.GetStringInsideQuotes(parts[2])
                };
                balls.Add(ball);
            }
        }
        private void ReadStadiumMap()
        {
            ReadMap("stadiums/map.txt",
                (team, parts) => team.HomeStadium = Utils.GetStringInsideQuotes(parts[1]),
                2, 
                2);
        }
        public void ReadSupporterColour(Team team, string filePath)
        {
            SupporterColour supporterColour = new SupporterColour();

            // Verificar si el archivo existe
            if (!File.Exists(filePath))
            {
                team.supporterColour = supporterColour;
                return;
            }
            string[] lines = File.ReadAllLines(filePath);

            // Leer el archivo línea por línea
            foreach (string line in lines)
            {
                // Ignorar líneas vacías
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                // Separar la línea en clave y valor
                string[] parts = line.Split('=');
                if (parts.Length != 2)
                {
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Asignar los valores a las propiedades del objeto
                switch (key)
                {
                    case "main.colour":
                        if (int.TryParse(value, out int mainColour))
                        {
                            supporterColour.MainColour = mainColour;
                        }
                        break;

                    case "secondary.colour":
                        if (int.TryParse(value, out int secondaryColour))
                        {
                            supporterColour.SecondaryColour = secondaryColour;
                        }
                        break;
                }
            }

            team.supporterColour = supporterColour;

        }
        private void ReadSupporterFlagsMap()
        {
            ReadMap("flags/map_supporters.txt",
            (team, parts) => team.SupporterFlagPath = Utils.GetStringInsideQuotes(parts[1]),
            2, 
            2);
            return;
        }
        private void ReadFlagsMap()
        {
            string mapFilePath = GetGDBPath("flags/map.txt");
            if (!File.Exists(mapFilePath)) return;

            string[] lines = File.ReadAllLines(mapFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new[] { ',' }, 3);
                if (parts.Length < 2)
                    continue;

                if (uint.TryParse(parts[0], out uint flagId))
                {
                    uint id = GetTeamIdFromFlagId(flagId);
                    if (id == 0xffffffff)
                    {
                        // for now we ignore those flag id but save them for later re add it
                        extraFlagMapLines.Add(line);
                        continue;
                    }
                    string flagTexturePath = Utils.GetStringInsideQuotes(parts[1]);

                    string smallFlagPath = "";
                    if (parts.Length == 3)
                    {
                        smallFlagPath = Utils.GetStringInsideQuotes(parts[2]);
                    }

                    Team team = teams.FirstOrDefault(t => t.Id == id);
                    if (team != null)
                    {
                        team.FlagPath = flagTexturePath;
                        team.SmallFlagPath = smallFlagPath;
                    }
                }
            }
        }
        private void ReadTeamCallnamesMap()
        {
            ReadMap("callnames/map_teams.txt",
            (team, parts) =>
            {
                team.CallnameNormal = Utils.GetStringInsideQuotes(parts[1]);
                team.CallnameVs = Utils.GetStringInsideQuotes(parts[2]);
                team.CallnameLoud = parts.Length == 4 ? Utils.GetStringInsideQuotes(parts[3]) : "";
            },
            4, 
            3);
        }
        private uint GetTeamIdFromFlagId(uint flagId)
        {
            uint totalNationalities = (uint)executable.Nationalities.Count();
            if (Team.TotalNations - Team.TotalClassicTeams <= flagId && flagId < totalNationalities) return 0xffffffff;
            return flagId < Team.TotalNations - Team.TotalClassicTeams ? flagId : flagId + Team.TotalNations - totalNationalities;
        }
        private uint GetFlagIdFromTeamId(uint teamId)
        {
            uint totalNationalities = (uint)executable.Nationalities.Count();
            if (Team.TotalNations - Team.TotalClassicTeams <= teamId && teamId < Team.TotalNations) return 0xffffffff;
            return teamId < Team.TotalNations - Team.TotalClassicTeams ? teamId : teamId - Team.TotalNations + totalNationalities;
        }
        private void WriteMap(string mapFileName, Action<Player, StreamWriter> writePlayerPaths)
        {
            string mapFilePath = GetGDBPath(mapFileName);

            using (FileStream fs = new FileStream(mapFilePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                DateTime localDate = DateTime.Now;
                sw.WriteLine(MAP_HEADER);
                sw.WriteLine($"# Last update on {localDate.ToString("yyyy-MM-dd HH:mm:ss tt")}");
                sw.WriteLine();

                foreach (Team team in teams)
                {
                    if (team.Players.Count > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine($"# {team.Name}");
                        sw.WriteLine();
                    }

                    foreach (Player player in team.Players)
                    {
                        writePlayerPaths(player, sw);
                    }
                }

                sw.WriteLine();
                sw.WriteLine("#Free Agents");

                foreach (Player player in optionFile.FreePlayers)
                {
                    writePlayerPaths(player, sw);
                }

                sw.WriteLine();
                sw.WriteLine("# End of map");
            }
        }
        private void WriteMap(string mapFileName, Action<Team, StreamWriter> writeTeamPaths)
        {
            string mapFilePath = GetGDBPath(mapFileName);

            using (FileStream fs = new FileStream(mapFilePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                DateTime localDate = DateTime.Now;
                sw.WriteLine(MAP_HEADER);
                sw.WriteLine($"# Last update on {localDate.ToString("yyyy-MM-dd HH:mm:ss tt")}");
                sw.WriteLine();

                foreach (Team team in teams)
                {
                    writeTeamPaths(team, sw);
                }
                sw.WriteLine();
                sw.WriteLine("# End of map");
            }
        }
        public void WriteFaceMap()
        {
            WriteMap("faces\\map.txt", (player, sw) =>
            {
                if (string.IsNullOrEmpty(player.FacePath)) return;
                sw.WriteLine($"{player.Id},\"{player.FacePath}\" # {player.Name}");
            });
            return;
        }
        public void WriteHairMap()
        {
            WriteMap("hair\\map.txt", (player, sw) =>
            {
                if (string.IsNullOrEmpty(player.HairPath)) return;
                sw.WriteLine($"{player.Id},\"{player.HairPath}\" # {player.Name}");
            });
        }
        public void WriteBootsMap()
        {
            WriteMap("boots\\map.txt", (player, sw) =>
            {
                if (string.IsNullOrEmpty(player.BootPath)) return;
                sw.WriteLine($"{player.Id},\"{player.BootPath}\" # {player.Name}");
            });
        }
        public void WriteSkinMap()
        {
            WriteMap("skins\\map.txt", (player, sw) =>
            {
                if (string.IsNullOrEmpty(player.SkinPath) && string.IsNullOrEmpty(player.GlovesPath)) return;
                if (string.IsNullOrEmpty(player.GlovesPath))
                {
                    sw.WriteLine($"{player.Id},\"{player.SkinPath}\" # {player.Name}");
                }
                else
                {
                    sw.WriteLine($"{player.Id},\"{player.SkinPath}\",\"{player.GlovesPath}\" # {player.Name}");
                }
            });
        }
        public void WriteGlovesMap()
        {
            WriteMap("gloves\\map.txt", (player, sw) =>
            {
                if (string.IsNullOrEmpty(player.GKLeftGlovePath) && string.IsNullOrEmpty(player.GKRightGlovePath)) return;
                if (string.IsNullOrEmpty(player.GKRightGlovePath))
                {
                    sw.WriteLine($"{player.Id},\"{player.GKLeftGlovePath}\" # {player.Name}");
                }
                else
                {
                    sw.WriteLine($"{player.Id},\"{player.GKLeftGlovePath}\",\"{player.GKRightGlovePath}\" # {player.Name}");
                }
            });
        }
        public void WriteUniMap()
        {
            WriteMap("uni\\map.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.KitsPath)) return;
                sw.WriteLine($"{team.Id},\"{team.KitsPath}\" # {team.Name}");
            });
            return;
        }
        public void WriteHomeBallMap()
        {
            WriteMap("balls\\home_map.txt", (team, sw) =>
            {
                if (team.HomeBall==null) return;
                sw.WriteLine($"{team.Id},\"{team.HomeBall.Name}\" # {team.Name}");
            });
            return;
        }
        public void WriteStadiumMap()
        {
            WriteMap("stadiums\\map.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.HomeStadium)) return;
                sw.WriteLine($"{team.Id},\"{team.HomeStadium}\" # {team.Name}");
            });
            return;
        }
        public void WriteChantsMap()
        {
            WriteMap("chants\\map.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.ChantsFolder)) return;
                sw.WriteLine($"{team.Id},\"{team.ChantsFolder}\" # {team.Name}");
            });
        }
        private void WriteSupporterColour(SupporterColour sc, string path)
        {
            if (string.IsNullOrEmpty(path) || sc.MainColour ==-1 || sc.SecondaryColour == -1) return;

            string bannersFolder = GetGDBPath(Path.Combine("banners", path));
            string coloursFileName = Path.Combine(bannersFolder, "colours.txt");
            using (FileStream fs = new FileStream(coloursFileName, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                DateTime localDate = DateTime.Now;
                sw.WriteLine(MAP_HEADER);
                sw.WriteLine($"# Last update on {localDate:en-GB}");
                sw.WriteLine();
                sw.WriteLine($"main.colour = {sc.MainColour}");
                sw.WriteLine($"secondary.colour = {sc.SecondaryColour}");
                sw.WriteLine();
            }
        }
        public void WriteBannersMap()
        {
            WriteMap("banners\\map.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.BannerFolder)) return;
                sw.WriteLine($"{team.Id},\"{team.BannerFolder}\" # {team.Name}");
                WriteSupporterColour(team.supporterColour, team.BannerFolder);
            });
            return;
        }
        public void WriteTeamCallnamesMap()
        {
            WriteMap("callnames\\map_teams.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.CallnameNormal) && string.IsNullOrEmpty(team.CallnameVs) && string.IsNullOrEmpty(team.CallnameLoud)) return;
                if (string.IsNullOrEmpty(team.CallnameLoud))
                {
                    sw.WriteLine($"{team.Id},\"{team.CallnameNormal}\",\"{team.CallnameVs}\" # {team.Name}");
                }
                else
                {
                    sw.WriteLine($"{team.Id},\"{team.CallnameNormal}\",\"{team.CallnameVs}\",\"{team.CallnameLoud}\" # {team.Name}");
                }
                
            });
            return;
        }
        public void WriteFlagsMap()
        {
            WriteMap("flags\\map.txt", (team, sw) =>
            {
                uint flagId = GetFlagIdFromTeamId(team.Id);

                if ((string.IsNullOrEmpty(team.FlagPath) && string.IsNullOrEmpty(team.SmallFlagPath)) || flagId == 0xffffffff) return;

                if (string.IsNullOrEmpty(team.SmallFlagPath))
                {
                    sw.WriteLine($"{flagId},\"{team.FlagPath}\" # {team.Name}");
                }
                else
                {
                    sw.WriteLine($"{flagId},\"{team.FlagPath}\",\"{team.SmallFlagPath}\" # {team.Name}");
                }

            });

            string mapFilePath = GetGDBPath("flags\\map.txt");

            using (FileStream fs = new FileStream(mapFilePath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                if (extraFlagMapLines.Count> 0) 
                {
                    fs.Seek(0, SeekOrigin.End);
                    sw.WriteLine();
                    sw.WriteLine("# Extra lines saved from map read");

                    foreach (string line in extraFlagMapLines)
                    {
                        sw.WriteLine(line);
                    }
                    sw.WriteLine();
                    sw.WriteLine("# End of map");
                }
            }

        }
        public void WriteSupporterFlagsMap()
        {
            WriteMap("flags\\map_supporters.txt", (team, sw) =>
            {
                if (string.IsNullOrEmpty(team.SupporterFlagPath)) return;
                sw.WriteLine($"{team.Id},\"{team.SupporterFlagPath}\" # {team.Name}");
            });
        }
        public void WriteAllPlayerMaps()
        {
            if (Directory.Exists(GetGDBPath($"faces")))
            {
                WriteFaceMap();
            }
            if (Directory.Exists(GetGDBPath($"hair")))
            {
                WriteHairMap();
            }
            if (Directory.Exists(GetGDBPath($"boots")))
            {
                WriteBootsMap();
            }
            if (Directory.Exists(GetGDBPath($"skins")))
            {
                WriteSkinMap();
            }
            if (Directory.Exists(GetGDBPath($"gloves")))
            {
                WriteGlovesMap();
            }
        }
        public void WriteAllTeamMaps()
        {
            if (Directory.Exists(GetGDBPath($"uni")))
            {
                WriteUniMap();
            }
            if (Directory.Exists(GetGDBPath($"balls")))
            {
                WriteHomeBallMap();
            }
            if (Directory.Exists(GetGDBPath($"stadiums")))
            {
                WriteStadiumMap();
            }
            if (Directory.Exists(GetGDBPath($"chants")))
            {
                WriteChantsMap();
            }
            if (Directory.Exists(GetGDBPath($"banners")))
            {
                WriteBannersMap();
            }
            if (Directory.Exists(GetGDBPath($"callnames")))
            {
                WriteTeamCallnamesMap();
            }
            if (Directory.Exists(GetGDBPath($"flags")))
            {
                WriteFlagsMap();
            }
            if (Directory.Exists(GetGDBPath($"flags")))
            {
                WriteSupporterFlagsMap();
            }
        }
    }
}
