using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using EZCameraShake;
using Lean.Pool;

public class PlayerShooter : MonoBehaviour
{
    public EntityStats playerStats;

    [Space]

    public PlayerController playerController;

    public delegate void AmmoAction(string ammoCount);
    public static AmmoAction OnAmmoChanged;

    public Camera playerCamera;
    public Transform gunPivot;
    public Gun currentGun;

    public Vector3 rotationOnShoot;

    public float maxPickupDistance = 2f;

    public float dropForce;

    public Transform shootPoint;
    public LayerMask ignorePlayerMask;
    public Bullet bullet;
    public BulletHole bulletHole;
    public BulletTrace bulletTrace;
    public GameObject enemyHitParticles;

    Crosshair crosshair;

    public delegate void PickupAction(Gun pickedUpGun);
    public PickupAction onPickedUpGun;

    float shootTimer = 0f;
    Quaternion startRotation;
    Vector3 startPosition;
    GunPickup currentlyHighlightedGun;
    bool isPickingUpGun;
    Vector3 gunPivotPosLastFrame;

    [HideInInspector] public int maxAmmo = 0;
    [HideInInspector] public int currentAmmo = 0;

    public string AmmoCount { get => $"{currentAmmo}/{maxAmmo}"; }

    public int rewindBonusDamageBullets = 0;

    private void Start()
    {
        startRotation = gunPivot.localRotation;
        startPosition = gunPivot.localPosition;

        isPickingUpGun = false;
        playerStats.onStatsChanged += UpdateStats;

        crosshair = Crosshair.instance;
    }

    void Update()
    {
        if (GameManager.isPaused || RewardsManager.isChoosingReward) return;

        gunPivot.localPosition = Vector3.Lerp(gunPivot.transform.localPosition, startPosition, Time.deltaTime * 5f);
        if(currentGun != null)
        {
            gunPivot.localRotation = Quaternion.Slerp(gunPivot.localRotation, startRotation, Time.deltaTime * 5f * currentGun.rotationSlowDownFactor);
        }else
        {
            gunPivot.localRotation = Quaternion.Slerp(gunPivot.localRotation, startRotation, Time.deltaTime * 5f);
        }

        gunPivotPosLastFrame = gunPivot.position;

        if (PlayerRewind.isRewinding) return;

        DetectPickups();

        if (currentlyHighlightedGun != null)
        {
            if (Input.GetButtonDown("Pickup"))
            {
                /*
                if (currentGun != null)
                {
                    DropCurrentGun();
                }
                */

                onPickedUpGun?.Invoke(currentlyHighlightedGun.scriptableObject);
                Destroy(currentlyHighlightedGun.gameObject);

                //StopAllCoroutines();
                //StartCoroutine(PickupGun(currentlyHighlightedGun.transform));
            }
        }

        if (currentGun != null && !isPickingUpGun)
        {
            /*
            if(Input.GetButtonDown("Drop"))
            {
                DropCurrentGun();

                return;
            }
            */

            shootTimer -= Time.deltaTime;

            bool fire = currentGun.holdToFire ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

            if (fire && shootTimer < 0 && currentAmmo > 0)
            {
                shootTimer = currentGun.shootCooldown * playerStats.fireRateMultiplier;
                Shoot();
            }
            if (fire && shootTimer < 0 && currentAmmo == 0)
            {
                shootTimer = currentGun.shootCooldown * playerStats.fireRateMultiplier;
                AudioManager.Play("emptyMagazine");
            }
        }
    }

