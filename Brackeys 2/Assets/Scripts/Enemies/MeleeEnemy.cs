using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    public EntityStats stats;

    public NavMeshAgent navMesh;
    public Animator anim;

    public MeleeWeapon weapon;
    public float minDistanceToAttack = 2f;

    public float rotSpeed = 7f;

    Transform player;
    bool isAttacking;

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var meshHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = meshHit.position;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        weapon.damage = stats.damage;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up, player.transform.position);

        if(isAttacking)
        {
            var lookdir = (player.position - (transform.position + Vector3.up));
            lookdir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookdir), Time.deltaTime * rotSpeed);

            anim.SetFloat("Speed", 0);

            return;
        }

        if(distanceToPlayer <= minDistanceToAttack && !isAttacking)
        {
            if (!navMesh.isStopped)
            {
                navMesh.ResetPath();
                navMesh.isStopped = true;
                navMesh.velocity = Vector3.zero;
            }

            isAttacking = true;
            anim.SetTrigger("Attack");

            Invoke(nameof(StopAttacking), 1.3f);
        }else if (distanceToPlayer > minDistanceToAttack && !isAttacking)
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(player.position);

            anim.SetFloat("Speed", navMesh.velocity.magnitude);
        }
    }

    void StopAttacking()
    {
        isAttacking = false;
    }

    public void EnableWeapon()
    {
        weapon.EnableCollider();
    }

    public void DisableWeapon()
    {
        weapon.DisableCollider();
    }

    private void OnDisable()
    {
        weapon.gameObject.SetActive(false);

        navMesh.ResetPath();
        navMesh.isStopped = true;
    }
}
