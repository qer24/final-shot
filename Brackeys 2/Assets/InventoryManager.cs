using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public float lerpSpeed = 5f;

    public GameObject inventoryPanel;
    public CanvasGroup canvasGroup;
    public RectTransform selection;
    RectTransform currentlySelectedGun;

    public Gun[] guns;
    PlayerShooter playerShooter;

    private void Start()
    {
        playerShooter = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooter>();
    }

    private void Update()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                if(playerShooter.currentGun != guns[i - 1])
                    SelectGun(i);
            }
        }

        if (inventoryPanel.activeSelf && currentlySelectedGun != null)
        {
            float lerpedY = Mathf.Lerp(selection.anchoredPosition.y, currentlySelectedGun.anchoredPosition.y, Time.deltaTime * lerpSpeed);
            selection.anchoredPosition = new Vector2(selection.anchoredPosition.x, lerpedY);
        }
    }

    void SelectGun(int index)
    {
        foreach(var gun in guns)
        {
            if(gun.transform)
                gun.transform.gameObject.SetActive(false);
        }

        playerShooter.currentGun = guns[index - 1];
        playerShooter.currentGun.transform = playerShooter.gunPivot.GetChild(index);
        playerShooter.currentGun.transform.gameObject.SetActive(true);
        playerShooter.StartCoroutine(playerShooter.PickupGun(playerShooter.currentGun.transform));

        currentlySelectedGun = inventoryPanel.transform.GetChild(index).GetComponent<RectTransform>();

        canvasGroup.alpha = 1f;
        inventoryPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(InventoryFadout());
    }

    IEnumerator InventoryFadout()
    {
        yield return new WaitForSeconds(1.5f);

        while(canvasGroup.alpha > 0.05f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * 4f);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        inventoryPanel.SetActive(false);
    }
}
