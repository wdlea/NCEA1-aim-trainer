using UnityEngine;

public class Target : Prop
{
    public api.objects.Target Tar
    {
        set
        {
            Frame = value;
            _ID = value.ID;
            _scale = value.Scale;
            _dScale = value.Dscale;
        }
    }

    [SerializeField] float _baseScale;
    [SerializeField] float _scale;
    [SerializeField] float _dScale;

    [SerializeField][ReadOnlyEditor] int _ID;
   
    public void OnHit()
    {
        Debug.Log("Ouch! ID: " + _ID.ToString());
        _ = api.Methods.HitTarget(_ID);

        DestroyTarget();
    }

    protected override void PreUpdatePosition()
    {
        _scale += _dScale * Time.deltaTime;

        transform.localScale = _baseScale * _scale * Vector3.one;

        if (_scale < 0.1f)
            Destroy(gameObject);
    }

    protected override void PostUpdatePosition()
    {

    }

    /// <summary>
    /// Here so i can add animations and custom behavior
    /// </summary>
    public void DestroyTarget()
    {
        ScreenShaker.Instance.Shake();
        SFXManager.Active.PlayShot();
        Destroy(gameObject);
    }
}
