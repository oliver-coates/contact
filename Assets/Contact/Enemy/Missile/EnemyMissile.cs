using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissle : MonoBehaviour, IRadarDetectable
{

    private Vector3 subPos = Vector3.zero;
    [SerializeField] private float speed;
    

    private int _bearing;
    public int bearing
    {
        get
        {
            return _bearing;
        }
    }

    public void Shotdown()
    {
        Radar.DeregisterRadarDetectable(this);
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetBearing(int bearing)
    {
        _bearing = bearing;
    }

    private void Start()
    {
        Radar.RegisterRadarDetectable(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, subPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, subPos) < 50)
        {
            // Missile Hit Target
            // Debug.Log("We're hit!!!");


            // Call Damage thingy
            Engineer.TakeDamage();

            Shotdown();
        }
    }

}
