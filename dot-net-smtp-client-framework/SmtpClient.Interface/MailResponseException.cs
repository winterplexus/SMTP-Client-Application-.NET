//
//  MailResponseException.cs
//
//  Copyright (c) Wiregrass Code Technology 2019
//
using System;
using System.Runtime.Serialization;

namespace SmtpClient.Interface
{
    [Serializable]
    public class MailResponseException : Exception
    {
        public MailResponseException()
        {
        }

        protected MailResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MailResponseException(string message) : base(message)
        {
        }

        public MailResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}