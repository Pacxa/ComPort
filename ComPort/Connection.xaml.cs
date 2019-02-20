using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComPort
{
    /// <summary>
    /// Логика взаимодействия для Connection.xaml
    /// </summary>
    public partial class Connection : Window
    {
        bool flag = false;
        public string com;
        public int comSpeed;
        public SerialPort port = new SerialPort();
        private string[] ports;
        private int[] rates = new int[]{4800, 9600, 19200, 38400, 57600, 115200};

        public Connection()
        {
            InitializeComponent();
            ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                comPort.Items.Add(ports[i]);
                comList.Items.Add(ports[i]);
            }
            for (int i = 0; i < rates.Length; i++)
            {
                comRate.Items.Add(rates[i]);
            }
            comRate.SelectedIndex = 4;
            comPort.SelectedIndex = 0;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            com = comPort.SelectedValue.ToString();
            comSpeed = Int32.Parse(comRate.SelectedValue.ToString());
            MainWindow mainWindow = new MainWindow();
            mainWindow.port.ReadTimeout = 5000;
            mainWindow.port = new SerialPort(com, comSpeed);
            try
            {
                flag = false;
                mainWindow.port.Open();
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                flag = true;
                MessageBox.Show(ex.Message);
            }
            if (flag == false)
            {
                mainWindow.Show();
                mainWindow.getStatuses();
                this.Hide();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
