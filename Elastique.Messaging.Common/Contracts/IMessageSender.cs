namespace Elastique.Messaging.Common.Contracts
{
    public interface IMessageSender
    {
        void Send(Message message);
        bool CanWrite { get; }
        void Close();
    }
}
