using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrace : MonoBehaviour
{
    Vector3 dir;
    Vector3 stopPoint;
    public float speed = 1f;

    public void Init(Vector3 hitDir, Vector3 hitPoint)
    {
        Destroy(gameObject, 5f);

        stopPoint = hitPoint;
        dir = hitDir;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, stopPoint) > 1f)
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}
