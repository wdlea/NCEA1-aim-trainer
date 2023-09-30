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

    Dictionary<int, Target> targets;//I know an array is what you think i need, but if i use a List, the length will consume much more data than it actually needs


    Promise<Game>? gamePromise;

    public static string myName = "";//todo, keep track of this in methods instead of here

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameManager() : base()//I know that this is bad Unity code(I'm meant to use Awake()), but Unity wasnt keeping its side of the deal and didn't actually call the code
    {
        Methods.onTargetSpawn = OnTargetSpawn;
        Methods.onHitTarget = OnTargetDestroy;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private void Start()
    {
        timeLeft.text = "??:??";
        targets = new();
        players = new();
    }

    private void Update()
    {
        if (Methods.IsGameRunning)
        {
            if (gamePromise == null)
                gamePromise = Methods.SendFrame(me.Frame);
            else if (gamePromise.Finished)
            {
                Exception? err = gamePromise.Get(out Game game);

                if (err is not null) throw err;

                ApplyGame(game);
                gamePromise = null;
            }
        }
    }

    void ApplyGame(Game game)
    {
        if (game.Players is null)
            return;

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

        targets[target.ID] = t;
    }

    void OnTargetDestroy(int id)
    {
        if(targets.TryGetValue(id, out Target destroyed))
        {
            targets.Remove(id);

            destroyed.DestroyTarget();
        }
    }
}
