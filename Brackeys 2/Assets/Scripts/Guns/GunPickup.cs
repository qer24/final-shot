    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    Material[] materials;
    public Gun scriptableObject;

    public Collider col;
    public Rigidbody rb;

    void Start()
    {
        materials = GetComponentInChildren<MeshRenderer>().materials;

        scriptableObject.transform = transform;
        scriptableObject.pickupScript = this;
        scriptableObject.currentAmmo = scriptableObject.baseAmmo;
    }

    public void ToggleHightlight(bool state)
    {
        var value = state ? 1 : 0;
        foreach (var mat in materials)
        {
            mat.SetFloat("Outline", value);
        }
    }

    public void Pickup()
    {
        ToggleHightlight(false);
        col.enabled = false;
        rb.isKinematic = true;
    }

    public void Drop(Vector3 dropForce)
    {
        col.enabled = true;
        rb.isKinematic = false;

        rb.angularVelocity = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
        rb.AddForce(dropForce, ForceMode.Impulse);
    }
}
