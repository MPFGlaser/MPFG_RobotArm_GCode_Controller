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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MPFG_RobotArm_GCode_Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort robotSerial = new SerialPort("COM5", 9600);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendCommand(CommandField.Text.ToString());
        }

        private void SendCommand(string command)
        {
            string commandFixed = command.ToUpper();
            if (!robotSerial.IsOpen)
            {
                robotSerial.Open();
            }
            robotSerial.Write(commandFixed + "\r");
            CommandField.Text = "";
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendCommand(CommandField.Text);
            }
        }
    }
}
