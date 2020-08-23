using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraceIndicator : MonoBehaviour
{
    public Image image;

    // Update is called once per frame
    void Update()
    {
        if (PlayerGrace.grace) image.enabled = true;
        else image.enabled = false;
    }
}
