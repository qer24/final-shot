using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        PlayerShooter.OnAmmoChanged += UpdateText;
    }

    private void OnDisable()
    {
        PlayerShooter.OnAmmoChanged -= UpdateText;
    }

    void UpdateText(string ammoCount)
    {
        text.text = ammoCount;
    }
}
