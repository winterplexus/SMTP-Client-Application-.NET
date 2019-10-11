//
//  MainWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System.Windows;
using SmtpClient.Interface;

namespace SmtpClient
{
    public partial class MainWindow
    {
        private static ConnectionData connectionData;

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();
            MenuItemSMTPSendMail.Visibility = Visibility.Hidden;

            connectionData = new ConnectionData();
        }

        private void MenuItemFileExitClick(object sender, RoutedEventArgs e)
        {
            CloseAllOpenedWindows();
            Close();
        }

        private void MenuItemSetConnectionClick(object sender, RoutedEventArgs e)
        {
            var connectionWindow = new ConnectionWindow(connectionData);
            connectionWindow.ShowDialog();

            MenuItemSMTPSendMail.Visibility = Visibility.Visible;
        }

        private void MenuItemSendMailClick(object sender, RoutedEventArgs e)
        {
            var sendMailWindow = new SendMailWindow(connectionData);
            sendMailWindow.ShowDialog();
        }

        private void MenuItemHelpAboutClick(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private static void CloseAllOpenedWindows()
        {
            for (var i = Application.Current.Windows.Count - 1; i >= 0; i--)
            {
                var window = Application.Current.Windows[i];
                window?.Close();
            }
        }
    }
}