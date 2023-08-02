using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shootables
{
    public class SmashTarget : Shootable
    {
        [SerializeField] private int pointDifference;
        public override void OnHit()
        {
            ScoreSystem.CurrentScore += pointDifference;
        }
    }
}
