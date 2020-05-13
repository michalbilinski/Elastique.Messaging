using System;

namespace Elastique.Messaging.Common.Commands
{
    [Serializable]
    public sealed class ServerCommandMessage : Message
    {
        public ServerCommand Command { get; set; }

        public ServerCommandMessage(ServerCommand command)
        {
            Command = command;
        }
    }
}
