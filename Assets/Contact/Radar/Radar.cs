using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private static Radar _Instance;

    
    public static event Action<RadarContact> OnRadarContactOccured;


    private const float MIMIMUM_ROTATION_INPUT_TIME = 0.5f;
    private const float MINIMUM_WIDTH = 20;
    private const float MAXIMUM_WIDTH = 180;

    public const float MAXIMUM_DISTANCE = 10000;
    public const float MINIMUM_DISTANCE = 3000;

    private const float CHANCE_OF_DETECTION_AT_MAX_DISTANCE = 0.4f;
    private const float CHANCE_OF_DETECTION_AT_MIN_DISTANCE = 1f;


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
        if (GameManager.IsGameRunning == false)
        {
            return;
        }

        RotateRadar();
    
        UpdateWidth();

        // Only do the following when not rotating:
        if (_isRotating == false)
        {
            UpdateSweepAngle();

            FindAllDetectables();

            AttemptSweepCheck();

            // Debug;
            // MakeContactWithEverything();
        }

    }

    private void MakeContactWithEverything()
    {
        foreach (IRadarDetectable detectable in _allDetectables)
        {
            OnRadarContactOccured?.Invoke(new RadarContact(detectable.GetPosition(), 1, detectable));
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
        if (!_isRotating && rotationInputThisFrame != 0)
        {
            // Start rotating
            _isRotating = true;

           if (rotationInputThisFrame == -1)
           {
                _rotationInput = -1;
           }
           else if (rotationInputThisFrame == 1)
           {
                _rotationInput = 1;
           }

            // AkSoundEngine.PostEvent("play_radar_rotate_start", gameObject);
            AkUnitySoundEngine.PostEvent("play_radar_rotate_start", gameObject);

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

                // AkSoundEngine.PostEvent("play_radar_rotate_end", gameObject);
                AkUnitySoundEngine.PostEvent("play_radar_rotate_end", gameObject);
            }
        }


    }
    #endregion


    #region Update Width

    private void UpdateWidth()
    {
        int widthInput = 0;

        if (Input.GetKey(KeyCode.W))
        {
            widthInput = -1;
        }
        else if (Input.GetKey(KeyCode.S))
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
            
            angle = 360 - angle;

            int bearing = Mathf.Clamp(Mathf.FloorToInt(angle), 0, 359);


            _bearings[bearing].AddDetectable(detectable);
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
        Bearing currentBearing = _bearings[bearing];

        foreach (IRadarDetectable detectable in currentBearing.GetDetectables())
        {

            float distance = detectable.GetPosition().magnitude;

            float distanceClamped = Mathf.Clamp(distance, MINIMUM_DISTANCE, MAXIMUM_DISTANCE);

            float distanceLerp = 1 - ((distanceClamped - MINIMUM_DISTANCE) / (MAXIMUM_DISTANCE - MINIMUM_DISTANCE));
            // Debug.Log($"Distance lerp: {distanceLerp}");

            float chanceOfDetection = Mathf.Lerp(distanceLerp, CHANCE_OF_DETECTION_AT_MIN_DISTANCE,
                                                               CHANCE_OF_DETECTION_AT_MAX_DISTANCE);

            // Debug.Log($"Chance of detection: {chanceOfDetection}");

            float randomRoll = UnityEngine.Random.Range(0f, 1f);

            if (randomRoll <= chanceOfDetection)
            {
                // Debug.Log("BINGO!");
                OnRadarContactOccured?.Invoke(new RadarContact(detectable.GetPosition(), 1f, detectable));
            }
        }         
    }

    #endregion 

    private class Bearing
    {
        private int _degree;
        private List<IRadarDetectable> _detectables;

        public Bearing(int degree)
        {
            _degree = 360 - degree;
            // Debug.Log($"Setup bearing {degree} on {_degree}");
            _detectables = new List<IRadarDetectable>();
        }

        public void Reset()
        {
            _detectables = new List<IRadarDetectable>();
        }

        public void AddDetectable(IRadarDetectable detectable)
        {

            _detectables.Add(detectable);
            detectable.SetBearing(_degree);
        }

        public List<IRadarDetectable> GetDetectables()
        {
            return _detectables;
        }
    }




}
