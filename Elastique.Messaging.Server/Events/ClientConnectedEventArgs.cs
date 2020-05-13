using System;
using System.Net;

namespace Elastique.Messaging.Server.Events
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public EndPoint ClientEndPoint { get; private set; }

        public ClientConnectedEventArgs(EndPoint clientEndPoint)
        {
            ClientEndPoint = clientEndPoint;
        }
    }
}
