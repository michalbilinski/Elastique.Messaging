using Elastique.Messaging.Common;
using Elastique.Messaging.Common.Commands;
using Elastique.Messaging.Common.Contracts;
using Elastique.Messaging.Common.Events;
using Elastique.Messaging.Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Elastique.Messaging.Server
{
    public class MessageHub<T>
    {
        /// <summary>
        /// Default 8192 bytes.
        /// </summary>
        public int SendBufferSize
        {
            get => _tcpListener.Server.SendBufferSize;
            set => _tcpListener.Server.SendBufferSize = value;
        }
        /// <summary>
        /// Default 8192 bytes.
        /// </summary>
        public int ReceiveBufferSize
        {
            get => _tcpListener.Server.ReceiveBufferSize;
            set => _tcpListener.Server.ReceiveBufferSize = value;
        }

        public event EventHandler<ClientConnectingEventArgs> ClientConnecting;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<DataReceivedEventArgs<T>> DataReceived;

        public EndPoint EndPoint => _tcpListener.LocalEndpoint;

        public IEnumerable<EndPoint> Clients => _peers.Select(c => c.ClientEndPoint);

        public int ClientsCount => _peers.Count;

        private readonly TcpListener _tcpListener;
        private readonly List<MessageHubCaller<T>> _peers;

        private CancellationTokenSource _acceptClientsLoopCancellationTokenSource;
        private readonly ISenderReceiverFactory _senderReceiverFactory;

        public MessageHub(IPEndPoint localEndPoint, ISenderReceiverFactory senderReceiverFactory)
        {
            _tcpListener = new TcpListener(localEndPoint);
            _senderReceiverFactory = senderReceiverFactory;
            _peers = new List<MessageHubCaller<T>>();
        }

        public MessageHub(IPEndPoint localEndPoint) : this(localEndPoint, new LengthPrefixBasedCommunicationFactory())
        {
        }

        private void OnCommandReceived(object sender, ClientCommandReceivedEventArgs e)
        {
            var peer = _peers.First(p => p.ClientEndPoint == e.ClientEndPoint);

            switch (e.Command)
            {
                case ClientCommand.Connect:
                    SendCommand(peer.ClientEndPoint, ServerCommand.ConnectionAccepted);
                    ClientConnected?.Invoke(this, new ClientConnectedEventArgs(peer.ClientEndPoint));
                    break;
                case ClientCommand.Disconnect:
                    var endPoint = peer.ClientEndPoint;
                    peer.Disconnect();
                    _peers.Remove(peer);
                    ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(endPoint));
                    break;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs<T> e)
        {
            var peer = _peers.First(p => p.ClientEndPoint == e.Sender);
            DataReceived?.Invoke(this, e);
        }

        public void Start()
        {
            _tcpListener.Start();
            _acceptClientsLoopCancellationTokenSource = new CancellationTokenSource();
            Task.Run(AcceptClientsLoop, _acceptClientsLoopCancellationTokenSource.Token);
        }

        private void AcceptClientsLoop()
        {
            while (!_acceptClientsLoopCancellationTokenSource.Token.IsCancellationRequested)
            {
                var client = _tcpListener.AcceptTcpClient();
                client.ReceiveBufferSize = ReceiveBufferSize;
                client.SendBufferSize = SendBufferSize;
                var mbPeer = new MessageHubCaller<T>(client, _senderReceiverFactory);
                OnClientConnecting(mbPeer);
            }
        }

        public void Stop()
        {
            _acceptClientsLoopCancellationTokenSource.Cancel();
            _tcpListener.Stop();
        }

        public void Send(EndPoint clientEndPoint, T data)
        {
            _peers.Where(c => c.ClientEndPoint == clientEndPoint).ToList().ForEach(c => { c.Send(new DataMessage<T>(data)); });
        }

        private void SendCommand(EndPoint clientEndPoint, ServerCommand command)
        {
            _peers.Where(c => c.ClientEndPoint == clientEndPoint).ToList().ForEach(c => { c.Send(new ServerCommandMessage(command)); });
        }

        private void OnClientConnecting(MessageHubCaller<T> mbPeer)
        {
            ClientConnecting?.Invoke(this, new ClientConnectingEventArgs(mbPeer.ClientEndPoint));
            _peers.Add(mbPeer);
            mbPeer.DataReceived += OnDataReceived;
            mbPeer.CommandReceived += OnCommandReceived;
            Task.Run(mbPeer.ReceiveLoop);
        }
    }
}
