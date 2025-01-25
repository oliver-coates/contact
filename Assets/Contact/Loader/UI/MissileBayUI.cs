using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissileBayUI : MonoBehaviour
{
    [SerializeField] private MissileBay _toDisplay;

    [Header("Color:")]
    [SerializeField] private Color _whiteColor;
    [SerializeField] private Color _grayColor;
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _orangeColor;

    [Header("UI References:")]
    [SerializeField] private Image _backerImage;
    [SerializeField] private Image _rearImage;
    [SerializeField] private Image[] _missileImages;
    [SerializeField] private TextMeshProUGUI[] _missileImageText;


    public void Setup(MissileBay toDisplay)
    {
        _toDisplay = toDisplay;
    
        RedrawMissiles(toDisplay.missiles);
        ShowIsNotLoading();

        toDisplay.OnFinishedLoading += ShowIsNotLoading;
        toDisplay.OnStartedLoad += ShowIsLoading;
        toDisplay.OnMissileCountChanged += RedrawMissiles;
        toDisplay.OnSelectionStateChanged += SetSelectionState;

        SetSelectionState(toDisplay.isSelected);
    }

    private void OnDestroy()
    {
        if (_toDisplay != null)
        {
            _toDisplay.OnFinishedLoading -= ShowIsNotLoading;
            _toDisplay.OnStartedLoad -= ShowIsLoading;
            _toDisplay.OnMissileCountChanged -= RedrawMissiles;
            _toDisplay.OnSelectionStateChanged -= SetSelectionState;

        }
        
    }

    private void RedrawMissiles(int count)
    {
        for (int missileIndex = 0; missileIndex < 4; missileIndex++)
        {
            if (missileIndex < count)
            {
                // Is loaded
                _missileImages[missileIndex].color = _whiteColor;

                _missileImageText[missileIndex].color = _whiteColor;
                _missileImageText[missileIndex].text = $"{missileIndex+1}";
            }
            else
            {
                // Is not loaded
                _missileImages[missileIndex].color = _redColor;

                _missileImageText[missileIndex].color = _orangeColor;
                _missileImageText[missileIndex].text = $"X";
            }
        }
    }

    private void ShowIsLoading()
    {
        _rearImage.color = _grayColor;
    }

    private void ShowIsNotLoading()
    {
        _rearImage.color = _whiteColor;
    }

    private void SetSelectionState(bool state)
    {
        if (state == true)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void Select()
    {
        _backerImage.color = _orangeColor;
    }

    public void Deselect()
    {
        _backerImage.color = Color.black;
    }

    public void AttemptStartLoad()
    {
        Loader.StartLoad(_toDisplay);
    }
    
}
