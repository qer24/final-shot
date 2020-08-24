using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeath : Death
{
    [SerializeField] Rigidbody rb = null;
    [SerializeField] Animator anim = null;
    [SerializeField] MonoBehaviour monoAi = null;
    [SerializeField] GameObject ambientSound = null;
    [SerializeField] Renderer rend = null;
    [SerializeField] Collider col = null;

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

        ToggleRagdoll(false);
    }

    public override void Die()
    {
        base.Die();

        ToggleRagdoll(true);

        rb.AddForce(hitDirection * 7f, ForceMode.Impulse);
        rb.useGravity = true;
        Vector3 vel = new Vector3(Random.Range(15, 30), Random.Range(15, 30), Random.Range(15, 30));
        rb.angularVelocity = vel;

        Invoke(nameof(DisableColliders), 5f);
        if (CorpseCleanupSettings.cleanupCorpses && gameObject.activeSelf)
        {
            StartCoroutine(TryCleanup());
        }
    }

    IEnumerator TryCleanup()
    {
        yield return new WaitForSeconds(4.5f);

        while (rend.isVisible)
        {
            yield return new WaitForSeconds(0.5f);
            if (!CorpseCleanupSettings.cleanupCorpses)
            {
                yield break;
            }
        }

        Destroy(gameObject);
    }

    void DisableColliders()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }

    void ToggleRagdoll(bool state)
    {
        anim.enabled = !state;
        ambientSound.SetActive(!state);
        if (monoAi)
            monoAi.enabled = !state;

        rb.isKinematic = !state;
    }
}
