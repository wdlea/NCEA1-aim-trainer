﻿using UnityEngine;

#nullable enable

namespace api
{
    public static partial class Methods
    {
        public static float? GAME_START_TIME = null;

        public static bool IsGameRunning { get; private set; }

        public enum Broadcast
        {
            StartGame = 'S',
            SpawnTarget = 'T',
            HitTarget = 'H'
        }

        public delegate void OnTargetSpawn(objects.Target target);
        public static OnTargetSpawn? onTargetSpawn;

        public delegate void OnHitTarget(int ID);
        public static OnHitTarget? onHitTarget;

        /// <summary>
        /// Handles a broadcast packet
        /// </summary>
        /// <param name="broadcast">The broadcast packet, 
        /// note that this will be transfomed to omit the
        /// first 'B'</param>
        internal static void HandleBroadcast(Packet broadcast)
        {
            //do stuff with packet
            switch ((Broadcast)broadcast.type)
            {
                case Broadcast.StartGame:
                    {
                        Debug.Log("Started Game");
                        IsGameRunning = true;
                        break;
                    }
                case Broadcast.SpawnTarget:
                    {
                        objects.Target spawned = JsonUtility.FromJson<objects.Target>(broadcast.content);
                        onTargetSpawn?.Invoke(spawned);
                        break;
                    }
                case Broadcast.HitTarget:
                    {
                        int id;
                        try
                        {
                            id = (int)float.Parse(broadcast.content);
                        }
                        catch
                        {
                            Debug.LogWarning("Unable to parse ID from server(trusted information source)");
                            break;
                        }

                        onHitTarget?.Invoke(id);

                        break;
                    }
                default:
                    {
                        throw new UnexpectedPacketException();
                    }
            }
        }
    }
}
