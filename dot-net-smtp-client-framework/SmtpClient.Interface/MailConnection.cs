//
//  MailConnection.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmtpClient.Interface
{
    public delegate void OutputHandler(string message);

    public class MailConnection
    {
        public event OutputHandler ConnectionOutput;
        private AggregateException ServerException { get; set; }

        public MailConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        public async Task TestConnection(string server, int port, bool enableTls)
        {
            ServerException = new AggregateException();

            using (var client = new TcpClient())
            {
                await client.ConnectAsync(server, port);

                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    var response = reader.ReadLine();
                    SendConnectionOutput(response);
                    ParseResponse(response);

                    await SendRequest("EHLO localhost", writer, reader);

                    if (enableTls)
                    {
                        await SendRequest("STARTTLS", writer, reader);

                        using (var sslStream = new SslStream(stream, true, ValidateRemoteCertificate, null, EncryptionPolicy.RequireEncryption))
                        {
                            await sslStream.AuthenticateAsClientAsync(server, null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);

                            var remoteCertificate = sslStream.RemoteCertificate;
                            if (remoteCertificate != null)
                            {
                                var remoteCertificateOutput = $"Remote certificate was issued to {remoteCertificate.Subject} and is valid from {remoteCertificate.GetEffectiveDateString()} until {remoteCertificate.GetExpirationDateString()}";
                                SendConnectionOutput(remoteCertificateOutput);
                            }

                            using (var sslReader = new StreamReader(sslStream))
                            using (var sslWriter = new StreamWriter(sslStream) { AutoFlush = true })
                            {
                                await SendRequest("QUIT", sslWriter, sslReader);
                            }
                        }
                    }
                    else
                    {
                        await SendRequest("QUIT", writer, reader);
                    }
                }
            }

            SendConnectionOutput("--------------------------------------------------------------------------------");

            if (ServerException.InnerExceptions.Count > 0)
            {
                throw ServerException;
            }
        }

        private async Task SendRequest(string message, StreamWriter writer, StreamReader reader)
        {
            SendConnectionOutput(message);

            await writer.WriteLineAsync(message);

            var response = await reader.ReadLineAsync();
            SendConnectionOutput(response);
            ParseResponse(response);

            while (response.Length > 3 && response[3] != ' ')
            {
                response = await reader.ReadLineAsync();
                SendConnectionOutput(response);
                ParseResponse(response);
            }
        }

        private void ParseResponse(string response)
        {
            if (response == null || !(response.StartsWith("2") || response.StartsWith("3")))
            {
                var exceptions = new List<Exception>(ServerException.InnerExceptions)
                {
                    new MailResponseException(response)
                };
                ServerException = new AggregateException("SMTP server has responded with one or more exceptions.", exceptions);
            }
        }

        private void SendConnectionOutput(string message)
        {
            ConnectionOutput?.Invoke(message);
        }

        protected bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            if (policyErrors != SslPolicyErrors.None)
            {
                SendConnectionOutput($"Remote certificate has policy errors: {policyErrors}");

                var certificateChainOutput = new StringBuilder();
                foreach (var chainElement in chain.ChainElements)
                {
                    certificateChainOutput.AppendLine($"Certificate chain certificate: {chainElement.Certificate.ToString(true)}");
                    certificateChainOutput.AppendLine($"Certificate chain information: {chainElement.Information}");
                    foreach (var certStatus in chainElement.ChainElementStatus)
                    {
                        certificateChainOutput.AppendLine($"Certificate chain status information: {certStatus.StatusInformation}");
                    }
                }

                SendConnectionOutput(certificateChainOutput.ToString());
            }

            return true;
        }
    }
}