using System;

namespace Elastique.Messaging.Common.Contracts
{
    public interface IMessageReceiver
    {
        Message Receive();
        bool CanRead { get; }
        void Close();
    }
}
