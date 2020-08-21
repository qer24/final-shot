using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrace : MonoBehaviour
{
    public EntityStats playerStats;
    public Damagable damagable;
    float graceTimer;

    public static bool grace = false;

    private void OnEnable()
    {
        damagable.OnTakeDamage += GraceOnHit;
        RewardsManager.OnRewardChosen += GraceAfterReward;
    }

    private void OnDisable()
    {
        damagable.OnTakeDamage -= GraceOnHit;
        RewardsManager.OnRewardChosen -= GraceAfterReward;
    }

    void GraceAfterReward()
    {
        AddGrace(1f);
    }

    void GraceOnHit()
    {
        AddGrace(playerStats.graceOnHit);
    }

    public void AddGrace(float time)
    {
        graceTimer += time;
    }

    // Update is called once per frame
    void Update()
    {
        if (graceTimer > 0)
        {
            grace = true;

            damagable.enabled = false;
            graceTimer -= Time.deltaTime;
        }else
        {
            grace = false;

            damagable.enabled = true;
            graceTimer = 0;
        }
    }
}
