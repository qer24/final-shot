using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Rigidbody rb;
    public Collider col;
    public Target indicator;
    public float minIndicatorDistance = 5f;
    public GameObject gfx;
    public float timeToExplode;
    public ParticleSystem particles;
    public float radius = 0.55f;
    public float damage = 30f;

    public LayerMask damagables;

    Vector3 target;
    Transform player;

    bool isBlinking;
    bool isExploding;

    private void Start()
    {
        var shape = particles.shape;
        shape.radius = radius * 0.2f;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(TickSound), 0f, 0.5f);
        Init();
    }

    public void Init()
    {
        StartCoroutine(SimulateProjectile());
    }

    private void Update()
    {
        if (isExploding) return;

        if(Vector3.Distance(player.position, transform.position) < minIndicatorDistance)
        {
            if (!isBlinking)
            {
                isBlinking = true;
                InvokeRepeating(nameof(BlinkIndicator), 0f, 0.3f);
            }
        }else
        {
            //gfx.SetActive(true);
            isBlinking = false;
            CancelInvoke(nameof(BlinkIndicator));
            indicator.enabled = false;
        }
    }

    void TickSound()
    {
        AudioManager.Play("granadeTick", transform.position);
    }

    void BlinkIndicator()
    {
        //gfx.SetActive(!gfx.activeSelf);
        indicator.enabled = !indicator.enabled;
    }

    IEnumerator SimulateProjectile()
    {
        target = player.position;

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;
        flightDuration *= 0.95f;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target - transform.position);

        rb.isKinematic = false;
        rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
        col.enabled = true;

        Invoke(nameof(Explode), timeToExplode);
    }

    void Explode()
    {
        AudioManager.Play("granadeExplosion", transform.position);

        isExploding = true;
        particles.transform.LookAt(particles.transform.position + Vector3.up);
        particles.Play();

        gfx.SetActive(false);
        CancelInvoke();
        indicator.enabled = false;

        DamageEntities();

        Destroy(gameObject, 3f);
    }

    private void DamageEntities()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, radius, damagables);

        foreach (var col in collidersInRange)
        {
            if(col.TryGetComponent<IDamagable>(out var damagable))
            {
                if(col.CompareTag("Player"))
                {
                    CameraShaker.Instance.ShakeOnce(20f, 4f, 0.1f, 1f);
                    if (PlayerRewind.isRewinding) continue;
                }
                damagable.TakeDamage(damage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Play("wallImpact", transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
