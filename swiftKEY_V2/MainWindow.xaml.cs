using System;
using System.Windows;
using System.IO.Ports;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace swiftKEY_V2
{
    public class FunctionDictionary
    {
        public string Name { get; set; }
        public string Function { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<FunctionDictionary> dictionary = new List<FunctionDictionary>();
        public List<string> COMPorts { get; set; } = new List<string>();
        public static int buttonAmount = 15;
        private ButtonConfig config;
        private SerialPort serialPort;
        private const string version = "swiftKEY 2.0.6.2";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            dictionary = new List<FunctionDictionary>
            {
                new FunctionDictionary { Name = "Hotkey", Function = "hotkey" },
                new FunctionDictionary { Name = "Increase Volume", Function = "volumeup" },
                new FunctionDictionary { Name = "Decrease Volume", Function = "volumedown" },
                new FunctionDictionary { Name = "Mute Volume", Function = "volumemute" },
                new FunctionDictionary { Name = "Shutdown", Function = "shutdown" },
                new FunctionDictionary { Name = "Restart", Function = "restart" },
                new FunctionDictionary { Name = "Lock", Function = "lock" },
                new FunctionDictionary { Name = "Open File", Function = "openfile" },
                new FunctionDictionary { Name = "Open Folder", Function = "openfolder" }
            };

            config = ConfigManager.LoadConfig();    // Load config
            LoadData();                             // Load Data
        }

        private void LoadData()
        {
            lbl_version.Content = version;
            cellText1.Text = config.ButtonConfigurations[0].Name;
            cellText2.Text = config.ButtonConfigurations[1].Name;
            cellText3.Text = config.ButtonConfigurations[2].Name;
            cellText4.Text = config.ButtonConfigurations[3].Name;
            cellText5.Text = config.ButtonConfigurations[4].Name;
            cellText6.Text = config.ButtonConfigurations[5].Name;
            cellText7.Text = config.ButtonConfigurations[6].Name;
            cellText8.Text = config.ButtonConfigurations[7].Name;
            cellText9.Text = config.ButtonConfigurations[8].Name;
            cellText10.Text = config.ButtonConfigurations[9].Name;
            cellText11.Text = config.ButtonConfigurations[10].Name;
            cellText12.Text = config.ButtonConfigurations[11].Name;
            cellText13.Text = config.ButtonConfigurations[12].Name;
            cellText14.Text = config.ButtonConfigurations[13].Name;
            cellText15.Text = config.ButtonConfigurations[14].Name;
        }

        #region COM-PORTS & DATAHANDLER
        private SerialPort OpenCOMPort(String port, int baudRate)
        {
            SerialPort serialPort = new SerialPort(port, baudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Öffnen der seriellen Verbindung: " + ex.Message);
            }
            return serialPort;
        }
        private void CloseCOMPort(SerialPort serialPort)
        {
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
        private void UpdateCOMPorts()
        {
            COMPorts.Clear();
            string[] ports = SerialPort.GetPortNames();

            if (ports != null && ports.Length > 0)
            {
                foreach (string port in ports)
                {
                    COMPorts.Add(port);
                }
                cb_COMPorts.ItemsSource = COMPorts;
            }
            else
            {
                MessageBox.Show("Es wurden keine COM-Ports gefunden.");
            }
        }
        private void cb_COMPorts_SelectionChanged(object sender, EventArgs e)
        {
            if (serialPort != null)
                CloseCOMPort(serialPort);

            if (cb_COMPorts.SelectedItem == null || cb_COMPorts.SelectedItem.ToString() == null)
                return;

            string selectedPort = cb_COMPorts.SelectedItem.ToString();
            if (!SerialPort.GetPortNames().Contains(selectedPort))
            {
                MessageBox.Show("Der ausgewählte COM-Port ist nicht mehr verfügbar.");
                cb_COMPorts.SelectedItem = null;
                return;
            }

            serialPort = OpenCOMPort(selectedPort, 115200);
        }
        private void cb_COMPorts_DropDownOpened(object sender, EventArgs e)
        {
            UpdateCOMPorts();
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();

            for (int i = 1; i <= buttonAmount; i++)
            {
                if (data.Equals("SmartPAD_key" + i + "_pressed\r"))
                {
                    config = ConfigManager.LoadConfig();
                    EventHandler.FetchFunction(config.ButtonConfigurations[i-1].Function);
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
                config.ButtonConfigurations[btn].Name = droppedText;
                config.ButtonConfigurations[btn].Title = droppedText;

                foreach (var item in dictionary)
                {
                    if (item.Name == droppedText)
                    {
                        config.ButtonConfigurations[btn].Function = item.Function;
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
                        expander2.IsExpanded = true; // Expand the expander if there's a match
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

            if (config.ButtonConfigurations[btnIndex].Title.ToLower() == "hotkey")
            {
                HotkeySettingsWindow hotkeySettingsWindow = new HotkeySettingsWindow(btnIndex);
                hotkeySettingsWindow.Closed += ModalWindow_Closed;
                hotkeySettingsWindow.ShowDialog();
            }
            else if (config.ButtonConfigurations[btnIndex].Title.ToLower() == "increase volume" || 
                config.ButtonConfigurations[btnIndex].Title.ToLower() == "decrease volume")
            {
                VolumeSettingsWindow volumeSettingsWindow = new VolumeSettingsWindow(btnIndex);
                volumeSettingsWindow.Closed += ModalWindow_Closed;
                volumeSettingsWindow.ShowDialog();
            }
            else if(config.ButtonConfigurations[btnIndex].Title.ToLower() == "open file" ||
                config.ButtonConfigurations[btnIndex].Title.ToLower() == "open folder")
            {
                OpenFileSettingsWindow openFileSettingsWindow = new OpenFileSettingsWindow(btnIndex);
                openFileSettingsWindow.Closed += ModalWindow_Closed;
                openFileSettingsWindow.ShowDialog();
            }
            else
            {
                if(config.ButtonConfigurations[btnIndex].Title.ToLower() == "" || config.ButtonConfigurations[btnIndex].Name.ToLower() == "")
                    return;

                DefaultSettingsWindow defaultSettingsWindow = new DefaultSettingsWindow(btnIndex);
                defaultSettingsWindow.Closed += ModalWindow_Closed;
                defaultSettingsWindow.ShowDialog();
            }
        }

        private void ModalWindow_Closed(object sender, EventArgs e)
        {
            config = ConfigManager.LoadConfig();
            LoadData();
        }
        #endregion
    }
}
