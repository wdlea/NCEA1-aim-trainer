using System;
using System.Net.Sockets;
using System.Net;

namespace api
{
    [Serializable]
    public static partial class Client
    {
        public static bool Connected => communicationSocket.Connected;

        private static Socket communicationSocket;

        private static EndPoint serverEndpoint;
        internal static EndPoint ServerEndpoint
        {
            get => serverEndpoint;
            set
            {
                if(serverEndpoint != value)
                {
                    Disconnect();
                    serverEndpoint = value;
                    Connect();
                }
            }
        }

        static Client()
        {
            RegenerateSocket();
        }

        private static bool Connect()
        {
            try
            {
                communicationSocket.Connect(serverEndpoint);
                return true;
            }
            catch
            {
                Disconnect();
                return false;
            }
            
        }

        private static bool Disconnect()
        {
            try//try do clean way first and disconnect
            {
                communicationSocket.Disconnect(true);//i do want to reuse the socket
                return true;
            }
            catch//if that doesnt work just aboandon the socket
            {
                communicationSocket.Dispose();
                RegenerateSocket();
                return false;
            }
            
        }

        private static void RegenerateSocket()
        {
            communicationSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);//create a TCP socket
        }
    }
}