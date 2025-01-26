using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Input:")]
    [SerializeField] private float _targetX;
    [SerializeField] private float _targetY;

    [Header("State:")]
    [SerializeField] private float _xRot;
    [SerializeField] private float _yRot;

    [Header("Settings:")]
    [SerializeField] private float _lookSpeed;
    [SerializeField] private float _lookSensitivity;

    [SerializeField] private float _maxY;
    [SerializeField] private float _maxX;

    [Header("References:")]
    [SerializeField] private Transform xAxisPivot;
    [SerializeField] private Transform yAxisPivot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            _targetX -= Input.GetAxis("Mouse Y") * _lookSensitivity;
            _targetY += Input.GetAxis("Mouse X") * _lookSensitivity;


            _targetX = Mathf.Clamp(_targetX, -_maxX, _maxX);
            _targetY = Mathf.Clamp(_targetY, -_maxY, _maxY);
        }

        _xRot = Mathf.Lerp(_xRot, _targetX, Time.deltaTime * _lookSpeed);
        _yRot = Mathf.Lerp(_yRot, _targetY, Time.deltaTime * _lookSpeed);


        xAxisPivot.localEulerAngles = new Vector3(_xRot, 0, 0);
        yAxisPivot.localEulerAngles = new Vector3(0, _yRot, 0);
    }
}
