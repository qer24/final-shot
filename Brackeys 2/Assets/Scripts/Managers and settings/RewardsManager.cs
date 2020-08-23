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
    string healRewardString = "Restore 4 health after rewind";
    string firerateRewardString = "+5% fire rate";
    string rewindRewardString = "-0.5s to rewind cooldown";
    string doubleDamageRewardString = "+2% chance to deal double damage";
    string projectileRewardString { get => $"10% chance to fire +{playerStats.additionalProjectiles + 1} bullet{(playerStats.additionalProjectiles > 0 ? 's' : char.MinValue)}"; }
    string graceOnHitRewardString = "+0.1s immunity after getting hit";
    string graceOnRewindRewardString = "+0.5s immunity after rewind";
    string bonusDamageOnRewindString { get => $"After rewind, 150% damage for {playerStats.bonusBulletsOnRewind + 1} bullet{(playerStats.bonusBulletsOnRewind > 0 ? 's' : char.MinValue)}"; }

    int currentReward = 0;

    public static Action OnRewardChosen;

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
        playerStats.graceOnHit = 0.2f;
        playerStats.graceOnRewind = 0f;
        playerStats.bonusBulletsOnRewind = 0;

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            DisplayRewards();
        }
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
        OnRewardChosen?.Invoke();

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
        playerStats.healOnRewind += 4;
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

    void IncreaseGraceOnHit()
    {
        playerStats.graceOnHit += 0.1f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseGraceOnRewind()
    {
        playerStats.graceOnRewind += 0.5f;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    void IncreaseBonusBulletsOnRewind()
    {
        playerStats.bonusBulletsOnRewind += 1;
        playerStats.onStatsChanged?.Invoke();

        ResumeGameplay();
    }

    List<Reward> RandomRewards()
    {
        List<Reward> rewards = new List<Reward>();
        List<int> takenIndexes = new List<int>();

        bool guaranteedGunGotten = false;
        int iterations = 0;
        while (rewards.Count < 3)
        {
            Action methodToCall;
            int randomIndex = Random.Range(1, 13);
            if (currentReward % 3 == 0 && !guaranteedGunGotten)
            {
                guaranteedGunGotten = true;
                randomIndex = 2;
            }else
            {
                while(takenIndexes.Contains(randomIndex) || (randomIndex == 5 && currentReward < 5)) //cant roll heal on rewind until reward 5
                {
                    iterations++;
                    if (iterations > 100)
                    {
                        randomIndex = 4;
                        break;
                    }
                    randomIndex = Random.Range(1, 13);
                }
            }
            takenIndexes.Add(randomIndex);

            if (randomIndex == 1 && playerStats.speedMultiplier < maxMoveSpeed)
            {
                rewards.Add(new Reward(moveSpeedRewardString, methodToCall = IncreaseMoveSpeed));
            }
            else if (randomIndex == 2 && droppableGuns.Count > 0)
            {
                GunPickup droppedGun = droppableGuns[0];
                string message = newWeaponRewardString + droppedGun.name;
                rewards.Add(new Reward(message, methodToCall = DropWeapon));
            }
            else if (randomIndex == 3)
            {
                rewards.Add(new Reward(magSizeRewardString, methodToCall = IncreaseMagSize));
            }
            else if (randomIndex == 4)
            {
                rewards.Add(new Reward(damageRewardString, methodToCall = IncreaseDamage));
            }
            else if (randomIndex == 5 && playerStats.healOnRewind == 0)
            {
                rewards.Add(new Reward(healRewardString, methodToCall = IncreaseHealOnRewind));
            }
            else if (randomIndex == 6 && playerStats.fireRateMultiplier > 0.5f)
            {
                rewards.Add(new Reward(firerateRewardString, methodToCall = IncreaseFirerate));
            }
            else if (randomIndex == 7 && playerStats.rewindCooldownReduction < 1f)
            {
                rewards.Add(new Reward(rewindRewardString, methodToCall = ReduceRewindCooldown));
            }
            else if (randomIndex == 8 && playerStats.chanceToDoubleDamage < 8)
            {
                rewards.Add(new Reward(doubleDamageRewardString, methodToCall = IncreaseDoubleDamageChance));
            }
            else if (randomIndex == 9 && playerStats.additionalProjectiles < 2)
            {
                rewards.Add(new Reward(projectileRewardString, methodToCall = IncreaseAdditionalProjectiles));
            }
            else if (randomIndex == 10 && playerStats.graceOnHit < 0.5f)
            {
                rewards.Add(new Reward(graceOnHitRewardString, methodToCall = IncreaseGraceOnHit));
            }
            else if (randomIndex == 11 && playerStats.graceOnRewind < 1f)
            {
                rewards.Add(new Reward(graceOnRewindRewardString, methodToCall = IncreaseGraceOnRewind));
            }
            else if (randomIndex == 12 && playerStats.bonusBulletsOnRewind < 3)
            {
                rewards.Add(new Reward(bonusDamageOnRewindString, methodToCall = IncreaseBonusBulletsOnRewind));
            }
        }

        return rewards;
    }
}