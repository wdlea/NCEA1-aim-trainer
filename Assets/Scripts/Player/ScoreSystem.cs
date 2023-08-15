using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Notify();


public static class ScoreSystem
{
    private static int currentScore;

    public static Notify OnScoreUpdate = null;

    public static int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            OnScoreUpdate?.Invoke();
            Debug.Log("New score: " + currentScore.ToString());
        }
    }
}
