using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewindCooldownCounter : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    Color normalColor;
    public Color disabledColor;

    private void Start()
    {
        normalColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerRewind.rewindCooldownRemaining <= 0)
        {
            text.enabled = false;
            image.color = normalColor;
        }
        else
        {
            text.enabled = true;
            image.color = disabledColor;
            text.text = PlayerRewind.rewindCooldownRemaining.ToString("F1");
        }
    }   
}
