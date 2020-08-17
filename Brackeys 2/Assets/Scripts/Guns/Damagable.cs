using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public interface IDamagable
{
    void TakeDamage(float amount);
    bool IsCrit { get; set; }
}

public class Damagable : MonoBehaviour, IDamagable
{
    public Health hp;
    public DamagePopup damagePopup;

    public bool IsCrit { get; set; }
    public bool isPlayer = false;

    public virtual void Start()
    {
        IsCrit = false;
    }

    public virtual void TakeDamage(float amount)
    {
        hp.RemoveHealth(amount);
        if(!isPlayer)
        {
            GameManager.score += (int)amount;

            AudioManager.Play("enemyHit", transform.position);
            Crosshair.instance.StartCoroutine(Crosshair.HitmarkCoroutine());
            Instantiate(damagePopup, transform.position, Quaternion.identity).Init((int)amount, IsCrit);
        }else
        {
            AudioManager.Play("playerHit");
            CameraShaker.Instance.ShakeOnce(5f, 2f, 0.1f, 0.5f);
            return;
        }

        if(IsCrit)
        {
            AudioManager.Play("hitCrit");
        }else
        {
            AudioManager.Play("hit");
        }
    }
}
