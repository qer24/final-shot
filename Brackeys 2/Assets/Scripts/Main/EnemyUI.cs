using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Health hp;
    public Slider hpSlider;

    private void Start()
    {
        hpSlider.enabled = false;
    }

    private void OnEnable()
    {
        hp.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        hp.OnHealthChanged -= UpdateHealth;
    }

    void UpdateHealth(float currentHealthAmount)
    {
        hpSlider.enabled = true;
        hpSlider.value = (float)currentHealthAmount / hp.stats.maxHealth;
    }
}
