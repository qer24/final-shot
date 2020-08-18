using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    public EntityStats stats;
    public Bullet bullet;
    public float bulletSpeed = 20f;

    public NavMeshAgent navMesh;
    public Animator anim;

    public float minDistanceToStop = 10f;

    public float rotSpeed = 7f;

    public float timeBetweenShots = 2f;
    public Transform PistolPivot;
    public Transform[] shootPositions;

    Transform player;

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var meshHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = meshHit.position;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(Shoot), timeBetweenShots, timeBetweenShots);
    }

    void Shoot()
    {
        AudioManager.Play("shootEnemy", transform.position);
        for (int i = 0; i < shootPositions.Length; i++)
        {
            var newBullet = Instantiate(bullet, shootPositions[i].position, shootPositions[i].rotation);
            newBullet.Init(stats.damage, transform, -newBullet.transform.right * bulletSpeed, true);
        }
    }

    private void Update()
    {
        PistolPivot.Rotate(0, 60f * Time.deltaTime, 0);

        float distanceToPlayer = Vector3.Distance(transform.position + Vector3.up, player.transform.position);

        if (distanceToPlayer <= minDistanceToStop)
        {
            var lookdir = (player.position - (transform.position + Vector3.up));
            lookdir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookdir), Time.deltaTime * rotSpeed);

            anim.SetFloat("Speed", 0);

            if (!navMesh.isStopped)
            {
                navMesh.ResetPath();
                navMesh.isStopped = true;
            }
        }
        else if (distanceToPlayer > minDistanceToStop)
        {
            navMesh.isStopped = false;
            navMesh.SetDestination(player.position);

            anim.SetFloat("Speed", navMesh.velocity.magnitude);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < shootPositions.Length; i++)
        {
            shootPositions[i].gameObject.SetActive(false);
        }

        CancelInvoke();

        navMesh.ResetPath();
        navMesh.isStopped = true;
    }
}
