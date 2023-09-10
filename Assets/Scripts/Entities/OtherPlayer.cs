using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : Prop {

    [SerializeField] private Text scoreText;
    [SerializeField] private Text nameText;

    [SerializeField] private Color[] colours;
    [SerializeField] private Image icon;

    private api.objects.Player player;
    
    public api.objects.Player Player
    {
        set
        {
            Frame = value;
            player = value;
        }
    }

    private void Start()
    {
        icon.color = RandomChoice(colours);
    }

    /// <summary>
    /// Chooses randomly
    /// </summary>
    /// <typeparam name="T">The type of item to choose from.</typeparam>
    /// <param name="choices">All the possible choices</param>
    /// <returns>The choice</returns>
    public T RandomChoice<T>(IList<T> choices)
    {
        int count = choices.Count;
        int choiceIndex = Random.Range(0, count);

        return choices[choiceIndex];
    }

    private void Update()
    {
        scoreText.text = player.Score.ToString().PadLeft(3, '0');
        nameText.text = player.Name;
    }
}