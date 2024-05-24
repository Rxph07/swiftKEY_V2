using System;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using swiftKEY_V2.Utils;

namespace swiftKEY_V2
{
    public class FunctionDictionary
    {
        public string Name { get; set; }
        public string Function { get; set; }
    }

    public partial class MainWindow : Window
    {
        public static SpotifyAuthentificator spotifyAuth = new SpotifyAuthentificator();
        public List<FunctionDictionary> dictionary = new List<FunctionDictionary>();
        public static SerialPort serialPort;
        public static int buttonAmount = 15;

        private ProfileConfig config;
        private static int selectedProfile = 0;
        private const string version = "Version 2.3.11.7";

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;
            DataContext = this;

            #region Function Dictionary
            dictionary = new List<FunctionDictionary>
            {
                new FunctionDictionary { Name = "Switch Profile", Function = "switchprofile_1" },

                new FunctionDictionary { Name = "Hotkey", Function = "hotkey" },
                new FunctionDictionary { Name = "Increase Volume", Function = "volumeup" },
                new FunctionDictionary { Name = "Decrease Volume", Function = "volumedown" },
                new FunctionDictionary { Name = "Mute Volume", Function = "volumemute" },
                new FunctionDictionary { Name = "Shutdown", Function = "shutdown" },
                new FunctionDictionary { Name = "Restart", Function = "restart" },
                new FunctionDictionary { Name = "Lock", Function = "lock" },
                new FunctionDictionary { Name = "Open File", Function = "openfile" },
                new FunctionDictionary { Name = "Open Folder", Function = "openfolder" },
                new FunctionDictionary { Name = "Open Website", Function = "openwebsite_" },

                new FunctionDictionary { Name = "Play / Pause", Function = "spotifyplaypause" },
                new FunctionDictionary { Name = "Previous Song", Function = "spotifyprevious" },
                new FunctionDictionary { Name = "Next Song", Function = "spotifynext" },
                new FunctionDictionary { Name = "Volume Up", Function = "spotifyvolumeup_10" },
                new FunctionDictionary { Name = "Volume Down", Function = "spotifyvolumedown_10" },
                new FunctionDictionary { Name = "Repeat Mode", Function = "spotifyrepeatmode" },
                new FunctionDictionary { Name = "Shuffle Mode", Function = "spotifyshufflemode" },
                new FunctionDictionary { Name = "Toggle Liked", Function = "spotifytoggleliked" }
            };
            #endregion

            LoadData();                             // Load Data
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if(serialPort != null)
                serialPort.WriteLine("keySWIFT >> disconnected with SmartPAD");
            CloseCOMPort(serialPort);
        }

        #region Load Data
        private void LoadData()
        {
            /* LOADING PROFILES */
            config = ConfigManager.LoadProfileConfig();
            if (config == null)
            {
                ConfigManager.CreateProfileConfig();
                config = ConfigManager.LoadProfileConfig();
            }
            cb_Profiles.Items.Add(config.ProfileConfigurations[0].Name);
            cb_Profiles.SelectedIndex = 0;

            lbl_version.Content = version;
            LoadButtonText();
        }

        public void LoadButtonText()
        {
            cellText1.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[0].Name;
            cellText2.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[1].Name;
            cellText3.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[2].Name;
            cellText4.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[3].Name;
            cellText5.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[4].Name;
            cellText6.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[5].Name;
            cellText7.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[6].Name;
            cellText8.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[7].Name;
            cellText9.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[8].Name;
            cellText10.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[9].Name;
            cellText11.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[10].Name;
            cellText12.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[11].Name;
            cellText13.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[12].Name;
            cellText14.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[13].Name;
            cellText15.Text = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[14].Name;
        }
        #endregion

        #region Profiles
        public static void SetSelectedProfile(int index)
        {
            selectedProfile = index;
        }

        public void UpdateProfiles()
        {
            config = ConfigManager.LoadProfileConfig();
            cb_Profiles.Items.Clear();

            for (int i = 0; i < config.ProfileConfigurations.Count; i++)
            {
                cb_Profiles.Items.Add(config.ProfileConfigurations[i].Name);
            }
            cb_Profiles.Items.Add("Profil hinzufügen");
            cb_Profiles.SelectedIndex = selectedProfile;
        }

