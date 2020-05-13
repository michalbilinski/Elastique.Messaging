using Elastique.Messaging.Common.Contracts;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Elastique.Messaging.Common
{
    public class LengthPrefixBasedSender : IMessageSender
    {
        private readonly NetworkStream _stream;

        public bool CanWrite => _stream.CanWrite;

        public LengthPrefixBasedSender(NetworkStream sendStream)
        {
            _stream = sendStream;
        }

        public void Send(Message message)
        {
            if (!_stream.CanWrite)
                throw new InvalidOperationException("Cannot write to the upload stream. Is the connection open?");

            var messageBytes = message.ToByteArray();

            //  Length-prefix framing. We first send the message length, so the opposite side knows what length to expect.
            var lengthBytes = BitConverter.GetBytes(messageBytes.Length);
            _stream.Write(lengthBytes, 0, lengthBytes.Length);

            _stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public void Close()
        {
            _stream.Close();
            _stream.Dispose();
        }
    }
}
