using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : Prop {

    [SerializeField] private Text scoreText;
    [SerializeField] private Text nameText;

    private api.objects.Player player;
    
    public api.objects.Player Player
    {
        set
        {
            Frame = value;
            player = value;
        }
    }

    private void Update()
    {
        scoreText.text = player.Score.ToString().PadLeft(3, '0');
        nameText.text = player.Name;
    }
}