using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake(){
        GameManager.OnGameEnd += OnGameEnd;
        GameManager.OnGameStart += OnGameStart;

        _animator.SetBool("isGameRunning", false);


    }

    private void OnDestroy()
    {
        GameManager.OnGameEnd -= OnGameEnd;
        GameManager.OnGameStart -= OnGameStart;

    }

    public void StartGame()
    {
        GameManager.StartGame();
    }

    public void Restart()
    {
        GameManager.Restart();
    }

    public void OnGameStart()
    {
        _animator.SetBool("isGameRunning", true);
    }

    public void OnGameEnd()
    {
        _animator.SetBool("isGameRunning", false);
    }
}
