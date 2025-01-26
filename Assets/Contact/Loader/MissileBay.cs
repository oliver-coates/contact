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
    public event Action<bool> OnSelectionStateChanged;

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

    [SerializeField] private bool _isSelected;
    public bool isSelected
    {
        get
        {
            return _isSelected;
        }
    }

    private float _loadTime;
    private float _loadTimer;

    public MissileBay()
    {
        _missiles = NUM_MISSILES;
        _isLoading = false;
        _isSelected = false;
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
        return (_missiles < NUM_MISSILES) && _isSelected == false;
    }

    public void StartLoading()
    {
        _isLoading = true;
        GetNewLoadTime();

        OnStartedLoad?.Invoke();
    }

    public void Load(float deltaTime)
    {
        if (_isLoading)
        {
            _loadTimer += deltaTime;

            if (_loadTimer > _loadTime)
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

    public void SetIsSelected(bool isSelected)
    {
        _isSelected = isSelected;

        OnSelectionStateChanged?.Invoke(isSelected);
    }
}
