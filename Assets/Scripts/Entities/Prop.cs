﻿using UnityEngine;

/// <summary>
/// A prop is an entity controlled exclusively by the server
/// </summary>
public abstract class Prop : Entity {
    public void Update()
    {
        PreUpdatePosition();
        X += Dx * Time.deltaTime;
        Y += Dy * Time.deltaTime;

        ClampPosition();
        ApplyPosition();
        PostUpdatePosition();
    }
    /// <summary>
    /// Called before the props new position is calculated
    /// </summary>
    protected abstract void PreUpdatePosition();

    /// <summary>
    /// Called after the props new position is calculated
    /// </summary>
    protected abstract void PostUpdatePosition();
}

