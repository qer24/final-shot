using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GranadeEnemy : MonoBehaviour
{
    public EntityStats stats;

    public LayerMask whatIsGround;
    public NavMeshAgent navMesh;
    public Animator anim;

    public Granade granadePrefab;
    public Transform throwPoint;

    public float rotSpeed;

    Transform player;
    Vector3 currentFurhestPoint;
    bool isRunningAway;

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var meshHit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = meshHit.position;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentFurhestPoint = new Vector3(-3.25f, 5.25f, 0f);
        InvokeRepeating(nameof(FindFurthestPointFromPlayer), 0f, 5f);
        InvokeRepeating(nameof(Throw), 0.5f, 4f);
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
            if (Vector3.Distance(position, player.position) > Vector3.Distance(currentFurhestPoint, player.position))
            {
                currentFurhestPoint = position + Vector3.up;
            }
        }
    }

    private void Update()
    {
        if (isRunningAway)
        { 
            navMesh.isStopped = false;
            navMesh.SetDestination(currentFurhestPoint);

            anim.SetFloat("Speed", navMesh.velocity.magnitude);
        }
        else
        {
            if (!navMesh.isStopped)
            {
                navMesh.ResetPath();
                navMesh.isStopped = true;
            }

            var lookdir = (player.position - (transform.position + Vector3.up));
            lookdir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookdir), Time.deltaTime * rotSpeed);

            anim.SetFloat("Speed", 0);
        }
    }

    void Throw()
    {
        isRunningAway = false;
        anim.SetTrigger("Throw");
        Invoke(nameof(StartRunningAway), 2f);
    }

    void StartRunningAway()
    {
        isRunningAway = true;
    }

    void SpawnGranade()
    {
        Instantiate(granadePrefab, throwPoint.position, Quaternion.identity).damage = stats.damage;
    }

    private void OnDisable()
    {
        throwPoint.gameObject.SetActive(false);

        navMesh.ResetPath();
        navMesh.isStopped = true;
    }
}
