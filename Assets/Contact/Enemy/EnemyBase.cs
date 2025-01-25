using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IRadarDetectable
{
    protected int bearing;

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    void OnDestroy()
    {
        Radar.DeregisterRadarDetectable(this);
    }

    public void SetBearing(int bearing)
    {
        this.bearing = bearing;
    }
}
