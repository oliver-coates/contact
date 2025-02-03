using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuffDuff : MonoBehaviour
{
    public static event Action<RadarDetectable> OnContactMade;

    public const float DETECTION_DISTANCE = 3500;

    private static HuffDuff _Instance;

    [Header("Settings:")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _timeToPowerUp;

    [Header("State:")]
    [SerializeField] private bool _poweringUp;
    [SerializeField] private bool _powered;
    public static bool PoweredOrPoweringUp
    {
        get
        {
            return _Instance._powered || _Instance._poweringUp;
        }
    }
    [SerializeField] private float _powerUpTimer;
    
    public static float PowerLerp
    {
        get
        {
            return (_Instance._powerUpTimer / _Instance._timeToPowerUp);
        }
    }

    [SerializeField, Range(0, 360)] private float _rotation;
    public static float Rotation
    {
        get
        {
            return _Instance._rotation;
        }
    }

    private int _lastCheckedBearing = 0;

    private void Awake()
    {
        _Instance = this;
    }

    public static void TogglePowered()
    {
        if (_Instance._poweringUp || _Instance._powered)
        {
            _Instance._powered = false;
            _Instance._poweringUp = false;
        }
        else
        {
            _Instance._poweringUp = true;
            _Instance._powerUpTimer = 0;
        }
    }

    private void Update()
    {
        if (_poweringUp)
        {
            _powerUpTimer += Time.deltaTime;
        }
        else if (_powered == false)
        {
            _powerUpTimer -= Time.deltaTime;
        }
        _powerUpTimer = Mathf.Clamp(_powerUpTimer, 0f, _timeToPowerUp);

        if (_powerUpTimer == _timeToPowerUp)
        {
            _powered = true;
            _poweringUp = false;
        }

        if (!_powered)
        {
            float powerLerp = Mathf.Clamp(PowerLerp - 0.5f, 0, 0.5f) * 2f;
            _rotation += Time.deltaTime * _rotationSpeed * powerLerp;
        }

        if (_powered)
        {
            _rotation += Time.deltaTime * _rotationSpeed;
        }

        if (_rotation < 0)
        {
            _rotation += 360;
        }
        else if (_rotation >= 360)
        {
            _rotation -= 360;
        }

        if (_poweringUp || _powered)
        {
            int bearing = Mathf.FloorToInt(_rotation);

            if (bearing != _lastCheckedBearing)
            {
                CheckForContacts(bearing);
                _lastCheckedBearing = bearing;
            }
        }
    }

    private void CheckForContacts(int bearing)
    {
        List<RadarDetectable> detectables = Radar.GetContactsOnBearing(bearing);

        foreach (RadarDetectable radarDetectable in detectables)
        {
            if (radarDetectable.GetPosition().magnitude <= DETECTION_DISTANCE)
            {
                Debug.Log($"HF/ DF CONTACT: {bearing}");

                OnContactMade?.Invoke(radarDetectable);
            }
        }
    }
}

