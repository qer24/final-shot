using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable] public class WeightedEnemy
    {
        public GameObject prefab;
        public int weight;
    }

    public RewardsManager rewardsManager;

    public Transform[] spawnPoints;
    public WeightedEnemy[] enemiesToSpawn;
    public GameObject boss;

    public EnemySpawner enemySpawnerPrefab;

    public int minEnemies = 2;
    public int maxEnemies = 3;
    public static int currentWave = 0;
    int wavesUntilNextReward = 2;
    bool spawnedBoss;

    public EnemyRuntimeSet enemyRuntimeSet;

    List<int> takenSpawnPoints;

    public static bool isSpawning;

    private void Start()
    {
        enemyRuntimeSet.Reset();
        isSpawning = false;

        currentWave = 0;
        spawnedBoss = false;
        StartCoroutine(SpawnWave(10f));
    }

    private void OnEnable()
    {
        enemyRuntimeSet.OnKilledAllEnemies += NextWave;
        GameManager.OnPlayerDeath += Stop;
    }

    private void OnDisable()
    {
        enemyRuntimeSet.OnKilledAllEnemies -= NextWave;
        GameManager.OnPlayerDeath -= Stop;
    }

    void Stop()
    {
        StopAllCoroutines();

        enabled = false;
    }

    void NextWave()
    {
        StopAllCoroutines();

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave(float delay = 0)
    {
        spawnedBoss = false;
        currentWave++;
        if (currentWave % 10 == 0)
        {
            wavesUntilNextReward++;
        }
        if(currentWave % wavesUntilNextReward == 1 && currentWave != 1)
        {
            rewardsManager.DisplayRewards();
        }

        yield return new WaitForSeconds(delay);

        isSpawning = true;
        Invoke(nameof(ResetSpawningBool), enemySpawnerPrefab.timeToSpawn);

        takenSpawnPoints = new List<int>();

        int enemyCount;
        if (currentWave == 25 || currentWave == 60)
        {
            enemyCount = 15;
        }
        else if (currentWave == 30)
        {
            enemyCount = 10;
        }
        else
        {
            enemyCount = Mathf.RoundToInt(Random.Range(minEnemies * DifficultyManager.difficultyMultiplier, maxEnemies * DifficultyManager.difficultyMultiplier));
        }

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    void ResetSpawningBool()
    {
        isSpawning = false;
    }

    void SpawnEnemy()
    {
        if (takenSpawnPoints.Count >= spawnPoints.Length) return;

        int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        while (takenSpawnPoints.Contains(randomSpawnPoint))
        {
            randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        }

        takenSpawnPoints.Add(randomSpawnPoint);

        GameObject enemy;
        if(currentWave == 1 || currentWave == 2)
        {
            enemy = enemiesToSpawn[0].prefab;
        }
        if (currentWave == 3)
        {
            enemy = enemiesToSpawn[3].prefab;
        }
        if (currentWave == 4)
        {
            enemy = enemiesToSpawn[2].prefab;
        }
        if (currentWave % 15 == 0 && !spawnedBoss)
        {
            enemy = boss;
            spawnedBoss = true; 
        }
        else if(currentWave == 25 || currentWave == 60)
        {
            enemy = enemiesToSpawn[4].prefab;
        }
        else if(currentWave == 30)
        {
            enemy = enemiesToSpawn[3].prefab;
        }
        else
        {
            enemy = enemiesToSpawn[GetRandomWeightedIndex(enemiesToSpawn)].prefab;
        }
        Instantiate(enemySpawnerPrefab, spawnPoints[randomSpawnPoint].position, Quaternion.identity).enemyToSpawn = enemy;
    }

    public int GetRandomWeightedIndex(WeightedEnemy[] enemies)
    {
        int weightSum = 0;
        foreach (var enemy in enemies)
        {
            weightSum += enemy.weight;
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = enemies.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < enemies[index].weight)
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= enemies[index++].weight;
        }

        // No other item was selected, so return very last index.
        return index;
    }

    private void OnGUI()
    {
        GUILayout.Label($"current wave = {currentWave}");
    }
}
