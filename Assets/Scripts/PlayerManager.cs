using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    string MyName = "drippy placeholder name";

    MyController me;
    Dictionary<string, OtherPlayer> others;

    // Use this for initialization
    void Start()
    {
        api.Client.JoinServer(this);
    }

    private string myName;
    public string MyName
    {
        get => myName;
        set
        {
            api.Client.EnqueueSend(
                new api.Packet(
                    )
            );
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
                others[player.Name].Frame = player;
            }
        }
    }
}
