using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Death : MonoBehaviour
{
    [SerializeField] Health hp = null;
    [SerializeField] EnemyRuntimeSet enemyRuntimeSet = null;
    [SerializeField] GameObject minimapIcon = null;

    public bool isEnemy = true;

    public virtual void Start()
    {
        hp.OnDeath += Die;

        if(isEnemy)
        {
            enemyRuntimeSet.AddEnemy(gameObject);
        }
    }

    void OnDestroy()
    {
        Die();
        hp.OnDeath -= Die;
    }

    private void OnDisable()
    {
        hp.OnDeath -= Die;
    }

    public virtual void Die()
    {
        if (isEnemy)
        {
            AudioManager.Play("enemyDeath", transform.position + Vector3.up * 0.5f);
            enemyRuntimeSet.RemoveEnemy(gameObject);
        }

        if(minimapIcon != null)
        {
            minimapIcon.SetActive(false);
        }
    }
}
 
