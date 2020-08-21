using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public EntityStats playerStats;

    public Health hp;

    public delegate void HealthAction(float currentHealth);
    public static HealthAction OnHealthChanged;

    private void OnEnable()
    {
        hp.OnHealthChanged += UpdateHealth;
        hp.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        hp.OnHealthChanged -= UpdateHealth;
        hp.OnDeath -= HandleDeath;
    }

    void UpdateHealth(float currentHealthAmount)
    {
        OnHealthChanged?.Invoke(currentHealthAmount);
    }

    void HandleDeath()
    {
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerShooter>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Damagable>().enabled = false;
        GetComponent<PlayerRewind>().enabled = false;

        GameManager.instance.PlayerDeath();
    }
}
