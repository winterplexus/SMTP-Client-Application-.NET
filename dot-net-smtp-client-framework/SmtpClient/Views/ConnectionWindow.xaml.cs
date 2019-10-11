//
//  ConnectionWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SmtpClient.Interface;

namespace SmtpClient
{
    public partial class ConnectionWindow
    {
        private readonly ConnectionData connectionData;
        private readonly MailConnection mailConnection = new MailConnection();

        public ConnectionWindow()
        {
            InitializeComponent();
        }

        public ConnectionWindow(ConnectionData connectionData)
        {
            this.connectionData = connectionData;

            InitializeComponent();
            SetFields();

            mailConnection.ConnectionOutput += AppendConnectionOutput;
        }

        private void SetConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            ResetFields();

            if (string.IsNullOrEmpty(ServerNameText.Text))
            {
                DisplayErrorMessage("Server name is missing");
                return;
            }
            if (string.IsNullOrEmpty(UsernameText.Text) && !string.IsNullOrEmpty(PasswordText.Text))
            {
                DisplayErrorMessage("User name is missing");
                return;
            }
            if (!string.IsNullOrEmpty(UsernameText.Text) && string.IsNullOrEmpty(PasswordText.Text))
            {
                DisplayErrorMessage("Password is missing");
                return;
            }
            if (!string.IsNullOrEmpty(UsernameText.Text) && !string.IsNullOrEmpty(PasswordText.Text))
            {
                connectionData.AuthenticationRequired = true;
            }

            connectionData.ServerName = ServerNameText.Text.Trim();
            connectionData.PortNumber = PortNumber();
            connectionData.UserName = UsernameText.Text.Trim();
            connectionData.Password = PasswordText.Text.Trim();
            connectionData.EnableTls = EnableTlsCheckBox.IsChecked == true;

            TestConnection();

            DisplayInformationMessage("Connection set and tested");
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
            ClearConnection();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetFields()
        {
            if (connectionData.PortNumber < 1)
            {
                connectionData.PortNumber = 25;
            }

            ServerNameText.Text = connectionData.ServerName;
            PortNumberText.Text = Convert.ToString(connectionData.PortNumber, System.Globalization.CultureInfo.InvariantCulture);
            UsernameText.Text = connectionData.UserName;
            PasswordText.Text = connectionData.Password;
            EnableTlsCheckBox.IsChecked = connectionData.EnableTls;
        }

        private void ResetFields()
        {
            MessageText.Text = string.Empty;
            ConnectionOutputText.Background = Brushes.Black;
            ConnectionOutputText.Foreground = Brushes.GreenYellow;
        }

        private void ClearFields()
        {
            ServerNameText.Text = string.Empty;
            PortNumberText.Text = "25";
            UsernameText.Text = string.Empty;
            PasswordText.Text = string.Empty;
            EnableTlsCheckBox.IsChecked = false;
            MessageText.Text = string.Empty;
            MessageText.Foreground = Brushes.Black;
            ConnectionOutputText.Text = string.Empty;
            ConnectionOutputText.Background = Brushes.White;
        }

        private void ClearConnection()
        {
            connectionData.ServerName = string.Empty;
            connectionData.PortNumber = 0;
            connectionData.UserName = string.Empty;
            connectionData.Password = string.Empty;
            connectionData.AuthenticationRequired = false;
            connectionData.EnableTls = false;
        }

        private void DisplayInformationMessage(string message)
        {
            MessageText.Text = message;
            MessageText.Foreground = Brushes.MediumBlue;
        }

        private void DisplayErrorMessage(string message)
        {
            MessageText.Text = message;
            MessageText.Foreground = Brushes.OrangeRed;
        }

        private int PortNumber()
        {
            var portNumber = 25;

            try
            {
                portNumber = Convert.ToInt32(PortNumberText.Text);
            }
            catch (OverflowException)
            {
            }
            catch (FormatException)
            {
            }

            return portNumber;
        }

        private static string ErrorString(Exception ex)
        {
            var errorString = new StringBuilder();

            errorString.AppendLine(ex.Message);

            if (ex.InnerException != null)
            {
                errorString.AppendLine(ErrorString(ex.InnerException));
            }

            return errorString.ToString();
        }

        private async void TestConnection()
        {
            try
            {
                await mailConnection.TestConnection(connectionData.ServerName, connectionData.PortNumber, connectionData.EnableTls);
            }
            catch (Exception ex)
            {
                AppendConnectionOutput(ErrorString(ex));
            }
        }

        protected delegate void AppendOutputConnectionDelegate(string text);

        protected void AppendConnectionOutput(string text)
        {
            if (Dispatcher != null && !Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new AppendOutputConnectionDelegate(AppendConnectionOutput), text);
                return;
            }
            ConnectionOutputText.AppendText(string.Concat(text, Environment.NewLine));
        }
    }
}