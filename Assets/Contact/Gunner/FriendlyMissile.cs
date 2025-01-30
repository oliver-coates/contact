using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyMissile : RadarDetectable
{

    [Header("State:")]
    [SerializeField] private bool _initialised;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private RadarDetectable _target;
    [SerializeField] private float _internalTimer;


    [Header("Settings:")]
    [SerializeField] private float speed;



    public void Initialise(RadarDetectable givenTarget)
    {
        _target = givenTarget;
    
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
        if (_internalTimer > 1)
        {
            _internalTimer = 0;
            SlowUpdate();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void SlowUpdate()
    {
        if (_target == null)
        {
            DestroyThis(DestructionReason.Despawn);
            return;
        }

        targetPos = _target.GetPosition();
        float distanceToTarget = Vector3.Distance(transform.position, targetPos); 


        if (distanceToTarget < (speed + 50))
        { 
            // Destroy the enemy we hit:
            _target.DestroyThis(DestructionReason.HitByMissile);
            DestroyThis(DestructionReason.Despawn);
        }
    }

 




    protected override void Shotdown()
    {

    }
}
