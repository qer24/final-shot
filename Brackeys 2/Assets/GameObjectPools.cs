using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPools : MonoBehaviour
{
    static GameObjectPools instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }
}
