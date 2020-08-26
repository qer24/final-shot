using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsSettings : MonoBehaviour
{
    public static float sensitivityMultiplier = 1.00f;
    static string sensitivityPrefString = "sensitivity";
    public static string rewindKey = "r";
    static string rewindKeyPrefString = "rewindKey";

    public TMP_Dropdown rewindKeyDropdown;
    public Slider sensSlider;
    public TextMeshProUGUI sensText;

    private void Start()
    {
        UpdateSensitivity(PlayerPrefs.GetFloat(sensitivityPrefString, 1.00f));
        UpdateRewindKey(PlayerPrefs.GetString(rewindKeyPrefString, "r"));
    }

    void UpdateRewindKey(string newKey)
    {
        int dropdownInt = 0;
        if (newKey == "r")
        {
            dropdownInt = 0;
        }
        else if (newKey == "f")
        {
            dropdownInt = 1;
        }
        else if (newKey == "e")
        {
            dropdownInt = 2;
        }

        rewindKeyDropdown.value = dropdownInt;

        rewindKey = newKey;
        PlayerPrefs.SetString(rewindKeyPrefString, newKey);
    }

    public void UpdateRewindKey(int newKey)
    {
        string keyString = "r";
        if(newKey == 0)
        {
            keyString = "r";
        }
        else if(newKey == 1)
        {
            keyString = "f";
        }
        else if(newKey == 2)
        {
            keyString = "e";
        }

        rewindKeyDropdown.value = newKey;

        rewindKey = keyString;
        PlayerPrefs.SetString(rewindKeyPrefString, keyString);
    }

    public void UpdateSensitivity(float newSens)
    {
        sensSlider.value = newSens;
        sensText.text = newSens.ToString("F2");
        sensitivityMultiplier = newSens;

        PlayerPrefs.SetFloat(sensitivityPrefString, newSens);
    }
}
