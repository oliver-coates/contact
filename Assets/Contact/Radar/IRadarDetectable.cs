using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRadarDetectable
{
    public Vector3 GetPosition(); 

    public void SetBearing(int bearing);

    public void Shotdown();

}
