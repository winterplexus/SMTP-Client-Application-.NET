//
//  MailClient.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SmtpClient.Interface
{
    public class MailClient
    {
        public MailClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        public async Task SendEmail(ConnectionData connectionData, MailData mailData)
        {
            using (var message = new MailMessage(FromAddress(mailData), ToAddress(mailData)))
            {
                if (!string.IsNullOrEmpty(mailData.Cc))
                {
                    var cc = new MailAddress(mailData.Cc);
                    message.CC.Add(cc);
                }
                if (!string.IsNullOrEmpty(mailData.Bcc))
                {
                    var bcc = new MailAddress(mailData.Bcc);
                    message.Bcc.Add(bcc);
                }
                if (!string.IsNullOrEmpty(mailData.SubjectLine))
                {
                    message.Subject = mailData.SubjectLine;
                }
                if (!string.IsNullOrEmpty(mailData.Body))
                {
                    message.Body = mailData.Body;
                }

                using (var client = new System.Net.Mail.SmtpClient())
                {
                    client.Host = connectionData.ServerName;
                    client.Port = connectionData.PortNumber;
                    client.EnableSsl = connectionData.EnableTls;
                    if (connectionData.AuthenticationRequired)
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(connectionData.UserName, connectionData.Password);
                    }
                    else
                    {
                        client.UseDefaultCredentials = true;
                    }
                    await client.SendMailAsync(message);
                }
            }
        }

        protected static MailAddress FromAddress(MailData mail)
        {
            return !string.IsNullOrEmpty(mail.FromDisplayName) ? new MailAddress(mail.From, mail.FromDisplayName) : new MailAddress(mail.From);
        }

        protected static MailAddress ToAddress(MailData mail)
        {
            return !string.IsNullOrEmpty(mail.ToDisplayName) ? new MailAddress(mail.To, mail.ToDisplayName) : new MailAddress(mail.To);
        }

        protected bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}