    private void DetectPickups()
    {
        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.distance < maxPickupDistance)
            {
                hit.collider.TryGetComponent<GunPickup>(out var gun);
                if (gun == null && currentlyHighlightedGun != null)
                {
                    currentlyHighlightedGun.ToggleHightlight(false);
                    currentlyHighlightedGun = null;
                }
                else if (gun != null)
                {
                    if(currentlyHighlightedGun != null)
                    {
                        currentlyHighlightedGun.ToggleHightlight(false);
                        currentlyHighlightedGun = null;
                    }

                    gun.ToggleHightlight(true);
                    currentlyHighlightedGun = gun;
                }
            }else if (currentlyHighlightedGun != null)
            {
                currentlyHighlightedGun.ToggleHightlight(false);
                currentlyHighlightedGun = null;
            }
        }else if(currentlyHighlightedGun != null)
        {
            currentlyHighlightedGun.ToggleHightlight(false);
            currentlyHighlightedGun = null;
        }

        crosshair.SetCrosshair(currentlyHighlightedGun != null);
    }

    public IEnumerator PickupGun(Transform pickedUpGun)
    {
        AudioManager.Play("equipGun");

        maxAmmo = Mathf.RoundToInt(currentGun.baseAmmo * playerStats.magazineMultiplier);
        currentAmmo = currentGun.currentAmmo;
        OnAmmoChanged?.Invoke(AmmoCount);

        isPickingUpGun = true;

        //var gunPickupScript = pickedUpGun.GetComponent<GunPickup>();
        //gunPickupScript.Pickup();
        //currentGun = gunPickupScript.scriptableObject;

        //pickedUpGun.gameObject.layer = 8;
        //pickedUpGun.GetComponentInChildren<MeshRenderer>().gameObject.layer = 8;

        pickedUpGun.transform.position = gunPivot.position - Vector3.up;

        float lerpMulti = 1f;
        float timeOutTimer = 0f;
        while (Vector3.Distance(pickedUpGun.position, gunPivot.position) > 0.05f)
        {
            pickedUpGun.position = Vector3.Slerp(pickedUpGun.position, gunPivot.position, Time.deltaTime * 8f * lerpMulti);
            pickedUpGun.rotation = Quaternion.Slerp(pickedUpGun.rotation, gunPivot.rotation, Time.deltaTime * 15f);
            lerpMulti += 0.05f;

            timeOutTimer += Time.deltaTime;
            if(timeOutTimer > 1f)
            {
                break;
            }

            yield return null;
        }

        //pickedUpGun.parent = gunPivot;
        pickedUpGun.localPosition = Vector3.zero;
        pickedUpGun.localRotation = Quaternion.identity;
        isPickingUpGun = false;

        yield return null;
    }

    private void Shoot()
    {
        AudioManager.Play(currentGun.shootSound);
        CameraShaker.Instance.ShakeOnce(currentGun.shakeAmount, 2f, 0.1f, 0.5f, Vector3.zero, Vector3.one);

        gunPivot.localPosition = startPosition;
        gunPivot.localPosition -= Vector3.forward * currentGun.visualHorizontalRecoilMulti;

        if (currentGun.knockup)
        {
            gunPivot.localRotation = startRotation;

            gunPivot.localRotation = Quaternion.Euler(rotationOnShoot);
        }

        currentAmmo = Mathf.Max(0, currentAmmo - 1);
        currentGun.currentAmmo = currentAmmo;
        OnAmmoChanged?.Invoke(AmmoCount);

        int bullets = currentGun.bullets + (Random.value <= 0.1f ? playerStats.additionalProjectiles : 0);

        for (int i = 0; i < bullets; i++)
        {
            float damage = CalculateDamage();

            Vector3 gunRecoil = new Vector3(
                Random.Range(-currentGun.maxRecoil, currentGun.maxRecoil),
                Random.Range(-currentGun.maxRecoil, currentGun.maxRecoil),
                Random.Range(-currentGun.maxRecoil, currentGun.maxRecoil))
                * 0.01f;

            if (playerController.isCrouching || playerController.isSliding) gunRecoil *= 0.45f;

            if (currentGun.isHitscan)
            {
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward + gunRecoil);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignorePlayerMask))
                {
                    //Debug.Log(hit.distance);
                    //Debug.DrawRay(playerCamera.transform.position, (playerCamera.transform.forward + gunRecoil) * hit.distance, Color.red, 999f);

                    if (hit.collider.CompareTag("Enemy"))
                    {
                        var ragdoll = hit.collider.GetComponent<RagdollDeath>();
                        if (ragdoll == null) ragdoll = hit.collider.GetComponentInParent<RagdollDeath>();
                        if (ragdoll != null) ragdoll.hitDirection = hit.collider.transform.position - transform.position;
                        var droneRagdoll = hit.collider.GetComponent<DroneDeath>();
                        if (droneRagdoll == null) droneRagdoll = hit.collider.GetComponentInParent<DroneDeath>();
                        if (droneRagdoll != null) droneRagdoll.hitDirection = hit.collider.transform.position - transform.position;

                        hit.collider.TryGetComponent<IDamagable>(out var damagable);
                        if (damagable != null)
                        {
                            float falloffDamageMulti = currentGun.damageFalloffCurve.Evaluate(hit.distance);
                            damagable.TakeDamage(damage * falloffDamageMulti);
                        }

                        LeanPool.Spawn(enemyHitParticles, hit.point - (hit.normal * 0.025f), Quaternion.identity).transform.LookAt(transform.position);
                    }
                    else if (hit.collider.gameObject.layer == 0) //Default layer, only obstacles there
                    {
                        AudioManager.Play("wallImpact", hit.point);
                        var hole = LeanPool.Spawn(bulletHole, hit.point + (hit.normal * 0.025f), Quaternion.identity);
                        hole.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                        hole.transform.localScale = bulletHole.transform.localScale;
                        LeanPool.Despawn(hole, 5f);
                    }

                    if (hit.distance < 100f && hit.distance > 0.5f)
                    {
                        LeanPool.Spawn(bulletTrace, shootPoint.position - shootPoint.transform.forward, Quaternion.identity).Init(hit.point - shootPoint.position, hit.point, shootPoint.position);
                    }
                }else
                {
                    LeanPool.Spawn(bulletTrace, shootPoint.position - shootPoint.transform.forward, Quaternion.identity).Init(ray.GetPoint(100f) - shootPoint.position, ray.GetPoint(100f), shootPoint.position);
                }
            }else
            {
                Vector3 lookPoint;
                var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignorePlayerMask))
                {
                    lookPoint = hit.point;
                }
                else
                {
                    if (!currentGun.holdToFire)
                    {
                        lookPoint = ray.GetPoint(100f);
                    }
                    else
                    {
                        lookPoint = ray.GetPoint(30f);
                    }
                }

                var projectile = LeanPool.Spawn(currentGun.projectile, shootPoint.position - shootPoint.transform.forward, Quaternion.identity);
                //lookPoint += gunRecoil;
                projectile.transform.LookAt(lookPoint);
                projectile.transform.Rotate(gunRecoil);
                projectile.transform.position = shootPoint.position;
                if (projectile.TryGetComponent<Bullet>(out var newBullet))
                {
                    newBullet.Init(damage, transform, projectile.transform.forward * currentGun.bulletSpeed);
                }
                else if (projectile.TryGetComponent<GranadeBullet>(out var newGranade))
                {
                    newGranade.Init(damage, transform, projectile.transform.forward * currentGun.bulletSpeed);
                }
            }
        }

        /*
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                var ragdoll = hit.collider.GetComponent<RagdollDeath>();
                if (ragdoll == null) ragdoll = hit.collider.GetComponentInParent<RagdollDeath>();
                if (ragdoll != null) ragdoll.hitDirection = hit.transform.position - transform.position;

                hit.collider.TryGetComponent<IDamagable>(out var damagable);
                if (damagable != null)
                {
                    damagable.TakeDamage(currentGun.damage);
                }
            }
        }
        */
    }

    private float CalculateDamage()
    {
        float damage;

        if(Random.Range(0.00f, 1.00f) <= playerStats.chanceToDoubleDamage * 0.01f)
        {
            damage = currentGun.damage * playerStats.damageMultiplier * 2;
        }else
        {
            damage = currentGun.damage * playerStats.damageMultiplier;
        }

        if(rewindBonusDamageBullets > 0)
        {
            rewindBonusDamageBullets--;
            damage *= 1.5f;
        }

        return damage;
    }

    void UpdateStats()
    {
        if (currentGun != null)
        {
            maxAmmo = Mathf.RoundToInt(currentGun.baseAmmo * playerStats.magazineMultiplier);
            currentGun.currentAmmo = maxAmmo;
            currentAmmo = currentGun.currentAmmo;
            OnAmmoChanged?.Invoke(AmmoCount);
        }
    }
}
