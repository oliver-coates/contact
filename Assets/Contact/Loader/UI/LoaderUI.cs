using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderUI : MonoBehaviour
{
    [SerializeField] private MissileBayUI[] _missileBayUIs;


    public void Start()
    {
        _missileBayUIs[0].Setup(Loader.MissileBays[0]);
        _missileBayUIs[1].Setup(Loader.MissileBays[1]);
    }

    public void SwitchBays()
    {
        Loader.AttemptSwitchBays();
    }


}
