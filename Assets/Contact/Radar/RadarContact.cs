using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarContact
{
    [SerializeField] private Vector3 _position;
    public Vector3 position
    {
        get
        {
            return _position;
        }	
    }

    [SerializeField] private float _strength;
    public float strength
    {
        get
        {
            return _strength;
        }	
    }

    [SerializeField] private IRadarDetectable _detectable;
    public IRadarDetectable detectable
    {
        get
        {
            return _detectable;
        }	
    }


    public RadarContact(Vector3 position, float strength, IRadarDetectable detectable)
    {
        _position = position;
        _strength = strength;
        _detectable = detectable;
    }
}
