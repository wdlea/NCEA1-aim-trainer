using UnityEngine;

#nullable enable

namespace api
{
    public static partial class Methods
    {
        public static float? GameStartInterval = null;

        public static bool IsGameActive => GameStartInterval is not null;
        public static bool IsGameRunning => IsGameActive && GameStartInterval <= 0;


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

        public delegate void OnResetGame();
        public static OnResetGame onResetGame = new OnResetGame(()=> { });

        internal static void ResetGame()
        {
            GameStartInterval = null;
            onResetGame.Invoke();
        }

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
                        GameStartInterval = float.Parse(broadcast.content);

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

    public class Broadcasts : Client.IPlugin
    {
        public Packet? Process(Packet p)
        {
            return p;
        }
    }
}
