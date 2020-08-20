using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Entity Stats")]
public class EntityStats : ScriptableObject
{
    public int maxHealth;
    public int damage;

    [Header("Player only stats")]
    public float damageMultiplier = 1;
    public float speedMultiplier = 1;
    public float magazineMultiplier = 1;
    public float fireRateMultiplier = 1;
    public float rewindCooldownReduction = 0;
    public float healOnRewind = 0;
    public float chanceToDoubleDamage = 0;
    public int additionalProjectiles = 0;

    public Action onStatsChanged;
}