        private void cb_Profiles_DropDownOpened(object sender, EventArgs e)
        {
            UpdateProfiles();
        }

        private void cb_Profiles_DropDownClosed(object sender, EventArgs e)
        {
            UpdateProfiles();
        }

        private void cb_Profiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Profiles.SelectedItem != null)
            {
                string selectedItem = cb_Profiles.SelectedItem.ToString();

                if(selectedItem == "Profil hinzufügen")
                {
                    ConfigManager.AddProfile("Profile" + (config.ProfileConfigurations.Count + 1), "Neues Profil");
                    config = ConfigManager.LoadProfileConfig();
                    selectedProfile = config.ProfileConfigurations.Count - 1;
                    cb_Profiles.SelectedIndex = selectedProfile;
                } else
                {
                    selectedProfile = cb_Profiles.SelectedIndex;
                    LoadButtonText();
                }
            }
        }

        private void EditProfile_Click(object sender, EventArgs e)
        {
            ProfileSettingsWindow profileSettingsWindow = new ProfileSettingsWindow(selectedProfile, this);
            profileSettingsWindow.Closed += ProfileSettingsWindow_Closed;
            profileSettingsWindow.Show();
        }

        private void ProfileSettingsWindow_Closed(object sender, EventArgs e)
        {
            UpdateProfiles();
        }
        #endregion

        #region COM-PORTS & DATAHANDLER
        private void CloseCOMPort(SerialPort serialPort)
        {
            if (serialPort == null)
                return;

            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Schließen der seriellen Verbindung: " + ex.Message);
                }
            }
        }
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();

            for (int i = 1; i <= buttonAmount; i++)
            {
                if (data.Equals("SmartPAD_key" + i + "_pressed\r"))
                {
                    config = ConfigManager.LoadProfileConfig();
                    EventHandler.FetchFunction(config.ProfileConfigurations[selectedProfile].ButtonConfigurations[i-1].Function, this);
                }
            }
        }
        #endregion

        #region Drag & Drop Handler / Search Bar Handler
        private void OnDrop(object sender, DragEventArgs e)
        {
            var droppedText = e.Data.GetData(DataFormats.Text) as string;
            if (droppedText != null)
            {
                int row = Grid.GetRow((UIElement)sender);
                int column = Grid.GetColumn((UIElement)sender);
                int btn;

                if (row == 0)
                    btn = column;
                else if (row == 1)
                    btn = column + 5;
                else
                    btn = column + 10;

                ((TextBlock)((Border)sender).Child).Text = droppedText;
                ((TextBlock) ((Border) sender).Child).Visibility = Visibility.Visible;
                config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btn].Name = droppedText;
                config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btn].Title = droppedText;

                foreach (var item in dictionary)
                {
                    if (item.Name == droppedText)
                    {
                        config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btn].Function = item.Function;
                    }
                }

                ConfigManager.SaveConfig(config);
            }
        }
        private void OnDragStart(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TextBlock textBlock = sender as TextBlock;
                if (textBlock != null)
                {
                    DataObject dragData = new DataObject(DataFormats.Text, textBlock.Text);
                    DragDrop.DoDragDrop(textBlock, dragData, DragDropEffects.Copy);
                }
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tb_searchBox.Text.ToLower();

            // Durchsuche List 1
            foreach (StackPanel stackPanel in stackPanel1.Children)
            {
                foreach (TextBlock textBlock in stackPanel.Children)
                {
                    if (textBlock.Text.ToLower().Contains(searchText))
                    {
                        stackPanel.Visibility = Visibility.Visible;
                        expander1.IsExpanded = true;
                    }
                    else
                    {
                        stackPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }

            // Durchsuche List 2
            foreach (StackPanel stackPanel in stackPanel2.Children)
            {
                foreach (TextBlock textBlock in stackPanel.Children)
                {
                    if (textBlock.Text.ToLower().Contains(searchText))
                    {
                        stackPanel.Visibility = Visibility.Visible;
                        expander2.IsExpanded = true;
                    }
                    else
                    {
                        stackPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }

            // Durchsuche List 3
            foreach (StackPanel stackPanel in stackPanel3.Children)
            {
                foreach (TextBlock textBlock in stackPanel.Children)
                {
                    if (textBlock.Text.ToLower().Contains(searchText))
                    {
                        stackPanel.Visibility = Visibility.Visible;
                        expander3.IsExpanded = true;
                    }
                    else
                    {
                        stackPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }

            if (searchText == "" || searchText == null)
            {
                expander1.IsExpanded = false;
                expander2.IsExpanded = false;
                expander3.IsExpanded = false;
            }
        }
        #endregion

        #region ModalHandler
        private void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border == null)
                return;

            TextBlock textBlock = border.Child as TextBlock;
            if (textBlock == null)
                return;

            int btnIndex = int.Parse(border.Name.Substring(4)) - 1;
            string buttonTitle = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Title.ToLower();
            string buttonName = config.ProfileConfigurations[selectedProfile].ButtonConfigurations[btnIndex].Name.ToLower();

            if (buttonTitle == "hotkey")
            {
                HotkeySettingsWindow hotkeySettingsWindow = new HotkeySettingsWindow(btnIndex, selectedProfile);
                hotkeySettingsWindow.Closed += ModalWindow_Closed;
                hotkeySettingsWindow.ShowDialog();
            }
            else if (buttonTitle == "increase volume" || buttonTitle == "decrease volume")
            {
                VolumeSettingsWindow volumeSettingsWindow = new VolumeSettingsWindow(btnIndex, selectedProfile);
                volumeSettingsWindow.Closed += ModalWindow_Closed;
                volumeSettingsWindow.ShowDialog();
            }
            else if(buttonTitle == "open file" || buttonTitle == "open folder")
            {
                OpenFileSettingsWindow openFileSettingsWindow = new OpenFileSettingsWindow(btnIndex, selectedProfile);
                openFileSettingsWindow.Closed += ModalWindow_Closed;
                openFileSettingsWindow.ShowDialog();
            }
            else if (buttonTitle == "open website")
            {
                OpenWebsiteSettingsWindow openWebsiteSettingsWindow = new OpenWebsiteSettingsWindow(btnIndex, selectedProfile);
                openWebsiteSettingsWindow.Closed += ModalWindow_Closed;
                openWebsiteSettingsWindow.ShowDialog();
            }
            else if (buttonTitle == "play / pause" || buttonTitle == "previous song" || buttonTitle == "next song" ||
                buttonTitle == "repeat mode" || buttonTitle == "shuffle mode" || buttonTitle == "toggle liked")
            {
                DefaultSpotifySettingsWindow defaultSpotifySettingsWindow = new DefaultSpotifySettingsWindow(btnIndex, selectedProfile);
                defaultSpotifySettingsWindow.Closed += ModalWindow_Closed;
                defaultSpotifySettingsWindow.ShowDialog();
            }
            else if(buttonTitle == "volume up" || buttonTitle == "volume down")
            {
                SpotifyVolumeSettingsWindow spotifyVolumeSettings = new SpotifyVolumeSettingsWindow(btnIndex, selectedProfile);
                spotifyVolumeSettings.Closed += ModalWindow_Closed;
                spotifyVolumeSettings.ShowDialog();
            }
            else if (buttonTitle == "switch profile")
            {
                SwitchProfileSettingsWindow switchProfileSettingsWindow = new SwitchProfileSettingsWindow(btnIndex, selectedProfile);
                switchProfileSettingsWindow.Closed += ModalWindow_Closed;
                switchProfileSettingsWindow.ShowDialog();
            }
            else
            {
                if(buttonTitle == "" || buttonName == "")
                    return;

                DefaultSettingsWindow defaultSettingsWindow = new DefaultSettingsWindow(btnIndex, selectedProfile);
                defaultSettingsWindow.Closed += ModalWindow_Closed;
                defaultSettingsWindow.ShowDialog();
            }
        }

        private void ModalWindow_Closed(object sender, EventArgs e)
        {
            config = ConfigManager.LoadProfileConfig();
            LoadButtonText();
        }
        #endregion
    }
}