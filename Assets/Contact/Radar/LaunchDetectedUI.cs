using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchDetectedUI : MonoBehaviour
{
    private EnemyMissle _enemyMissile;
    private bool _trackingMissile;

    [Header("State:")]
    [SerializeField] private float _offBearing;
    [SerializeField] private float _trueBearing;

    [SerializeField] private float _offDistance;
    [SerializeField] private float _trueDistance;

    [SerializeField] private float _bearingTimer;
    [SerializeField] private float _timeToGetBearing;

    [SerializeField] private float _distanceTimer;
    [SerializeField] private float _timeToGetDistance;

    [Header("UI References:")]
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _bearingText;
    [SerializeField] private Animator _animator;


    #region Initialisation & Destruction
    
    private void Awake()
    {
        JetEnemy.OnFiredMissile += MissileFired;
    }
    
    private void OnDestroy()
    {
        JetEnemy.OnFiredMissile -= MissileFired;
    }
    
    #endregion

    private void MissileFired(EnemyMissle missile)
    {
        _enemyMissile = missile;
        _trackingMissile = true;
        _animator.SetTrigger("OnLaunchDetected");

        _timeToGetBearing = Random.Range(1, 4f);
        _timeToGetDistance = Random.Range(1, 4f);
    
        _bearingTimer  = 0;
        _distanceTimer = 0;

        _trueDistance = missile.GetPosition().magnitude;

        _offDistance = _trueDistance * Random.Range(0.8f, 1.2f);
    
        Invoke("GetBearingDelayed", 0.25f);
    }

    private void GetBearingDelayed()
    {
        if (_trackingMissile)
        {
            _trueBearing = _enemyMissile.bearing;
            _offBearing = _trueBearing * Random.Range(0.8f, 1.2f);
            _offBearing = Mathf.Clamp(_offBearing, 0, 360);

        }

    }



    private void Update()
    {
        if (_trackingMissile)
        {
            if (_enemyMissile == null)
            {
                
                _distanceText.text = $"-----";
                _bearingText.text = $"---";

                _trackingMissile = false;
                return;
            }

            _bearingTimer += Time.deltaTime;
            _distanceTimer += Time.deltaTime;

            float bearingLerp = _bearingTimer / _timeToGetBearing;
            float distanceLerp = _distanceTimer / _timeToGetDistance;
        
            float bearing = Mathf.Lerp(_offBearing, _trueBearing, bearingLerp);
            float distance = Mathf.Lerp(_offDistance, _trueDistance, distanceLerp);

            _distanceText.text = $"{distance:00000}";
            _bearingText.text = $"{bearing:000}";
        }

        
    }


}
