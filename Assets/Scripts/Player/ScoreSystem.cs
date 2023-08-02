using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Notify();


public static class ScoreSystem
{
    private static int currentScore;

    public static Notify OnScoreUpdate;

    public static int CurrentScore
    {
        get => currentScore;
        set
        {
            value = currentScore;
            OnScoreUpdate();
        }
    }
}
