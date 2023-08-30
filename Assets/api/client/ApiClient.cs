using System;
using System.Net.Sockets;
using System.Net;

namespace api
{
    /// <summary>
    /// This is what communicates with the API server.
    /// </summary>
    [Serializable]
    public static partial class Client
    {
        public static bool IsConnected => communicationSocket.Connected;

        private static Socket communicationSocket;

        private static EndPoint serverEndpoint;
        internal static EndPoint ServerEndpoint
        {
            get => serverEndpoint;
            set
            {
                if(serverEndpoint != value)//if the endpoint has changed
                {
                    //disconnect and reconnect
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

        /// <summary>
        /// Connects to server
        /// </summary>
        /// <returns>Whether the operation succeeded.</returns>
        private static void Connect()
        {
                communicationSocket.Connect(serverEndpoint);
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        /// <returns>Whether it managed to disconnect 
        /// "cleanly". However, this operation will **always**
        /// result in disconnect.</returns>
        private static bool Disconnect()
        {
            try//try do clean way first and disconnect
            {

                communicationSocket.Send(System.Text.Encoding.Unicode.GetBytes("t\n"));//terminate cleanly on server
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

        /// <summary>
        /// Recreates the socket used for communication, 
        /// this gets used to hard reset the socket if there 
        /// is an error.
        /// </summary>
        private static void RegenerateSocket()
        {
            communicationSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);//create a TCP socket
        }
    }
}