using System;
using System.Net;

namespace Elastique.Messaging.Server.Events
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public EndPoint ClientEndPoint { get; private set; }

        public ClientDisconnectedEventArgs(EndPoint clientEndPoint)
        {
            ClientEndPoint = clientEndPoint;
        }
    }
}
