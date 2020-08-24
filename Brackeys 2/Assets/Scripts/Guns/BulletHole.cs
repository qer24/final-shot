using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float fadeoutSpeed = 5f;
    bool canFade;

    public Vector3 startScale;

    private void OnEnable()
    {
        if (Time.timeSinceLevelLoad > 1f)
        {
            transform.localScale *= Random.Range(0.9f, 1.1f);

            canFade = false;
            Invoke(nameof(EnableFade), Random.Range(3f, 4f));
        }
    }
        
    void EnableFade() => canFade = true;

    private void Update()
    {
        if(canFade)
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.zero, Time.deltaTime * fadeoutSpeed);
    }
}
