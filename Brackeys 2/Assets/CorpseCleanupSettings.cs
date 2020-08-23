using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorpseCleanupSettings : MonoBehaviour
{
    static string corpseCleanupPrefString = "corpseCleanup";
    public Toggle toggle;

    public static bool cleanupCorpses;

    private void Start()
    {
        UpdateCorpseCleanup(PlayerPrefs.GetInt(corpseCleanupPrefString, 0) == 1);
    }

    public void UpdateCorpseCleanup(bool state)
    {
        toggle.isOn = state;
        cleanupCorpses = state;

        PlayerPrefs.SetInt(corpseCleanupPrefString, state.GetHashCode());
    }
}
