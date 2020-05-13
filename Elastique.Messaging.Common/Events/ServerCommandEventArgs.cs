using Elastique.Messaging.Common.Commands;
using System;
using System.Net;

namespace Elastique.Messaging.Common.Events
{
    public class ServerCommandEventArgs : EventArgs
    {
        public EndPoint ClientEndPoint { get; private set; }
        public ServerCommand Command { get; private set; }

        public ServerCommandEventArgs(EndPoint clientEndPoint, ServerCommand command)
        {
            ClientEndPoint = clientEndPoint;
            Command = command;
        }
    }
}
