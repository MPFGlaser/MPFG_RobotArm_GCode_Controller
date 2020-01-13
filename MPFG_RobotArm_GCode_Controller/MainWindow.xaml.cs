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
        SerialPort robotSerial;
        string homePosition = "X0 Y120 Z120";
        string restPosition = "X0 Y25 Z63";
        string bottomPosition = "X0 Y131 Z-94";
        double currentPosX = 0;
        double currentPosY = 25;
        double currentPosZ = 63;
        int smallStep = 2;
        int bigStep = 10;
        int waitTime = 5000;
        string serialLog;
        int stepValue = 2;
        bool useBigStep = false;
        int movementSpeed = 100;
        string[] commandQueue;
        string[] comPortNames;


        public MainWindow()
        {
            InitializeComponent();
            comPortNames = SerialPort.GetPortNames();
            comboBoxComPort.ItemsSource = comPortNames;
        }

        public void ConnectSerialCom()
        {
            try
            {
                robotSerial = new SerialPort(comboBoxComPort.SelectedItem.ToString(), int.Parse(comboBoxBaudRate.Text));
                robotSerial.ReadTimeout = 2500;
                robotSerial.WriteTimeout = 2500;
                if (!robotSerial.IsOpen)
                {
                    robotSerial.Open();
                    Log("Connection opened on " + robotSerial.PortName + " running on a baud rate of " + robotSerial.BaudRate);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ConnectionEstablished()
        {
            bool connected = false;
            if (robotSerial.IsOpen)
            {
                connected = true;
            }
            return connected;
        }

        private void comSettings_Changed(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendCommand(CommandField.Text.ToString());
        }

        private void SendCommand(string command)
        {
            string commandFixed = command.ToUpper();
            robotSerial.Write(commandFixed + "\r");
            CommandField.Text = "";
            Log(LogType.tx, command);
            CurrentXYZDisplay.Text = "X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ;
            Log(LogType.rx, robotSerial.ReadLine().ToString());
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
            ConnectSerialCom();
            if (ConnectionEstablished())
            {
                SendCommand("G1 " + restPosition);
                SendCommand("M17");
            }
            else
            {
                // this doesn't fire even if wrong comport is selected, because it will still be open. Need to check for identification perhaps?
                CheckboxPower.IsChecked = false;
                Log(LogType.ERROR, "Connecting failed.\nHave you selected the correct\nCOM port and baud rate?");
            }

        }

        private void DisablePower(object sender, RoutedEventArgs e)
        {
            SendCommand("G1 " + homePosition);
            SendCommand("G1 " + restPosition);
            SendCommand("M18");
            robotSerial.Close();
        }

        private void SmallStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SmallStepSize.Text.Length > 0)
            {
                int.TryParse(SmallStepSize.Text, out int newStepSize);
                smallStep = newStepSize;
            }
        }

        private void WaitTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WaitTime.Text.Length > 0)
            {
                int.TryParse(WaitTime.Text, out int newWaitTime);
                waitTime = newWaitTime;
            }
        }

        private void BigStepSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BigStepSize.Text.Length > 0)
            {
                int.TryParse(BigStepSize.Text, out int newStepSize);
                bigStep = newStepSize;
            }
        }

        private void MovementSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BigStepSize.Text.Length > 0)
            {
                int.TryParse(MovementSpeed.Text, out int newSpeed);
                movementSpeed = newSpeed;
            }
        }

        private void CommandQueue_TextChanged(object sender, TextChangedEventArgs e)
        {
            commandQueue = CommandQueue.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var typedSender = (Button)sender;
            var parameter = (ButtonAction)typedSender.CommandParameter;
            switch (parameter)
            {
                case ButtonAction.XPos:
                    currentPosX = currentPosX + stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.XNeg:
                    currentPosX = currentPosX - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.YPos:
                    currentPosY = currentPosY + stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.YNeg:
                    currentPosY = currentPosY - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.ZPos:
                    currentPosZ = currentPosZ + stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.ZNeg:
                    currentPosZ = currentPosZ - stepSize();
                    SendCommand("G1" + " X" + currentPosX + " Y" + currentPosY + " Z" + currentPosZ + " F" + movementSpeed);
                    break;
                case ButtonAction.Home:
                    currentPosX = 0;
                    currentPosY = 120;
                    currentPosZ = 120;
                    SendCommand("G1 " + homePosition + " F" + movementSpeed);
                    break;
                case ButtonAction.Rest:
                    currentPosX = 0;
                    currentPosY = 25;
                    currentPosZ = 63;
                    SendCommand("G1 " + homePosition + " F" + movementSpeed);
                    SendCommand("G1 " + restPosition + " F" + movementSpeed);
                    break;
                case ButtonAction.Bottom:
                    currentPosX = 0;
                    currentPosY = 131;
                    currentPosZ = -94;
                    SendCommand("G1 " + homePosition + " F" + movementSpeed);
                    SendCommand("G1 " + bottomPosition + " F" + movementSpeed);
                    break;
                case ButtonAction.Wait:
                    SendCommand("G4 T" + waitTime);
                    break;
                case ButtonAction.SendSequence:
                    foreach (var cmd in commandQueue)
                    {
                        if (cmd.Contains("F"))
                        {
                            SendCommand(cmd);
                        }
                        else
                        {
                            SendCommand(cmd + " F" + movementSpeed);
                        }
                    }
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

        private void Log(string message)
        {
            serialLog = serialLog + "\n" + message;
            SerialLog.Text = serialLog;
            SerialLog.ScrollToEnd();
        }

        private void Log (Enum LogTypes, string message)
        {
            switch (LogTypes)
            {
                case Models.LogType.tx:
                    serialLog = serialLog + "\ntx: " + message;
                    break;
                case Models.LogType.rx:
                    serialLog = serialLog + "\nrx: " + message;
                    break;
                case Models.LogType.ERROR:
                    serialLog = serialLog + "\nERROR: " + message;
                    break;
            };
            SerialLog.Text = serialLog;
            SerialLog.ScrollToEnd();
        }
    }
}
