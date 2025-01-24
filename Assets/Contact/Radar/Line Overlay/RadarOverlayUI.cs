using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarOverlayUI : MonoBehaviour
{
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private LineRenderer _lineRenderer;


    [Header("Settings:")]
    [SerializeField] private float _radius;

    private void Update()
    {
        _lineTransform.localEulerAngles = new Vector3(0, 0, Radar.Rotation);
    }
}
