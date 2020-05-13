using Elastique.Messaging.Common.Contracts;
using System.Net.Sockets;

namespace Elastique.Messaging.Common
{
    public class LengthPrefixBasedCommunicationFactory : ISenderReceiverFactory
    {
        public IMessageReceiver CreateReceiver(NetworkStream stream)
        {
            return new LengthPrefixBasedReceiver(stream);
        }

        public IMessageSender CreateSender(NetworkStream stream)
        {
            return new LengthPrefixBasedSender(stream);
        }
    }
}
