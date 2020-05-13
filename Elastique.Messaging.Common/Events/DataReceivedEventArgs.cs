using System;
using System.Net;

namespace Elastique.Messaging.Common.Events
{
    public class DataReceivedEventArgs<T> : EventArgs
    {
        public DataMessage<T> Message { get; private set; }
        public EndPoint Sender { get; private set; }

        public DataReceivedEventArgs(EndPoint sender, DataMessage<T> message) : base()
        {
            Message = message;
            Sender = sender;
        }
    }
}
