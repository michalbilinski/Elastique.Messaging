using Elastique.Messaging.Common.Contracts;
using Elastique.Messaging.Common.Exceptions;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Elastique.Messaging.Common
{
    public class LengthPrefixBasedReceiver : IMessageReceiver
    {
        private readonly NetworkStream _stream;

        public bool CanRead => _stream.CanRead;

        public LengthPrefixBasedReceiver(NetworkStream receiveStream)
        {
            _stream = receiveStream;
        }

        public Message Receive(TimeSpan timeout)
        {
            byte[] receivedData = null;

            if (!_stream.CanRead)
            {
                throw new InvalidOperationException("Cannot read from the download stream. Is the connection open?");
            }

            var start = DateTime.Now;
            while (true)
            {
                if (timeout != TimeSpan.Zero && DateTime.Now - start > timeout)
                {
                    throw new ReceiveTimeoutException();
                }

                if (_stream.DataAvailable)
                {
                    var messageLength = ReceiveMessageLength();
                    receivedData = new byte[messageLength];
                    _stream.Read(receivedData, 0, receivedData.Length);
                }

                if (receivedData != null)
                {
                    var message = Message.FromByteArray(receivedData);
                    return message;
                }

                Thread.Sleep(200);
            }
        }

        private uint ReceiveMessageLength()
        {
            var lengthBytes = new byte[4];

            // Length-prefix framing. We first read 4 bytes, which contain the message's length.
            _stream.Read(lengthBytes, 0, lengthBytes.Length);
            return BitConverter.ToUInt32(lengthBytes, 0);
        }

        public void Close()
        {
            _stream.Close();
            _stream.Dispose();
        }
    }
}
