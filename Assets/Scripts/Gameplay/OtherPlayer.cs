using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : Prop {

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _nameText;

    [SerializeField] private Color[] _colours;
    [SerializeField] private SpriteRenderer _renderer;

    private api.objects.Player _player;
    
    public api.objects.Player Player
    {
        set
        {
            Frame = value;
            _player = value;
        }
    }

    private void Start()
    {
        _renderer.color = RandomChoice(_colours);
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

    protected override void PreUpdatePosition()
    {
        if (_player == null)//return early to avoid error
            return;

        _scoreText.text = _player.Score.ToString().PadLeft(3, '0');
        _nameText.text = _player.Name;
    }

    protected override void PostUpdatePosition()
    {
        
    }
}