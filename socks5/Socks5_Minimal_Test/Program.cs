using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
namespace Socks5_Minimal_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server f = new Socks5Server(IPAddress.Any, 10003);
            f.Authentication = true;
            f.OnAuthentication += f_OnAuthentication;
            f.OnDataReceivedClient += f_OnDataReceivedClient;
            f.OnDataReceivedRemote += f_OnDataReceivedRemote;
            f.Start();
        }

        //static socks5.Socks.Frame f_OnDataReceivedRemote(object sender, socks5.TCP.FrameEventArgs e)
        static byte[] f_OnDataReceivedRemote(object sender, socks5.TCP.FrameEventArgs e)
        {
            //return e.Frame;
            return e.Data;
        }

        //static socks5.Socks.Frame f_OnDataReceivedClient(object sender, socks5.TCP.FrameEventArgs e)
        static byte[] f_OnDataReceivedClient(object sender, socks5.TCP.FrameEventArgs e)
        {
            //return e.Frame;
            return e.Data;
        }

        static socks5.Socks.LoginStatus f_OnAuthentication(object sender, socks5.TCP.SocksAuthenticationEventArgs e)
        {
            if(e.User.Username == "Thr" && e.User.Password == "yoloswag")
                return socks5.Socks.LoginStatus.Correct;
            return socks5.Socks.LoginStatus.Denied;
        }
    }
}
