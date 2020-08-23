using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitParticles : MonoBehaviour
{
    public ParticleSystem particles;

    private void OnEnable()
    {
        if (Time.timeSinceLevelLoad > 10f)
        {
            particles.Play();
            Lean.Pool.LeanPool.Despawn(gameObject, 1f);
        }
    }
}
