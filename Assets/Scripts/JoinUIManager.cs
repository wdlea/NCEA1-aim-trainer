using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinUIManager : MonoBehaviour
{

    [SerializeField] InputField nameInput;
    [SerializeField] InputField codeInput;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateName()
    {
        PlayerManager.Instance.MyName = nameInput.text;
    }

    public void JoinGame()
    {
        UpdateName();

        api.Client.EnqueueSend(
            new api.Packet(
                api.PacketType.ServerBoundJoin,
                codeInput.text
                ),
            new api.ClaimTicket
            {
                expectedType=api.PacketType.ClientBoundJoinResponse,
                onResponse = (api.Packet p) => {
                    if(p.type == api.PacketType.ClientBoundJoinResponse)
                    {
                        PlayerManager.Instance.inGame = true;
                    }
                    else
                    {
                        PlayerManager.Instance.inGame = false;
                        throw new api.UnexpectedPacketException();
                    }
                }
            }
        );
    }
}
