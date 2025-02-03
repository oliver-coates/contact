using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : MonoBehaviour
{
    private static Captain _Instance;

    void Awake()
    {
        _Instance = this;
    }

    public static void GiveDetectionNotification(RadarDetectable radarDetectable, int bearing, JetEnemy.DetectionDistance jetDistance)
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

        float distanceToTarget = 0;
        if (jetDistance == JetEnemy.DetectionDistance.Near)
        {
            distanceToTarget = 0.5f;
        }
        else if (jetDistance == JetEnemy.DetectionDistance.Far)
        {
            distanceToTarget = 1.5f;
        }

        DialogueManager.PlayDialogue(new ContactSpottedDialogueType(bearingOrder, distanceToTarget));
    }

}
