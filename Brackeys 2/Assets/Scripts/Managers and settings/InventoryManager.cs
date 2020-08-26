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

    int currentlySelectedGunIndex = 0;

    private void Start()
    {
        playerShooter = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooter>();
        playerShooter.onPickedUpGun += UnlockNewGun;
    }

    private void OnDisable()
    {
        playerShooter.onPickedUpGun -= UnlockNewGun;
    }

    public void UnlockNewGun(Gun newGun)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if(guns[i] == newGun)
            {
                inventoryPanel.transform.GetChild(i + 1).gameObject.SetActive(true);
                newGun.currentAmmo = Mathf.RoundToInt(newGun.baseAmmo * playerShooter.playerStats.magazineMultiplier);
                SelectGun(i + 1);
                return;
            }
        }
    }

    private void Update()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(i.ToString()) && inventoryPanel.transform.GetChild(i).gameObject.activeSelf)
            {
                if(playerShooter.currentGun != guns[i - 1])
                    SelectGun(i);
            }
        }

        var scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.01f && (currentlySelectedGunIndex < guns.Length))
        {
            if(inventoryPanel.transform.GetChild(currentlySelectedGunIndex + 1).gameObject.activeSelf)
                SelectGun(currentlySelectedGunIndex + 1);
        }
        else if (scroll < -0.01f && (currentlySelectedGunIndex - 1 > 0))
        {
            if(inventoryPanel.transform.GetChild(currentlySelectedGunIndex - 1).gameObject.activeSelf)
                SelectGun(currentlySelectedGunIndex - 1);
        }

        if (inventoryPanel.activeSelf && currentlySelectedGun != null)
        {
            float lerpedY = Mathf.Lerp(selection.anchoredPosition.y, currentlySelectedGun.anchoredPosition.y, Time.deltaTime * lerpSpeed);
            selection.anchoredPosition = new Vector2(selection.anchoredPosition.x, lerpedY);
        }
    }

    void SelectGun(int index)
    {
        currentlySelectedGunIndex = index;

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
