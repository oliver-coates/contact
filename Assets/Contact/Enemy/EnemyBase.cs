using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyBase : MonoBehaviour, IRadarDetectable
{
    protected int bearing;
    public bool isJet;
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    public void Shotdown()
    {   
        if (isJet)
        {
            JetEnemy jet = GetComponent<JetEnemy>();
            jet.ShotDown();
        }
        else
        {
            // Enemy is missle
            Debug.Log($"Enemy Missile Destroyed");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Radar.DeregisterRadarDetectable(this);
    }

    public void SetBearing(int bearing)
    {
        this.bearing = bearing;
    }

    public GameObject GetObject()
    {
        return this.gameObject;
    }
}
