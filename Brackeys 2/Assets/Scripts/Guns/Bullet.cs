using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public MeshRenderer rend;
    public Collider col;
    public TrailRenderer trail;
    public Material enemyMaterial;

    float damage;
    Transform owner;

    bool ignoreWallCollision = false;

    public void Init(float damage, Transform owner, Vector3 startingVelocity, bool isEnemy = false)
    {
        trail.Clear();

        this.damage = damage;
        this.owner = owner;
        rb.velocity = startingVelocity;

        rend.enabled = true;
        col.enabled = true;

        if (Time.timeSinceLevelLoad > 1f)
            Lean.Pool.LeanPool.Despawn(gameObject, 4f);

        if(isEnemy)
        {
            rend.material = enemyMaterial;
            trail.material = enemyMaterial;
        }else
        {
            ignoreWallCollision = true;
            Invoke(nameof(EnableWallCollision), 0.1f);
        }
    }

    void EnableWallCollision()
    {
        ignoreWallCollision = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == owner) return;

        if (other.gameObject.layer == 0 && !ignoreWallCollision) //Default layer, only obstacles there
        {
            AudioManager.Play("wallImpact", transform.position);
            StartCoroutine(DestroyBullet());
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            var ragdoll = other.GetComponent<RagdollDeath>();
            if (ragdoll == null) ragdoll = other.GetComponentInParent<RagdollDeath>();
            if (ragdoll != null) ragdoll.hitDirection = other.transform.position - owner.transform.position;
        }

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            StartCoroutine(DestroyBullet());

            other.TryGetComponent<IDamagable>(out var damagable);
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
            }
        }
    }

    IEnumerator DestroyBullet()
    {
        rb.velocity = Vector3.zero;
        rend.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(trail.time + 0.1f);

        Lean.Pool.LeanPool.Despawn(gameObject);
    }
}
