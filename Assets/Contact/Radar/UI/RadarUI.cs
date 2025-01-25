using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadarUI : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] private float _refreshRateSeconds;
    private float _refreshTimer = 0;

    [SerializeField] private Color _activeSweepColor;
    [SerializeField] private Color _inactiveSweepColor;
    [SerializeField] private GameObject _contactPrefab;
    [SerializeField] private float _contactMaximumDrawDistance = 43;


    [Header("UI References:")]
    [SerializeField] private Transform _centerIcon;
    [SerializeField] private Transform _sweepTransform;
    [SerializeField] private LineRenderer _sweepLine;
    [SerializeField] private TextMeshProUGUI _rotaionText;
    
    [SerializeField] private Transform _contactsHolder;

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

        RefreshUI();
    }

    private void RefreshUI()
    {
        _centerIcon.localEulerAngles = new Vector3(0, 0, Radar.Rotation); 

        _sweepTransform.localEulerAngles = new Vector3(0, 0, Radar.SweepAngle);

        if (Radar.IsRotating)
        {
            _sweepLine.startColor = _inactiveSweepColor;
            _sweepLine.endColor = _inactiveSweepColor;
        }
        else
        {
            _sweepLine.startColor = _activeSweepColor;
            _sweepLine.endColor = _activeSweepColor;
        }

    }

    private void RefreshUIDelayed()
    {
        _rotaionText.text = $"{Radar.Rotation:000}";
    }

    public void DrawContact(RadarContact contact)
    {
        Vector3 contactPosition = contact.position;

        RectTransform newContact = Instantiate(_contactPrefab, _contactsHolder).GetComponent<RectTransform>();

        float xPos = (contactPosition.x / Radar.MAXIMUM_DISTANCE) * _contactMaximumDrawDistance;
        float yPos = (contactPosition.z / Radar.MAXIMUM_DISTANCE) * _contactMaximumDrawDistance;


        newContact.anchoredPosition = new Vector2(xPos, yPos);
    }
}
