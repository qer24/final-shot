using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float fadeoutSpeed = 5f;
    bool canFade;

    private void Start()
    {
        transform.localScale *= Random.Range(0.9f, 1.1f);

        canFade = false;
        Destroy(gameObject, 5f);
        Invoke(nameof(EnableFade), Random.Range(3f, 4f));
    }

    void EnableFade() => canFade = true;

    private void Update()
    {
        if(canFade)
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.zero, Time.deltaTime * fadeoutSpeed);
    }
}
