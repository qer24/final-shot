using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSettings : MonoBehaviour
{
    static string damagePopupText = "damagePopup";
    public Toggle toggle;

    public static bool spawnPopupText;

    private void Start()
    {
        UpdatePopup(PlayerPrefs.GetInt(damagePopupText, 1) == 1);
    }

    public void UpdatePopup(bool state)
    {
        toggle.isOn = state;
        spawnPopupText = state;

        PlayerPrefs.SetInt(damagePopupText, state.GetHashCode());
    }
}
