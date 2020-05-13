using Elastique.Messaging.Client;
using Elastique.Messaging.Common;
using Elastique.Messaging.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace MB.Tcp.Testing
{
    [TestClass]
    public class ConnectionTest
    {
        private readonly List<string> _messagesClientSendTest = new List<string>();
        private readonly List<string> _messagesServerSendTest = new List<string>();

        [TestMethod]
        public void ConnectShouldSucceed()
        {
            var client = new MessageHubClient<string>() { ConnectionTimeout = TimeSpan.FromSeconds(1) };
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Loopback, 12340));
            server.Start();

            Assert.IsFalse(client.Connected);
            Assert.AreEqual(0, server.ClientsCount);

            client.Connect(new IPEndPoint(IPAddress.Loopback, 12340));

            Assert.IsTrue(client.Connected);
            Assert.AreEqual(1, server.ClientsCount);            
        }

        [TestMethod]
        public void DisconnectShouldSucceed()
        {
            var client = new MessageHubClient<string>();
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Loopback, 12341));
            server.Start();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12341));            
            
            Assert.IsTrue(client.Connected);
            Assert.AreEqual(1, server.ClientsCount);
            
            client.Disconnect();

            // Let's wait 100ms to make sure that the connection is established and servers sees the client.
            Thread.Sleep(100);

            Assert.IsFalse(client.Connected);
            Assert.AreEqual(0, server.ClientsCount);
        }

        [TestMethod]
        public void ClientSendServerReceiveTest()
        {
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Any, 12342));
            var client = new MessageHubClient<string>();
            
            server.Start();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12342));

            server.DataReceived += ((sender, e) => { _messagesClientSendTest.Add(e.Data); });

            client.Send("Message from client 1");
            client.Send("Message from client 2");
            client.Send("Message from client 3");
            client.Send("Message from client 4");

            // Let's wait 100ms to make sure that the messages arrived at the server.
            Thread.Sleep(100);

            client.Disconnect();
            server.Stop();

            Assert.AreEqual(4, _messagesClientSendTest.Count);
            Assert.AreEqual("Message from client 1", _messagesClientSendTest[0]);
            Assert.AreEqual("Message from client 2", _messagesClientSendTest[1]);
            Assert.AreEqual("Message from client 3", _messagesClientSendTest[2]);
            Assert.AreEqual("Message from client 4", _messagesClientSendTest[3]);
        }

        [TestMethod]
        public void ServerSendClientReceiveTest()
        {
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Any, 12343));
            var client = new MessageHubClient<string>();

            server.Start();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12343));

            client.DataReceived += ((sender, e) => { _messagesServerSendTest.Add(e.Data); });

            server.Send(server.Clients.First(), "Message from server 1");
            server.Send(server.Clients.First(), "Message from server 2");
            server.Send(server.Clients.First(), "Message from server 3");

            // Let's wait 300ms to make sure that the connection is established and servers sees the client.
            Thread.Sleep(500);

            client.Disconnect();
            server.Stop();

            Assert.AreEqual(3, _messagesServerSendTest.Count);
            Assert.AreEqual("Message from server 1", _messagesServerSendTest[0]);
            Assert.AreEqual("Message from server 2", _messagesServerSendTest[1]);
            Assert.AreEqual("Message from server 3", _messagesServerSendTest[2]);
        }


        [TestMethod]
        public void ServerUsingInjectedSenderReceiverFactoryShouldWorkTest()
        {
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Any, 12342), new LengthPrefixBasedCommunicationFactory());
            var client = new MessageHubClient<string>();

            server.Start();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12342));

            Assert.IsTrue(client.Connected);
            Assert.AreEqual(1, server.ClientsCount);

            client.Disconnect();
            server.Stop();

            // Let's wait 100ms to make sure that the disconnect message is processed.
            Thread.Sleep(100);

            Assert.IsFalse(client.Connected);
            Assert.AreEqual(0, server.ClientsCount);
        }
    }
}
