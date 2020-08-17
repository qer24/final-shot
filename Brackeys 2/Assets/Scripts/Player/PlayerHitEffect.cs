using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitEffect : MonoBehaviour
{
    public EntityStats playerStats;
    public Image image;
    float currentHealth;

    private void Start()
    {
        currentHealth = playerStats.maxHealth;
    }

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += HitEffect;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= HitEffect;
    }

    void HitEffect(float amount)
    {
        if (amount >= currentHealth)
        {
            currentHealth = amount;
            return;
        }else
        {
            currentHealth = amount;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
        }
    }

    void Update()
    {
        if (image.color.a == 0) return;

        if (image.color.a < 0.05f) image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        else
        {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 0), Time.deltaTime * 2f);
        }
    }
}
