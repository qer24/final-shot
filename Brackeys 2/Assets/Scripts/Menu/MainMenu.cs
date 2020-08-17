using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsPannel = null;
    public GameObject music;
    bool closing;

    public GameObject startGame;
    static string tutorialDoneString = "tutDone";

    public GameObject levelSelect;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        bool tutorialDone = PlayerPrefs.GetInt(tutorialDoneString, 0) == 1;
        if(!tutorialDone)
        {
            startGame.SetActive(false);
        }
    }

    public void ToggleSettings()
    {
        if (closing) return;

        if (!settingsPannel.activeSelf)
        {
            settingsPannel.SetActive(true);
        }
        else
        {
            closing = true;

            var scaleTween = settingsPannel.GetComponentInChildren<ScaleTween>();
            scaleTween.Close();
            Invoke("DisablePanel", scaleTween.duration);
        }

        AudioManager.Play("menuClick");
    }

    public void DisableLevelSelect()
    {
        levelSelect.SetActive(false);
    }

    void DisablePanel()
    {
        settingsPannel.SetActive(false);
        closing = false;
    }

    public void Tutorial()
    {
        AudioManager.Play("menuClick");
        music.SetActive(false);

        SceneManager.LoadScene("Tutorial");
    }

    public void Play()
    {
        AudioManager.Play("menuClick");
        music.SetActive(false);

        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        AudioManager.Play("menuClick");

        Application.Quit();
    }

    public void PlayHoverSound()
    {
        AudioManager.Play("menuHover");
    }
    
    public static void TutorialComplete()
    {
        PlayerPrefs.SetInt(tutorialDoneString, 1);
    }
}
