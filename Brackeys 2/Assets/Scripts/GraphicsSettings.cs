using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    static string qualityPrefsString = "currentQuality";

    // Start is called before the first frame update
    void Start()
    {
        UpdateQuality(PlayerPrefs.GetInt(qualityPrefsString, 2));
    }

    public void UpdateQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        dropdown.value = index;

        PlayerPrefs.SetInt(qualityPrefsString, index);
    }
}
