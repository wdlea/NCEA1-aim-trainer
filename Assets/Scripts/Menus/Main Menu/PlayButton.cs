using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private Carousel menuCarousel;
    [SerializeField] private float nextMenuPosition = 1;
    
    public void PlayGame() => menuCarousel.TargetPosition = nextMenuPosition;
}
