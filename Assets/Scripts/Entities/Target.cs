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

    protected override void PreUpdatePosition()
    {

    }

    protected override void PostUpdatePosition()
    {

    }

    /// <summary>
    /// Here so i can add animations and custom behavior
    /// </summary>
    public void DestroyTarget()
    {
        
        Destroy(this.gameObject);
    }
}
