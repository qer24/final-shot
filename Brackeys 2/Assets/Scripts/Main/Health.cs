using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public EntityStats stats;
    public float currentHealth = 0;

    public delegate void HealthAction(float currentHealthAmount);
    public HealthAction OnHealthChanged;
    public Action OnDeath;

    public bool isDead;

    void Start()
    {
        currentHealth = stats.maxHealth;

        isDead = false;
    }

    public void RemoveHealth(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0) Death();
    }

    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Min(stats.maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }

    void Death()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
    }
}
