using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _Instance;

    [Header("State:")]
    [SerializeField] private bool _isPlaying;
    private Queue<string> _dialogueQueue;

    [SerializeField] private float _stress;
    public static float stress
    {
        get
        {
            return _Instance._stress;
        }
    }

    #region Initialisation
    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        _dialogueQueue = new Queue<string>();
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AkUnitySoundEngine.SetRTPCValue("bearing_RTPC", 4);
            
            DialogueManager.PlayDialogue("play_captain_high_stress");
        }

        _stress = (5 - Engineer.healthRemaining);
    }


    public static void PlayDialogue(string toPlay)
    {
        if (_Instance._isPlaying)
        {
            // Queue:
            _Instance.QueueDialogue(toPlay);
        }
        else
        {
            // Nothing playing, play:
            _Instance.Play(toPlay);
        }
    }

    private void QueueDialogue(string toQueue)
    {
        _dialogueQueue.Enqueue(toQueue);
    }

    private void Play(string toPlay)
    {
        if (_Instance == null)
        {
            return;
        }

        // Debug.Log($"dialogue starting: {toPlay}");
        _isPlaying = true;

        AkCallbackManager.EventCallback endCallback = new AkCallbackManager.EventCallback(DialogueFinished);

        AkUnitySoundEngine.PostEvent(toPlay, gameObject,(uint) AkCallbackType.AK_EndOfEvent, endCallback, null);
    }


    private void DialogueFinished(object in_cookie, AkCallbackType in_type, object in_info)
    {
        // Debug.Log($"dialogue finished?");
        if (_dialogueQueue.Count == 0)
        {
            // Nothing else to play
            _isPlaying = false;
        }
        else
        {
            string nextInQueue = _dialogueQueue.Dequeue();
            Play(nextInQueue);
        }
    }
}
