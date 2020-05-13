using Elastique.Messaging.Common.Commands;
using System;
using System.Net;

namespace Elastique.Messaging.Server.Events
{
    internal class ClientCommandReceivedEventArgs : EventArgs
    {
        public EndPoint ClientEndPoint { get; private set; }
        public ClientCommand Command { get; private set; }

        public ClientCommandReceivedEventArgs(EndPoint clientEndPoint, ClientCommand command)
        {
            ClientEndPoint = clientEndPoint;
            Command = command;
        }
    }
}
