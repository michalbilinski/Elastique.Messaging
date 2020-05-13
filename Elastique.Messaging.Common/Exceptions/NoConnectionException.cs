using System;

namespace Elastique.Messaging.Common.Exceptions
{
    public class NoConnectionException : Exception
    {

        public NoConnectionException() : base("No connection established.")
        {

        }
    }
}
