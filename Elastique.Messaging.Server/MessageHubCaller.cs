using Elastique.Messaging.Common;
using Elastique.Messaging.Common.Commands;
using Elastique.Messaging.Common.Contracts;
using Elastique.Messaging.Common.Events;
using Elastique.Messaging.Server.Events;
using System;
using System.Net;
using System.Net.Sockets;

namespace Elastique.Messaging.Server
{
    internal class MessageHubCaller<T>
    {
        /// <summary>
        /// Client end point.
        /// </summary>
        public EndPoint ClientEndPoint => _tcpClient.Client.RemoteEndPoint;
        /// <summary>
        /// Client connected flag.
        /// </summary>
        public bool Connected => _tcpClient.Connected;
        /// <summary>
        /// Server end point.
        /// </summary>
        public EndPoint ServerEndPoint => _tcpClient.Client.LocalEndPoint;

        /// <summary>
        /// The event is raised when message from server is received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs<T>> DataReceived;
        public event EventHandler<ClientCommandReceivedEventArgs> CommandReceived;

        private readonly TcpClient _tcpClient;
        private readonly IMessageSender _sender;
        private readonly IMessageReceiver _receiver;

        public MessageHubCaller(TcpClient tcpClient, ISenderReceiverFactory senderReceiverFactory)
        {
            _tcpClient = tcpClient;
            _sender = senderReceiverFactory.CreateSender(_tcpClient.GetStream());
            _receiver = senderReceiverFactory.CreateReceiver(_tcpClient.GetStream());
        }

        public void Disconnect()
        {
            _tcpClient.Close();
        }

        public void Send(Message message)
        {
            _sender.Send(message);
        }

        public void ReceiveLoop()
        {
            if (!Connected)
            {
                throw new InvalidOperationException("Cannot receive messages without an established connection.");
            }

            while (Connected)
            {
                var message = _receiver.Receive(TimeSpan.Zero);

                if (message is ClientCommandMessage)
                {
                    var command = ((ClientCommandMessage)message).Command;
                    CommandReceived?.Invoke(this, new ClientCommandReceivedEventArgs(_tcpClient.Client.RemoteEndPoint, command));
                }

                if (message is DataMessage<T> data)
                {
                    DataReceived?.Invoke(this, new DataReceivedEventArgs<T>(_tcpClient.Client.RemoteEndPoint, data.Data));
                }
            }
        }
    }
}
