using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthCounter : MonoBehaviour
{
    public EntityStats playerStats;
    TextMeshProUGUI text;

    float currentHealth;
    float displayedHealth;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        displayedHealth = playerStats.maxHealth;
        currentHealth = Mathf.RoundToInt(displayedHealth);
    }

    void Update()
    {
        displayedHealth = Mathf.Lerp(displayedHealth, currentHealth, Time.deltaTime * 5f);
        text.text = Mathf.Round(displayedHealth).ToString();
    }

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateText;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateText;
    }

    void UpdateText(float currentHealth)
    {
        this.currentHealth = currentHealth;
    }
}
