using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadshotDamagable : Damagable, IDamagable
{
    public int headshotMultiplier;

    public override void Start()
    {
        hp.OnDeath += DisableCollider;
        IsCrit = true;
    }

    private void OnDisable()
    {
        hp.OnDeath -= DisableCollider;
    }

    void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount * headshotMultiplier);
    }
}
