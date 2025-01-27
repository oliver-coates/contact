using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueType
{
    protected string _dialogueEventName;
    
    protected DialogueType() { }

    public virtual void SetRTPCs() { }
    
    public string GetDialogueEventName()
    {
        return _dialogueEventName;
    }

}

public class ContactSpottedDialogueType : DialogueType
{
    private int _bearingToTarget;
    private float _distanceToTarget;

    public ContactSpottedDialogueType(int bearing, float distance)
    {
        _bearingToTarget = bearing;
        _distanceToTarget = distance;

        _dialogueEventName = "play_captain_high_stress";
    }

    public override void SetRTPCs()
    {
        AkUnitySoundEngine.SetRTPCValue("bearing_RTPC", (_bearingToTarget / 10));
        AkUnitySoundEngine.SetRTPCValue("distance_to_target", _distanceToTarget);
    }
}
