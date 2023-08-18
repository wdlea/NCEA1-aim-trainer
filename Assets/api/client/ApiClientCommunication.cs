using System;
using System.Text;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        internal static void Send(Packet packet)
        {

        }
    }

    internal enum PacketType
    {
        Error = 'E',

        ServerBoundFrame = 'f',
        ClientBoundFrameResponse = 'F',


    }

    [Serializable]
    internal struct Packet
    {
        public readonly PacketType type;

        public readonly string message;

        public Packet(PacketType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        public Packet(byte[] packet) : this(
            (PacketType)(char)packet[0], 
            Encoding..GetString(packet[1..])) { }

        public static Packet FromObject(PacketType type, object message)
        {
            return new Packet(type, JsonUtility.ToJson(message));
        }

        public byte[] ToBytes()
        {

        }
    }   
}