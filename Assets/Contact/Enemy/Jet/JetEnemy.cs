using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sys = System;

public class JetEnemy : RadarDetectable
{
    public static event Sys.Action OnShotDown;
    public static event Sys.Action<EnemyMissle> OnFiredMissile;


    [Header("Object References")]
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private GameObject missile;

    [Header("Position References")]
    [SerializeField] private Vector3 subPos = Vector3.zero;

    //Spawn Position determined by the spawn manager
    [SerializeField] private Vector3 startPos;
    
    // Random point on a line between startPos and subPos
    // Excludes any area larger than maxFirePos and smaller than minFirePos
    [SerializeField] private Vector3 firePos;
    
    // Neither of these values should be higher than 1000
    // I lied they can be larger if you want them too :3
    [SerializeField] private float minFirePos;
    [SerializeField] private float maxFirePos;
    [SerializeField] private Vector3 escapePos;
    
    [Header("Bezier Curve")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    private List<Vector3> _bombingPositions;
    private List<Vector3> _escapePositions;
    private int _counter;
    private const float DistanceToTarget = 1;

    [SerializeField] private float speed;

    [Header("Behaviors")]
    [SerializeField] private bool bombing;
    [SerializeField] private bool escaping;
    [SerializeField] private bool detectedFar;
    [SerializeField] private bool detectedNear;

    [Header("Detection Distance")]
    [SerializeField] private float farDetectionRadius;
    [SerializeField] private float nearDetectionRadius;
    public enum DetectionDistance {Near, Far};
    
    protected override void Start() 
    {
        base.Start();

        bombing = true;
        escaping = false;
        _counter = 0;

        _enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        _waveManager = GameObject.Find("EnemySpawner").GetComponent<WaveManager>();
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

    protected override void Shotdown()
    {
        Debug.Log($"ENEMY JET SHOT DOWN");
        OnShotDown?.Invoke();
    }

    void Update()  
    {
        if (bombing) 
        {
            BombManeuver();
        }
        
        if (escaping) 
        {
            EscapeManeuver();
        }
        
        if ((Vector3.Distance(transform.position, subPos) < farDetectionRadius) && (!detectedFar))
        {
            detectedFar = true;
            Captain.GiveDetectionNotification(this, bearing, DetectionDistance.Far);
        }
        
        if ((Vector3.Distance(transform.position, subPos) < nearDetectionRadius) && (!detectedNear))
        {
            detectedNear = true;
            Captain.GiveDetectionNotification(this, bearing, DetectionDistance.Near);
        }
    }


    #region Maneuvers

    void BombManeuver()
    {
        if (_counter < _bombingPositions.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, _bombingPositions[_counter],  speed * Time.deltaTime);
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
        if (_counter < _escapePositions.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, _escapePositions[_counter],  speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _escapePositions[_counter]) < DistanceToTarget) _counter ++;
        }
        else
        {
            // Jet Escaped, destroy
            Destroy(gameObject);
        }
    }

    private Vector3 CubicCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        // curbve
        return (((-start + 3 * (control1 - control2) + end) * t + (3 * (start + control2) - 6 * control1)) * t + 3 * (control1 - start)) * t + start;
    }

    
    public Vector3 GetFirePosition(Vector3 startPos, Vector3 subPos)
    {
        Vector3 differance = startPos - subPos;
        Vector3 point = differance * Random.Range(minFirePos / 1000, maxFirePos / 1000);
        return point;
    }


    #endregion

    void FireMissle()
    {
        // Fires a missile towards the submarine
        EnemyMissle firedMissile = Instantiate(missile, transform.position, Quaternion.identity).GetComponent<EnemyMissle>();
        OnFiredMissile?.Invoke(firedMissile);
    }


    #region Gizmos:
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

 
    #endregion
}
