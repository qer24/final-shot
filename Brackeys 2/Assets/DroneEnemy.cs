using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    public EntityStats stats;

    public float floatingHeight = 4;
    public float speed = 5;
    public float stoppingDistance = 10;
    public float shootingDistance = 16;

    public Transform shootPoint;
    public Bullet bullet;
    public float bulletSpeed = 30;
    public LayerMask shootingMask;
    public float shootCooldown = 1f;
    public float timeBetweenShots = 0.3f;
    public float maxRecoil = 3;
    public int bulletCount = 3;

    float shootTimer;

    Transform player;
    Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        shootTimer = shootCooldown;
    }

    void Update()
    {
        transform.LookAt(player.position - Vector3.up);
        shootPoint.LookAt(player);
    }

    void FixedUpdate()
    {
        Vector3 desiredPos = new Vector3(0, (player.position.y - rb.position.y) + floatingHeight, 0);

        Vector3 currentPosXZ = rb.position;
        currentPosXZ.y = 0;

        Vector3 playerPosXZ = player.position;
        playerPosXZ.y = 0;

        if(Vector3.Distance(currentPosXZ, playerPosXZ) > stoppingDistance)
        {
            Vector3 dir = player.position - rb.position;
            desiredPos.x = dir.x;
            desiredPos.z = dir.z;
        }
        if (Vector3.Distance(currentPosXZ, playerPosXZ) < shootingDistance)
        {
            Vector3 dir = player.position - rb.position;
            Ray ray = new Ray(rb.position, dir);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, shootingDistance + 1, shootingMask))
            {
                Debug.DrawRay(transform.position, dir * hit.distance);
                shootTimer -= Time.fixedDeltaTime;

                if(shootTimer < 0)
                {
                    shootTimer = shootCooldown;
                    StartCoroutine(Burst());
                }
            }else
            {
                shootTimer = shootCooldown;
            }
        }else
        {
            shootTimer = shootCooldown;
        }
        //rb.MovePosition(rb.position + desiredPos * Time.fixedDeltaTime * speed * 0.01f);
        rb.AddForce((desiredPos.normalized * speed) - rb.velocity, ForceMode.VelocityChange);
    }

    IEnumerator Burst()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    void Shoot()
    {
        AudioManager.Play("shootDrone", transform.position);

        Vector3 recoil = new Vector3(
                Random.Range(-maxRecoil, maxRecoil),
                Random.Range(-maxRecoil, maxRecoil),
                Random.Range(-maxRecoil, maxRecoil));

        var newBullet = Lean.Pool.LeanPool.Spawn(bullet, shootPoint.position, shootPoint.rotation);
        newBullet.transform.Rotate(recoil);
        newBullet.Init(stats.damage, transform, newBullet.transform.forward * bulletSpeed, true);
    }
}
