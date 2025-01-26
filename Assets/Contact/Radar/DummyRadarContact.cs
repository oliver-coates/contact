using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRadarContact : MonoBehaviour, IRadarDetectable
{
    private void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetBearing(int bearing)
    {
        
    }

    public void DestroyYou()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Radar.DeregisterRadarDetectable(this);
    }
}
