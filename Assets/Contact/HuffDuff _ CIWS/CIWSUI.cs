using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CIWSUI : MonoBehaviour
{
    [Header("UI References:")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private Button _OnButton;
    [SerializeField] private Button _OffButton;
    [SerializeField] private Button _loadButton;

    [SerializeField] private TextMeshProUGUI _stateTExt;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _rotationText;

    [SerializeField] private TextMeshProUGUI _loadText;

    [Header("Colors")]
    [SerializeField] private Color _orangeColor;
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _greenColor;
    private void Start()
    {
        _OnButton.interactable = true;
        _OffButton.interactable = false;
    }

    private void Update()
    {
        _canvasGroup.alpha = CIWS.PowerLerp;

        if (CIWS.PowerLerp == 1 || CIWS.LoadLerp == 1)
        {
            // Powered and ready to load:
            _loadButton.interactable = true;
        }
        else
        {
            _loadButton.interactable = false;
        }

        _ammoText.text = CIWS.Ammo.ToString();
        _rotationText.text = $"{CIWS.Rotation:000}";
        
        if (CIWS.PowerLerp < 1)
        {
            _stateTExt.text = "PWR";
            _stateTExt.color = _orangeColor;
        }
        else
        {
            if (CIWS.Ammo == 0)
            {
                _stateTExt.text = "OUT";
                _stateTExt.color = Color.red;
            }
            else if (CIWS.LoadLerp < 1)
            {
                _stateTExt.text = "LOAD";
                _stateTExt.color = _orangeColor;
            }
            else
            {
                _stateTExt.text = "YES";
                _stateTExt.color = _greenColor;
            }
        }

        if (CIWS.LoadLerp < 1)
        {
            _loadText.text = $"{Mathf.FloorToInt(CIWS.LoadLerp * 100):00}%";
            _loadText.color = _orangeColor;
        }
        else
        {
            _loadText.text = "---";
            _loadText.color = _greenColor;
        }
    }

    public void TurnOn()
    {
        CIWS.TogglePowered();

        _OnButton.interactable = false;
        _OffButton.interactable = true;
    }

    public void TurnOff()
    {
        CIWS.TogglePowered();

        _OnButton.interactable = true;
        _OffButton.interactable = false;
    }

    public void Load()
    {
        CIWS.StartLoad();
    }
}
