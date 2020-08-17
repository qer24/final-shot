using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun")]
public class Gun : ScriptableObject
{
    [HideInInspector]
    public Transform transform;
    [HideInInspector]
    public GunPickup pickupScript;

    public string shootSound = "shootPistol";

    [Header("Stats")]
    public float shootCooldown = 0.5f;
    public float damage = 20;
    public int baseAmmo = 15;
    public float bulletSpeed = 20f;
    public bool holdToFire = false;
    public bool knockup = true;
    public float visualHorizontalRecoilMulti = 0.5f;
    public float maxRecoil;
    public int bullets = 1;
    public float shakeAmount = 1f;
    public float rotationSlowDownFactor = 1f;
    public bool isHitscan = false;
    public GameObject projectile;   

    [HideInInspector]
    public int currentAmmo;
}
