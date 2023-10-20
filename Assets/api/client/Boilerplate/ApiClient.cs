using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace api
{


    /// <summary>
    /// This is what communicates with the API server.
    /// </summary>
    [Serializable]
    public static partial class Client
    {
        const int BACKOFF_MULTIPLIER = 2;
        const int BACKOFF_CAP = 10_000;//10 seconds

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
        private static async void Connect()
        {
            EndPoint endpoint = serverEndpoint;

            int currentBackoff = 10; //ms

            while(endpoint == serverEndpoint)//stop trying if the endpoint changes because this will be called again
            {
                try
                {
                    await communicationSocket.ConnectAsync(serverEndpoint);
                    Debug.Log("Connected to server");
                    StartCommunication();
                    return;
                }
                catch
                {
                    //exponentially backoff
                    await Task.Delay(currentBackoff);

                    currentBackoff *= BACKOFF_MULTIPLIER;
                    currentBackoff = Math.Min(currentBackoff, BACKOFF_CAP);

                    Debug.Log("Failed to connect to server, current backoff: " + currentBackoff.ToString() + "ms");
                }
            }
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