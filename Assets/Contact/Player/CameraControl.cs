using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraControl : MonoBehaviour
{
    [Header("Input:")]
    [SerializeField] private float _targetX;
    [SerializeField] private float _targetY;

    [Header("State:")]
    [SerializeField] private bool _enabled;
    [SerializeField] private float _xRot;
    [SerializeField] private float _yRot;

    [Header("Settings:")]
    [SerializeField] private float _lookSpeed;
    [SerializeField] private float _lookSensitivity;

    [SerializeField] private float _maxY;
    [SerializeField] private float _maxX;

    [Header("   Camera Shake")]
    [SerializeField] private bool shaking;
    [SerializeField] private float maxShakeIntensity;
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float currentShake;
    [SerializeField] private VolumeProfile globalVolume;
    [SerializeField] private DepthOfField blur;
    private Vector3 originalPos;


    [Header("References:")]
    [SerializeField] private Transform xAxisPivot;
    [SerializeField] private Transform yAxisPivot;

    private void Awake()
    {
        GameManager.OnGameStart += Enable;
        globalVolume.TryGet(out blur);
        //blur.focalLength.value = 1;
        blur.focalLength.Override(1);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        originalPos = transform.position;
    }

    private void Enable()
    {
        _enabled = true;
    }

    private void Update()
    {
        if (_enabled)
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


            if (shaking)
            {
                if (currentShake >= shakeDuration)
                {
                    shaking = false;
                    currentShake = 0;
                    blur.focalLength.Override(1);
                }

                currentShake += Time.deltaTime;
                shakeAmount -= Mathf.Pow(currentShake, 2);
                shakeAmount = Mathf.Clamp(shakeAmount, 0, maxShakeIntensity);

                blur.focalLength.Override(Mathf.Lerp(1,50,shakeAmount));

                Vector3 newPos = originalPos + Random.insideUnitSphere * (Time.deltaTime * shakeAmount);
                //newPos.y = transform.position.y;
                //newPos.x = transform.position.x;
                //newPos.z = transform.position.z;

                transform.position = newPos;

            }

            // // Shaking Debug
            // if (Input.GetKeyDown(KeyCode.X))
            // {
            //     ShakeCamera();
            // }



        }

      
    }

    public void ShakeCamera()
    {
        if (!shaking)
        {
            shaking = true;
            shakeAmount = maxShakeIntensity;
        }
    }
    
}
