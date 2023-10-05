using api;
using api.Plugins;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [ReadOnlyEditor] [SerializeField] private bool _isCountingDown = false;

    [SerializeField] private float _yOffset;
    [SerializeField] private float _yMultiplier;
    [SerializeField] private float _slideCount;

    [SerializeField] private RectTransform _target;

    [SerializeField] [Range(1, 1000)] private int timerSpeedMS = 600;
    [SerializeField] [Range(1, 1000)] private int timerTransitionMS = 200;


    [SerializeField] [ReadOnlyEditor] private bool _hasCountedDown = false;

    private void Start()
    {
        Broadcasts.onResetGame += () =>
        {
            _hasCountedDown = false;
        };
    }

    private void Update()
    {
        if(!_hasCountedDown && Broadcasts.IsGameRunning)
        {
            StartCountdown();
            _hasCountedDown = true;
        }
    }

    private async void StartCountdown()
    {
        if (_isCountingDown)
            return;

        _isCountingDown = true;

        //_slideCount + 1 beucase i want to slide to empty at the end
        for (int i = 1; i <= _slideCount + 1; i++)
        {
            Debug.Log(i);
            float startTime = Time.realtimeSinceStartup;
            float transitionDuration = timerTransitionMS / 1000f;
            float endTime = startTime + transitionDuration;

            float endPosition = _yMultiplier * i + _yOffset;
            float startPosition = endPosition - _yMultiplier;

            while(Time.realtimeSinceStartup < endTime)
            {
                float progress = (Time.realtimeSinceStartup - startTime) / transitionDuration;

                float current = Mathf.Lerp(startPosition, endPosition, progress);

                _target.anchoredPosition = new Vector2(0, current);

                await Task.Yield();
            }
            
            await Task.Delay(timerSpeedMS - timerTransitionMS);
        }

        _isCountingDown = false;
    }
}
