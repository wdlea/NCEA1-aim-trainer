using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace api
{
    internal delegate bool IsPacketSuitable(Packet p);

    public static partial class Client
    {
        const int BUFFER_SIZE = 1024;
        const byte PACKET_DELIMETER = (byte)'\n';
        readonly static IEnumerable<byte> PACKET_DELIMETER_ARR = new[] { PACKET_DELIMETER };

        private static readonly ConcurrentQueue<Claim> _claimQueue = new();
        private static readonly Queue<byte[]> _fragments = new();

        /// <summary>
        /// Gets the next packet recieved that fulfils isSuitable
        /// </summary>
        /// <param name="isSuitable">The delegate to check if a packet is ok.</param>
        /// <returns>The first packet isSuitable returned <c>true</c> for</returns>
        internal static async Task<Packet> GetResponse(IsPacketSuitable isSuitable)
        {
            Claim claim = new Claim(isSuitable);

            _claimQueue.Enqueue(claim);

            Packet? result = await claim.WaitForResult() ?? throw new UnexpectedPacketException();
            
            return result;
        }

        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="p">The packet to send</param>
        /// <returns></returns>
        internal static async Task SendPacket(Packet p)
        {
            byte[] message = p.ToBytes(PACKET_DELIMETER_ARR);

            await communicationSocket.SendAsync(message, SocketFlags.None);
        }

        /// <summary>
        /// Shortcut for calling 
        /// <code>
        ///     await Client.SendPacket(p);
        ///     await Client.GetResponse(isSuitable);
        /// </code>
        /// </summary>
        /// <param name="p">The packet to send</param>
        /// <param name="isSuitable">A delegate for matching a recieved packet to the response</param>
        /// <returns></returns>
        internal static async Task<Packet> SendPacket(Packet p, IsPacketSuitable isSuitable)
        {
            await SendPacket(p);
            return await GetResponse(isSuitable);
        }
        private delegate void Cleanup();
        private static Cleanup? _cleanup;

        /// <summary>
        /// Starts the neccessary threads for sending and recieving packets
        /// </summary>
        private static void StartCommunication()
        {
            _cleanup?.Invoke();

            Task recieveTask = Task.Run(RecievePackets);
            Task processTask = Task.Run(ProcessFragments);

            _cleanup = async () =>
            {
                _cleanup = null;
                Disconnect();

                _claimQueue.Clear();
                _fragments.Clear();

                await Task.WhenAll(recieveTask, processTask);

                recieveTask.Dispose();
                processTask.Dispose();
            };

            Surrogate.onQuit.Enqueue(() => _cleanup?.Invoke());
        }

        /// <summary>
        /// The thread that handles reading data from the socket
        /// </summary>
        private static async void RecievePackets()
        {
            Thread.CurrentThread.Name = "Recieve Thread";//for debugger

            byte[] buffer = new byte[BUFFER_SIZE];
            int num;

            Task disconnect = ServerDisconnect(10);

            while (true)
            {
                Task<int> recieveTask = communicationSocket.ReceiveAsync(buffer, SocketFlags.None);

                await Task.WhenAny(
                    recieveTask,
                    disconnect
                );

                if(!IsConnected)
                    break;

                num = await recieveTask;//should complete immediately if the above check passes becuase the WhenAny must have returned becuase of this task
                _fragments.Enqueue(buffer[0..num]);
            }
            Debug.Log("Connection closed");
        }


        static SemaphoreSlim _claimLock = new(0, 1);
        /// <summary>
        /// Handles executing plugins then matching a packet to a claim
        /// </summary>
        /// <param name="p">The packet to match</param>
        private static async Task<bool> HandlePacket(Packet p)
        {
            Packet? processed = ApplyPlugins(p);
            if (processed is null) return true;
            p = processed;
            
            Claim claim;
            await _claimLock.WaitAsync();//stop claims from getting out of order
            
            try{

                while(!_claimQueue.TryDequeue(out claim)){
                    await Task.Yield();
                    
                    if(!IsConnected) 
                        return false;
                }

            }finally{
                _claimLock.Release();
            }
            
            return claim.CheckPacket(p);
        }

        /// <summary>
        /// The thread that processes recieved data from the socket into packets
        /// </summary>
        private static async void ProcessFragments()
        {
            Thread.CurrentThread.Name = "Process Thread";//for debugger
            List<byte> currentPacket = new();

            while (IsConnected)
            {
                if (_fragments.TryDequeue(out byte[] fragment))
                {
                    int index = Array.IndexOf(fragment, PACKET_DELIMETER);

                    //IndexOf returns -1 if it could not find the packet

                    if(index >= 0)
                    {
                        currentPacket.AddRange(fragment[0..index]);

                        Packet p = new Packet(currentPacket.ToArray());

                        Task<bool> match = HandlePacket(p);

                        currentPacket.Clear();
                        currentPacket.TrimExcess();

                        if (!await match)
                            Debug.Log("Unable to match packet");
                    }  
                    else{
                        currentPacket.AddRange(fragment);
                        Debug.Log("No EOP recieved, adding to packet, current content:" + System.Text.Encoding.UTF8.GetString(currentPacket.ToArray()));
                    }   
                }

                await Task.Yield();
            }
            Debug.Log("Connection closed");
        }

        /// <summary>
        /// Returns when client disconnects from the server.
        /// </summary>
        /// <param name="checkFrequency">The frequency (per second) to check
        /// for a disconnect</param>
        /// <returns>A task that completes when the server disconnects</returns>
        public static async Task ServerDisconnect(int checkFrequency){
            int checkInterval = 1000/checkFrequency;

            while(IsConnected)
                await Task.Delay(checkInterval);

            return;
        }


        /// <summary>
        /// A delegate which cancels the operation when it returns true.
        /// </summary>
        /// <returns>Whether to cancel.</returns>
        public delegate bool CancelSignal();

        /// <summary>
        ///  Returns when the client disconnects from the server or when cancel returns true.
        /// </summary>
        /// <param name="checkFrequency">The frequency (per second) to check
        /// for a disconnect.</param>
        /// <param name="cancel">A delegate which is executed checkFrequency times a second.</param>
        /// <returns>A task that completes when the server disconnects or this is canceled</returns>
        public static async Task ServerDisconnect(int checkFrequency, CancelSignal cancel){
            int checkInterval = 1000/checkFrequency;

            while(IsConnected && !cancel.Invoke())
                await Task.Delay(checkInterval);

            return;
        }
    }

    /// <summary>
    /// Represents a Claim to a packet
    /// </summary>
    internal class Claim
    {
        public bool Cancelled { get; private set; }

        private IsPacketSuitable _isSuitable;

        private Packet? _result;

        public Claim(IsPacketSuitable isSuitable)
        {
            _isSuitable = isSuitable;
        }

        public void Cancel()
        {
            Cancelled = true;
        }

        public bool CheckPacket(Packet p)
        {
            bool suitable = _isSuitable(p);

            if (suitable)
                _result = p;

            return suitable;
        }

        public async Task<Packet?> WaitForResult()
        {
            while (!Cancelled && _result is null && Client.IsConnected) await Task.Yield();

            return _result;
        }
    }
}
