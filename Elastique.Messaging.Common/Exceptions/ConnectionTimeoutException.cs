using System;
using System.Net;

namespace Elastique.Messaging.Common.Exceptions
{
    public class ConnectionTimeoutException : Exception
    {

        public ConnectionTimeoutException(EndPoint endPoint) : this(endPoint, null)
        {

        }

        public ConnectionTimeoutException(EndPoint endPoint, Exception innerException) : base("Communication has not ended in a timely fashion.", innerException)
        {

        }
    }
}
