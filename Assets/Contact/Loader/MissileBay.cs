using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MissileBay
{
    public const int NUM_MISSILES = 4;


    private GameObject _gameObject;
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

    public MissileBay(GameObject gameObject)
    {
        _gameObject = gameObject;
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
        GetNewLoadTime(true);

        AkUnitySoundEngine.PostEvent("Play_missile_bay_lower", _gameObject);

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
                GetNewLoadTime(false);
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

        AkUnitySoundEngine.PostEvent("Play_missile_bay_raise", _gameObject);

        OnFinishedLoading?.Invoke();
    }

    private void GetNewLoadTime(bool isFirstLoad)
    {
        float random = UnityEngine.Random.Range(2, 5);

        if (isFirstLoad)
        {
            _loadTime = 4 + random;

        }
        else
        {
            _loadTime = random;
        }
    }

    public void SetIsSelected(bool isSelected)
    {
        _isSelected = isSelected;

        OnSelectionStateChanged?.Invoke(isSelected);
    }
}
