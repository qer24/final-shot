using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDeath : Death
{
    [SerializeField] Transform ragdollBase = null;
    [SerializeField] Animator anim = null;
    [SerializeField] Collider mainCollider = null;
    [SerializeField] Collider headshotCollider = null;
    [SerializeField] Collider playerCollider = null;
    [SerializeField] FlowMachine ai = null;
    [SerializeField] MonoBehaviour monoAi = null;
    [SerializeField] GameObject ambientSound = null;
    [SerializeField] SkinnedMeshRenderer rend = null;

    Rigidbody[] ragdollBodies;
    List<Collider> ragdollColliders;

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

        ragdollBodies = ragdollBase.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = new List<Collider>(ragdollBase.GetComponentsInChildren<Collider>());
        ragdollColliders.Remove(mainCollider);
        ragdollColliders.Remove(headshotCollider);

        ToggleRagdoll(false);
    }

    public override void Die()
    {
        base.Die();

        ToggleRagdoll(true);

        foreach (var rb in ragdollBodies)
        {
            rb.AddForce(hitDirection * 7f, ForceMode.Impulse);
            //rb.AddExplosionForce(107f, hitDirection, 1f, 0, ForceMode.Impulse);
        }

        Invoke(nameof(DisableColliders), 5f);
        if(CorpseCleanupSettings.cleanupCorpses && gameObject.activeSelf)
        {
            StartCoroutine(TryCleanup());
        }
    }

    IEnumerator TryCleanup()
    {
        yield return new WaitForSeconds(4.5f);

        while(rend.isVisible)
        {
            yield return new WaitForSeconds(0.5f);
            if(!CorpseCleanupSettings.cleanupCorpses)
            {
                yield break;
            }
        }

        Destroy(gameObject);
    }

    void DisableColliders()
    {
        foreach (var rb in ragdollBodies)
        {
            Destroy(rb.GetComponent<CharacterJoint>());
            rb.GetComponent<Collider>().enabled = false;    

            rb.isKinematic = true;
        }
    }

    void ToggleRagdoll(bool state)
    {
        anim.enabled = !state;
        mainCollider.enabled = !state;
        playerCollider.enabled = !state;
        headshotCollider.enabled = !state;
        ambientSound.SetActive(!state);
        if (ai)
            ai.enabled = !state;
        if (monoAi)
            monoAi.enabled = !state;

        foreach(var rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = state;
        }
    }
}
