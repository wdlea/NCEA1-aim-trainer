using api;
using api.objects;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class GameManager : MonoBehaviour
{
    [SerializeField] MyController me;

    private Dictionary<string, OtherPlayer> players = new();

    [SerializeField] private OtherPlayer otherPlayerPrefab;
    [SerializeField] private Transform playerParent;

    Promise<Game>? gamePromise;

    string myName;

    private void Update()
    {
        if(gamePromise == null)
            gamePromise = Methods.SendFrame(me.Frame);
        else if (gamePromise.Finished)
        {
            Exception? err = gamePromise.Get(out Game game);

            if (err != null) throw err;

            ApplyGame(game);
            gamePromise = null;
        }
    }


    void ApplyGame(Game game)
    {
        foreach (Player player in game.Players)
        {
            if(player.Name == myName)
            {
                //skip for now
            }
            else
            {
                if (!players.ContainsKey(player.Name))
                    players.Add(player.Name, Instantiate(otherPlayerPrefab, playerParent));

                players[player.Name].Frame = player;
            }
        }
    }
}
