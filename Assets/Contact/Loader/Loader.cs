using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public static Loader _Instance;

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


    private void Awake()
    {
        _Instance = this;

        Gunner.OnFired += Fired;

        _missileBays = new List<MissileBay>();
        for (int missileBayIndex = 0; missileBayIndex < _numMissileBays; missileBayIndex++)
        {
            _missileBays.Add(new MissileBay());
        }

        _activeMissileBay = _missileBays[0];
    }

    private void Update()
    {
        foreach (MissileBay missileBay in _missileBays)
        {
            if (missileBay.isLoading)
            {
                missileBay.Load(Time.deltaTime);
            }
        }
    }

    public static bool CanFire()
    {
        return _Instance._activeMissileBay.CanFire();
    }

    private void Fired(IRadarDetectable radarDetectable)
    {
        RemoveMissile();
    }   

    private void RemoveMissile()
    {
        _activeMissileBay.RemoveMissile();
    }

    public void SwitchBays()
    {
        if (_activeMissileBay == _missileBays[0])
        {
            _activeMissileBay = _missileBays[1];
        }
        else
        {
            _activeMissileBay = _missileBays[0];
        }
    }

    public void StartLoad()
    {
        _activeMissileBay.StartLoading();
    }

}


