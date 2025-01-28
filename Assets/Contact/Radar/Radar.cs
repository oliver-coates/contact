using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private static Radar _Instance;

    public enum State
    {
        Long,
        Short
    }


    public static event Action<RadarContact> OnRadarContactOccured;


    // --- Settings:
    private const float MINIMUM_ROTATION_INPUT_TIME_LONG = 0.7f;
    private const float MINIMUM_ROTATION_INPUT_TIME_SHORT = 0.25f;
    

    private const float MINIMUM_WIDTH_LONG = 20;
    private const float MAXIMUM_WIDTH_LONG = 180;

    private const float MINIMUM_WIDTH_SHORT = 10;
    private const float MAXIMUM_WIDTH_SHORT = 60;

    private const float ROTATION_SPEED_LONG = 45;
    private const float ROTATION_SPEED_SHORT = 12;
    private const float WIDTH_CHANGE_SPEED_LONG = 0.4f;
    private const float WIDTH_CHANGE_SPEED_SHORT = 0.2f;
    
    private const float SWEEP_SPEED = 22;

    public const float MAXIMUM_DISTANCE = 14000;
    public const float MINIMUM_DISTANCE = 3000;

    private const float TIME_TO_CHANGE_DISH = 0.8f;

    private const float CHANCE_OF_DETECTION_AT_MAX_DISTANCE = 0.4f;
    private const float CHANCE_OF_DETECTION_AT_MIN_DISTANCE = 1f;


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
    [SerializeField] private State _dishState;
    public static State DishState
    {
        get
        {
            return _Instance._dishState;
        }	
    }

    private List<IRadarDetectable> _allDetectables;
    private Bearing[] _bearings;
    

    [Range(MINIMUM_WIDTH_SHORT, MAXIMUM_WIDTH_LONG)] 
    [SerializeField] private float _width;
    public static float Width
    {
        get
        {
            return _Instance._width;
        }
    }
    [Range(0,1)]
    [SerializeField] private float _widthInput;

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
    private float _dishChangeTimer;

    [Header("Input:")]
    [SerializeField] private bool _isRotating;
    [SerializeField] private float _rotationTime;
    [SerializeField] private int _rotationInput;
    [SerializeField] private bool _isChangingDishState;
    public static bool IsChangingDishState
    {
        get
        {
            return _Instance._isChangingDishState;
        }
    }









    private void Awake()
    {
        _Instance = this;
        _allDetectables = new List<IRadarDetectable>();
    
        _widthInput = 0.5f;
        _rotation = 0f;
        _sweepAngle = _width/2f;
        _previousSweepBearing = -1;

        GameManager.OnGameStart += GameStart;

        SetupBearings();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= GameStart;
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


    private void GameStart()
    {
        AkUnitySoundEngine.PostEvent("play_radar_rotate_end", gameObject);
    }

    public void Update()
    {   
        if (GameManager.IsGameRunning == false)
        {
            return;
        }

        if (_isChangingDishState == false)
        {
            RotateRadar();
        }

        if (_isChangingDishState)
        {
            UpdateDishState();
        }

        UpdateWidth();

        // Only do the following when not rotating:
        if (_isRotating == false && _isChangingDishState == false)
        {
            UpdateSweepAngle();

            FindAllDetectables();

            AttemptSweepCheck();

            // Debug;
            // MakeContactWithEverything();
        }

    }


    #region Rotation, Width & Dish State
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

            AkUnitySoundEngine.PostEvent("play_radar_rotate_start", gameObject);

        }

        // If we are rotating, rotate the radar
        if (_isRotating)
        {
            if (rotationInputThisFrame != 0)
            {
                _rotationInput = rotationInputThisFrame;
            }

            float speed = 0;
            if (_dishState == State.Long)
            {
                speed = ROTATION_SPEED_LONG;
            }
            else if (_dishState == State.Short)
            {
                speed = ROTATION_SPEED_SHORT;
            }

            // Grab input:
            _rotation += _rotationInput * Time.deltaTime * speed;

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
            float minRotationInputTime = 0;
            if (_dishState == State.Long)
            {
                minRotationInputTime = MINIMUM_ROTATION_INPUT_TIME_LONG;
            }
            else if (_dishState == State.Short)
            {
                minRotationInputTime = MINIMUM_ROTATION_INPUT_TIME_SHORT;
            }

            if (_rotationTime > minRotationInputTime)
            {
                _isRotating = false;
                _rotationTime = 0f;
                _rotationInput = 0;

                // AkSoundEngine.PostEvent("play_radar_rotate_end", gameObject);
                AkUnitySoundEngine.PostEvent("play_radar_rotate_end", gameObject);
            }
        }


    }

    private void UpdateDishState()
    {
        _dishChangeTimer += Time.deltaTime;

        if (_dishChangeTimer > TIME_TO_CHANGE_DISH)
        {
            if (_dishState == State.Long)
            {
                _dishState = State.Short;
            }
            else if (_dishState == State.Short)
            {
                _dishState = State.Long;
            }

            _dishChangeTimer = 0;
            _isChangingDishState = false;
        }
    }

    private void UpdateWidth()
    {
        // --- Width input:        
        int widthInputChange = 0;
        if (Input.GetKey(KeyCode.W))
        {
            widthInputChange = -1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            widthInputChange = 1;
        }

        // --- Input speed:
        float widthChangeSpeed = 0;
        if (_isChangingDishState)
        {
            widthChangeSpeed = 0;
        }
        else if (_dishState == State.Short)
        {
            widthChangeSpeed = WIDTH_CHANGE_SPEED_SHORT;
        }
        else if (_dishState == State.Long)
        {
            widthChangeSpeed = WIDTH_CHANGE_SPEED_LONG;
        }
        
        // --- Change width input:
        _widthInput += widthInputChange * Time.deltaTime * widthChangeSpeed;
        _widthInput = Mathf.Clamp(_widthInput, 0, 1);


        // Find mind and max width:        
        float minimumWidth = 0;
        float maximumWidth = 0;
        if (_isChangingDishState)
        {
            // Dish state is currently being changed:
            float oldMax = 0;
            float newMax = 0;
            float oldMin = 0;
            float newMin = 0;

            if (_dishState == State.Short)
            {
                oldMin = MINIMUM_WIDTH_SHORT;
                oldMax = MAXIMUM_WIDTH_SHORT;

                newMin = MINIMUM_WIDTH_LONG;
                newMax = MAXIMUM_WIDTH_LONG;
            }
            else if (_dishState == State.Long)
            {
                oldMin = MINIMUM_WIDTH_LONG;
                oldMax = MAXIMUM_WIDTH_LONG;

                newMin = MINIMUM_WIDTH_SHORT;
                newMax = MAXIMUM_WIDTH_SHORT;
            }

            float lerpAmount = _dishChangeTimer / TIME_TO_CHANGE_DISH;
            minimumWidth = Mathf.Lerp(oldMin, newMin, lerpAmount);
            maximumWidth = Mathf.Lerp(oldMax, newMax, lerpAmount);
        }
        else
        {
            // Dish state is not being changed:
            if (_dishState == State.Short)
            {
                minimumWidth = MINIMUM_WIDTH_SHORT;
                maximumWidth = MAXIMUM_WIDTH_SHORT;
            }
            else if (_dishState == State.Long)
            {
                minimumWidth = MINIMUM_WIDTH_LONG;
                maximumWidth = MAXIMUM_WIDTH_LONG;
            }
        }
        
        // Apply actual width:
        _width = Mathf.Lerp(minimumWidth, maximumWidth, _widthInput);
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
            if (detectable == null)
            {
                continue;
            }

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

        _sweepAngle -= Time.deltaTime * SWEEP_SPEED;
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



    #region Public methods

    public static void SwapDishState()
    {
        if (_Instance._isChangingDishState == false)
        {
            _Instance._isChangingDishState = true;
        }
    }

    #endregion


    #region Debug

    private void MakeContactWithEverything()
    {
        foreach (IRadarDetectable detectable in _allDetectables)
        {
            OnRadarContactOccured?.Invoke(new RadarContact(detectable.GetPosition(), 1, detectable));
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
