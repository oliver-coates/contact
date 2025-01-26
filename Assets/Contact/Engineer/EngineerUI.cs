using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
        Engineer.OnHealthChanged += HealthChanged;
    }

    private void OnDestroy()
    {
        Engineer.OnHealthChanged -= HealthChanged;
    }

    private void HealthChanged(int newHealth)
    {
        _animator.SetFloat("Health", newHealth);
    }


}
