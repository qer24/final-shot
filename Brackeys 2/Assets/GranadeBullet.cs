using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeBullet : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem explosionParticles;
    public float radius;
    public LayerMask damagables;

    float damage;
    Transform owner;

    bool ignoreWallCollision = false;

    public void Init(float damage, Transform owner, Vector3 startingVelocity)
    {
        this.damage = damage;
        this.owner = owner;
        GetComponent<Rigidbody>().velocity = startingVelocity;
        Destroy(gameObject, 4f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == owner) return;

        if (other.gameObject.layer == 0 && !ignoreWallCollision) //Default layer, only obstacles there
        {
            AudioManager.Play("wallImpact", transform.position);
            DestroyBullet();
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            var ragdoll = other.GetComponent<RagdollDeath>();
            if (ragdoll == null) ragdoll = other.GetComponentInParent<RagdollDeath>();
            if (ragdoll != null) ragdoll.hitDirection = other.transform.position - owner.transform.position;

            DestroyBullet();
        }
    }

    void Explode()
    {
        AudioManager.Play("granadeExplosion", transform.position);

        var particles = Instantiate(explosionParticles, transform.position, explosionParticles.transform.rotation);
        particles.Play();
        particles.transform.localScale = new Vector3(radius, radius, radius);

        CancelInvoke();

        DamageEntities();
    }

    private void DamageEntities()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, radius, damagables);

        foreach (var col in collidersInRange)
        {
            if (col.TryGetComponent<IDamagable>(out var damagable))
            {
                if (col.CompareTag("Player"))
                {
                    CameraShaker.Instance.ShakeOnce(20f, 4f, 0.1f, 1f);
                    if (PlayerRewind.isRewinding) continue;

                    damagable.TakeDamage(damage * 0.5f);
                    continue;
                }
                damagable.TakeDamage(damage);
            }
        }
    }

    void DestroyBullet()
    {
        Explode();

        Destroy(gameObject);
    }
}
