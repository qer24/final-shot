using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxamount = 0.03f;
    public float smooth = 3;
    private Vector3 def;

    void Start()
    {
        def = transform.localRotation.eulerAngles;
    }

    void Update()
    {

        float factorY = -(Input.GetAxis("Mouse X")) * amount;
        float factorX = -(Input.GetAxis("Mouse Y")) * amount * 0.5f;

        if (factorX > maxamount)
            factorX = maxamount;

        if (factorX < -maxamount)
            factorX = -maxamount;

        if (factorY > maxamount)
            factorY = maxamount;

        if (factorY < -maxamount)
            factorY = -maxamount;

        Quaternion Final = Quaternion.Euler(def.x + factorX, def.y + factorY, def.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Final, (Time.deltaTime * amount) * smooth);
    }
}
