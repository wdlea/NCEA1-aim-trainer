using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace api
{
    public delegate bool IsPacketSuitable(Packet p);

    public static partial class Client
    {
        const int BUFFER_SIZE = 1024;
        const int PACKET_SIZE = 1024;
        const byte PACKET_DELIMETER = (byte)'\n';

        private static ConcurrentQueue<Claim> _claimQueue = new();

        public static async Task<Packet> GetResponse(IsPacketSuitable isSuitable)
        {
            Claim claim = new Claim(isSuitable);

            _claimQueue.Enqueue(claim);

            Packet? result = await claim.WaitForResult();

            if (result is null)
                throw new UnexpectedPacketException();

            return result;
        }

        public static void DistributePacket(Packet p)
        {
            lock (_claimQueue)
            {
                while (IsConnected)
                {
                    if(_claimQueue.TryDequeue(out Claim claim))
                    {
                        if (claim.CheckPacket(p))
                            return;
                        else throw new UnexpectedPacketException();
                    }
                }
            }
        }

        public static async Task SendPacket(Packet p)
        {
            byte[] message = p.ToBytes();

            await communicationSocket.SendAsync(message, SocketFlags.None);
        }

        public static async Task<Packet> SendPacket(Packet p, IsPacketSuitable isSuitable)
        {
            await SendPacket(p);
            return await GetResponse(isSuitable);
        }


        private static Queue<byte[]> _fragments = new();
        private static async void RecievePackets()
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            while (IsConnected)
            {
                int num = await communicationSocket.ReceiveAsync(buffer, SocketFlags.None);
                _fragments.Enqueue(buffer[0..num]);
            }
        }

        private static async void RecombobulatePackets()
        {
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

                        byte[] bytes = currentPacket.ToArray();
                        Packet p = new Packet(bytes);



                        #pragma warning disable CS4014// I don't really care about when this returns, it handles that internally
                        
                        Task.Run(() => DistributePacket(p));

                        #pragma warning restore CS4014

                        currentPacket.Clear();
                    }  
                    else
                        currentPacket.AddRange(fragment);
                    
                }
                else await Task.Yield();
            }
        }
    }

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
            while (!Cancelled && _result is null && Client.IsConnected)
                await Task.Yield();

            return _result;
        }
    }
}
