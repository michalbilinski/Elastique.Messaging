using System;

namespace Elastique.Messaging.Common.Exceptions
{
    public class ReceiveTimeoutException : Exception
    {
        public ReceiveTimeoutException() : base("The server did not send a response in the specified amount of time.")
        {

        }
    }
}
