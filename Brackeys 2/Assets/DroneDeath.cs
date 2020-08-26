using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeath : Death
{
    /*
    [SerializeField] Rigidbody rb = null;
    [SerializeField] Animator anim = null;
    [SerializeField] MonoBehaviour monoAi = null;
    [SerializeField] Renderer rend = null;
    [SerializeField] Collider col = null;
    */
    [SerializeField] ParticleSystem particles = null;
    [SerializeField] Vector3 particleScale = Vector3.one;

    [HideInInspector]
    public Vector3 hitDirection;

    private void Update()
    {
        if (transform.position.y < -10f)
        {
            GetComponent<Health>().RemoveHealth(9999f);
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Die()
    {
        base.Die();

        AudioManager.Play("droneDeath", transform.position);

        particles.Play();
        particles.transform.parent = null;
        particles.transform.localScale = particleScale;
        Destroy(particles.gameObject, 2f);

        Destroy(gameObject);

        /*
        rb.AddForce(hitDirection * 7f, ForceMode.Impulse);
        rb.useGravity = true;
        Vector3 vel = new Vector3(Random.Range(15, 30), Random.Range(15, 30), Random.Range(15, 30));
        rb.angularVelocity = vel;

        Invoke(nameof(DisableColliders), 5f);
        if (gameObject.activeSelf)
        {
            StartCoroutine(TryCleanup());
        }
        */
    }

    /*
    IEnumerator TryCleanup()
    {
        yield return new WaitForSeconds(4.5f);

        for(; ; )
        {
            yield return new WaitForSeconds(0.5f);

            if (!rend.isVisible && CorpseCleanupSettings.cleanupCorpses)
            {
                Destroy(gameObject);
                yield break;
            }
        }
    }

    void DisableColliders()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }
    */
}
