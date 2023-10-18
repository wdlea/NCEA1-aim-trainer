using api;
using api.Plugins;
using api.objects;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

#nullable enable

/// <summary>
/// manages the game part of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private Me _me;

    private Dictionary<string, OtherPlayer> _players = new();

    [SerializeField] private Target _targetPrefab;
    [SerializeField] private OtherPlayer _otherPlayerPrefab;

    [SerializeField] private Transform _playerParent;

    [SerializeField] private Text _myScore;
    [SerializeField] private Text _timeLeft;

    private Dictionary<int, Target> _targets;//I know an array is what you think i need, but if i use a List, the length will consume much more data than it actually needs


    public static string MyName = "";//todo, keep track of this in methods instead of here

    private void Awake()
    {
        Broadcasts.onTargetSpawn = OnTargetSpawn;
        Broadcasts.onHitTarget = OnTargetDestroy;
    }
    private void Start()
    {
        Debug.Log("Start");
        _timeLeft.text = "??:??";//I renamed it and hadn't set it in the editior, thus NullReferenceException
        _targets = new();
        _players = new();

        StartCoroutine(UpdateGame());//so this was not called
    }

    IEnumerator UpdateGame()
    {
        Task<Game>? game = null;

        while (!Broadcasts.IsGameRunning)
            yield return null;//Yield until game start

        while (Broadcasts.IsGameRunning)
        {
            if (game is not null)
            {
                yield return null;

                try{
                    if(game.IsCompletedSuccessfully){
                        Game result = game.GetAwaiter().GetResult();
                        ApplyGame(result);
                    }else if(game.IsFaulted || game.IsCanceled){
                        game.GetAwaiter().GetResult();
                    }else continue;//go back to start if promise not fulfiled
                }catch(Exception e){
                    Debug.LogError(e);
                }
            }

            //create a new frame request
            game = Methods.SendFrame(_me.Frame);
        }  
        Debug.Log("Game stopped");
    }

    void ApplyGame(Game game)
    {
        if (game.Players is null)
            return;

        foreach (Player player in game.Players)
        {
            if(player.Name == MyName)
            {
                _myScore.text = player.Score.ToString().PadLeft(3, '0');
            }
            else
            {
                if (!_players.ContainsKey(player.Name))
                    _players.Add(player.Name, Instantiate(_otherPlayerPrefab, _playerParent));

                _players[player.Name].Player = player;
            }
        }
    }

    void OnTargetSpawn(api.objects.Target target)
    {
        Target t = Instantiate(_targetPrefab, _playerParent);
        t.Tar = target;
        t.Update();

        _targets[target.ID] = t;
    }

    void OnTargetDestroy(int id)
    {
        if(_targets.TryGetValue(id, out Target destroyed))
        {
            _targets.Remove(id);

            destroyed.DestroyTarget();
        }
    }
}
