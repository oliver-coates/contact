using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : MonoBehaviour
{

    private static Captain Instance;
    void Awake()
    {
        Instance = this;
    }

    public static void Detection(IRadarDetectable radarDetectable, int bearing, JetEnemy.DetectionDistance jetDistance)
    {
        float tempBearing = bearing / 10;
        int bearingOrder = (int)tempBearing * 10;
        if (bearingOrder == 360)
        {
            bearingOrder = 0;
        }

        Debug.Log($"Enemy Bearing: {bearingOrder} | {bearing} | {jetDistance}");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
