using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarOverlayUI : MonoBehaviour
{
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private LineRenderer _lineRenderer;

    private float _refreshTimer;


    [Header("Settings:")]
    [SerializeField] private float _radius;
    [SerializeField] private float _refreshRate;
    [SerializeField] private float _degreesPerMidPoint = 10;

    
    [Header("Gunner Settings:")]
    [SerializeField] private Color _restColor;
    [SerializeField] private Color _attemptingLockColor;
    [SerializeField] private Color _readyToFireColor;

    private void Update()
    {
        _lineTransform.localEulerAngles = new Vector3(0, 0, Radar.Rotation);


        _refreshTimer += Time.deltaTime;

        if (_refreshTimer > _refreshRate)
        {
            RedrawLines();
            _refreshTimer = 0;
        }
    }

    private void RedrawLines()
    {
        int numberOfMidPoints = Mathf.FloorToInt(Radar.Width / _degreesPerMidPoint);
        // int numberOfMidPoints = 3;

        float degreesPerPoint = Radar.Width / numberOfMidPoints;


        // 2   -> start point
        // + 1 -> point branching out
        // + number of mid points
        Vector3[] points = new Vector3[2 + 1 + numberOfMidPoints];


        // Start Point:
        points[0] = Vector3.zero;

        for (int pointIndex = 0; pointIndex < 1 + numberOfMidPoints; pointIndex++)
        {
            float degree = (Radar.Width / 2f) - (degreesPerPoint * pointIndex);
            float theta = degree * Mathf.Deg2Rad;

            float x = Mathf.Sin(theta) * _radius;
            float y = Mathf.Cos(theta) * _radius;

            points[pointIndex + 1] = new Vector3(x, y, 0);
        }

        // End point
        points[2 + numberOfMidPoints] = Vector3.zero;

        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);

        SetLineColor();
    }

    private void SetLineColor()
    {

        // Coloration:
        Color colorThisFrame;
        if (Gunner.ReadyToFire)
        {
            colorThisFrame = _readyToFireColor;
        }
        else if (Gunner.AttemptingLock)
        {
            colorThisFrame = _attemptingLockColor;
        }
        else
        {
            colorThisFrame = _restColor;
        }

        _lineRenderer.startColor = colorThisFrame;
        _lineRenderer.endColor = colorThisFrame;
    }
}
