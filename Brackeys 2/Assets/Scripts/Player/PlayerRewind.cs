using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerRewind : MonoBehaviour
{
    public EntityStats playerStats;

    public float recordTime = 5f;
    public Health hp;
    //public PlayerController playerController;
    public PlayerShooter playerShooter;
    public CharacterController characterController;
    public Damagable damagable;
    public PlayerGrace grace;

    List<PlayerPointInTime> pointsInTime;
    public static bool isRewinding = false;
    public float rewindCooldown;
    public static float rewindCooldownRemaining;

    int currentFrame;
    float health;

    void Start()
    {
        pointsInTime = new List<PlayerPointInTime>();
        rewindCooldownRemaining = rewindCooldown;
        currentFrame = 1;
    }

    void Update()
    {
        if (!RewardsManager.isChoosingReward && !GameManager.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.F) && !isRewinding && (rewindCooldownRemaining) < 0)
                StartRewind();
        }
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            rewindCooldownRemaining -= Time.fixedDeltaTime;
            Record();
        }
    }

    void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PlayerPointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            //transform.rotation = pointInTime.rotation;
            health = pointInTime.health;
            //hp.RestoreHealth(pointInTime.health - hp.currentHealth);
            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }

    void Record()
    {
        if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        currentFrame++;
        if(currentFrame >= 3)
        {
            currentFrame = 1;
            pointsInTime.Insert(0, new PlayerPointInTime(transform.position, transform.rotation, hp.currentHealth));
        }
    }

    public void StartRewind()
    {
        AudioManager.Play("rewind");

        rewindCooldownRemaining = rewindCooldown - playerStats.rewindCooldownReduction;
        isRewinding = true;

        //playerShooter.enabled = false;
        //playerController.enabled = false;
        characterController.enabled = false;
        damagable.enabled = false;
    }

    public void StopRewind()
    {
        isRewinding = false;

        //playerShooter.enabled = true;
        //playerController.enabled = true;
        characterController.enabled = true;
        damagable.enabled = true;

        if (health - hp.currentHealth > 0)
        {
            hp.RestoreHealth((health - hp.currentHealth));
        }

        hp.RestoreHealth(playerStats.healOnRewind);

        grace.AddGrace(playerStats.graceOnRewind);

        playerShooter.rewindBonusDamageBullets += playerStats.bonusBulletsOnRewind;

        if (playerShooter.currentGun != null)
        {
            playerShooter.currentAmmo = playerShooter.maxAmmo;
            playerShooter.currentGun.currentAmmo = playerShooter.maxAmmo;
            PlayerShooter.OnAmmoChanged?.Invoke(playerShooter.AmmoCount);
        }
    }
}
