using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private Camera _shakeCamera = Camera.main;

    public static bool ReducedMotion = false;

    public static ScreenShaker Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("More than 1 active ScreenShaker in scene");

        Instance = this;
    }

    Vector3? _shakeOrigin = null;

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="rate">The rate of each shake in Hz.</param>
    /// <param name="movement">The maximum movement for the X and Y axis of each shake in M.</param>
    /// <param name="duration">The duration of the shaking in S.</param>
    /// <param name="resetTime">The duration of the reset in S.</param>
    public async void Shake(float rate = 5f, float movement = 0.2f, float duration = 0.5f, float resetTime = 0.1f)
    {
        float startTime = Time.realtimeSinceStartup;
        float endime = startTime + duration;

        float shakeDuration = 1 / rate;

        Transform shakeTransform = _shakeCamera.transform;

        //keep this to use as backup
        Vector3 myOrigin = shakeTransform.position;

        bool doUnsetOrigin = _shakeOrigin is null;

        //don't set other shakes origin unless they unset it
        _shakeOrigin ??= myOrigin;

        while (Time.realtimeSinceStartup < endime)
        {
            float shakeStartTime = Time.realtimeSinceStartup;
            float shakeEndTime = shakeStartTime + shakeDuration;

            shakeEndTime = Mathf.Min(shakeEndTime, endime);



            Vector3 shakeOffset = new Vector3(
                Random.Range(-movement, movement),
                Random.Range(-movement, movement)
            );

            Vector3 startShakePos = shakeTransform.position;
            Vector3 endShakePos = (_shakeOrigin ?? myOrigin) + shakeOffset;

            while(Time.realtimeSinceStartup < shakeEndTime)
            {
                float shakeProgress = (Time.realtimeSinceStartup - shakeStartTime) / shakeDuration;

                shakeTransform.position = Vector3.Lerp(startShakePos, endShakePos, shakeProgress);

                await Task.Yield();
            }
        }

        float startResetTime = Time.realtimeSinceStartup;
        float endResetTime = startResetTime + resetTime;

        Vector3 startResetPos = shakeTransform.position;

        while(Time.realtimeSinceStartup < endResetTime)
        {
            float resetProgress = (Time.realtimeSinceStartup - startResetTime) / resetTime;

            shakeTransform.position = Vector3.Lerp(startResetPos, (_shakeOrigin ?? myOrigin), resetProgress);

            await Task.Yield();
        }

        shakeTransform.position = (_shakeOrigin ?? myOrigin);

        if (doUnsetOrigin)
            _shakeOrigin = null;
    }
}
