using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    private static RadarManager _Instance;

    public static event Action<RadarContact> OnRadarContactOccured;

    private List<IRadarDetectable> _allDetectables;

    [Header("Settings:")]
    [SerializeField] private float _rotationSpeed;


    [Header("State:")]
    
    // The rotation of the radar dish
    [Range(0, 360)] [SerializeField] private float _rotation;
    private bool _isRotating;
    private float _rotationTime;
    private int _rotationInput;

    private const float MIMIMUM_ROTATION_INPUT_TIME = 2f;


    [Range(0, 280)] [SerializeField] private float _width;
    [Range(0, 100)] [SerializeField] private float _length;




    private void Awake()
    {
        _Instance = this;
        _allDetectables = new List<IRadarDetectable>();
    }


    #region Adding/Removing detectables
    public static void RegisterRadarDetectable(IRadarDetectable detectable)
    {
        _Instance._allDetectables.Add(detectable);
    }

    public static void DeregisterRadarDetectable(IRadarDetectable detectable)
    {
        _Instance._allDetectables.Remove(detectable);
    }
    #endregion



    public void Update()
    {
        RotateRadar();
    }


    private void RotateRadar()
    {
        bool isInputtingThisFrame = false;
        if (!_isRotating)
        {
            if (Input.GetKey(KeyCode.Q))
            {  
                _rotationInput = -1;

                _isRotating = true;
                isInputtingThisFrame = true;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _rotationInput = 1;
                
                _isRotating = true;
                isInputtingThisFrame = true;
            }
        }

        // If we are rotating, rotate the radar
        if (_isRotating)
        {
            // Grab input:
            _rotation += _rotationInput * Time.deltaTime * _rotationSpeed;

            if (_rotation > 360)
            {
                _rotation -= 360f;
            }
            else if (_rotation < 0)
            {
                _rotation += 360f;
            }

            _rotationTime += Time.deltaTime;
        }

        // If we are not rotating,
        // only stop rotating once we are past the minimum rotation input time
        if (isInputtingThisFrame)
        {
            if (_rotationTime > MIMIMUM_ROTATION_INPUT_TIME)
            {
                _isRotating = false;
                _rotationTime = 0f;
                _rotationInput = 0;

            }
        }


    }



}
