using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private static Loader _Instance;

    [Header("Settings:")]
    [SerializeField] private int _numMissileBays = 2;

    [Header("State:")]
    [SerializeField] private List<MissileBay> _missileBays;
    public static List<MissileBay> MissileBays
    {
        get
        {
            return _Instance._missileBays;
        }
    }

    private MissileBay _activeMissileBay;

    [SerializeField] private bool _isSwitching = false;
    public static bool IsSwitching
    {
        get
        {
            return _Instance._isSwitching;
        }
    }
    private MissileBay _bayCurrentlySwitchingTo;
    [SerializeField] private float _switchTimer = 0f;
    [SerializeField] private float _switchTime;

    public static bool IsLoading
    {
        get
        {
            if (_Instance._activeMissileBay == null)
            {
                return false;
            }
            else
            {
                return _Instance._activeMissileBay.isLoading;
            }
        }
    }

    public static bool NeedsLoad
    {
        get
        {
            if (_Instance._activeMissileBay == null)
            {
                return false;
            }
            else
            {
                return _Instance._activeMissileBay.missiles == 0;
            }
        }
    }

    private void Awake()
    {
        _Instance = this;

        Gunner.OnFired += Fired;

        _missileBays = new List<MissileBay>();
        for (int missileBayIndex = 0; missileBayIndex < _numMissileBays; missileBayIndex++)
        {
            _missileBays.Add(new MissileBay(gameObject));
        }    

        _activeMissileBay = _missileBays[0];
        _activeMissileBay.SetIsSelected(true);

    }

    private void Update()
    {
        if (GameManager.IsGameRunning == false)
        {
            return;
        }

        foreach (MissileBay missileBay in _missileBays)
        {
            if (missileBay.isLoading)
            {
                missileBay.Load(Time.deltaTime);
            }
        }

        if (_isSwitching)
        {
            _switchTimer += Time.deltaTime;

            if (_switchTimer > _switchTime)
            {
                SwitchBays();
            }
        }
    }

    public static bool CanFire()
    {
        if (_Instance._activeMissileBay == null)
        {
            return false;
        }
        else
        {
            return _Instance._activeMissileBay.CanFire();
        }
    }

    private void Fired(IRadarDetectable radarDetectable)
    {
        RemoveMissile();
    }   

    private void RemoveMissile()
    {
        _activeMissileBay.RemoveMissile();
    }

    public static void AttemptSwitchBays()
    {
        MissileBay currentBay = _Instance._activeMissileBay;

        MissileBay otherBay;
        if (currentBay == _Instance._missileBays[0])
        {
            otherBay = _Instance._missileBays[1];
        }
        else
        {
            otherBay = _Instance._missileBays[0];
        }

        if (_Instance._isSwitching)
        {
            return;
        }

        if (otherBay.isLoading)
        {
            return;
        }

        _Instance._switchTime = 7f;
        _Instance._switchTimer = 0f;
        _Instance._isSwitching = true;
        _Instance._bayCurrentlySwitchingTo = otherBay;

        currentBay.SetIsSelected(false);
        _Instance._activeMissileBay = null;
    }

    public static void SwitchBays()
    {
        _Instance._isSwitching = false;

        _Instance._activeMissileBay = _Instance._bayCurrentlySwitchingTo;
        _Instance._bayCurrentlySwitchingTo = null;

        _Instance._activeMissileBay.SetIsSelected(true);
    }

    public static void StartLoad(MissileBay bay)
    {
        if (bay.isLoading == false && bay.isSelected == false)
        {
            if (bay.CanLoad())
            {
                bay.StartLoading();
            }
        }
    }

}


