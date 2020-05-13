using System;
using System.Net;

namespace Elastique.Messaging.Common.Events
{
    public class DataReceivedEventArgs<T> : EventArgs
    {
        public T Data { get; private set; }
        public EndPoint Sender { get; private set; }

        public DataReceivedEventArgs(EndPoint sender, T data) : base()
        {
            Data = data;
            Sender = sender;
        }
    }
}
