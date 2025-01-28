using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadarUI : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] private float _refreshRateSeconds;
    private float _refreshTimer = 0;

    [SerializeField] private Color _activeSweepColor;
    [SerializeField] private Color _inactiveSweepColor;
    [SerializeField] private GameObject _contactPrefab;
    [SerializeField] private Color _activeButtonColor;
    [SerializeField] private Color _inactiveButtonColor;
    [SerializeField] private float _contactMaximumDrawDistance = 43;



    [Header("UI References:")]
    [SerializeField] private Transform _centerIcon;
    [SerializeField] private Transform _sweepTransform;
    [SerializeField] private LineRenderer _sweepLine;

    [SerializeField] private Animator _dishAnimator;
    [SerializeField] private Image _shortDishButtonBacker;
    [SerializeField] private Image _longDishButtonBacker;

    [SerializeField] private TextMeshProUGUI _rotaionText;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _lockText;
    [SerializeField] private TextMeshProUGUI _missileText;


    [SerializeField] private Transform _contactsHolder;

    [Header("Color:")]
    [SerializeField] private Color _greenColor;
    [SerializeField] private Color _whiteColor;
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _yellowColor;
    [SerializeField] private Color _orangeColor;



    private void Awake()
    {
        Radar.OnRadarContactOccured += DrawContact;
    }

    private void OnDestroy()
    {
        Radar.OnRadarContactOccured -= DrawContact;
    }

    private void Update()
    {
        _refreshTimer += Time.deltaTime;

        if (_refreshTimer > _refreshRateSeconds)
        {
            RefreshUIDelayed();
            _refreshTimer = 0f;
        }

        if (Radar.IsChangingDishState)
        {
            _longDishButtonBacker.color = _inactiveButtonColor;
            _shortDishButtonBacker.color = _inactiveButtonColor;
        }
        else
        {
            if (Radar.DishState == Radar.State.Long)
            {
                _shortDishButtonBacker.color = _activeButtonColor;
                _longDishButtonBacker.color = _inactiveButtonColor;
            }
            else if (Radar.DishState == Radar.State.Short)
            {
                _shortDishButtonBacker.color = _inactiveButtonColor;
                _longDishButtonBacker.color = _activeButtonColor;
            }    
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        _centerIcon.localEulerAngles = new Vector3(0, 0, Radar.Rotation); 

        _sweepTransform.localEulerAngles = new Vector3(0, 0, Radar.SweepAngle);

        if (Radar.IsRotating || Radar.IsChangingDishState)
        {
            _sweepLine.startColor = _inactiveSweepColor;
            _sweepLine.endColor = _inactiveSweepColor;
        }
        else
        {
            _sweepLine.startColor = _activeSweepColor;
            _sweepLine.endColor = _activeSweepColor;
        }

        

        if (Gunner.ReadyToFire)
        {
            float distanceToTarget = Gunner.CurrentTrackedDetectable.GetPosition().magnitude;
            _distanceText.text = $"{distanceToTarget:0000}"; 

            _distanceText.color = _redColor;

            _lockText.text = "GOOD";
            _lockText.color = _greenColor;
        }

    }

    private void RefreshUIDelayed()
    {
        _rotaionText.text = $"{Radar.Rotation:000}";

        if (Gunner.AttemptingLock && !Gunner.ReadyToFire)
        {
            float randomDistance = UnityEngine.Random.Range(0, 10000);
            _distanceText.text = $"{randomDistance:0000}"; 

            _distanceText.color = _orangeColor;

            _lockText.text = "HOLD";
            _lockText.color = _redColor;
        }
        else
        {
            _distanceText.text = "0000";
            _distanceText.color = _whiteColor;

            _lockText.text = "NULL";
            _lockText.color = _whiteColor;
        }

        if (Loader.IsSwitching)
        {
            _missileText.text = "WAIT";
            _missileText.color = _yellowColor;
        }
        else if (Loader.IsLoading || Loader.NeedsLoad)
        {
            _missileText.text = "LOAD";
            _missileText.color = _redColor;
        }
        else
        {
            _missileText.text = "READY";
            _missileText.color = _greenColor;
        }

    }

    public void DrawContact(RadarContact contact)
    {
        if (contact.position.magnitude > 10000)
        {
            // Ignore those outside of radar range
            return;
        }

        AkUnitySoundEngine.PostEvent("Play_ping_lp", gameObject);

        Vector3 contactPosition = contact.position;

        RectTransform newContact = Instantiate(_contactPrefab, _contactsHolder).GetComponent<RectTransform>();

        float xPos = (contactPosition.x / Radar.MAXIMUM_DISTANCE) * _contactMaximumDrawDistance;
        float yPos = (contactPosition.z / Radar.MAXIMUM_DISTANCE) * _contactMaximumDrawDistance;


        newContact.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void AttemptSwitchToShortDish()
    {
        if (Radar.IsChangingDishState)
        {
            return;
        }

        if (Radar.DishState == Radar.State.Short)
        {
            return;
        }

        _dishAnimator.SetBool("dishIsLong", false);

        Radar.SwapDishState();
    }

    public void AttemptSwitchToLongDish()
    {
        if (Radar.IsChangingDishState)
        {
            return;
        }

        if (Radar.DishState == Radar.State.Long)
        {
            return;
        }

        _dishAnimator.SetBool("dishIsLong", true);

        Radar.SwapDishState();
    }
}
