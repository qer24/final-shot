using FMODUnity;
using System;
using UnityEngine;

public static class AudioManager
{
    public static void Play(string sound)
    {
        try
        {
            RuntimeManager.PlayOneShot($"event:/{sound}");
        }
        catch (Exception)
        {
            Debug.LogWarning($"Sound: {sound} + not found!");
            return;
        }
    }

    public static void Play(string sound, Vector3 position)
    {
        try
        {
            RuntimeManager.PlayOneShot($"event:/{sound}", position);
        }
        catch (Exception)
        {
            Debug.LogWarning($"Sound: {sound} + not found!");
            return;
        }
    }
}
