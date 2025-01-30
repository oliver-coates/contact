using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    private static Gunner _Instance;

    public static event Action OnReadyToFire;
    public static event Action<RadarDetectable> OnFired;
    public static event Action OnFailedToFire;

    [Header("Settings:")]
    [SerializeField] private int _requiredHitsForLock = 4;
    [SerializeField] private float _timeToLoseLock = 3;

    [Header("State")]
    [SerializeField] private RadarDetectable _currentTrackedDetectable;
    public static RadarDetectable CurrentTrackedDetectable
    {
        get
        {
            return _Instance._currentTrackedDetectable;
        }
    }

    [SerializeField] private int _hits;


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
                AkUnitySoundEngine.SetRTPCValue("amount_of_lock", 3);
                _fireTimer += Time.deltaTime;

                if (_fireTimer > _timeToFire)
                {
                    AttemptFire();
                }
            }
            else
            {
                AkUnitySoundEngine.SetRTPCValue("amount_of_lock", 2);
            }
        
        }
        else
        {
            // Not attempting lock:
            _loseLockTimer = 0;

            if (_hits > 0)
            {
                _hits = 0;
                _currentTrackedDetectable = null;
            }
        
            if (Radar.IsRotating)
            {
                // AkUnitySoundEngine.SetR  TPCValue("amount_of_lock", 0);
            }
            else
            {
                AkUnitySoundEngine.SetRTPCValue("amount_of_lock", 1);
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

        if (contact == null)
        {
            return;
        }

        if (_currentTrackedDetectable != null)
        {
            if (contact.detectable == _currentTrackedDetectable)
            {
                SimilarContactMade();
            }
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
    }

    private void NewContactMade(RadarDetectable detectable)
    {
        _currentTrackedDetectable = detectable;
        _loseLockTimer = 0f;
        _hits = 1;
    }

    private void LoseLock()
    {
        _hits = 0;
        _currentTrackedDetectable = null;


        if (_readyToFire)
        {
            FireFailed();
        }
    }

    private void SetReadyToFire()
    {
        _readyToFire = true;

        _fireTimer = 0;
        _timeToFire = UnityEngine.Random.Range(1.5f, 3f);

        OnReadyToFire?.Invoke();
    }

    private void AttemptFire()
    {
        if (Loader.CanFire())
        {
            Fire(_currentTrackedDetectable);
        }
        else
        {
            Debug.Log($"No missile to fire");
        }

        _fireTimer = 0f;
        _readyToFire = false;
        _hits = 0;
        _currentTrackedDetectable = null;
    }

    private void Fire(RadarDetectable target)
    {
        AkUnitySoundEngine.PostEvent("Play_missile_launch", gameObject);

        GameObject firedMissileObject = Instantiate(friendlyMissile, new Vector3(0, 100, 0), Quaternion.identity);
        
        FriendlyMissile firedMissile = firedMissileObject.GetComponent<FriendlyMissile>();

        firedMissile.Initialise(target);

        OnFired?.Invoke(_currentTrackedDetectable);
    }

    private void FireFailed()
    {
        _readyToFire = false;

        OnFailedToFire?.Invoke();
    }

    
}
