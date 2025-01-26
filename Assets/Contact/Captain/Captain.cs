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
        bearing += Random.Range(-25, 25);

        if (bearing > 360)
        {
            bearing -= 360;
        }
        else if (bearing < 0)
        {
            bearing += 360;
        }

        float tempBearing = bearing / 10;
        int bearingOrder = (int)tempBearing * 10;
        if (bearingOrder == 360)
        {
            bearingOrder = 0;
        }

        Debug.Log($"Enemy Bearing: {bearingOrder} | {bearing} | {jetDistance}");

        float distanceToTarget = 0;
        if (jetDistance == JetEnemy.DetectionDistance.Near)
        {
            distanceToTarget = 0.5f;
        }
        else if (jetDistance == JetEnemy.DetectionDistance.Far)
        {
            distanceToTarget = 1.5f;
        }

        AkUnitySoundEngine.SetRTPCValue("bearing_RTPC", (bearingOrder / 10));
        AkUnitySoundEngine.SetRTPCValue("distance_to_target", distanceToTarget);

        DialogueManager.PlayDialogue("play_captain_high_stress");
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
