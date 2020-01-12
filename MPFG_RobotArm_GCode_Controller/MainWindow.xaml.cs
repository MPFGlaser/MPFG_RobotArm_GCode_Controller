using MPFG_RobotArm_GCode_Controller.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        int stepValue = 2;
        bool useBigStep = false;

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
            CurrentXYZDisplay.Text = "X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ;
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
            SendCommand("G1 " + homePosition);
        }

        private void DisablePower(object sender, RoutedEventArgs e)
        {
            SendCommand("M18");
        }

        private void SmallStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(SmallStepSize.Text.Length > 0)
            {
                Int32.TryParse(SmallStepSize.Text, out int newStepSize);
                smallStep = newStepSize;
            }
        }

        private void BigStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(BigStepSize.Text.Length > 0)
            {
                Int32.TryParse(BigStepSize.Text, out int newStepSize);
                bigStep = newStepSize;
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var typedSender = (Button) sender;
            var parameter = (ButtonAction)typedSender.CommandParameter;
            switch (parameter)
            {
                case ButtonAction.XPos:
                    currentPosX = currentPosX - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.XNeg:
                    currentPosX = currentPosX - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.YPos:
                    currentPosY = currentPosY + stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.YNeg:
                    currentPosY = currentPosY - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.ZPos:
                    currentPosZ = currentPosZ + stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.ZNeg:
                    currentPosZ = currentPosZ - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ);
                    break;
                case ButtonAction.Home:
                    currentPosX = 0;
                    currentPosY = 120;
                    currentPosZ = 120;
                    SendCommand("G1 X0 Y120 Z120");
                    break;
                default:
                    break;
            }
        }

        private void EnableBigSteps(object sender, RoutedEventArgs e)
        {
            stepValue = bigStep;
        }

        private void DisableBigSteps(object sender, RoutedEventArgs e)
        {
            stepValue = smallStep;
        }

        public int stepSize()
        {
            if (checkBoxUseBigSteps.IsChecked == true)
            {
                return bigStep;
            }
            else
            {
                return smallStep;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
