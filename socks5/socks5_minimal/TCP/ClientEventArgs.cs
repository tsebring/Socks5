using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using socks5.Socks;
namespace socks5.TCP
{
    public class ClientEventArgs : EventArgs
    {
        public Client Client { get; private set; }
        public ClientEventArgs(Client client)
        {
            Client = client;
        }
    }
    public class SocksClientEventArgs : EventArgs
    {
        public SocksClient Client { get; private set; }
        public SocksClientEventArgs(SocksClient client)
        {
            Client = client;
        }
    }

    public class SocksAuthenticationEventArgs : EventArgs
    {
        public User User { get; private set; }
        public SocksAuthenticationEventArgs(User loginInfo)
        {
            User = loginInfo;
        }
    }

    public class FrameEventArgs : EventArgs
    {
        //public Frame Frame { get; private set; }
        public byte[] Data { get; set; }
        //public FrameEventArgs(Frame frame)
        public FrameEventArgs(byte[] data)
        {
            Data = data;
            //Frame = frame;
        }
    }
}
