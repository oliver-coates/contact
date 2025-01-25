using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class JetEnemy : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("Position References")]
    [SerializeField] private Vector3 subPos = Vector3.zero;

    //Spawn Position determined by the spawn manager
    [SerializeField] private Vector3 startPos;
    
    // Random point on a line between startPos and subPos
    // Excludes any area larger than maxFirePos and smaller than minFirePos
    [SerializeField] private Vector3 firePos;
    
    // Neither of these values should be higher than 1000
    [SerializeField] private float minFirePos = 80;
    [SerializeField] private float maxFirePos = 400;
    [SerializeField] private Vector3 escapePos;
    
    [Header("Bezier Curve")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    private List<Vector3> _bombingPositions;
    private List<Vector3> _escapePositions;
    private int _counter;
    private const float DistanceToTarget = 1;

    [SerializeField] private float speed;

    // Behaviors
    [SerializeField] private bool bombing;
    [SerializeField] private bool escaping;

    void Awake()
    {
        bombing = true;
        escaping = false;
        _counter = 0;
        // I AM NOT MAKING A STATE MACHINE! 

        _enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        startPos = transform.position;
        firePos = GetFirePosition(startPos, subPos);
        escapePos = _enemySpawner.RandomDonutPosition();


        // curve maxxing
        _escapePositions = new List<Vector3>(100);
        _bombingPositions = new List<Vector3>(100);

        for (var i = 0; i < 100; i++)
        {
            var newEscapePosition = CubicCurve(firePos, firePos + offset, firePos + offset, escapePos, (float)i/100);
            _escapePositions.Add(newEscapePosition);

            var newBombingPosition = CubicCurve(startPos, startPos + offset, startPos + offset, firePos, (float)i/100);
            _bombingPositions.Add(newBombingPosition);
        }
    }

    public Vector3 GetFirePosition(Vector3 startPos, Vector3 subPos)
    {
        Vector3 differance = startPos - subPos;
        Vector3 point = differance * Random.Range(minFirePos / 1000, maxFirePos / 1000);
        return point;
    }

    void Update()  
    {
        if (bombing) {BombManeuver();}
        if (escaping) {EscapeManeuver();}
    }

    void BombManeuver()
    {
        // Straight Line Boring Movement
        // transform.position = Vector3.MoveTowards(transform.position, firePos, speed);

        // Jet has reached fire position, fire missile
        // if (Vector3.Distance(transform.position, firePos) < 0.1)
        // {
        //     FireMissle();
        //     bombing = false;
        //     escaping = true;
        //     _counter = 0;
        // }

        if (_counter < _bombingPositions.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, _bombingPositions[_counter], 200 * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _bombingPositions[_counter]) < DistanceToTarget) _counter ++;
        }
        else
        {
            // should be in firePos, swap to escape maneuver
            FireMissle();
            bombing = false;
            escaping = true;
            _counter = 0;
        }

    }

    void EscapeManeuver()
    {
        // Straight Line Boring Movement
        //transform.position = Vector3.MoveTowards(transform.position, escapePos, speed);

        if (_counter < _escapePositions.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, _escapePositions[_counter], 200 * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _escapePositions[_counter]) < DistanceToTarget) _counter ++;
        }
        else
        {
            // Jet Escaped, destroy
            Debug.Log("Jet Escaped");
            Destroy(gameObject);
        }

        // if (Vector3.Distance(transform.position, escapePos) < 1)
        // {
        //     // Jet Escaped, destroy
        //     Debug.Log("Jet Escaped");
        //     Destroy(gameObject);
        // }
    }

    private Vector3 CubicCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        // curbve
        return (((-start + 3 * (control1 - control2) + end) * t + (3 * (start + control2) - 6 * control1)) * t + 3 * (control1 - start)) * t + start;
    }

    void FireMissle()
    {
        // Fires a missile towards the submarine
        Debug.Log($"Incoming Missle!");
    }

    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, subPos);
        Gizmos.DrawSphere(firePos, 25f);
        
        for (var i = 0; i < 100; i++)
        {
            Gizmos.color = Color.yellow;
            var newPosition = CubicCurve(startPos, startPos + offset, startPos + offset, firePos, (float)i / 100);
            Gizmos.DrawSphere(newPosition, 5f);
        }
        for (var i = 0; i < 100; i++)
        {
            Gizmos.color = Color.green;
            var newPosition = CubicCurve(firePos, firePos + offset, firePos + offset, escapePos, (float)i / 100);
            Gizmos.DrawSphere(newPosition, 5f);
        }
    }
}
