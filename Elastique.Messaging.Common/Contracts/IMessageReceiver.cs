using System;

namespace Elastique.Messaging.Common.Contracts
{
    public interface IMessageReceiver
    {
        Message Receive(TimeSpan timeout);
        bool CanRead { get; }
        void Close();
    }
}
