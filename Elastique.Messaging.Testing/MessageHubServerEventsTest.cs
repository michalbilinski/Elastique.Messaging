using Elastique.Messaging.Client;
using Elastique.Messaging.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;

namespace Elastique.Messaging.Testing
{
    [TestClass]
    public class MessageHubServerEventsTest
    {
        readonly private MessageHub<int> _server;
        private readonly IPEndPoint _endPoint;

        private DateTime _clientConnecting = DateTime.MinValue;
        private DateTime _clientConnected = DateTime.MinValue;
        private DateTime _dataReceived = DateTime.MinValue;
        private DateTime _clientDisconnected = DateTime.MinValue;

        public MessageHubServerEventsTest()
        {
            _endPoint = new IPEndPoint(IPAddress.Loopback, 12000);
            _server = new MessageHub<int>(_endPoint);
            _server.Start();
        }

        [TestMethod]
        public void EventsShouldBeInvokedAsExpected()
        {
            var client = new MessageHubClient<int>();

            _server.ClientConnecting += (sender, e) => { _clientConnecting = DateTime.Now; };
            _server.ClientConnected += (sender, e) => { _clientConnected = DateTime.Now; };
            _server.DataReceived += (sender, e) => { _dataReceived = DateTime.Now; };
            _server.ClientDisconnected += (sender, e) => { _clientDisconnected = DateTime.Now; };

            client.Connect(_endPoint);
            client.Send(1);
            client.Disconnect();

            // Let's sleep 200ms to make sure that all events are invoked.
            Thread.Sleep(200);

            Assert.AreNotEqual(DateTime.MinValue, _clientConnecting);
            Assert.AreNotEqual(DateTime.MinValue, _clientConnected);
            Assert.AreNotEqual(DateTime.MinValue, _dataReceived);
            Assert.AreNotEqual(DateTime.MinValue, _clientDisconnected);

            Assert.IsTrue(_clientConnecting < _clientConnected);
            Assert.IsTrue(_clientConnected < _dataReceived);
            Assert.IsTrue(_dataReceived < _clientDisconnected);
        }
    }
}
