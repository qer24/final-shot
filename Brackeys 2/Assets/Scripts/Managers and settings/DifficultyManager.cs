using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DifficultyManager : MonoBehaviour
{
    public RewardsManager rewardsManager;
    public static float difficultyMultiplier;
    public AnimationCurve difficultyCurve;
    public TextMeshProUGUI timerText;

    int timePassed;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdateDifficulty), 0, 1f);
    }

    void UpdateDifficulty()
    {
        if (GameManager.isPlayerDead) return;

        timePassed ++;
        if (timePassed == rewardsManager.timeAtNextReward) rewardsManager.DisplayRewards();
        UpdateUI();
        difficultyMultiplier = difficultyCurve.Evaluate(timePassed);
    }

    private void UpdateUI()
    {
        int minutes = timePassed / 60;
        int seconds = timePassed - minutes * 60;

        timerText.text = seconds >= 10 ? $"{minutes}:{seconds}" : $"{minutes}:0{seconds}";
    }
}
