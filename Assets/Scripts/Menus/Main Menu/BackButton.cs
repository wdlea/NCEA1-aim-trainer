using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Carousel menuCarousel;
    [SerializeField] private Animator buttonAnimator;

    public void BackMenu() => menuCarousel.TargetPosition = Mathf.Clamp(Mathf.Round(menuCarousel.TargetPosition - 1), 0, float.PositiveInfinity);

    private void Update()
    {
        buttonAnimator.SetFloat("Position", menuCarousel.CurrentPosition);
    }
}
