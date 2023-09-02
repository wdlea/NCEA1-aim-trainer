using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Indicator : MonoBehaviour
{
    Image image;

    [SerializeField] private Sprite completed;
    [SerializeField] private Sprite pending;
    [SerializeField] private Sprite awaiting;

    private void Start()
    {
        image = GetComponent<Image>();
        State = IndicatorState.Awaiting;
    }

    public enum IndicatorState
    {
        Completed,
        Pending,
        Awaiting
    }

    public IndicatorState State
    {
        set
        {
            switch (value)
            {
                case IndicatorState.Completed:
                    {
                        image.sprite = completed;
                        break;
                    }
                case IndicatorState.Pending:
                    {
                        image.sprite = pending;
                        break;
                    }
                case IndicatorState.Awaiting:
                    {
                        image.sprite = awaiting;
                        break;
                    }
            }
        }
    }
}
