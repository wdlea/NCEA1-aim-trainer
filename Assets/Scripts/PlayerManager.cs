using api;
using api.objects;
using System.Collections.Generic;
using UnityEngine;


#nullable enable

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private MyController me;
    private Dictionary<string, OtherPlayer> others;

    [SerializeField] private OtherPlayer otherPrefab;
    [SerializeField] private Transform gameParent;

    [System.NonSerialized] public bool inGame = false;

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("More than 1 instance of PlayerManager in game");
        }
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        api.Client.JoinServer(this);
        MyName = Random.Range(0, 10000).ToString();
    }

    Promise<string>? namePromise;

    private string myName = "";
    public string MyName
    {
        get => myName;
        set
        {
            namePromise = Methods.SetName(value);
        }
    }

    private void OnNameResp(api.Packet p)
    {
        if(p.type == api.PacketType.ClientBoundNameResponse)
        {
            myName = p.message;
        }
        else
        {
            throw new api.UnexpectedPacketException();
        }
    }

    Promise<Game>? gamePromise;

    // Update is called once per frame
    void Update()
    {
        if (inGame)
        {
            if (gamePromise == null)
            {
                gamePromise = Methods.SendFrame(me.Frame);
            }
            else
            {
                if (gamePromise.Finished)
                {
                    if (gamePromise.Get(out Game game) is null)
                    {
                        ApplyGame(game);
                        gamePromise = null;
                    }
                    else
                        Debug.Log("Error recieving game");
                }
            }

            
        }
        if(namePromise != null && namePromise.Finished)
        {
            if (namePromise.Get(out string? newName) != null)
            {
                myName = newName ?? myName;
                namePromise = null;
            }
            else Debug.Log("error getting name");
        }
    }

    private void ApplyGame(Game game)
    {
        foreach(Player player in game.Players)
        {
            if(player.Name == MyName)
            {
                continue;//ignore
            }
            else
            {
                if(!others.ContainsKey(player.Name)){
                    others.Add(player.Name, Instantiate(otherPrefab, gameParent));
                }

                others[player.Name].Frame = player;
            }
        }
    }
}
