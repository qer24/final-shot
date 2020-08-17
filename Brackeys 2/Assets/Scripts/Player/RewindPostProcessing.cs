using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RewindPostProcessing : MonoBehaviour
{
    public float lerpSpeed;
    Volume volume;

    void Start()
    {
        volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        float lerpValue = PlayerRewind.isRewinding ? 1 : 0;
        volume.weight = Mathf.Abs(volume.weight - lerpValue) <= 0.1f ? volume.weight = lerpValue : Mathf.Lerp(volume.weight, lerpValue, Time.deltaTime * lerpSpeed);
    }
}
