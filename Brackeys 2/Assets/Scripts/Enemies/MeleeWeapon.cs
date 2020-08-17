using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public Collider col;
    public float damage;

    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamagable>(out var damagable))
        {
            if(other.CompareTag("Player") && !PlayerRewind.isRewinding)
            {
                damagable.TakeDamage(damage);
                DisableCollider();
            }
        }
    }
}
