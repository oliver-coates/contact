using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIWS : MonoBehaviour
{
    public static CIWS _Instance;


    [Header("Settings:")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _timeToPowerUp;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private float _loadTime;
    [SerializeField] private float _fireRate = 250;
    [SerializeField] private float _getTargetTime;
    [Range(0, 1)] [SerializeField] private float _chanceToDestoryTargetPerSecondWhenFiring;


    [Header("State:")]
    [SerializeField] private RadarDetectable _currentContact;
    [SerializeField] private float _getTargetTimer;
    public static float GetTargetTimeLerp
    {
        get
        {
            return _Instance._getTargetTimer / _Instance._getTargetTime;
        }
    }    
    
    [SerializeField] private int _ammo;
    public static int Ammo
    {
        get
        {
            return _Instance._ammo;
        }
    }

    [SerializeField] private bool _isLoading;

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

    [SerializeField] private float _loadTimer;
    public static float LoadLerp
    {
        get
        {
            return (_Instance._loadTimer / _Instance._loadTime);
        }
    }

    [SerializeField] private float _shootTimer;
    

    private void Awake()
    {
        HuffDuff.OnContactMade += ContactMade;
    
        _ammo = _maxAmmo;
        _Instance = this;

        _loadTimer = _loadTime;
    }

    private void OnDestroy()
    {
        HuffDuff.OnContactMade -= ContactMade;
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

    public static void StartLoad()
    {
        _Instance._isLoading = true;
        _Instance._loadTimer = 0;
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

        if (_isLoading)
        {
            _loadTimer += Time.deltaTime;
        }

        if (_loadTimer > _loadTime)
        {
            _isLoading = false;
            _loadTimer = _loadTime;
            _ammo = _maxAmmo;
        }

        if (_currentContact != null)
        {
            _getTargetTimer += Time.deltaTime;
            _getTargetTimer = Mathf.Clamp(_getTargetTimer, 0, _getTargetTime);

            _rotation = Mathf.MoveTowards(_rotation, _currentContact.bearing, _rotationSpeed * Time.deltaTime);
    
            if (_getTargetTimer == _getTargetTime && _ammo > 0)
            {
                Shoot();
            }
        }
    }

    private void ContactMade(RadarDetectable radarDetectable)
    {
        if (_currentContact == null)
        {
            TargetContact(radarDetectable);
        }
    }

    private void TargetContact(RadarDetectable radarDetectable)
    {
        Debug.Log($"Setting contact!");
        _shootTimer = 0;
        _getTargetTimer = 0;
        _currentContact = radarDetectable;
    }

    private void Shoot()
    {
        _ammo -= (int) (Time.deltaTime * _fireRate);
        _ammo = Mathf.Clamp(_ammo, 0, _maxAmmo);

        _shootTimer += Time.deltaTime;

        if (_shootTimer > 1)
        {
            RollToDestroyIncoming();
            _shootTimer = 0;
        }

        CameraControl.ShakeCamera(0.1f);
    }

    private void RollToDestroyIncoming()
    {
        float randomRoll = UnityEngine.Random.Range(0f, 1f);

        if (randomRoll <= _chanceToDestoryTargetPerSecondWhenFiring)
        {
            _currentContact.DestroyThis(RadarDetectable.DestructionReason.HitByCiws);
        }
    }


}
