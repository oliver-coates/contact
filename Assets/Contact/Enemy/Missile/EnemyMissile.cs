using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissle : MonoBehaviour
{

    private Vector3 subPos = Vector3.zero;
    [SerializeField] private float speed;

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

            Destroy(gameObject);
        }
    }
}
