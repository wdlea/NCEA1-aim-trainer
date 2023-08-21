using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private MyController me;
    private Dictionary<string, OtherPlayer> others;

    [SerializeField] private OtherPlayer otherPrefab;
    [SerializeField] private Transform gameParent;

    // Use this for initialization
    void Start()
    {
        api.Client.JoinServer(this);
        MyName = Random.Range(0, 10000).ToString();
    }

    private string myName;
    public string MyName
    {
        get => myName;
        set
        {
            api.Client.EnqueueSend(
                new api.Packet(
                    api.PacketType.ServerBoundName,
                    value
                ),
                new api.ClaimTicket
                {
                    onResponse= OnNameResp,
                    expectedType=api.PacketType.ClientBoundNameResponse
                }
            );
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

    // Update is called once per frame
    void Update()
    {
        api.Client.EnqueueSend(
            api.Packet.FromObject(api.PacketType.ServerBoundFrame, me.Frame),
            new api.ClaimTicket
            {
                onResponse= OnResponse,
                expectedType=api.PacketType.ClientBoundFrameResponse
            }
        );
    }

    private void OnResponse(api.Packet p)
    {
        switch (p.type)
        {
            case api.PacketType.ClientBoundFrameResponse:
                {
                    //do normal things
                    ApplyPacket(p);
                    break;
                }
            case api.PacketType.Error:
                {
                    //handle error
                    Debug.Log("An error packet was recieved: " + p.message);
                    break;
                }
            default:
                {
                    throw new api.UnexpectedPacketException();
                }
        }
    }

    private void ApplyPacket(api.Packet p)
    {
        //p WILL have the correct type because of the above function

        api.objects.Game game = JsonUtility.FromJson<api.objects.Game>(p.message);

        foreach(api.objects.Player player in game.Players)
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
