using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class SniperEnemy : MonoBehaviour
{
    public Animator anim;
    public Bullet bullet;
    public LayerMask mask;
    public float rotSpeed = 4f;
    public float catchupSpeed = 2f;
    public EntityStats stats;
    public float timeToShoot = 5f;
    public Transform shootPoint;
    public Transform torsoPoint;
    public LineRenderer laser;
    public float bulletSpeed = 50f;

    public LayerMask whatIsGround;
    public NavMeshAgent navMesh;
    public float minDistanceToRun = 15f;
    public Rig aimRig;

    Transform player;

    Vector3 lastPlayerPos;
    Vector3 dirToPlayer;
    bool canShoot;
    float shootTimer;

    int currentFrame;

    Vector3 currentFurhestPoint;
    bool isRunningAway = false;

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var meshHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = meshHit.position;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = 0;

        currentFurhestPoint = new Vector3(-3.25f, 5.25f, 0f);
        InvokeRepeating(nameof(FindFurthestPointFromPlayer), 0f, 5f);
    }

    void FindFurthestPointFromPlayer()
    {
        List<Vector3> positions = new List<Vector3>();

        while (positions.Count < 10)
        {
            Vector3 point = new Vector3(Random.Range(-80, 80), 40, Random.Range(-80, 80));

            if (Physics.Raycast(point, Vector3.down, out var hit, Mathf.Infinity, whatIsGround))
            {
                if (NavMesh.SamplePosition(hit.point, out var meshHit, 1.0f, NavMesh.AllAreas))
                {
                    positions.Add(meshHit.position);
                }
            }
        }

        foreach (var position in positions)
        {
            if(Vector3.Distance(position, player.position) > Vector3.Distance(currentFurhestPoint, player.position))
            {
                currentFurhestPoint = position + Vector3.up;
            }
        }
    }

    void Update()
    {
        if (!PlayerRewind.isRewinding && !GameManager.isPlayerDead)
        {
            if (isRunningAway)
            {
                aimRig.weight = 0;

                navMesh.isStopped = false;
                navMesh.SetDestination(currentFurhestPoint);

                anim.SetFloat("Speed", navMesh.velocity.magnitude);
            }else
            {
                anim.SetFloat("Speed", 0);
                aimRig.weight = 1;
            }

            if (Vector3.Distance(player.position, transform.position) < minDistanceToRun && !isRunningAway)
            {
                anim.SetBool("PlayerInSight", false);

                isRunningAway = true;

                canShoot = false;
                shootTimer = timeToShoot * 0.5f - 0.5f;
                laser.enabled = false;
                currentFrame = 30;

                Invoke(nameof(StopRunningAway), 3f);
            }

            if(!isRunningAway)
            {
                if (!navMesh.isStopped)
                {
                    navMesh.ResetPath();
                    navMesh.isStopped = true;
                }

                RotateTowardsPlayer();

                if (shootTimer >= timeToShoot * 0.5f && canShoot)
                {
                    shootTimer += Time.deltaTime;
                    laser.enabled = true;
                }
                else if (shootTimer < timeToShoot * 0.5f && canShoot)
                {
                    shootTimer += Time.deltaTime;
                    laser.enabled = false;
                }
                else if (!canShoot)
                {
                    shootTimer = 0;
                }

                if (shootTimer >= timeToShoot)
                {
                    shootTimer = 0;
                    Shoot();
                }
            }

            torsoPoint.position = Vector3.Lerp(torsoPoint.position, lastPlayerPos, Time.deltaTime * catchupSpeed);
        }
        else
        {
            anim.SetBool("PlayerInSight", false);
        }
    }

    private void StopRunningAway()
    {
        var lookdir = (lastPlayerPos - shootPoint.position).normalized;
        lookdir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookdir);
        isRunningAway = false;
    }

    private void Shoot()
    {
        AudioManager.Play("shootEnemySniper", transform.position);
        anim.SetTrigger("Shoot");

        var newBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        newBullet.Init(stats.damage, transform, -newBullet.transform.right * bulletSpeed, true);
    }

    private void RotateTowardsPlayer()
    {
        dirToPlayer = (player.position - (transform.position + Vector3.up * 2));

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 2, dirToPlayer);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                canShoot = true;
                anim.SetBool("PlayerInSight", true);
            }
            else
            {
                shootTimer = timeToShoot * 0.5f - 0.5f;
                laser.enabled = false;

                currentFrame = 30;

                anim.SetBool("PlayerInSight", false);
            }
        }

        if (currentFrame >= 30)
        {
            lastPlayerPos = player.position;
            currentFrame = 1;
        }
        else
        {
            currentFrame++;
        }

        var lookdir = (lastPlayerPos - shootPoint.position).normalized;
        lookdir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookdir), Time.deltaTime * rotSpeed);
    }

    private void OnDisable()
    {
        laser.enabled = false;

        navMesh.ResetPath();
        navMesh.isStopped = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToRun);
        Gizmos.DrawWireSphere(currentFurhestPoint, 3f);
    }
}
