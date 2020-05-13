using System.Net.Sockets;

namespace Elastique.Messaging.Common.Contracts
{
    public interface ISenderReceiverFactory
    {
        IMessageReceiver CreateReceiver(NetworkStream stream);
        IMessageSender CreateSender(NetworkStream stream);
    }
}
