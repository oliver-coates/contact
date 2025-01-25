using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class JetEnemy : MonoBehaviour
{
    
    [SerializeField] private Vector3 subPos = Vector3.zero;

    //Spawn Position determined by the spawn manager
    [SerializeField] private Vector3 startPos;
    
    // Point between startPos and subPos where the jet will fire its missle and then move towards escape point
    [SerializeField] private Vector3 firePos;
    [SerializeField] private Vector3 escapePos;

    void Awake()
    {
        firePos = GetFirePosition(startPos, subPos, 4);
    }

    public Vector3 GetFirePosition(Vector3 startPos, Vector3 subPos, float x)
    {
        firePos = x * Vector3.Normalize(subPos - startPos) + subPos;
        return firePos;
    }

    void Update()  
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, subPos);
        Gizmos.DrawSphere(firePos, 50f);
    }
}
