using System;

namespace Elastique.Messaging.Common.Exceptions
{
    public class UnexpectedMessageException : Exception
    {
        public Message ActualMessage { get; private set; }
        public Type ExpectedMessageType { get; private set; }

        public UnexpectedMessageException(Type expectedMessageType, Message actualMessage) : base($"Unexpected message {actualMessage.GetType()} while epexcting {expectedMessageType}")
        {
            ExpectedMessageType = expectedMessageType;
            ActualMessage = actualMessage;
        }
    }
}
