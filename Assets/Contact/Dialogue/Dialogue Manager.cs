using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _Instance;

    [Header("State:")]
    [SerializeField] private bool _isPlaying;
    private Queue<DialogueType> _dialogueQueue;

    [SerializeField] [Range(0, 10)] private float _stress;
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
        _dialogueQueue = new Queue<DialogueType>();
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayDialogue(new ContactSpottedDialogueType(0, 1.5f));
        }

        _stress = (5 - Engineer.healthRemaining) * 2f;
        _stress = Mathf.Clamp(_stress, 0, 10f);
    }


    public static void PlayDialogue(DialogueType toPlay)
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

    private void QueueDialogue(DialogueType toQueue)
    {
        _dialogueQueue.Enqueue(toQueue);
    }

    private void Play(DialogueType toPlay)
    {
        if (_Instance == null)
        {
            return;
        }

        // Debug.Log($"dialogue starting: {toPlay}");
        _isPlaying = true;

        AkCallbackManager.EventCallback endCallback = new AkCallbackManager.EventCallback(DialogueFinished);


        string eventName = toPlay.GetDialogueEventName(); 
        toPlay.SetRTPCs();

        AkUnitySoundEngine.PostEvent(eventName, gameObject,(uint) AkCallbackType.AK_EndOfEvent, endCallback, null);
    }


    private void DialogueFinished(object in_cookie, AkCallbackType in_type, object in_info)
    {
        Debug.Log($"dialogue finished?");
        if (_dialogueQueue.Count == 0)
        {
            // Nothing else to play
            _isPlaying = false;
        }
        else
        {
            DialogueType nextInQueue = _dialogueQueue.Dequeue();
            Play(nextInQueue);
        }
    }
}
