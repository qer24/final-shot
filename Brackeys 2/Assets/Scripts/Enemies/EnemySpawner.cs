using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;

    public GameObject arrow;
    public ParticleSystem particles;

    public float timeToSpawn = 3f;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        AudioManager.Play("enemySpawnWarning", transform.position);

        yield return new WaitForSeconds(timeToSpawn / 5);

        arrow.SetActive(false);

        yield return new WaitForSeconds(timeToSpawn / 5);

        arrow.SetActive(true);
        AudioManager.Play("enemySpawnWarning", transform.position);

        yield return new WaitForSeconds(timeToSpawn / 5);

        arrow.SetActive(false);

        yield return new WaitForSeconds(timeToSpawn / 5);

        arrow.SetActive(true);
        AudioManager.Play("enemySpawnWarning", transform.position);

        yield return new WaitForSeconds(timeToSpawn / 5);

        AudioManager.Play("enemySpawn", transform.position);

        arrow.SetActive(false);
        particles.Play();
        if(enemyToSpawn != null)
        {
            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 3f);
    }
}
