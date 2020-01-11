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
        string homePosition = "X0 Y120 Z120";
        double currentPosX = 0;
        double currentPosY = 120;
        double currentPosZ = 120;
        int smallStep = 2;
        int bigStep = 10;
        string serialLog;

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
            serialLog = serialLog + "\n" + robotSerial.ReadLine().ToString();
            SerialLog.Text = serialLog;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendCommand(CommandField.Text);
            }
        }

        private void EnablePower(object sender, RoutedEventArgs e)
        {
            SendCommand("M17");
            SendCommand("G1" + homePosition);
        }

        private void DisablePower(object sender, RoutedEventArgs e)
        {
            SendCommand("M18");
        }

        private void SmallStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                smallStep = Int32.Parse(SmallStepSize.Text);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BigStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bigStep = Int32.Parse(BigStepSize.Text);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
