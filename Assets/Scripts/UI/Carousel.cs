using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
    public float TargetPosition { set { dirty = true; targetPosition = value; } get => targetPosition; }
    private bool dirty;

    [SerializeField] [Range(float.Epsilon, float.PositiveInfinity)] private float transitionDuration;
    [SerializeField] [Range(float.Epsilon, float.PositiveInfinity)] private float slideWidth = 600f;
    
    [SerializeField] [ReadOnlyEditor] private float targetPosition;
    [SerializeField] [ReadOnlyEditor] private float currentPosition;
    [SerializeField] [ReadOnlyEditor] private float startTransitionTime;
    [SerializeField] [ReadOnlyEditor] private float startTransitionPosition;

    RectTransform rectTransform;

    /// <summary>
    /// Shamelessly stolen from https://easings.net/#easeInOutQuint
    /// </summary>
    /// <param name="x">The position in the animation, clamped internally between 0 and 1.</param>
    /// <returns>The progress of the easing function. Always between 0 and 1.</returns>
    float EaseInOutQuint(float x) {
        x = Mathf.Clamp01(x);

        return x < 0.5 ? 16 * x* x* x* x* x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float currentTime = Time.realtimeSinceStartup;

        if (dirty)
        {
            startTransitionTime = currentTime;
            startTransitionPosition = currentPosition;
            dirty = false;
        }

        float progress = (currentTime - startTransitionTime) / transitionDuration;

        float easedProgress = EaseInOutQuint(progress);

        currentPosition = Mathf.Lerp(startTransitionPosition, targetPosition * -slideWidth, easedProgress);

        rectTransform.anchoredPosition = new Vector3(currentPosition, 0);
    }
}
