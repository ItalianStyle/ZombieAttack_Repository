using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public static event Action OnStaminaFull;
    public static event Action OnStaminaEmpty;
    [SerializeField] [Min(.1f)] float maxStamina = 5f;
    [SerializeField] [Min(.1f)] float usedStaminaFactor = 1f;
    [SerializeField] [Min(.1f)] float recoverStaminaFactor = .5f;
    
    [SerializeField] SimpleHealthBar staminaBar;
    public bool isPlayerRunning = false;
    float _currentStamina;

    public float CurrentStamina
    {
        get => _currentStamina;

        set
        {
            _currentStamina = value;
            if (_currentStamina < 0)
            {
                _currentStamina = 0;
                OnStaminaEmpty?.Invoke();
            }
            else if (_currentStamina > maxStamina)
            {
                _currentStamina = maxStamina;
                OnStaminaFull?.Invoke();
            }
            staminaBar.UpdateBar(_currentStamina, maxStamina);
        }
    }
    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    private void OnEnable()
    {       
        isPlayerRunning = false;
        OnStaminaEmpty += ()=> isPlayerRunning = false;
    }

    private void OnDisable()
    {
        OnStaminaEmpty -= () => isPlayerRunning = false;
    }

    private void Update()
    {
        if (isPlayerRunning)
            CurrentStamina -= Time.deltaTime * usedStaminaFactor;
        else if (CurrentStamina < maxStamina)
            CurrentStamina += Time.deltaTime * recoverStaminaFactor;
    }  
}