using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static event Action OnGameStart;
    public static event Action OnGameEnd;





    
    private static GameManager _Instance;

 
    [SerializeField] private bool _isGameRunning;
    public static bool IsGameRunning
    {
        get
        {
            return _Instance._isGameRunning;
        }
    }
 

    private void Awake()
    {
        _isGameRunning = false;
        _Instance = this;

        
    }

    public static void StartGame()
    {
        _Instance._isGameRunning = true;
        OnGameStart?.Invoke();
    }

    public static void EndGame()
    {
        _Instance._isGameRunning = false;
        OnGameEnd?.Invoke();
    }

    public static void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
