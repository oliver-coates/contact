using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : MonoBehaviour
{
    private static Engineer _Instance;

    [Header("Settings:")]
    [SerializeField] private int _shipHealth = 5;

    [Header("State:")]
    [SerializeField] private int _healthRemaining = 0;
    public static int healthRemaining
    {
        get
        {
            return _Instance._healthRemaining;
        }
    }



    private void Awake()
    {
        _Instance = this;
        _healthRemaining = _shipHealth;
    } 


    public static void TakeDamage()
    {
        _Instance._healthRemaining -= 1;

        if (_Instance._healthRemaining == 0)
        {
            _Instance.GameOver();
        }
    }



    public void GameOver()
    {
        Application.Quit();
    }

}
