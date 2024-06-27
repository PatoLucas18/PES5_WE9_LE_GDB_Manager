using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Media;

namespace PES5_WE9_LE_GDB_Manager
{
    public partial class frmMain : Form
    {
        private SoundPlayer soundPlayer = new SoundPlayer();
        private bool isPlaying = false;
        public GDBManagerConfig config;
        private OptionFile optionFile;
        private Executable executable;
        private GDB gdb;
        const string ALL_PLAYERS = "All Players";
        const string FREE_PLAYERS = "Free Players";
        private bool FilterByNation;
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            panel1.Dock = DockStyle.Fill;
            // for now we hide this tab pages as they're not ready yet
            tpReferees.Parent = null;
            tpCompetitions.Parent = null;

            optionFile = new OptionFile(config.OFPath);
            executable = new Executable(config.ExePath);
            gdb = new GDB(optionFile, executable, config.GDBFolderPath);
            gdb.LoadTeams();
            optionFile.SetPlayersInTeam(gdb.teams);
            optionFile.SetFreePlayers();
            gdb.LoadStadiums();

            gdb.LoadAllMaps();

            byTeamToolStripMenuItem.Checked = true;
            LoadPlayerTeamFilter();

            LoadPlayersListBox(optionFile.Players);
            LoadTeamsListBox(gdb.teams);
            LoadStadiumsComboBox();
            LoadBallsCombobox();
            LoadSupportersColoursList();
        }
        #region PLAYER TAB
        private void LoadPlayerNationalityFilter()
        {
            cboPlayerFilter.Items.Clear();
            foreach (Nationality nationality in executable.Nationalities)
            {
                cboPlayerFilter.Items.Add(nationality);
            }
            cboPlayerFilter.Items.Add(FREE_PLAYERS);
            cboPlayerFilter.Items.Add(ALL_PLAYERS);
            if (cboPlayerFilter.SelectedIndex != cboPlayerFilter.Items.Count - 1)
            {
                cboPlayerFilter.SelectedIndex = cboPlayerFilter.Items.Count - 1;
            }
        }
        private void LoadPlayerTeamFilter()
        {
            cboPlayerFilter.Items.Clear();
            foreach (Team team in gdb.teams)
            {
                cboPlayerFilter.Items.Add(team);
            }
            cboPlayerFilter.Items.Add(FREE_PLAYERS);
            cboPlayerFilter.Items.Add(ALL_PLAYERS);
            if (cboPlayerFilter.SelectedIndex != cboPlayerFilter.Items.Count - 1)
            {
                cboPlayerFilter.SelectedIndex = cboPlayerFilter.Items.Count - 1;
            }
        }
        private void cboPlayerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPlayerFilter.SelectedItem == null) return;

            string selectedItem = cboPlayerFilter.SelectedItem.ToString();

            if (selectedItem == ALL_PLAYERS)
            {
                LoadPlayersListBox(optionFile.Players);
                return;
            }
            else if (selectedItem == FREE_PLAYERS)
            {
                LoadPlayersListBox(optionFile.FreePlayers);
                return;
            }

            if (FilterByNation)
            {
                Nationality nationality = (Nationality)cboPlayerFilter.SelectedItem;

                List<Player> playersByNation = GetPlayersByNationality(nationality);

                LoadPlayersListBox(playersByNation);
            }
            else
            {
                Team team = (Team)cboPlayerFilter.SelectedItem;

                LoadPlayersListBox(team.Players);
            }
        }

        private List<Player> GetPlayersByNationality(Nationality nationality)
        {
            List<Player> playersByNation = new List<Player>();

            foreach (Player player in optionFile.Players)
            {
                if (player.Nationality.getValue() == nationality.Id)
                {
                    playersByNation.Add(player);
                }
            }
            return playersByNation;
        }

        private void LoadPlayersListBox(List<Player> players)
        {
            lstPlayers.Items.Clear();
            foreach (Player player in players)
            {
                lstPlayers.Items.Add(player);
            }

        }
        private void lstPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            LoadPlayerInfo(player);

        }
        private void TxtPlayerFilterByName_TextChanged(object sender, EventArgs e)
        {
            if (cboPlayerFilter.SelectedItem == null) return;
            string selectedItem = cboPlayerFilter.SelectedItem.ToString();
            string nameToLookup = txtPlayerFilterByName.Text.ToLower();

            List<Player> currentFilter;

            if (selectedItem == FREE_PLAYERS)
            {
                currentFilter = optionFile.FreePlayers;
            }
            else if (selectedItem != ALL_PLAYERS && FilterByNation)
            {
                Nationality nationality = (Nationality)cboPlayerFilter.SelectedItem;
                currentFilter = GetPlayersByNationality(nationality);
            }
            else if (selectedItem != ALL_PLAYERS && !FilterByNation)
            {
                Team team = (Team)cboPlayerFilter.SelectedItem;
                currentFilter = team.Players;
            }
            else
            {
                currentFilter = optionFile.Players;
            }

            if (string.IsNullOrEmpty(nameToLookup))
            {
                LoadPlayersListBox(currentFilter);
            }
            if (nameToLookup.Length < 3) return;
            List<Player> playersByName = new List<Player>();

            foreach (Player player in currentFilter)
            {
                if (player.Name.ToLower().Contains(nameToLookup))
                {
                    playersByName.Add(player);
                }
            }
            LoadPlayersListBox(playersByName);
        }
        #endregion

        #region TEAM TAB
        private void lstTeams_SelectedIndexChanged(Object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            LoadTeamInfo(team);
        }

        private void LoadTeamInfo(Team team)
        {
            string kitsFullPath = gdb.GetGDBPath($"uni\\{team.KitsPath}\\");
            string bannersFullPath = gdb.GetGDBPath($"banners\\{team.BannerFolder}\\");
            string chantsFullPath = gdb.GetGDBPath($"chants\\{team.ChantsFolder}\\");
            string stadiumFullPath = gdb.GetGDBPath($"stadiums\\{team.HomeStadium}\\");
            string flagFullPath = gdb.GetGDBPath($"flags\\{team.FlagPath}");
            string smallFlagFullPath = gdb.GetGDBPath($"flags\\{team.SmallFlagPath}");
            string supporterFlagFullPath = gdb.GetGDBPath($"flags\\{team.SupporterFlagPath}");

            string stadiumPreviewPNG = $"{stadiumFullPath}\\preview.png";
            string stadiumPreviewBMP = $"{stadiumFullPath}\\preview.bmp";
            string[] chantsAllowedExtensions = new string[] { ".adx", ".wav" };
            string[] bannersAllowedExtensions = new string[] { ".png", ".str", ".bin", ".txs" };

            txtTeamId.Text = team.Id.ToString();
            txtTeamKits.Text = team.KitsPath;
            txtTeamBanners.Text = team.BannerFolder;
            txtTeamChants.Text = team.ChantsFolder;
            txtTeamFlag.Text = team.FlagPath;
            txtTeamSmallFlag.Text = team.SmallFlagPath;
            txtTeamSupporterFlag.Text = team.SupporterFlagPath;
            txtTeamCallnameNormal.Text = team.CallnameNormal;
            txtTeamCallnameVs.Text = team.CallnameVs;
            txtTeamCallnameLoud.Text = team.CallnameLoud;

            cboTeamKits.SelectedItem = null;
            cboTeamBall.SelectedItem = null;
            cboTeamStadium.SelectedItem = null;
            cboTeamBanners.SelectedItem = null;
            cboTeamChants.SelectedItem = null;
            cboTeamSuppMainColour.SelectedItem = null;
            cboTeamSuppSecColour.SelectedItem = null;
            cboTeamSuppMainColour.Enabled = false;
            cboTeamSuppSecColour.Enabled = false;

            cboTeamKits.Items.Clear();
            cboTeamBanners.Items.Clear();
            cboTeamChants.Items.Clear();

            StopPlaying(btnTeamChantsPlay);
            StopPlaying(btnTeamCallnameNormalPlay);


            picTeamBall.Image = null;
            picTeamBanners.Image = null;
            picTeamStadium.Image = null;
            picTeamFlag.Image = null;
            picTeamSmallFlag.Image = null;
            picTeamSupporterFlag.Image = null;

            if (!string.IsNullOrEmpty(team.KitsPath) && Directory.Exists(kitsFullPath))
            {
                LoadSubdirectoriesIntoCombobox(kitsFullPath, cboTeamKits);
            }

            if (!string.IsNullOrEmpty(team.ChantsFolder) && Directory.Exists(chantsFullPath))
            {
                LoadFilesIntoCombobox(chantsFullPath, cboTeamChants, chantsAllowedExtensions);
            }

            if (!string.IsNullOrEmpty(team.BannerFolder) && Directory.Exists(bannersFullPath))
            {
                cboTeamSuppMainColour.Enabled = true;
                cboTeamSuppSecColour.Enabled = true;
                LoadFilesIntoCombobox(bannersFullPath, cboTeamBanners, bannersAllowedExtensions);
                if (cboTeamBanners.SelectedItem != null && !string.IsNullOrEmpty(cboTeamBanners.SelectedItem.ToString()) && File.Exists($"{bannersFullPath}\\{cboTeamBanners.SelectedItem}"))
                {
                    Bitmap bitmap = LoadTextureFromBin($"{bannersFullPath}\\{cboTeamBanners.SelectedItem}", 0, true, false);
                    picTeamBanners.Image = bitmap;
                }
            }

            if (!string.IsNullOrEmpty(team.HomeStadium) && cboTeamStadium.Items.Contains(team.HomeStadium))
            {
                cboTeamStadium.SelectedItem = team.HomeStadium;

                if (!string.IsNullOrEmpty(team.HomeStadium) && Directory.Exists(stadiumFullPath) && File.Exists(stadiumPreviewPNG))
                {
                    picTeamStadium.Image = Image.FromFile(stadiumPreviewPNG);
                }
                else if (!string.IsNullOrEmpty(team.HomeStadium) && Directory.Exists(stadiumFullPath) && File.Exists(stadiumPreviewBMP))
                {
                    picTeamStadium.Image = Image.FromFile(stadiumPreviewBMP);
                }
            }

            if (team.HomeBall != null && !string.IsNullOrEmpty(team.HomeBall.TexturePath) && cboTeamBall.Items.Contains(team.HomeBall))
            {
                cboTeamBall.SelectedItem = team.HomeBall;
                string ballTextureFullPath = gdb.GetGDBPath($"balls\\{team.HomeBall.TexturePath}");

                if (File.Exists(ballTextureFullPath))
                {
                    picTeamBall.Image = Image.FromFile(ballTextureFullPath);
                }
            }
            if (team.supporterColour != null)
            {
                cboTeamSuppMainColour.SelectedIndex = team.supporterColour.MainColour;
                cboTeamSuppSecColour.SelectedIndex = team.supporterColour.SecondaryColour;
            }
            if (!string.IsNullOrEmpty(team.FlagPath) && File.Exists(flagFullPath))
            {
                picTeamFlag.Image = Image.FromFile(flagFullPath);
            }
            if (!string.IsNullOrEmpty(team.SmallFlagPath) && File.Exists(smallFlagFullPath))
            {
                picTeamSmallFlag.Image = Image.FromFile(smallFlagFullPath);
            }
            if (!string.IsNullOrEmpty(team.SupporterFlagPath) && File.Exists(supporterFlagFullPath))
            {
                picTeamSupporterFlag.Image = Image.FromFile(supporterFlagFullPath);
            }
        }

        private void CboTeamSuppMainColour_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || team.supporterColour == null || cboTeamSuppMainColour.SelectedItem == null) return;
            team.supporterColour.MainColour = cboTeamSuppMainColour.SelectedIndex;
        }
        private void CboTeamSuppSecColour_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || team.supporterColour == null || cboTeamSuppSecColour.SelectedItem == null) return;
            team.supporterColour.SecondaryColour = cboTeamSuppSecColour.SelectedIndex;
        }

        private void cboTeamBanners_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || cboTeamStadium.SelectedItem == null) return;

            string bannersFullPath = gdb.GetGDBPath($"banners\\{team.BannerFolder}\\");

            if (!string.IsNullOrEmpty(cboTeamBanners.SelectedItem.ToString()) && File.Exists($"{bannersFullPath}\\{cboTeamBanners.SelectedItem}"))
            {
                Bitmap bitmap = LoadTextureFromBin($"{bannersFullPath}\\{cboTeamBanners.SelectedItem}", 0, true, false);
                picTeamBanners.Image = bitmap;
            }
        }

        private void cboTeamSuppColour_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            
            if (e.Index >= 0)
            {
                ComboBox cbo = (ComboBox)sender;
                string txt = cbo.GetItemText(cbo.Items[e.Index]);
                Color color = (Color)cbo.Items[e.Index];
                Rectangle r1 = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1,
                    2 * (e.Bounds.Height - 2), e.Bounds.Height - 2);
                Rectangle r2 = Rectangle.FromLTRB(r1.Right + 2, e.Bounds.Top,
                    e.Bounds.Right, e.Bounds.Bottom);
                using (SolidBrush b = new SolidBrush(color))
                    e.Graphics.FillRectangle(b, r1);
                e.Graphics.DrawRectangle(Pens.Black, r1);
                TextRenderer.DrawText(e.Graphics, txt, cbo.Font, r2,
                    cbo.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
        private void cboTeamStadium_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || cboTeamStadium.SelectedItem == null) return;

            string stadium = cboTeamStadium.SelectedItem.ToString();
            team.HomeStadium = stadium;
            LoadTeamInfo(team);
        }

        private void cboTeamBall_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || cboTeamBall.SelectedItem == null) return;

            Ball ball = (Ball)cboTeamBall.SelectedItem;
            team.HomeBall = ball;
            LoadTeamInfo(team);
        }
        private void LoadSupportersColoursList()
        {
            List<Color> mainSupporterColors = Utils.GetSupporterColours();
            cboTeamSuppMainColour.DataSource = null;
            cboTeamSuppMainColour.Items.Clear();
            cboTeamSuppMainColour.DataSource = mainSupporterColors;
            cboTeamSuppMainColour.SelectedItem = null;

            List<Color> secSupporterColors = Utils.GetSupporterColours();
            cboTeamSuppSecColour.DataSource = null;
            cboTeamSuppSecColour.Items.Clear();
            cboTeamSuppSecColour.DataSource = secSupporterColors;
            cboTeamSuppSecColour.SelectedItem = null;
        }
        private void LoadBallsCombobox()
        {
            cboTeamBall.Items.Clear();
            foreach (Ball ball in gdb.balls)
            {
                cboTeamBall.Items.Add(ball);
            }
        }

        private void LoadStadiumsComboBox()
        {
            cboTeamStadium.Items.Clear();
            foreach (string stadium in gdb.stadiums)
            {
                cboTeamStadium.Items.Add(stadium);
            }
        }

        private void LoadTeamsListBox(List<Team> teams)
        {
            lstTeams.Items.Clear();
            foreach (Team team in teams)
            {
                lstTeams.Items.Add(team);
            }
        }
        #endregion

        #region FILE MENU
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConfig configForm = new frmConfig();
            configForm.config = new GDBManagerConfig
            {
                ExePath = config.ExePath,
                OFPath = config.OFPath,
                GDBFolderPath = config.GDBFolderPath
            };

            DialogResult result = configForm.ShowDialog();
            if (
                result == DialogResult.OK
                && (
                    configForm.config.ExePath != config.ExePath
                    || configForm.config.OFPath != config.OFPath
                    || configForm.config.GDBFolderPath != config.GDBFolderPath
                )
                )
            {
                config.ExePath = configForm.config.ExePath;
                config.OFPath = configForm.config.OFPath;
                config.GDBFolderPath = configForm.config.GDBFolderPath;
                frmMain_Load(null, EventArgs.Empty);
            }

        }

        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.SaveConfig(config);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to save the configuration file\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Configuration file saved successfully");
        }
        private void saveALLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        #region SAVE MAPS
        private void facesMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"faces");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteFaceMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save faces map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Faces map saved correctly");
        }

        private void hairsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"hair");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteHairMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save hairs map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Hairs map saved correctly");
        }

        private void bootsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"boots");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteBootsMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save boots map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Boots map saved correctly");
        }
        private void skinsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"skins");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteSkinMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save skins map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Skins map saved correctly");

        }
        private void glovesMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"gloves");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteGlovesMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save gloves map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Gloves map saved correctly");
        }

        private void allMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gdb.WriteAllPlayerMaps();
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to save all players maps\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("All players map saved correctly");
        }

        private void kitsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"uni");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteUniMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save kits map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Kits map saved correctly");
        }

        private void homeBallMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"balls");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteHomeBallMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save home balls map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Home balls map saved correctly");
        }

        private void stadiumsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"stadiums");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteStadiumMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save stadiums map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Stadiums map saved correctly");
        }

        private void chantsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"chants");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteChantsMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save chants map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Chants map saved correctly");
        }

        private void bannersMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"banners");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteBannersMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save banners map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Banners map saved correctly");
        }

        private void flagsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"flags");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteFlagsMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save flags map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Flags map saved correctly");
        }

        private void suppFlagsMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"flags");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteSupporterFlagsMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save supporters flags map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Supporters flags map saved correctly");
        }

        private void callnamesMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            try
            {
                gdb.WriteTeamCallnamesMap();
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to save team callnames map\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("Team callnames map saved correctly");
        }

        private void allMapsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                gdb.WriteAllTeamMaps();
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to save all teams maps\nError: {ex.Message}");
                return;
            }
            Utils.ShowInfo("All teams map saved correctly");
        }
        #endregion

        #endregion
        #region EDIT MENU
        private void byTeamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterByNation = false;
            byNationToolStripMenuItem.Checked = FilterByNation;
            byTeamToolStripMenuItem.Checked = !FilterByNation;
            LoadPlayerTeamFilter();
        }
        private void byNationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterByNation = true;
            byNationToolStripMenuItem.Checked = FilterByNation;
            byTeamToolStripMenuItem.Checked = !FilterByNation;
            LoadPlayerNationalityFilter();
        }

        #endregion

        #region HELP MENU
        private void searchForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utils.CheckForUpdates();
        }

        #endregion

        private void LoadSubdirectoriesIntoCombobox(string path, ComboBox cbo)
        {
            string[] subdirectories = Directory.GetDirectories(path);

            foreach (string subdirectory in subdirectories)
            {
                cbo.Items.Add(Utils.GetRelativePath(subdirectory, path));
            }

            if (subdirectories.Length > 0)
            {
                cbo.SelectedIndex = 0;
            }

        }
        private void LoadFilesIntoCombobox(string path, ComboBox cbo, string[] allowedExtensions)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                if (allowedExtensions.Contains(extension))
                {
                    cbo.Items.Add(Utils.GetRelativePath(file, path));
                }
            }

            if (cbo.Items.Count > 0)
            {
                cbo.SelectedIndex = 0;
            }

        }
        private Player GetPlayer()
        {
            if (lstPlayers.SelectedItem == null) return null;

            return (Player)lstPlayers.SelectedItem;

        }
        private Team GetTeam()
        {
            if (lstTeams.SelectedItem == null) return null;

            return (Team)lstTeams.SelectedItem;
        }
        private void LoadPlayerInfo(Player player)
        {
            if (player == null) return;

            txtPlayerID.Text = player.Id.ToString();
            txtPlayerName.Text = player.Name;
            txtPlayerFace.Text = player.FacePath;
            txtPlayerHair.Text = player.HairPath;
            txtPlayerBoot.Text = player.BootPath;
            txtPlayerSkin.Text = player.SkinPath;
            txtPlayerWinterGloves.Text = player.GlovesPath;
            txtGKLeftGlove.Text = player.GKLeftGlovePath;
            txtGKRightGlove.Text= player.GKRightGlovePath;

            picPlayerFace.Image = null;
            picPlayerHair.Image = null;
            picPlayerBoot.Image = null;
            picPlayerSkin.Image = null;
            picPlayerWinterGloves.Image = null;
            picGKLeftGlove.Image = null;
            picGKRightGlove.Image = null;

            string faceFullPath = gdb.GetGDBPath($"faces/{player.FacePath}");
            string facePNGFullPath = Path.ChangeExtension(faceFullPath, ".png");
            string hairFullPath = gdb.GetGDBPath($"hair/{player.HairPath}");
            string hairPNGFullPath = Path.ChangeExtension(hairFullPath, ".png");
            string bootFullPath = gdb.GetGDBPath($"boots/{player.BootPath}");
            string skinFullPath = gdb.GetGDBPath($"skins/{player.SkinPath}");
            string glovesFullPath = gdb.GetGDBPath($"skins/{player.GlovesPath}");
            string gkLeftFullPath = gdb.GetGDBPath($"gloves/{player.GKLeftGlovePath}");
            string gkRightFullPath = gdb.GetGDBPath($"gloves/{player.GKRightGlovePath}");

            if (!string.IsNullOrEmpty(player.FacePath) && File.Exists(faceFullPath) && File.Exists(facePNGFullPath))
            {
                picPlayerFace.Image = Image.FromFile(facePNGFullPath);
            }
            else if (!string.IsNullOrEmpty(player.FacePath) && File.Exists(faceFullPath))
            {
                Bitmap bitmap = LoadTextureFromBin(faceFullPath, 2, true, true);
                picPlayerFace.Image = bitmap;
            }
            if (!string.IsNullOrEmpty(player.HairPath) && File.Exists(hairFullPath) && File.Exists(hairPNGFullPath))
            {
                picPlayerHair.Image = Image.FromFile(hairPNGFullPath);
            }
            else if(!string.IsNullOrEmpty(player.HairPath) && File.Exists(hairFullPath))
            {
                Bitmap bitmap = LoadTextureFromBin(hairFullPath, 1, true, true);
                picPlayerHair.Image = bitmap;
            }
            if (!string.IsNullOrEmpty(player.BootPath) && File.Exists(bootFullPath))
            {
                picPlayerBoot.Image = Image.FromFile(bootFullPath);
            }
            if (!string.IsNullOrEmpty(player.SkinPath) && File.Exists(skinFullPath))
            {
                picPlayerSkin.Image = Image.FromFile(skinFullPath);
            }
            if (!string.IsNullOrEmpty(player.GlovesPath) && File.Exists(glovesFullPath))
            {
                picPlayerWinterGloves.Image = Image.FromFile(glovesFullPath);
            }
            if (!string.IsNullOrEmpty(player.GKLeftGlovePath) && File.Exists(gkLeftFullPath))
            {
                picGKLeftGlove.Image = Image.FromFile(gkLeftFullPath);
            }
            if (!string.IsNullOrEmpty(player.GKRightGlovePath) && File.Exists(gkRightFullPath))
            {
                picGKRightGlove.Image = Image.FromFile(gkRightFullPath);
            }
        }
        private Bitmap LoadTextureFromBin(string faceFullPath, int fileIndex, bool disableAlpha, bool readFileIndexes)
        {
            byte[] unzlibedFile = Utils.Unzlib(faceFullPath);
            PESTexture pesTexture = Utils.ReadPESTexture(unzlibedFile, fileIndex, readFileIndexes);
            pesTexture.ConvertPalette();
            if (disableAlpha )
            {
                pesTexture.DisableAlpha();
            }
            Bitmap bitmap = Utils.CreateImageFromPaletteAndPixels(pesTexture.Palette, pesTexture.Pixels, pesTexture.Width, pesTexture.Height);
            return bitmap;
        }
        private void SelectFileFromFolder(string workingDirectory, ref string parameter, string openDialogTitle)
        {
            string newFilePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} {openDialogTitle} Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            if (!Utils.IsSubPath(workingDirectory, newFilePath))
            {
                try
                {
                    newFilePath = Utils.CopyToFolder(workingDirectory, newFilePath);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
            }
            try
            {
                parameter = Utils.GetRelativePath(newFilePath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
        }
        private void PlayCallname(string callnamePath, Button btn)
        {
            if (string.IsNullOrEmpty(callnamePath)) return;

            string callnameRelativePath = Path.Combine($"callnames", callnamePath);
            string callnameFullPath = gdb.GetGDBPath(callnameRelativePath);
            if (!File.Exists(callnameFullPath)) return;
            byte[] wavData;
            try
            {
                byte[] adxData = File.ReadAllBytes(callnameFullPath);


                if (adxData[0] != 0x52 && adxData[1] != 0x49 && adxData[2] != 0x46 && adxData[3] != 0x46)
                {
                    wavData = ADX.ToWav(adxData);
                }
                else
                {
                    wavData = adxData;
                }


            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error while trying to play file {callnameFullPath}\nError: {ex.Message}");
                return;
            }

            PlayFromWavData(wavData, btn, false);
        }
        private void AddFromWav(string workingDirectory, ref string parameter, string openDialogTitle)
        {
            string newFilePath;
            byte[] wavData;
            byte[] adxData;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} {openDialogTitle} Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            try
            {
                wavData = File.ReadAllBytes(newFilePath);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to read {newFilePath}\nError: {ex.Message}");
                return;
            }
            try
            {
                adxData = ADX.FromWav(wavData, false);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to convert {newFilePath} into adx\nError: {ex.Message}");
                return;
            }
            string fileName = Path.GetFileName(newFilePath);
            string adxFilePath = Path.ChangeExtension(Path.Combine(workingDirectory, fileName), ".adx");
            try
            {
                File.WriteAllBytes(adxFilePath, adxData);

            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to create {adxFilePath}\nError: {ex.Message}");
                return;
            }
            parameter = adxFilePath;
        }
        private void PlayFromWavData(byte[] wavData, Button btn, bool loop)
        {
            if (isPlaying)
            {
                StopPlaying(btn);
                return;
            }
            using (MemoryStream wavStream = new MemoryStream(wavData))
            {
                soundPlayer.Stream = wavStream;
                if (loop)
                {
                    soundPlayer.PlayLooping();
                }
                else
                {
                    soundPlayer.Play();
                }
                isPlaying = true;

                btn.Text = "Stop";
            }
        }
        private void StopPlaying(Button btn)
        {
            if (soundPlayer != null && soundPlayer.Stream != null)
            {
                soundPlayer.Stop();
                soundPlayer.Stream.Dispose();
                soundPlayer.Stream = null;
            }

            isPlaying = false;
            btn.Text = "Play";
        }
        private void btnPlayerFaceClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.FacePath = "";
            LoadPlayerInfo(player);
        }
        private void btnPlayerHairClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.HairPath = "";
            LoadPlayerInfo(player);
        }
        private void btnPlayerBootClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.BootPath = "";
            LoadPlayerInfo(player);
        }
        private void btnPlayerSkinClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.SkinPath = "";
            LoadPlayerInfo(player);
        }
        private void btnPlayerWinterGlovesClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.GlovesPath = "";
            LoadPlayerInfo(player);
        }
        private void btnGKLeftGloveClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.GKLeftGlovePath = "";
            LoadPlayerInfo(player);
        }
        private void btnGKRightGloveClear_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            player.GKRightGlovePath = "";
            LoadPlayerInfo(player);
        }
        private void btnPlayerCallnamesConfigure_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            Utils.ShowInfo("Sorry not available yet! Wait for callname module update");

        }
        private void btnPlayerFaceSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("faces\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFilePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Faces Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            if (!Utils.IsSubPath(workingDirectory, newFilePath))
            {
                try
                {
                    newFilePath = Utils.CopyToFolder(workingDirectory, newFilePath);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
                // this is optional, the png file could or could not be there and we dont care about the new location
                try
                {
                    Utils.CopyToFolder(workingDirectory, Path.ChangeExtension(newFilePath, ".png"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            try
            {
                player.FacePath = Utils.GetRelativePath(newFilePath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            LoadPlayerInfo(player);
        }
        private void btnPlayerHairSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("hair\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFilePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Hairs Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            if (!Utils.IsSubPath(workingDirectory, newFilePath))
            {
                try
                {
                    newFilePath = Utils.CopyToFolder(workingDirectory, newFilePath);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
                // this is optional, the png file could or could not be there and we dont care about the new location of it
                try
                {
                    Utils.CopyToFolder(workingDirectory, Path.ChangeExtension(newFilePath, ".png"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            try
            {
                player.HairPath = Utils.GetRelativePath(newFilePath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            LoadPlayerInfo(player);
        }
        private void btnPlayerBootSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("boots\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref player.BootPath, "Boots");
            LoadPlayerInfo(player);
        }
        private void btnPlayerSkinSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("skins\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref player.SkinPath, "Skins");
            LoadPlayerInfo(player);
        }
        private void btnPlayerWinterGlovesSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("skins\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref player.GlovesPath, "Winter Gloves");
            LoadPlayerInfo(player);

        }
        private void btnGKLeftGloveSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("gloves\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref player.GKLeftGlovePath, "GK Left Glove");

            LoadPlayerInfo(player);

        }
        private void btnGKRightGloveSelect_Click(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            string workingDirectory = gdb.GetGDBPath("gloves\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref player.GKRightGlovePath, "GK Right Glove");
            LoadPlayerInfo(player);

        }
        private void btnTeamChantsPlay_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            if (string.IsNullOrEmpty(team.ChantsFolder) && cboTeamChants.SelectedItem == null) return;

            string chantFullPath = gdb.GetGDBPath(Path.Combine($"chants\\{team.ChantsFolder}", cboTeamChants.SelectedItem.ToString()));
            if (!File.Exists(chantFullPath)) return;
            byte[] wavData;
            try
            {
                byte[] adxData = File.ReadAllBytes(chantFullPath);
                wavData = ADX.ToWav(adxData);

            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error while trying to play file {chantFullPath}\nError: {ex.Message}");
                return;
            }

            PlayFromWavData(wavData, btnTeamChantsPlay, true);
        }
        private void btnTeamBallClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.HomeBall = null;
            cboTeamBall.SelectedItem = null;
            picTeamBall.Image = null;
        }
        private void btnTeamStadiumClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.HomeStadium = "";
            cboTeamStadium.SelectedItem = null;
            picTeamStadium.Image = null;

        }
        private void btnTeamKitsClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.KitsPath = "";
            txtTeamKits.Text = "";
            cboTeamKits.SelectedItem = null;
            cboTeamKits.Items.Clear();
            cboTeamKits.DropDownHeight = 106;
        }
        private void btnTeamBannersClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.BannerFolder = "";
            team.supporterColour = null;
            txtTeamBanners.Text = "";
            cboTeamBanners.SelectedItem = null;
            cboTeamBanners.Items.Clear();
            cboTeamBanners.DropDownHeight = 106;
            picTeamBanners.Image = null;
            cboTeamSuppMainColour.SelectedItem = null;
            cboTeamSuppSecColour.SelectedItem = null;
            cboTeamSuppMainColour.Enabled = false;
            cboTeamSuppSecColour.Enabled = false;

        }
        private void btnTeamChantsClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.ChantsFolder = "";
            txtTeamChants.Text = "";
            cboTeamChants.SelectedItem = null;
            cboTeamChants.Items.Clear();
            cboTeamChants.DropDownHeight = 106;

        }
        private void btnTeamKitsSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("uni\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFolderPath;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = workingDirectory;
                if (fbd.ShowDialog() != DialogResult.OK) return;
                newFolderPath = fbd.SelectedPath;
            }

            if (!Utils.IsSubDirectory(workingDirectory, newFolderPath))
            {
                try
                {
                    newFolderPath = Utils.CopyFolder(newFolderPath, workingDirectory);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
            }
            try
            {
                team.KitsPath = Utils.GetRelativePath(newFolderPath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            LoadTeamInfo(team);

        }
        private void btnTeamBannersSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("banners\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFolderPath;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = workingDirectory;
                if (fbd.ShowDialog() != DialogResult.OK) return;
                newFolderPath = fbd.SelectedPath;
            }

            if (!Utils.IsSubDirectory(workingDirectory, newFolderPath))
            {
                try
                {
                    newFolderPath = Utils.CopyFolder(newFolderPath, workingDirectory);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
            }
            try
            {
                team.BannerFolder = Utils.GetRelativePath(newFolderPath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            string supporterColourFilePath = Path.Combine(gdb.GetGDBPath($"banners\\{team.BannerFolder}"), "colours.txt");
            team.supporterColour = new SupporterColour();

            gdb.ReadSupporterColour(team, supporterColourFilePath);

            LoadTeamInfo(team);
        }
        private void btnTeamChantsSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("chants\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFolderPath;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = workingDirectory;
                if (fbd.ShowDialog() != DialogResult.OK) return;
                newFolderPath = fbd.SelectedPath;
            }

            if (!Utils.IsSubDirectory(workingDirectory, newFolderPath))
            {
                try
                {
                    newFolderPath = Utils.CopyFolder(newFolderPath, workingDirectory);

                }
                catch (Exception ex)
                {
                    Utils.ShowError(ex.Message);
                    return;
                }
            }
            try
            {
                team.ChantsFolder = Utils.GetRelativePath(newFolderPath, workingDirectory);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            LoadTeamInfo(team);
        }
        private void btnTeamBannersAdd_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || string.IsNullOrEmpty(team.BannerFolder)) return;
            string workingDirectory = gdb.GetGDBPath($"banners\\{team.BannerFolder}\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFilePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Banner Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            if (Utils.IsSubPath(workingDirectory, newFilePath))
            {
                Utils.ShowInfo($"The file {newFilePath} already exist in this banners folder");
                return;
            }

            try
            {
                newFilePath = Utils.CopyToFolder(workingDirectory, newFilePath);

            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            
            LoadTeamInfo(team);
        }
        private void btnTeamBannersRemove_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || string.IsNullOrEmpty(team.BannerFolder) || cboTeamBanners.SelectedItem == null) return;
            string workingDirectory = gdb.GetGDBPath($"banners\\{team.BannerFolder}\\");

            string bannerFullPath = Path.Combine(workingDirectory, cboTeamBanners.SelectedItem.ToString());
            DialogResult result = MessageBox.Show($"Are you sure you want to delete the file {bannerFullPath}?", $"{Text}", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            try
            {
                File.Delete(bannerFullPath);
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to delete {bannerFullPath}\nError: {ex.Message}");
                return;
            }
            LoadTeamInfo(team);
            Utils.ShowInfo($"File {bannerFullPath} deleted successfully");
        }
        private void btnTeamChantsRemove_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || string.IsNullOrEmpty(team.ChantsFolder) || cboTeamChants.SelectedItem == null) return;
            string workingDirectory = gdb.GetGDBPath($"chants\\{team.ChantsFolder}\\");

            string chantFullPath = Path.Combine(workingDirectory, cboTeamChants.SelectedItem.ToString());
            DialogResult result = MessageBox.Show($"Are you sure you want to delete the file {chantFullPath}?", $"{Text}", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            try
            {
                File.Delete(chantFullPath);
            }
            catch (Exception ex)
            {

                Utils.ShowError($"An error has ocurred while trying to delete {chantFullPath}\nError: {ex.Message}");
                return;
            }
            LoadTeamInfo(team);
            Utils.ShowInfo($"File {chantFullPath} deleted successfully");
        }
        private void btnTeamChantsAdd_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || string.IsNullOrEmpty(team.ChantsFolder)) return;
            string workingDirectory = gdb.GetGDBPath($"chants\\{team.ChantsFolder}\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFilePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Chant Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            if (Utils.IsSubPath(workingDirectory, newFilePath))
            {
                Utils.ShowInfo($"The file {newFilePath} already exist in this chants folder");
                return;
            }
            try
            {
                newFilePath = Utils.CopyToFolder(workingDirectory, newFilePath);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
                return;
            }
            LoadTeamInfo(team);
        }
        private void btnTeamChantsAddFromWav_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null || string.IsNullOrEmpty(team.ChantsFolder)) return;
            string workingDirectory = gdb.GetGDBPath($"chants\\{team.ChantsFolder}\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            string newFilePath;
            byte[] wavData;
            byte[] adxData;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = $"{Text} Chant Browser";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Filter = "All Files (*.*)|*.*";
                ofd.InitialDirectory = workingDirectory;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                newFilePath = ofd.FileName;
            }
            try
            {
                wavData = File.ReadAllBytes(newFilePath);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to read {newFilePath}\nError: {ex.Message}");
                return;
            }
            try
            {
                adxData = ADX.FromWav(wavData, false);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to convert {newFilePath} into adx\nError: {ex.Message}");
                return;
            }
            string fileName = Path.GetFileName(newFilePath);
            string adxFilePath = Path.ChangeExtension(Path.Combine(workingDirectory, fileName), ".adx");
            try
            {
                File.WriteAllBytes(adxFilePath, adxData);

            }
            catch (Exception ex)
            {
                Utils.ShowError($"An error has ocurred while trying to create {adxFilePath}\nError: {ex.Message}");
                return;
            }
            LoadTeamInfo(team);
            Utils.ShowInfo($"File converted from wav into adx successfully, saved on: {adxFilePath}");
        }
        private void btnTeamKitsConfigure_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            Utils.ShowInfo("Sorry not available yet!");
        }
        private void btnTeamSupporterFlagSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("flags\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.SupporterFlagPath, "Supporter Flag");
            LoadTeamInfo(team);
        }
        private void btnTeamSupporterFlagClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.SupporterFlagPath = "";
            txtTeamSupporterFlag.Text = team.SupporterFlagPath;

        }
        private void btnTeamFlagSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("flags\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.FlagPath, "Flags");
            LoadTeamInfo(team);
        }
        private void btnTeamFlagClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.FlagPath = "";
            txtTeamFlag.Text = team.FlagPath;

        }
        private void btnTeamSmallFlagSelect_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath("flags\\");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.SmallFlagPath, "Flag");

            LoadTeamInfo(team);
        }
        private void btnTeamSmallFlagClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.SmallFlagPath = "";
            txtTeamSmallFlag.Text = team.SmallFlagPath;

        }
        private void btnTeamCallnameNormalPlay_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            PlayCallname(team.CallnameNormal, btnTeamCallnameNormalPlay);
        }
        private void btnTeamCallnameVsPlay_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            PlayCallname(team.CallnameVs, btnTeamCallnameVsPlay);
        }
        private void btnTeamCallnameLoudPlay_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            PlayCallname(team.CallnameLoud, btnTeamCallnameLoudPlay);
        }
        private void btnTeamCallnameNormalClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.CallnameNormal = "";
            txtTeamCallnameNormal.Text = team.CallnameNormal;
        }
        private void btnTeamCallnameVsClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.CallnameVs = "";
            txtTeamCallnameVs.Text = team.CallnameVs;
        }
        private void btnTeamCallnameLoudClear_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;

            team.CallnameLoud = "";
            txtTeamCallnameLoud.Text = team.CallnameLoud;
        }
        private void btnTeamCallnameNormalAdd_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.CallnameNormal, "Callnames");
            LoadTeamInfo(team);
        }
        private void btnTeamCallnameVsAdd_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.CallnameVs, "Callnames");
            LoadTeamInfo(team);
        }
        private void btnTeamCallnameLoudAdd_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            SelectFileFromFolder(workingDirectory, ref team.CallnameLoud, "Callnames");
            LoadTeamInfo(team);
        }
        private void btnTeamCallnameNormalAddFromWav_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            AddFromWav(workingDirectory, ref team.CallnameNormal, "Callnames");

        }
        private void btnTeamCallnameVsAddFromWav_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            AddFromWav(workingDirectory, ref team.CallnameVs, "Callnames");
        }
        private void btnTeamCallnameLoudAddFromWav_Click(object sender, EventArgs e)
        {
            Team team = GetTeam();
            if (team == null) return;
            string workingDirectory = gdb.GetGDBPath($"callnames");
            if (!Directory.Exists(workingDirectory))
            {
                Utils.ShowError($"The folder {workingDirectory} doesn't exist inside GDB folder, please create it");
                return;
            }
            AddFromWav(workingDirectory, ref team.CallnameLoud, "Callnames");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
    "PES5/WE9/LE GDB Manager Help\n\n" +
    "In case of having any doubts please go it https://github.com/moth1995/PES5_WE9_LE_GDB_Manager/wiki.\n\n",
    "Help",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
    "PES5/WE9/LE GDB Manager\n\n" +
    "Version: 1.0.0\n\n" +
    "Developed by: marqisspes5\n\n" +
    "Description:\n" +
    "PES5/WE9/LE GDB Manager is an application for managing the graphical database of Pro Evolution Soccer 5, Winning Eleven 9, and Winning Eleven 9 LE.\n\n",
    "About",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
        }
    }
}
