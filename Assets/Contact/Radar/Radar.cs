using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private static Radar _Instance;

    
    public static event Action<RadarContact> OnRadarContactOccured;


    private const float MIMIMUM_ROTATION_INPUT_TIME = 0.75f;
    private const float MINIMUM_WIDTH = 20;
    private const float MAXIMUM_WIDTH = 180;



    private List<IRadarDetectable> _allDetectables;
    private Bearing[] _bearings;
    

    [Header("Settings:")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _widthSpeed;
    [SerializeField] private float _sweepSpeed;


    [Header("State:")]
    
    // The rotation of the radar dish
    [Range(0, 360)] [SerializeField] private float _rotation;
    public static float Rotation
    {
        get
        {
            return _Instance._rotation;
        }
    }
    public static bool IsRotating
    {
        get
        {
            return _Instance._isRotating;
        }
    }


    [Range(MINIMUM_WIDTH, MAXIMUM_WIDTH)] [SerializeField] private float _width;
    public static float Width
    {
        get
        {
            return _Instance._width;
        }
    }
    
    [Range(0, 100)] [SerializeField] private float _length;
    
    [SerializeField] private float _sweepAngle;
    public static float SweepAngle
    {
        get
        {
            return _Instance._sweepAngle;
        }	
    }

    private int _previousSweepBearing;

    [Header("Input:")]
    [SerializeField] private bool _isRotating;
    [SerializeField] private float _rotationTime;
    [SerializeField] private int _rotationInput;









    private void Awake()
    {
        _Instance = this;
        _allDetectables = new List<IRadarDetectable>();
    
        _width = Mathf.Lerp(MINIMUM_WIDTH, MAXIMUM_WIDTH, 0.75f);
        _rotation = 0f;
        _sweepAngle = _width/2f;
        _previousSweepBearing = -1;

        SetupBearings();
    }

    private void SetupBearings()
    {
        _bearings = new Bearing[360];
        for (int degree = 0; degree < 360; degree++)
        {
            _bearings[degree] = new Bearing(degree);
        }
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
    
        UpdateWidth();

        // Only do the following when not rotating:
        if (_isRotating == false)
        {
            UpdateSweepAngle();

            FindAllDetectables();

            AttemptSweepCheck();
        }
        
    }



    #region Rotation
    private void RotateRadar()
    {
        // Get the raw input this frame:
        int rotationInputThisFrame = 0;
        if (Input.GetKey(KeyCode.A))
        {  
            rotationInputThisFrame = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotationInputThisFrame = -1;
        }

        // If we are not rotating, and a button input is detected,
        // start rotating
        if (!_isRotating && rotationInputThisFrame != 0)
        {
            _isRotating = true;

           if (rotationInputThisFrame == -1)
           {
                _rotationInput = -1;
           }
           else if (rotationInputThisFrame == 1)
           {
                _rotationInput = 1;
           }
        }

        // If we are rotating, rotate the radar
        if (_isRotating)
        {
            if (rotationInputThisFrame != 0)
            {
                _rotationInput = rotationInputThisFrame;
            }

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
        if (rotationInputThisFrame == 0)
        {
            // Stop Rotating
            if (_rotationTime > MIMIMUM_ROTATION_INPUT_TIME)
            {
                _isRotating = false;
                _rotationTime = 0f;
                _rotationInput = 0;

            }
        }


    }
    #endregion

    #region Update Width

    private void UpdateWidth()
    {
        int widthInput = 0;

        if (Input.GetKey(KeyCode.Q))
        {
            widthInput = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            widthInput = 1;
        }
        
        _width += widthInput * Time.deltaTime * _widthSpeed;

        _width = Mathf.Clamp(_width, MINIMUM_WIDTH, MAXIMUM_WIDTH);


    }

    #endregion


    #region Calculate all detectables

    private void FindAllDetectables()
    {
        // Reset all bearings:
        foreach (Bearing bearing in _bearings)
        {
            bearing.Reset();
        }

        // Go through all detectables:
        foreach (IRadarDetectable detectable in _allDetectables)
        {
            Vector3 pos = detectable.GetPosition();

            float angle = Vector3.Angle(Vector3.forward, pos.normalized);

            if (pos.x < 0)
            {
                angle = 359.9f - angle;
            }

            int bearing = Mathf.FloorToInt(angle);

            _bearings[bearing].AddDetectable(detectable);
            // Debug.Log($"Added detectable under bearing {bearing} ({angle})");
        }
    }

    #endregion


    #region Sweep

    private void UpdateSweepAngle()
    {
        float angleMax = _width / 2f; 
        if (_sweepAngle > angleMax || _sweepAngle < -angleMax)
        {
            _sweepAngle = angleMax;
        }

        _sweepAngle -= Time.deltaTime * _sweepSpeed;
    }

    private void AttemptSweepCheck()
    {
        float absoluteAngle = _sweepAngle + _rotation;
        
        if (absoluteAngle < 0)
        {
            absoluteAngle += 360;
        }
        else if (absoluteAngle > 360)
        {
            absoluteAngle -= 360;
        }

        int currentBearing = Mathf.FloorToInt(absoluteAngle);

        if (currentBearing != _previousSweepBearing)
        {
            CheckForDetectablesOnBearing(currentBearing);
            _previousSweepBearing = currentBearing;
        }
    }

    private void CheckForDetectablesOnBearing(int bearing)
    {

         
    }

    #endregion 

    private class Bearing
    {
        private int _degree;
        private List<IRadarDetectable> _detectables;

        public Bearing(int degree)
        {
            _degree = degree;
            _detectables = new List<IRadarDetectable>();
        }

        public void Reset()
        {
            _detectables = new List<IRadarDetectable>();
        }

        public void AddDetectable(IRadarDetectable detectable)
        {
            _detectables.Add(detectable);
        }

        public List<IRadarDetectable> GetDetectables()
        {
            return _detectables;
        }
    }




}
