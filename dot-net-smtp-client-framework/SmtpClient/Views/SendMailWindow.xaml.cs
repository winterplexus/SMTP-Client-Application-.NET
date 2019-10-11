//
//  SendMailWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System;
using System.Windows;
using System.Windows.Media;
using SmtpClient.Interface;

namespace SmtpClient
{
    public partial class SendMailWindow
    {
        private readonly ConnectionData connectionData;

        public SendMailWindow()
        {
            InitializeComponent();
        }

        public SendMailWindow(ConnectionData connectionData)
        {
            InitializeComponent();

            this.connectionData = connectionData;
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!ConnectionDataValid())
            {
                Close();
            }
            if (string.IsNullOrEmpty(FromAddressText.Text))
            {
                DisplayErrorMessage("FROM mail address is missing");
                return;
            }
            if (string.IsNullOrEmpty(ToAddressText.Text))
            {
                DisplayErrorMessage("TO mail address is missing");
                return;
            }
            if (string.IsNullOrEmpty(SubjectLineText.Text))
            {
                DisplayErrorMessage("Mail subject line is missing");
                return;
            }

            var mailData = new MailData
            {
                From = FromAddressText.Text.Trim(),
                FromDisplayName = FromDislplayAddressText.Text.Trim(),
                To = ToAddressText.Text.Trim(),
                ToDisplayName = ToDisplayAddressText.Text.Trim(),
                Cc = CcAddressText.Text.Trim(),
                Bcc = BccAddressText.Text.Trim(),
                SubjectLine = SubjectLineText.Text.Trim(),
                Body = BodyText.Text
            };

            SendMail(mailData);

            DisplayInformationMessage($"Mail sent to: {ToAddressText.Text}");
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearFields()
        {
            FromAddressText.Text = string.Empty;
            FromDislplayAddressText.Text = string.Empty;
            ToAddressText.Text = string.Empty;
            ToDisplayAddressText.Text = string.Empty;
            CcAddressText.Text = string.Empty;
            BccAddressText.Text = string.Empty;
            SubjectLineText.Text = string.Empty;
            BodyText.Text = string.Empty;
            MessageText.Text = string.Empty;
            MessageText.Foreground = Brushes.Black;
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

        private bool ConnectionDataValid()
        {
            if (connectionData == null)
            {
                MessageBox.Show("connectionData structure is null", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (connectionData.PortNumber < 1)
            {
                connectionData.PortNumber = 25;
            }
            if (string.IsNullOrEmpty(connectionData.ServerName))
            {
                MessageBox.Show("Connection data is missing. Click 'Set Connection' menu item to enter data. ", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private async void SendMail(MailData mailData)
        {
            try
            {
                var mailClient = new MailClient();
                await mailClient.SendEmail(connectionData, mailData);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage($"Error: {ex.Message}");
            }
        }
    }
}