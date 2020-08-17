using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Runtime Set", menuName = "RuntimeSets/Enemy Runtime Set")]
public class EnemyRuntimeSet : ScriptableObject
{
    public List<GameObject> enemiesAlive = new List<GameObject>();
    public Action OnKilledAllEnemies;

    public void Reset()
    {
        enemiesAlive = new List<GameObject>();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemiesAlive.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemiesAlive.Remove(enemy);

        if(enemiesAlive.Count <= 0 && !SpawnManager.isSpawning)
        {
            OnKilledAllEnemies?.Invoke();
        }
    }
}
