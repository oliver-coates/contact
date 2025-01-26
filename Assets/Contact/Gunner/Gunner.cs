using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    private static Gunner _Instance;

    public static event Action OnReadyToFire;
    public static event Action<IRadarDetectable> OnFired;
    public static event Action OnFailedToFire;

    [Header("Settings:")]
    [SerializeField] private int _requiredHitsForLock = 4;
    [SerializeField] private float _timeToLoseLock = 3;

    [Header("State")]
    [SerializeField] private int _hits;
    private IRadarDetectable _currentTrackedDetectable;
    public static IRadarDetectable CurrentTrackedDetectable
    {
        get
        {
            return _Instance._currentTrackedDetectable;
        }
    }

    [SerializeField] private bool _attemptingLock;
    public static bool AttemptingLock
    {
        get
        {
            return _Instance._attemptingLock;
        }	
    }


    [SerializeField] private bool _readyToFire;
    public static bool ReadyToFire
    {
        get
        {
            return _Instance._readyToFire;
        }
    }
    [SerializeField] private float _timeToFire;
    private float _loseLockTimer = 0;
    private float _fireTimer = 0;

    [SerializeField] private GameObject friendlyMissile;
    
    private void Awake()
    {
        _Instance = this;
    
        Radar.OnRadarContactOccured += OnContact;
    }

    private void OnDestroy()
    {
        Radar.OnRadarContactOccured -= OnContact;
    }


    private void Update()
    {
        if (GameManager.IsGameRunning == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _attemptingLock = true;
        }
        else
        {
            _attemptingLock = false;
        }

        if (_attemptingLock)
        {
            if (_currentTrackedDetectable != null)
            {
                _loseLockTimer += Time.deltaTime;
                
                if (_loseLockTimer > _timeToLoseLock)
                {
                    LoseLock();
                    _loseLockTimer = 0;
                }
            }

            if (_readyToFire)
            {
                AkUnitySoundEngine.SetRTPCValue("rtpc_radar_warble_intensity", 3);
                _fireTimer += Time.deltaTime;

                if (_fireTimer > _timeToFire)
                {
                    AttemptFire();
                }
            }
            else
            {
                AkUnitySoundEngine.SetRTPCValue("rtpc_radar_warble_intensity", 2);
            }
        
        }
        else
        {
            _loseLockTimer = 0;

            if (_hits > 0)
            {
                LoseLock();
            }
        
            if (Radar.IsRotating)
            {
                AkUnitySoundEngine.SetRTPCValue("rtpc_radar_warble_intensity", 0);
            }
            else
            {
                AkUnitySoundEngine.SetRTPCValue("rtpc_radar_warble_intensity", 1);
            }
        }
        

        
    }

    private void OnContact(RadarContact contact)
    {
        if (_readyToFire == true)
        {
            return;
        }

        if (_attemptingLock == false)
        {
            return;
        }


        if (contact.detectable == _currentTrackedDetectable)
        {
            SimilarContactMade();
        }
        else
        {
            NewContactMade(contact.detectable);
        }
    }

    private void SimilarContactMade()
    {
        _hits += 1;
        _loseLockTimer = 0f;

        if (_hits == _requiredHitsForLock)
        {
            SetReadyToFire();
        }

        Debug.Log($"Repeat hit - {_currentTrackedDetectable.GetHashCode()}");
    }

    private void NewContactMade(IRadarDetectable detectable)
    {
        _currentTrackedDetectable = detectable;
        _loseLockTimer = 0f;
        _hits = 1;

        Debug.Log($"New hit - {_currentTrackedDetectable.GetHashCode()}");
    }

    private void LoseLock()
    {
        _hits = 0;
        _currentTrackedDetectable = null;

        Debug.Log($"Lost Lock");

        if (_readyToFire)
        {
            FireFailed();
        }
    }

    private void SetReadyToFire()
    {
        _readyToFire = true;

        _fireTimer = 0;
        _timeToFire = UnityEngine.Random.Range(1.5f, 4f);

        Debug.Log($"Ready to fire");
        OnReadyToFire?.Invoke();
    }

    private void AttemptFire()
    {
        _fireTimer = 0f;
        _readyToFire = false;
        _hits = 0;

        // TODO: Chjeck for ammo:
        if (Loader.CanFire())
        {
            Debug.Log($"Fired");
            OnFired?.Invoke(_currentTrackedDetectable);
            Fire(_currentTrackedDetectable);
        }
        else
        {
            Debug.Log($"No missile to fire");
        }

        
    }

    private void Fire(IRadarDetectable target)
    {
        GameObject firedMissile = Instantiate(friendlyMissile, new Vector3(0, 100, 0), Quaternion.identity);
        FriendlyMissile missileTarget = firedMissile.GetComponent<FriendlyMissile>();
        missileTarget.SetTarget(target);
    }

    private void FireFailed()
    {
        _readyToFire = false;
        Debug.Log($"Failed to fire");

        OnFailedToFire?.Invoke();
    }

    
}
