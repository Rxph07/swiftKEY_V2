using System;
using System.Windows;
using System.IO.Ports;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace swiftKEY_V2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> COMPorts { get; set; } = new List<string>();
        public static int buttonAmount = 15;
        private ButtonConfig config;
        private SerialPort serialPort;
        private const string version = "swiftKEY 2.0.1.0";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            config = ConfigManager.LoadConfig();    // Load config
            LoadData();                             // Load Data
        }

        private void LoadData()
        {
            lbl_version.Content = version;
            cellText1.Text = config.ButtonFunctions["Button1"];
            cellText2.Text = config.ButtonFunctions["Button2"];
            cellText3.Text = config.ButtonFunctions["Button3"];
            cellText4.Text = config.ButtonFunctions["Button4"];
            cellText5.Text = config.ButtonFunctions["Button5"];
            cellText6.Text = config.ButtonFunctions["Button6"];
            cellText7.Text = config.ButtonFunctions["Button7"];
            cellText8.Text = config.ButtonFunctions["Button8"];
            cellText9.Text = config.ButtonFunctions["Button9"];
            cellText10.Text = config.ButtonFunctions["Button10"];
            cellText11.Text = config.ButtonFunctions["Button11"];
            cellText12.Text = config.ButtonFunctions["Button12"];
            cellText13.Text = config.ButtonFunctions["Button13"];
            cellText14.Text = config.ButtonFunctions["Button14"];
            cellText15.Text = config.ButtonFunctions["Button15"];
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
                    EventHandler.FetchFunction(config.ButtonFunctions["Button" + i]);
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
                string btnName = "Button";

                if (row == 0)
                    btnName += $"{column+1}";
                else if (row == 1)
                    btnName += $"{column+6}";
                else
                    btnName += $"{column+11}";

                ((TextBlock) ((Border) sender).Child).Text = droppedText;
                ((TextBlock) ((Border) sender).Child).Visibility = Visibility.Visible;
                config.ButtonFunctions[btnName] = droppedText;
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
                        expander1.IsExpanded = true; // Expand the expander if there's a match
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

        #region PopUpMenuHandler
        private void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            // TODO IN FUTURE
        }
        #endregion
    }
}
