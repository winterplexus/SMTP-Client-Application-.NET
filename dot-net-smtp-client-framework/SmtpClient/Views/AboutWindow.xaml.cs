//
//  AboutWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System.Reflection;
using System.Windows;

namespace SmtpClient
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            SetFields();
        }

        private void SetFields()
        {
            SetAssemblyInformationFields();
            SetProductDescriptionField();
        }

        private void SetAssemblyInformationFields()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var descriptionAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (descriptionAttributes.Length > 0)
            {
                DescriptionTextBlock.Text = ((AssemblyDescriptionAttribute)descriptionAttributes[0]).Description;
                VersionTextBlock.Text = "v" + assembly.GetName().Version;
            }

            var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (copyrightAttributes.Length > 0)
            {
                CopyrightTextBlock.Text = ((AssemblyCopyrightAttribute)copyrightAttributes[0]).Copyright;
            }
        }

        private void SetProductDescriptionField()
        {
            ProductDescriptionTextBlock.Text = "Description: desktop client application for sending mail via SMTP server";
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}