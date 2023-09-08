using api;
using api.objects;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// manages the game part of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] Me me;

    private Dictionary<string, Entity> players = new();

    [SerializeField] private Target targetPrefab;
    [SerializeField] private OtherPlayer otherPlayerPrefab;

    [SerializeField] private Transform playerParent;
    

    Promise<Game>? gamePromise;

    public static string myName = "";//todo, keep track of this in methods instead of here

    private void Awake()
    {
        Methods.onTargetSpawn = OnTargetSpawn;
        players = new();
    }

    private void Update()
    {
        if (Methods.IsGameStarted)
        {
            if (gamePromise == null)
                gamePromise = Methods.SendFrame(me.Frame);
            else if (gamePromise.Finished)
            {
                Exception? err = gamePromise.Get(out Game game);

                if (err != null) throw err;

                ApplyGame(game);
                gamePromise = null;
            }
        }
    }

    void ApplyGame(Game game)
    {
        foreach (Player player in game.Players)
        {
            if(player.Name == myName)
            {
                Debug.Log(player.Dx);
            }
            else
            {
                if (!players.ContainsKey(player.Name))
                    players.Add(player.Name, Instantiate(otherPlayerPrefab, playerParent));

                players[player.Name].Frame = player;
            }
        }
    }

    void OnTargetSpawn(api.objects.Target target)
    {
        Target t = Instantiate(targetPrefab, playerParent);
        t.Tar = target;
        t.Update();
    }
}
