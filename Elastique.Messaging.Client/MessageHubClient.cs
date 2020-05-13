using Elastique.Messaging.Common;
using Elastique.Messaging.Common.Commands;
using Elastique.Messaging.Common.Contracts;
using Elastique.Messaging.Common.Events;
using Elastique.Messaging.Common.Exceptions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Elastique.Messaging.Client
{
    public class MessageHubClient<T> : IDisposable
    {
        /// <summary>
        /// Client end point.
        /// </summary>
        public EndPoint ClientEndPoint { get; set; }
        /// <summary>
        /// Client connected flag.
        /// </summary>
        public bool Connected => _connectionAccepted && (_receiver != null && _receiver.CanRead || _sender != null && _sender.CanWrite);
        /// <summary>
        /// Server end point.
        /// </summary>
        public EndPoint ServerEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the amount of time an MbClient will wait for server to accept the connection.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; }


        /// <summary>
        /// The event is raised when message from server is received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs<T>> DataReceived;
        public event EventHandler<ServerCommandEventArgs> CommandReceived;

        private CancellationTokenSource _receiveLoopCancellationTokenSource;
        private bool _connectionAccepted;

        private IMessageSender _sender;
        private IMessageReceiver _receiver;
        private readonly ISenderReceiverFactory _senderReceiverFactory;

        public MessageHubClient(ISenderReceiverFactory senderReceiverFactory)
        {
            ConnectionTimeout = TimeSpan.Zero;
            _senderReceiverFactory = senderReceiverFactory;
        }

        public MessageHubClient() : this(new LengthPrefixBasedCommunicationFactory())
        {

        }

        public void Connect(IPEndPoint remoteEndPoint)
        {
            try
            {
                var tcpClient = new TcpClient();
                tcpClient.Connect(remoteEndPoint);
                _sender = _senderReceiverFactory.CreateSender(tcpClient.GetStream());
                _receiver = _senderReceiverFactory.CreateReceiver(tcpClient.GetStream());

                ServerEndPoint = remoteEndPoint;
                ClientEndPoint = tcpClient.Client.LocalEndPoint;
            }
            catch (SocketException exc)
            {
                throw new ServerUnavailableException(remoteEndPoint, exc);
            }

            _sender.Send(new ClientCommandMessage(ClientCommand.Connect));
            var response = (ServerCommandMessage)_receiver.Receive(ConnectionTimeout);

            if (response == null)
                throw new UnexpectedMessageException(typeof(ServerCommandMessage), response);

            if (response.Command == ServerCommand.ConnectionAccepted)
            {
                _connectionAccepted = true;
            }

            _receiveLoopCancellationTokenSource = new CancellationTokenSource();
            Task.Run(ReceiveLoop, _receiveLoopCancellationTokenSource.Token);
        }

        public void Disconnect()
        {
            _sender.Send(new ClientCommandMessage(ClientCommand.Disconnect));
            _receiveLoopCancellationTokenSource.Cancel();
            _connectionAccepted = false;
        }

        public void Send(T data)
        {
            if (!Connected)
            {
                throw new NoConnectionException();
            }

            _sender.Send(new DataMessage<T>(data));
        }

        private void ReceiveLoop()
        {
            while (!_receiveLoopCancellationTokenSource.Token.IsCancellationRequested)
            {
                if (!Connected)
                {
                    throw new NoConnectionException();
                }

                var message = _receiver.Receive(TimeSpan.Zero);

                if (message is ServerCommandMessage)
                {
                    var command = ((ServerCommandMessage)message).Command;
                    CommandReceived?.Invoke(this, new ServerCommandEventArgs(ClientEndPoint, command));
                }

                if (message is DataMessage<T> data)
                {
                    DataReceived?.Invoke(this, new DataReceivedEventArgs<T>(ClientEndPoint, data.Data));
                }
            }
        }

        public void Dispose()
        {
            _sender.Close();
            _receiver.Close();
        }
    }
}
