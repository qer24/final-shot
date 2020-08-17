using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Sprite baseSprite;
    public Sprite selectSprite;
    Image image;

    static GameObject hitmarker;
    public static Crosshair instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        hitmarker = transform.GetChild(0).gameObject;
        image = GetComponent<Image>();
    }

    public void SetCrosshair(bool select)
    {
        image.sprite = select ? selectSprite : baseSprite;
    }

    public static IEnumerator HitmarkCoroutine()
    {
        hitmarker.SetActive(true);

        yield return new WaitForSecondsRealtime(Time.deltaTime * 5f);

        hitmarker.SetActive(false);
    }
}
