using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Elastique.Messaging.Common
{
    [Serializable]
    public abstract class Message
    {
        public virtual byte[] ToByteArray()
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static Message FromByteArray(byte[] array)
        {
            using (var memoryStream = new MemoryStream(array))
            {
                var obj = new BinaryFormatter().Deserialize(memoryStream);

                if (obj is Message)
                {
                    return (Message)obj;
                }
            }

            return null;
        }

        public static DataMessage<T> FromByteArray<T>(byte[] array)
        {
            using (var memoryStream = new MemoryStream(array))
            {
                var obj = new BinaryFormatter().Deserialize(memoryStream);

                if (obj is DataMessage<T>)
                {
                    return (DataMessage<T>)obj;
                }
            }

            return null;
        }
    }
}
