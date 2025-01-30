using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissle : RadarDetectable
{
    public static event Action OnShotdown;

    private Vector3 subPos = Vector3.zero;
    [SerializeField] private float speed;


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, subPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, subPos) < 50)
        {
            // Missile Hit Target
            Engineer.TakeDamage();

            DestroyThis(DestructionReason.HitPlayer);
        }
    }

    protected override void Shotdown()
    {
        OnShotdown?.Invoke();
    }


}
