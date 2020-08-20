using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class RewardsManager : MonoBehaviour
{
    [System.Serializable] public class RewardContainer
    {
        public Button button;
        public TextMeshProUGUI text;
    }

    public static bool isChoosingReward = false;

    public EntityStats playerStats;
    public RewardContainer[] rewardContainers;
    public GameObject rewardPanel;
    public TextMeshProUGUI rewardsText;

    [SerializeField] float maxMoveSpeed = 1.5f;

    string moveSpeedRewardString = "+10% movement speed";
    string newWeaponRewardString = "New weapon: ";
    string magSizeRewardString = "+10% magazine size";
    string damageRewardString = "+10% damage";
    string healRewardString = "Restore 5 health after rewind";
    string firerateRewardString = "+5% fire rate";
    string rewindRewardString = "-0.5s to rewind cooldown";
    string doubleDamageRewardString = "+2% to deal double damage";
    string additionalProjectileRewardString { get => $"10% chance to fire +{playerStats.additionalProjectiles + 1} bullet"; }

    int currentReward = 0;

    public InventoryManager inventoryManager;

    public class Reward
    {
        public string message;
        Delegate method;

        public void RunMethod()
        {
            method.DynamicInvoke();
        }

        public Reward(string message, Delegate method)
        {
            this.message = message;
            this.method = method;
        }
    }

    Transform player;

    public List<GunPickup> droppableGuns = new List<GunPickup>();
    GunPickup currentGunToDrop;

    List<Reward> rewards = new List<Reward>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        playerStats.fireRateMultiplier = 1;
        playerStats.damageMultiplier = 1;
        playerStats.magazineMultiplier = 1;
        playerStats.speedMultiplier = 1;
        playerStats.rewindCooldownReduction = 0;
        playerStats.healOnRewind = 0;
        playerStats.chanceToDoubleDamage = 0;
        playerStats.additionalProjectiles = 0;
    }

    public void DisplayRewards()
    {
        currentReward++;

        isChoosingReward = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        rewardsText.text = $"You survived {Mathf.RoundToInt(SpawnManager.currentWave - 1)} waves";

        rewards = RandomRewards();

        for (int i = 0; i < rewardContainers.Length; i++)
        {
            rewardContainers[i].text.text = rewards[i].message;
            rewardContainers[i].button.interactable = false;
            StartCoroutine(EnableButtons());
        }

        rewardPanel.SetActive(true);
    }

    IEnumerator EnableButtons()
    {
        yield return new WaitForSecondsRealtime(2f);

        for (int i = 0; i < rewardContainers.Length; i++)
        {
            rewardContainers[i].button.interactable = true;
        }
    }

    public void Reward1()
    {
        rewards[0].RunMethod();
    }

    public void Reward2()
    {
        rewards[1].RunMethod();
    }

    public void Reward3()
    {
        rewards[2].RunMethod();
    }

    void ResumeGameplay()
    {
        rewardPanel.SetActive(false);

        isChoosingReward = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;
    }

    void IncreaseMoveSpeed()
    {
        playerStats.speedMultiplier += 0.1f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseMagSize()
    {
        playerStats.magazineMultiplier += 0.1f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseDamage()
    {
        playerStats.damageMultiplier += 0.1f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseHealOnRewind()
    {
        playerStats.healOnRewind += 5;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void DropWeapon()
    {
        GunPickup gun = droppableGuns[0];

        //Instantiate(gun, player.position + (Vector3.up * 0.35f), Quaternion.identity).Drop(player.forward * 15f);
        inventoryManager.UnlockNewGun(gun.scriptableObject);

        droppableGuns.RemoveAt(0);

        ResumeGameplay();
    }

    void IncreaseFirerate()
    {
        playerStats.fireRateMultiplier -= 0.05f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void ReduceRewindCooldown()
    {
        playerStats.rewindCooldownReduction += 0.5f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseDoubleDamageChance()
    {
        playerStats.chanceToDoubleDamage += 2f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseAdditionalProjectiles()
    {
        playerStats.additionalProjectiles += 1;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    List<Reward> RandomRewards()
    {
        List<Reward> rewards = new List<Reward>();
        List<int> takenIndexes = new List<int>();

        bool guaranteedGunGotten = false;
        while (rewards.Count < 3)
        {
            int randomIndex = Random.Range(1, 10);
            if (currentReward % 3 == 0 && !guaranteedGunGotten)
            {
                guaranteedGunGotten = true;
                randomIndex = 2;
            }else
            {
                while(takenIndexes.Contains(randomIndex))
                {
                    randomIndex = Random.Range(1, 10);
                }
            }
            takenIndexes.Add(randomIndex);

            if (randomIndex == 1 && playerStats.speedMultiplier < maxMoveSpeed)
            {
                Action methodToCall;
                rewards.Add(new Reward(moveSpeedRewardString, methodToCall = IncreaseMoveSpeed));
            }
            else if (randomIndex == 2 && droppableGuns.Count > 0)
            {
                GunPickup droppedGun = droppableGuns[0];
                string message = newWeaponRewardString + droppedGun.name;
                Action methodToCall;
                rewards.Add(new Reward(message, methodToCall = DropWeapon));
            }
            else if (randomIndex == 3)
            {
                Action methodToCall;
                rewards.Add(new Reward(magSizeRewardString, methodToCall = IncreaseMagSize));
            }
            else if (randomIndex == 4)
            {
                Action methodToCall;
                rewards.Add(new Reward(damageRewardString, methodToCall = IncreaseDamage));
            }
            else if (randomIndex == 5 && playerStats.healOnRewind == 0)
            {
                Action methodToCall;
                rewards.Add(new Reward(healRewardString, methodToCall = IncreaseHealOnRewind));
            }
            else if (randomIndex == 6 && playerStats.fireRateMultiplier > 0.5f)
            {
                Action methodToCall;
                rewards.Add(new Reward(firerateRewardString, methodToCall = IncreaseFirerate));
            }
            else if (randomIndex == 7 && playerStats.rewindCooldownReduction < 1f)
            {
                Action methodToCall;
                rewards.Add(new Reward(rewindRewardString, methodToCall = ReduceRewindCooldown));
            }
            else if (randomIndex == 8 && playerStats.chanceToDoubleDamage < 8)
            {
                Action methodToCall;
                rewards.Add(new Reward(doubleDamageRewardString, methodToCall = IncreaseDoubleDamageChance));
            }
            else if (randomIndex == 9 && playerStats.additionalProjectiles < 2)
            {
                Action methodToCall;
                rewards.Add(new Reward(additionalProjectileRewardString, methodToCall = IncreaseAdditionalProjectiles));
            }
        }

        return rewards;
    }
}