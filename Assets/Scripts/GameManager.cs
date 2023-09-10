using api;
using api.objects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

/// <summary>
/// manages the game part of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] Me me;

    private Dictionary<string, OtherPlayer> players = new();

    [SerializeField] private Target targetPrefab;
    [SerializeField] private OtherPlayer otherPlayerPrefab;

    [SerializeField] private Transform playerParent;

    [SerializeField] private Text myScore;
    [SerializeField] private Text timeLeft;


    Promise<Game>? gamePromise;

    public static string myName = "";//todo, keep track of this in methods instead of here

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public GameManager() : base()//I know that this is bad Unity code(I'm meant to use Awake()), but Unity wasnt keeping its side of the deal and didn't actually call the code
    {
        Methods.onTargetSpawn = OnTargetSpawn;
        players = new();
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    void Start()
    {
        timeLeft.text = "??:??";
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
                myScore.text = player.Score.ToString().PadLeft(3, '0');
            }
            else
            {
                if (!players.ContainsKey(player.Name))
                    players.Add(player.Name, Instantiate(otherPlayerPrefab, playerParent));

                players[player.Name].Player = player;
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
