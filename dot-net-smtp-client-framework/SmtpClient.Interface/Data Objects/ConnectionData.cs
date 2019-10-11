//
//  ConnectionData.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
namespace SmtpClient.Interface
{
    public class ConnectionData
    {
        public ConnectionData()
        {
            PortNumber = 21;
        }
        public string ServerName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool AuthenticationRequired { get; set; }
        public bool EnableTls { get; set; }
    }
}