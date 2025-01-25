using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadarUI : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] private float _refreshRateSeconds;
    private float _refreshTimer = 0;


    [Header("UI References:")]
    [SerializeField] private Transform _centerIcon;
    [SerializeField] private Transform _sweepTransform;

    [SerializeField] private TextMeshProUGUI _rotaionText;

    private void Start()
    {

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

    }

    private void RefreshUIDelayed()
    {
        _rotaionText.text = $"{Radar.Rotation:000}";
    }

}
