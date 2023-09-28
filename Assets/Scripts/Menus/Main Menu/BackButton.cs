using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Carousel menuCarousel;
    [SerializeField] private Animator buttonAnimator;

    public delegate void OnBack();
    public Queue<OnBack> onBackCalls;

    private void Start()
    {
        onBackCalls ??= new Queue<OnBack>();
    }

    public void BackMenu() { 
        menuCarousel.TargetPosition = Mathf.Clamp(Mathf.Round(menuCarousel.TargetPosition - 1), 0, float.PositiveInfinity);
        
        while(onBackCalls.TryDequeue(out OnBack call))
        {
            call();
        }
    }

    private void Update()
    {
        buttonAnimator.SetFloat("Position", menuCarousel.CurrentPosition);
    }
}
