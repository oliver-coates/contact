using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyMissile : MonoBehaviour, IRadarDetectable
{

    private Vector3 targetPos;
    private bool _initialised;
    private IRadarDetectable target;
    [SerializeField] private float speed;
    protected int bearing;
    private float _internalTimer;



    public void Initialise(IRadarDetectable givenTarget)
    {
        Debug.Log($"Initialised with {givenTarget}");
        Debug.Log($"->> {givenTarget as UnityEngine.Object}");
        target = givenTarget;
    
        Radar.RegisterRadarDetectable(this);

        SlowUpdate();
        _internalTimer = 0;

        _initialised = true;
    }

    private void Update()
    {
        if (_initialised == false)
        {
            return;
        }

        _internalTimer += Time.deltaTime;
        if (_internalTimer > 4)
        {
            _internalTimer = 0;
            SlowUpdate();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void SlowUpdate()
    {
        if ((target.Equals(null)))
        {
            Debug.Log($"Destroying myself!!!");
            Shotdown();
            return;
        }

        targetPos = target.GetPosition();
    
        if (Vector3.Distance(transform.position, targetPos) < 50)
        { 
            target.Shotdown();
            Shotdown();
        }
    }

 



    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetBearing(int bearing)
    {
        this.bearing = bearing;
    }

    public void Shotdown()
    {
        Radar.DeregisterRadarDetectable(this);
        Destroy(gameObject);
    }

}
