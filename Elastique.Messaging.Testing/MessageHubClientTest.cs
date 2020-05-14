using Elastique.Messaging.Client;
using Elastique.Messaging.Common.Exceptions;
using Elastique.Messaging.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace MB.Tcp.Testing
{
    [TestClass]
    public class MessageHubClientTest
    {
        [TestMethod]
        [ExpectedException(typeof(NoConnectionException))]
        public void SendShouldThrowExceptionWhenExecutedWithoutAConnection()
        {
            var client = new MessageHubClient<string>();
            client.Send("Test");
        }

        [TestMethod]
        public void ConnectedFlagShouldBeFalseWithoutAConnection()
        {
            var client = new MessageHubClient<string>();
            Assert.IsFalse(client.Connected);
        }

        [TestMethod]
        public void ConnectedFlagShouldBeTrueWhenConnectionIsOpenAndFalseAfterDiconnect()
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 12345);
            var client = new MessageHubClient<string>();
            var server = new MessageHub<string>(endPoint);
            server.Start();
            client.Connect(endPoint);
            Assert.IsTrue(client.Connected);
            client.Disconnect();
            Assert.IsFalse(client.Connected);
        }

        [TestMethod]
        [ExpectedException(typeof(ServerUnavailableException))]
        public void ConnectionShouldFailWhenNoServerListening()
        {
            var client = new MessageHubClient<string>();
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12346));
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionTimeoutException))]
        public void ConnectionShouldTimeOutWithin1s()
        {
            var client = new MessageHubClient<string>();
            Assert.IsFalse(client.Connected);
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12346), TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void ClientReconnectShouldWork()
        {
            var client = new MessageHubClient<string>();
            var server = new MessageHub<string>(new IPEndPoint(IPAddress.Loopback, 12347));
            server.Start();

            Assert.IsFalse(client.Connected);
            
            client.Connect(new IPEndPoint(IPAddress.Loopback, 12347));
            client.Send("Test 1");
            Assert.IsTrue(client.Connected);
            
            client.Disconnect();
            Assert.IsFalse(client.Connected);

            client.Connect(new IPEndPoint(IPAddress.Loopback, 12347));
            client.Send("Test 2");
            Assert.IsTrue(client.Connected);

            client.Disconnect();
        }
    }
}
