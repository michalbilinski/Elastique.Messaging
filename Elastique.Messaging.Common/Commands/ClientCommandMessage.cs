using System;

namespace Elastique.Messaging.Common.Commands
{
    [Serializable]
    public sealed class ClientCommandMessage : Message
    {
        public ClientCommand Command { get; set; }

        public ClientCommandMessage(ClientCommand command)
        {
            Command = command;
        }
    }
}
