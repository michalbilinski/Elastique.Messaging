using System;

namespace Elastique.Messaging.Common
{
    [Serializable]
    public class DataMessage<T> : Message
    {
        public T Data { get; set; }

        public DataMessage(T data) : base()
        {
            Data = data;
        }
    }
}
