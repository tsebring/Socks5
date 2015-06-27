using socks5.Socks;
using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace socks5
{
    class SocksTunnel
    {
        public event SocksClient.DataReceivedEventHandler OnDataReceivedRemote = null;
        public event SocksClient.DataReceivedEventHandler OnDataReceivedClient = null;

        public SocksRequest Req;

        public SocksClient Client;
        public Client RemoteClient;

        private int Timeout = 10000;
        private int PacketSize = 65535;

        public SocksTunnel(SocksClient p, SocksRequest req, int packetSize, int timeout)
        {
            RemoteClient = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), PacketSize);
            Client = p;
            Req = req;
            PacketSize = packetSize;
            Timeout = timeout;
        }

        public void Open()
        {
            if (Req.Address == null || Req.Port <= -1) { Client.Client.Disconnect(); return; }
            var socketArgs = new SocketAsyncEventArgs { RemoteEndPoint = new IPEndPoint(Req.IP, Req.Port) };
            socketArgs.Completed += socketArgs_Completed;
            RemoteClient.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (!RemoteClient.Sock.ConnectAsync(socketArgs))
                ConnectHandler(socketArgs);
        }

        void socketArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            byte[] request = Req.GetData(); // Client.Client.Send(Req.GetData());
            if (e.SocketError != SocketError.Success)
            {
                Console.WriteLine("Error while connecting: {0}", e.SocketError.ToString());
                request[1] = (byte)SocksError.Unreachable;
            }
            else
            {
                request[1] = 0x00;
            }

            Client.Client.Send(request);

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    //connected;
                    ConnectHandler(e);
                    break;               
            }
        }

        private void ConnectHandler(SocketAsyncEventArgs e)
        {
            //start receiving from both endpoints.
            try
            {
                Client.Client.onDataReceived += Client_onDataReceived;
                RemoteClient.onDataReceived += RemoteClient_onDataReceived;
                RemoteClient.onClientDisconnected += RemoteClient_onClientDisconnected;
                Client.Client.ReceiveAsync();
                RemoteClient.ReceiveAsync();
            }
            catch
            {
            }
        }

        void RemoteClient_onClientDisconnected(object sender, ClientEventArgs e)
        {
            Client.Client.Disconnect();
        }

        void RemoteClient_onDataReceived(object sender, DataEventArgs e)
        {
            if (OnDataReceivedRemote != null)
            {
                var frame = OnDataReceivedRemote(
                    sender,
                    new FrameEventArgs(new Frame(new ArraySegment<byte>(e.Buffer, e.Offset, e.Count))));

                Client.Client.SendAsync(frame.Data.Array, frame.Data.Offset, frame.Data.Count);
            }
            else Client.Client.SendAsync(e.Buffer, e.Offset, e.Count);

            RemoteClient.ReceiveAsync();
        }

        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            if (OnDataReceivedClient != null)
            {
                var frame = OnDataReceivedClient(
                    sender,
                    new FrameEventArgs(new Frame(new ArraySegment<byte>(e.Buffer, e.Offset, e.Count))));

                RemoteClient.SendAsync(frame.Data.Array, frame.Data.Offset, frame.Data.Count);
            }
            else RemoteClient.SendAsync(e.Buffer, e.Offset, e.Count);

            Client.Client.ReceiveAsync();
        }
    }
}
