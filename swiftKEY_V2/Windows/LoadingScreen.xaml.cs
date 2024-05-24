using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace swiftKEY_V2
{
    public partial class LoadingScreen : Window
    {
        private MainWindow mainWindow;

        public LoadingScreen()
        {
            InitializeComponent();
            Loaded += LoadingScreen_Loaded;
        }

        private async void LoadingScreen_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = new MainWindow();
            progressBar.Maximum = 50;
            OpenCOMPort();

            for(int i = 0; i < 50; i++)
            {
                progressBar.Value = i;
                await Task.Delay(1);
            }

            mainWindow.Show();
            Close();
        }

        private void OpenCOMPort()
        {
            MainWindow.serialPort = FindCOMPort();
            if (MainWindow.serialPort != null)
            {
                MainWindow.serialPort.Open();
                MainWindow.serialPort.DataReceived += new SerialDataReceivedEventHandler(mainWindow.DataReceivedHandler);
            }
            else
            {
                MainWindow.serialPort = FindCOMPort();
                if (MainWindow.serialPort != null)
                {
                    MainWindow.serialPort.Open();
                    MainWindow.serialPort.DataReceived += new SerialDataReceivedEventHandler(mainWindow.DataReceivedHandler);
                }
                else
                {
                    MessageBox.Show("Es wurde kein passendes Gerät erkannt!");
                }
            }
        }

        private SerialPort FindCOMPort()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                try
                {
                    using (SerialPort testPort = new SerialPort(port))
                    {
                        testPort.BaudRate = 115200;
                        testPort.Open();
                        testPort.WriteLine("keySWIFT >> looking for SmartPAD");

                        DateTime startTime = DateTime.Now;
                        while ((DateTime.Now - startTime).TotalMilliseconds < 50)
                        {
                            if (testPort.BytesToRead > 0)
                            {
                                string response = testPort.ReadExisting();
                                Console.WriteLine(response);
                                if (response.Contains("SmartPAD >> connected to keySWIFT"))
                                {
                                    testPort.Close();
                                    return testPort;
                                }
                            }
                            Thread.Sleep(10);
                        }
                        testPort.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
    }
}