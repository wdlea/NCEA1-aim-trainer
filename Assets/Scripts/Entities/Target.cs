using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Prop
{
    public api.objects.Target Tar
    {
        set
        {
            Frame = value;
            ID = value.ID;
        }
    }

    [SerializeField] int ID;

    public void OnHit()
    {
        Debug.Log("Ouch! ID: " + ID.ToString());
        api.Methods.HitTarget(this.ID);
    }
}
