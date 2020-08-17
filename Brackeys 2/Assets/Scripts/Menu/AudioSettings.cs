using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    float musicVolume = 1f;
    float sfxVolume = 1f;
    float masterVolume = 1f;

    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SFX;
    FMOD.Studio.Bus Master;

    [SerializeField] Slider musicSlider = null;
    [SerializeField] Slider sfxSlider = null;
    [SerializeField] Slider masterSlider = null;

    private void Start()
    {
        Music = RuntimeManager.GetBus("bus:/Master/Music");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        Master = RuntimeManager.GetBus("bus:/Master");

        SetVolume();
    }

    void SetVolume()
    {
        Music.setVolume(musicVolume);
        SFX.setVolume(sfxVolume);
        Master.setVolume(masterVolume);

        UpdateUI();
    }

    public void UpdateMusicVolume(float vol)
    {
        musicVolume = vol;
        SetVolume();
    }

    public void UpdateSFXVolume(float vol)
    {
        if(Time.time > 2f)
            AudioManager.Play("menuAudioTest");
        sfxVolume = vol;
        SetVolume();
    }

    public void UpdateMasterVolume(float vol)
    {
        masterVolume = vol;
        SetVolume();
    }

    void OnEnable()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        musicVolume = PlayerPrefs.GetFloat("MusicVol", 0.75f);

        UpdateUI();
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat("MasterVol", masterVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);

        UpdateUI();
    }

    void UpdateUI()
    {
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        masterSlider.value = masterVolume;
    }
}
