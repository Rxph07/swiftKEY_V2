using System;
using System.Collections.Generic;
using System.Windows;
using System.IO.Ports;
using System.Linq;

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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            config = ConfigManager.LoadConfig();    // Load config
            //LoadButtonFunctions();                // Display config functions (TODO)

            UpdateCOMPorts();
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
            string[] ports = SerialPort.GetPortNames();

            if (ports != null && ports.Length > 0)
            {
                COMPorts.Clear();
                COMPorts.AddRange(ports);
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

            if (cb_COMPorts.SelectedItem.ToString() == null)
                return;

            string selectedPort = cb_COMPorts.SelectedItem.ToString();

            if (!SerialPort.GetPortNames().Contains(selectedPort))
            {
                MessageBox.Show("Der ausgewählte COM-Port ist nicht mehr verfügbar.");
                cb_COMPorts.SelectedItem = null;
                return;
            }

            serialPort = OpenCOMPort(selectedPort, 115200);
            MessageBox.Show("Der COM-Port \"" + selectedPort + "\" wurde erfolgreich ausgewählt.");
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
    }
}
