using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuicideEnemy : MonoBehaviour
{
    public EntityStats stats;
    public Health hp;

    public NavMeshAgent navMesh;
    public Animator anim;

    public ParticleSystem explosionParticles;
    public float minDistanceToExplode = 2f;
    public LayerMask damagables;
    public float explosionRadius = 4f;

    public Collider headshotCollider;

    public float rotSpeed = 7f;

    Transform player;
    bool isExploding;

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var meshHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = meshHit.position;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        hp.OnDeath += Explode;

        InvokeRepeating(nameof(TickSound), 0f, 0.5f);
    }

    void TickSound()
    {
        AudioManager.Play("granadeTick", transform.position);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up, player.transform.position);

        if (isExploding)
        {
            var lookdir = (player.position - (transform.position + Vector3.up));
            lookdir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookdir), Time.deltaTime * rotSpeed);

            anim.SetFloat("Speed", 0);

            return;
        }

        if (distanceToPlayer <= minDistanceToExplode && !isExploding)
        {
            if (!navMesh.isStopped)
            {
                navMesh.ResetPath();
                navMesh.isStopped = true;
            }

            isExploding = true;
            anim.SetTrigger("Explode");

            Invoke(nameof(Explode), 0.8f);
        }
        else if (distanceToPlayer > minDistanceToExplode && !isExploding)
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(player.position);

            anim.SetFloat("Speed", navMesh.velocity.magnitude);
        }
    }

    void Explode()
    {
        hp.OnDeath -= Explode;

        AudioManager.Play("granadeExplosion", transform.position);

        isExploding = true;
        explosionParticles.transform.parent = null;
        explosionParticles.Play();

        headshotCollider.enabled = false;
        DamageEntities();

        Destroy(gameObject);
    }

    private void DamageEntities()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, explosionRadius, damagables);

        foreach (var col in collidersInRange)
        {
            if (col.transform == transform) continue;

            if (col.TryGetComponent<IDamagable>(out var damagable))
            {
                if (col.CompareTag("Player"))
                {
                    CameraShaker.Instance.ShakeOnce(20f, 4f, 0.1f, 1f);
                    if (PlayerRewind.isRewinding) continue;
                }
                damagable.TakeDamage(stats.damage);
            }
        }
    }

    private void OnDisable()
    {
        CancelInvoke();

        navMesh.ResetPath();
        navMesh.isStopped = true;

        hp.OnDeath -= Explode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
