using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CarouselNavigationButton : MonoBehaviour
{
    [SerializeField] private Carousel menuCarousel;
    [SerializeField] private float targetMenuPosition;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() => menuCarousel.TargetPosition = targetMenuPosition;
}
