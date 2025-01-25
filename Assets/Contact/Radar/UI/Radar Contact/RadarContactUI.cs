using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarContactUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    [Header("Settings:")]
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Color _color;
    private Color _fadeColor;
    [SerializeField] private float _minimumFadeTime;
    [SerializeField] private float _maximumFadeTime;

    [Header("State:")]
    [SerializeField] private float _timer;
    [SerializeField] private float _fadeTime;
    
    public void Start()
    {
        _timer = 0f;
        _fadeTime = UnityEngine.Random.Range(_minimumFadeTime, _maximumFadeTime);

        _image.sprite = _sprites[UnityEngine.Random.Range(0, _sprites.Length)];
        
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));

        _fadeColor = _color;
        _fadeColor.a = 0;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        _image.color = Color.Lerp(_color, _fadeColor, _timer / _fadeTime);

        if (_timer > _fadeTime)
        {
            Destroy(gameObject);
        }
    }

}
