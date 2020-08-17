using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public Rigidbody rb;
    public TextMeshPro text;

    public Color critColor;

    public Vector3 forceMin;
    public Vector3 forceMax;

    public void Init(int damage, bool isCrit)
    {
        transform.position += Vector3.up;
        Vector3 force = new Vector3(Random.Range(forceMin.x, forceMax.x), Random.Range(forceMin.y, forceMax.y), Random.Range(forceMin.z, forceMax.z));
        rb.AddForce(force, ForceMode.VelocityChange);
        text.text = damage.ToString();
        if (isCrit) text.color = critColor;
        Destroy(gameObject, 1f);
    }
}
