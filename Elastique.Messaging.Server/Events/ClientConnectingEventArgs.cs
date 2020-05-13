using System;
using System.Net;

namespace Elastique.Messaging.Server.Events
{
    public class ClientConnectingEventArgs : EventArgs
    {
        public EndPoint ClientEndPoint { get; private set; }

        public ClientConnectingEventArgs(EndPoint clientEndPoint)
        {
            ClientEndPoint = clientEndPoint;
        }
    }
}
