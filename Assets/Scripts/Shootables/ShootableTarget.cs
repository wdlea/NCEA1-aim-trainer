using System.Collections;
using UnityEngine;

namespace Shootables
{
    public abstract class Shootable: MonoBehaviour
    {
        public abstract void OnHit();
    }

}
