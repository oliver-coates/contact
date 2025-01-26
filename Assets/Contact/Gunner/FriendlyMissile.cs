using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyMissile : MonoBehaviour, IRadarDetectable
{

    private Vector3 targetPos;
    private IRadarDetectable target;
    [SerializeField] private float speed;
    protected int bearing;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            DestroyYou();
        }

        targetPos = target.GetPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 10)
        {
            // Missile Hit Target
            Debug.Log("Enemy Target Destroyed!");
            target.DestroyYou();
            DestroyYou();
        }
    }

    public void SetTarget(IRadarDetectable givenTarget)
    {
        target = givenTarget;
    }

    void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    void OnDestroy()
    {
        Radar.DeregisterRadarDetectable(this);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetBearing(int bearing)
    {
        this.bearing = bearing;
    }

    public void DestroyYou()
    {
        Destroy(gameObject);
    }
}
