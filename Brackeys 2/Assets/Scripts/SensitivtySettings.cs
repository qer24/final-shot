using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivtySettings : MonoBehaviour
{
    public static float sensitivityMultiplier = 1.00f;
    static string sensitivityPrefString = "sensitivity";

    public Slider sensSlider;
    public TextMeshProUGUI sensText;

    private void Start()
    {
        UpdateSensitivity(PlayerPrefs.GetFloat(sensitivityPrefString, 1.00f));
    }

    public void UpdateSensitivity(float newSens)
    {
        sensSlider.value = newSens;
        sensText.text = newSens.ToString("F2");
        sensitivityMultiplier = newSens;

        PlayerPrefs.SetFloat(sensitivityPrefString, newSens);
    }
}
