# Elastique.Messaging

## General information

This repository contains three .NET Standard 2.0 class library projects:
- Elastique.Messaging.Server - Server library for simple TCP messaging,
- Elastique.Messaging.Common - Common library for simple TCP messaging used by client and server,
- Elastique.Messaging.Client - Client library for simple TCP messaging.

## Usage

### Elastique.Messaging.Server

Starting the server is very simple. Just provide the interface to listen on and a port number. Below an example that starts the server on port 12345 on all available interfaces.
```
var server = new MessageHub<string>(new IPEndPoint(IPAddress.Any, 12345));
server.Start();
```

### Elastique.Messaging.Client

Connecting to the server is very simple too. Just provide the end point the server is running on. Below an example that connect to a server running on port 12345 on the same machine.
```
var client = new MessageHubClient<string>();
client.Connect(new IPEndPoint(IPAddress.Loopback, 12345));
```

### Additional information
Both server and client can send and receive information. Once data is received DataReceived event handler is invoked. In the example above messages were simple strings, but one can define their own class used for transferring messages between the client and server.

By default 4 byte length-prefix framing protocol is used. This means that first 4 bytes contain the message length and are followed by the message. This can be changed by writing own implementations of `IMessageSender`, `IMessageReceiver` and `ISenderReceiverFactory` and supplying the factory upon object creation. Make sure to use the same protocol on server and client sides.

## License

This project is licensed under the MIT License - see the <a href="https://licenses.nuget.org/MIT">license file</a> for details
