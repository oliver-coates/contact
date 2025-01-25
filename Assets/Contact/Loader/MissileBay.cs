using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MissileBay
{
    public const int NUM_MISSILES = 4;

    public event Action OnStartedLoad;
    public event Action OnFinishedLoading;
    public event Action<int> OnMissileCountChanged;

    [SerializeField] private int _missiles;
    public int missiles
    {
        get
        {
            return _missiles;
        }	
    }

    [SerializeField] private bool _isLoading;
    public bool isLoading
    {
        get
        {
            return _isLoading;
        }	
    }

    private float _loadTime;
    private float _loadTimer;

    public MissileBay()
    {
        _missiles = NUM_MISSILES;
    }

    public void RemoveMissile()
    {
        _missiles -= 1;

        OnMissileCountChanged?.Invoke(_missiles);
    }

    private void AddMissile()
    {
        _missiles += 1;

        OnMissileCountChanged?.Invoke(_missiles);
    }

    public bool CanFire()
    {
        return (_missiles > 0);
    }

    public bool CanLoad()
    {
        return (_missiles < NUM_MISSILES);
    }

    public void StartLoading()
    {
        _isLoading = true;
        GetNewLoadTime();

        OnStartedLoad?.Invoke();
    }

    public void Load(float deltaTime)
    {
        _loadTimer += deltaTime;

        if (_loadTime > _loadTimer)
        {
            AddMissile();
            GetNewLoadTime();
            _loadTimer = 0f;
        
            if (_missiles == NUM_MISSILES)
            {
                FinishedLoading();
            }
        }
    }

    private void FinishedLoading()
    {
        _isLoading = false;

        OnFinishedLoading?.Invoke();
    }

    private void GetNewLoadTime()
    {
        _loadTime = UnityEngine.Random.Range(2, 5);
    }
}
