//
//  MailData.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
namespace SmtpClient.Interface
{
    public class MailData
    {
        public string From { get; set; }
        public string FromDisplayName { get; set; }
        public string To { get; set; }
        public string ToDisplayName { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string SubjectLine { get; set; }
        public string Body { get; set; }
    }
}