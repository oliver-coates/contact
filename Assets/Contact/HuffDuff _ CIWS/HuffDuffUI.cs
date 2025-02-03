using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HuffDuffUI : MonoBehaviour
{
    [Header("UI References:")]
    [SerializeField] private Transform _rotationDial;
    [SerializeField] private CanvasGroup _huffDuffRadar;

    [SerializeField] private Button _OnButton;
    [SerializeField] private Button _OffButton;

    private void Start()
    {
        _OnButton.interactable = true;
        _OffButton.interactable = false;
    }

    private void Update()
    {
        _rotationDial.transform.localEulerAngles = new Vector3(0, 0, HuffDuff.Rotation);
        _huffDuffRadar.alpha = HuffDuff.PowerLerp;
    }

    public void TurnOn()
    {
        HuffDuff.TogglePowered();
        
        _OnButton.interactable = false;
        _OffButton.interactable = true;
    }

    public void TurnOff()
    {
        HuffDuff.TogglePowered();

        _OnButton.interactable = true;
        _OffButton.interactable = false;
    }
}
