using System;
using System.Net;

namespace Elastique.Messaging.Common.Exceptions
{
    public class ServerUnavailableException : Exception
    {

        public ServerUnavailableException(EndPoint endPoint) : this(endPoint, null)
        {

        }

        public ServerUnavailableException(EndPoint endPoint, Exception innerException) : base($"Server did not respond within the expected amount of time.", innerException)
        {

        }
    }
}
