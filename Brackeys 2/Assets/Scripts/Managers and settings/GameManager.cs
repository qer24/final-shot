using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    public static int score = 0;
    public static bool isPlayerDead;
    public static bool isPaused;
    public static GameManager instance;
    public static Action OnPlayerDeath;

    public GameObject gameOverCanvas;
    public TextMeshProUGUI gameOverScoreText;

    public GameObject pauseCanvas;
    public GameObject settingsPanel;

    bool closing;

    public void RestartGame()
    {
        score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        float lowPass = 0f;

        if(PlayerRewind.isRewinding)
        {
            lowPass = 0.5f;
        }
        else
        {
            lowPass = 0;
        }

        if (isPaused || isPlayerDead || RewardsManager.isChoosingReward)
        {
            lowPass = 1;
        }

        RuntimeManager.StudioSystem.setParameterByName("LowPass", lowPass);

        if(Input.GetKeyDown(KeyCode.Escape) && !RewardsManager.isChoosingReward)
        {
            if(!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void PlayHoverSound()
    {
        AudioManager.Play("menuHover");
    }

    public void ToggleSettings()
    {
        if (closing) return;

        if (!settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            closing = true;

            var scaleTween = settingsPanel.GetComponentInChildren<ScaleTween>();
            scaleTween.Close();
            StartCoroutine(DisablePanel(scaleTween.duration));
        }

        AudioManager.Play("menuClick");
    }

    IEnumerator DisablePanel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        settingsPanel.SetActive(false);
        closing = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        isPaused = false;

        AudioManager.Play("menuClick");

        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        AudioManager.Play("menuClick");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        closing = false;
        isPaused = false;
        pauseCanvas.SetActive(false);
        settingsPanel.SetActive(false);

        CancelInvoke();
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AudioManager.Play("menuOpen");

        Time.timeScale = 0f;

        isPaused = true;
        pauseCanvas.SetActive(true);
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        score = 0;
        isPlayerDead = false;
        isPaused = false;

        RuntimeManager.StudioSystem.setParameterByName("LowPass", 0f);
    }

    public void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();

        isPlayerDead = true;
        gameOverScoreText.text = $"Score : {score}";
        gameOverCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
