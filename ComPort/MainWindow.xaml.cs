using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Pirit;

namespace ComPort
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string sd = "";
        private int currID = 35;
        private string param = "";
        private string result = "";
        private string toMessage = "";
        private string sendMessage = "";
        private string password = "PIRI";
        private string st = byteToString(STX);
        private string et = byteToString(ETX);
        private string sep = byteToString(Separator);
        private static byte[] STX = new byte[] { 0x02 };
        private static byte[] ETX = new byte[] { 0x03 };
        private static byte[] Separator = new byte[] { 0x1C };
        Commands commands = new Commands();
        public SerialPort port = new SerialPort();
        Connection conn = new Connection();
        public MainWindow()
        {
            InitializeComponent();
            comN.Focus();
        }

        public void getStatuses()
        {
            //port.Open();
            result = "";
            toMessage = password + Convert.ToChar(currID) + "00" + et;
            sd = st + toMessage + getBCC(stringToByte(toMessage)).ToString("X");
            port.WriteLine(sd);
            System.Threading.Thread.Sleep(100);
            result = port.ReadExisting().Replace("\u001c", " ").Replace("\u0002", "").Replace("\u0003", "");
            statKSA.Items.Add(commands.statusKSA(result));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            tb2.Text = "";
            try
            {
                sendMessage += Convert.ToString(tb1.Text);
                if (currID < 47)
                {
                    currID++;
                }
                else currID = 33;

                param = "";
                if (tbP0.Text != "") param += tbP0.Text + sep;
                if (tbP1.Text != "") param += tbP1.Text + sep;
                if (tbP2.Text != "") param += tbP2.Text + sep;
                if (tbP3.Text != "") param += tbP3.Text + sep;
                if (tbP4.Text != "") param += tbP4.Text + sep;
                if (tbP5.Text != "") param += tbP5.Text + sep;
                if (tbP6.Text != "") param += tbP6.Text + sep;
                if (tbP7.Text != "") param += tbP7.Text + sep;
                if (tbP8.Text != "") param += tbP8.Text + sep;
                if (tbP9.Text != "") param += tbP9.Text + sep;
                if (tbP10.Text != "") param += tbP10.Text + sep;
                if (tbP11.Text != "") param += tbP11.Text + sep;
                if (tbP12.Text != "") param += tbP12.Text + sep;
                if (tbP13.Text != "") param += tbP13.Text + sep;
                if (tbP14.Text != "") param += tbP14.Text + sep;
                if (param != "")
                {
                    toMessage = password + Convert.ToChar(currID) + comN.Text + param + et;
                }
                else
                    toMessage = password + Convert.ToChar(currID) + comN.Text + et;

                sd = st + toMessage + getBCC(stringToByte(toMessage)).ToString("X");
                tb1.Text += "=>" + sd + "\n";
                
                try
                {
                    for (int i = 0; i < Int32.Parse(tbRep.Text); i++)
                        port.WriteLine(sd);
                }catch(Exception ex)
                {
                    MessageBox.Show("buttonClick_execCommand: " + ex.Message);
                }

                try
                {
                    while (tb2.Text == "")
                    {
                        result = port.ReadExisting().Replace("\u001c", " ").Replace("\u0002", "").Replace("\u0003", "");
                        tb2.Text = result.Replace("\u001c", "◘");
                    }
                    tb1.Text += "<=" + result.Replace("\u001c", "◘") + "\n";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("buttonClick_getResult: " + ex.Message);
                }
                statKSA.Items.Clear();
                getStatuses();
            }
            catch (Exception ex) { MessageBox.Show("buttonClick: " + ex.ToString()); }
        }

        private byte getBCC(byte[] inputStream)
        {
            byte bcc = 0x00;
            if (inputStream != null && inputStream.Length > 0)
            {
                // Exclude SOH during BCC calculation
                for (int i = 0; i < inputStream.Length; i++)
                {
                    bcc ^= inputStream[i];
                }
            }
            return bcc;
        }

        public byte[] stringToByte(string S)
        {
            return System.Text.Encoding.ASCII.GetBytes(S);
        }

        private static string byteToString(byte[] B)
        {
            return System.Text.Encoding.ASCII.GetString(B);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void btMac_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                port.ReadTimeout = 5000;
                
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (ofd.FileName != "")
                    readAndSend(ofd.FileName);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка:" + ex.Message); }
        }


        private void readAndSend(string path)
        {
            try
            {
                int cnt = 0;
                string count = "";
                string[] macros;
                string[] lines;
                bool flag = true;
                string param = "";
                string command = "";

                lines = File.ReadAllLines(path, Encoding.GetEncoding("Windows-1251"));

                for (int i = 0; i < 1; i++)
                {
                    string s = lines[0];
                    if (s.Contains("[FITO Macro]") == true)
                    {
                        if ((cnt = s.IndexOf("Count=")) != 0)    //----------------------------------------------------------------------Есть ли "Count=" есть в строке
                        {
                            for (int j = (cnt + 6); j < s.Length; j++)
                            {
                                if (s[j] != ' ')                //-----------------------------------------------------------------------Запоминаем сколько раз нужно проиграть файл
                                {
                                    count += s[j];
                                }
                                else { break; }
                            }
                        }
                    }
                }

                for (int i = 0; i < Int32.Parse(count); i++)
                {
                    flag = false;
                    foreach (string readFile in lines)
                    {
                        if (readFile.Contains("[FITO Macro]") == true) { /*Пропускаем первую строку*/ }
                        else
                        {
                            command = "";
                            param = "";
                            toMessage = "";

                            if (currID < 47)
                            {
                                currID++;
                            }
                            else currID = 33;

                            macros = readFile.Split(new char[] { ';' });
                            command = macros[0];

                            for (int j = 1; j < macros.Length - 1; j++)  //-----------------------------------------------------------Записываем параметры с разделителем 
                            {
                                param += macros[j].Normalize() + sep;
                            }

                            if (param != "")
                            {
                                toMessage = password + Convert.ToChar(currID) + command + param + et;
                            }
                            else
                                toMessage = password + Convert.ToChar(currID) + command + et;

                            string toM = password + Convert.ToChar(currID) + "00" + et;
                            string sd1 = st + toM + getBCC(stringToByte(toM)).ToString("X");

                            sd = st + toMessage + getBCC(stringToByte(toMessage)).ToString("X");

                            execCommand(sd);  //-------------------------------------------------------Отправка команды

                            if (command == "31") //-----------------------------------------------------------------------------------После 31 команды шлем каоманду 00
                            {
                                tb2.Text = "";
                                toMessage = password + "♠" + "03" + "2" + sep + et;
                                sd = st + toMessage + getBCC(stringToByte(toMessage)).ToString("X");
                                tb1.Text += "==>" + sd.Replace("\u001c", "◘") + "\n";
                                port.WriteLine(sd);
                                while (tb2.Text == "")
                                {
                                    result = port.ReadExisting().Replace("\u001c", " ").Replace("\u0002", "").Replace("\u0003", "");
                                    tb2.Text = "<==" + result;
                                }
                                tb1.Text += "<== " + result.Replace("\u001c", "◘") + "\n";
                            }
                        }
                    }
                }
                statKSA.Items.Clear();
                getStatuses();
                MessageBox.Show("Sended");
            }
            catch (Exception ex)
            {
                MessageBox.Show("readAndSend: " + ex.Message);
            }
        }

        private void execCommand(string sd)
        {
            try
            {
                tb2.Text = "";

                port.WriteLine(sd);
                tb1.Text += "=>" + sd.Replace("\u001c", "◘") + "\n";

                while (tb2.Text == "")
                {
                    result = port.ReadExisting().Replace("\u001c", " ").Replace("\u0002", "").Replace("\u0003", "");
                    tb2.Text = result;
                }

                tb1.Text += "<= " + result.Replace("\u001c", "◘") + "\n";

                System.Threading.Thread.Sleep(50);
            }
            catch(Exception ex) { MessageBox.Show("execCommand: "+ex.Message); }
        }
    }
}
