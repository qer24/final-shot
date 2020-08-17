using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Granade Runtime Set", menuName = "RuntimeSets/Granade Runtime Set")]
public class GranadeRuntimeSet : ScriptableObject
{
    public List<GameObject> granadesActive = new List<GameObject>();

    public void Reset()
    {
        granadesActive = new List<GameObject>();
    }

    public void AddEnemy(GameObject enemy)
    {
        granadesActive.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        granadesActive.Remove(enemy);
    }
}
