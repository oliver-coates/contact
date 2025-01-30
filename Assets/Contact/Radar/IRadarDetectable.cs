using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RadarDetectable : MonoBehaviour
{
    public enum DestructionReason
    {
        HitByMissile,
        HitPlayer,
        Despawn
    }

    [SerializeField] private int _bearing;
    public int bearing
    {
        get
        {
            return _bearing;
        }	
    }

    protected virtual void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    protected virtual void OnDestroy()
    {
        Radar.DeregisterRadarDetectable(this);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetBearing(int bearing)
    {
        _bearing = bearing;
    }

    public void DestroyThis(DestructionReason reason)
    {
        Debug.Log($"{gameObject.name} was shot down! Reason: {reason}");

        if (reason == DestructionReason.HitByMissile)
        {
            Shotdown();   
        }

        Destroy(gameObject);
    }

    protected abstract void Shotdown();


}
