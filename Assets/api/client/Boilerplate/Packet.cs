﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace api
{
    /// <summary>
    /// The type of packet.
    /// </summary>
    public enum PacketType
    {
        Error = 'E',

        ServerBoundFrame = 'f',
        ClientBoundFrameResponse = 'F',

        ServerBoundName = 'n',
        ClientBoundNameResponse = 'N',

        ServerBoundJoin = 'j',
        ClientBoundJoinResponse = 'J',

        ServerBoundLeave = 'l',
        ClientBoundLeaveResponse = 'L',

        ServerBoundCreate = 'c',
        ClientBoundCreateResponse = 'C',

        ClientBoundBroadcast = 'B',

        ServerBoundHitTarget = 'h',
        ClientBoundHitTargetResponse = 'H',


        ServerBoundTerminate = 't',
    }

    /// <summary>
    /// A packet is a single "unit"
    /// that gets sent to the server.
    /// </summary>
    [Serializable]
    public class Packet
    {
        public readonly PacketType Type;

        public readonly string Content;

        public Packet(PacketType type, string message)
        {
            Type = type;
            Content = message;
        }

        public Packet(byte[] packet) : this(
            (PacketType)(char)packet[0],
            Encoding.ASCII.GetString(packet[1..]))
        { }

        public static Packet FromObject(PacketType type, object message)
        {
            return new Packet(type, JsonUtility.ToJson(message));
        }

        public byte[] ToBytes(IEnumerable<byte> suffix)
        {
            byte[] encodedType = Encoding.ASCII.GetBytes(
                    new[] { (char)Type }
            );

            byte[] encodedMessage = Encoding.ASCII.GetBytes(Content);


            return encodedType.Concat(encodedMessage).Concat(suffix).ToArray();
        }

        public override bool Equals(object obj)
        {
            return obj is Packet p && p.Type == Type && p.Content == Content;
        }

        public override int GetHashCode()
        {
            return (int)Type + Content.Length;
        }

        public static bool operator ==(Packet left, Packet right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Packet left, Packet right)
        {
            return !left.Equals(right);
        }
    }
}