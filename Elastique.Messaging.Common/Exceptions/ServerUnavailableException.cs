using System;
using System.Net;

namespace Elastique.Messaging.Common.Exceptions
{
    public class ServerUnavailableException : Exception
    {

        public ServerUnavailableException(EndPoint endPoint) : base($"Cannot connect to server {endPoint}")
        {

        }

        public ServerUnavailableException(EndPoint endPoint, Exception innerException) : base($"Cannot connect to server {endPoint}", innerException)
        {

        }
    }
}
