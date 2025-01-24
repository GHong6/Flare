using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager; // Reference to InventoryManager
    [SerializeField] private Item bulletItem; // The item type representing bullets

    [SerializeField] private Animator armsAnimator;  // Reference for arms animation
    [SerializeField] private Animator gunAnimator;
    [Header("Sound")]
    [SerializeField] private AudioSource gunShot;

    [Header("References")]
    [SerializeField] private GunData gunData;
    public GunData GunData => gunData; // Public read-only property


    [SerializeField] private Transform cam;
    [SerializeField] private PlayerUI playerUI; // Reference to the PlayerUI script
    

    float timeSinceLastShot;

    private void Start()
    {
        armsAnimator = GetComponent<Animator>();
        gunShot = GetComponent<AudioSource>();

        // Ensure ammo UI is updated at the start
        UpdateAmmoUI();
        UpdateAmmoInventoryUI();

        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
    }

    private void UpdateAmmoUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateAmmo(gunData.currentAmmo, gunData.magSize);
        }
    }

    public void UpdateAmmoInventoryUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateAmmoInventory(CountBulletsInInventory());
        }
    }

    private void OnDisable() => gunData.reloading = false;

    private void StartReload()
    {
        // Check if the gun is not already reloading and the gun is active
        if (!gunData.reloading && this.gameObject.activeSelf)
        {
            // Only start reloading if there are bullets in the inventory
            if (CountBulletsInInventory() > 0 && gunData.currentAmmo < 6)
            {
                StartCoroutine(Reload());
            }
            else
            {
                Debug.Log("No bullets in inventory to reload!");
            }
        }
    }
    private IEnumerator Reload()
    {
        gunData.reloading = true;

        TriggerReloadAnimations();
        yield return new WaitForSeconds(gunData.reloadTime);

        int bulletsNeeded = gunData.magSize - gunData.currentAmmo;
        int bulletsAvailable = CountBulletsInInventory();
        

        int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);

        if (bulletsToReload > 0)
        {
            
            RemoveBulletsFromInventory(bulletsToReload);
            gunData.ammoInventory -= bulletsToReload;
            gunData.currentAmmo += bulletsToReload;
        }
        else
        {
            Debug.Log("No bullets available to reload!");
        }

        gunData.reloading = false;
        
        UpdateAmmoUI();
        UpdateAmmoInventoryUI();
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    damageable?.TakeDamage(gunData.damage);
                }

                TriggerShootAnimations();
                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                UpdateAmmoUI();
                UpdateAmmoInventoryUI();
            }
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, cam.forward * gunData.maxDistance);
    }

    private void TriggerReloadAnimations()
    {
        armsAnimator.Play("Reload", 0, 0f); // Play the reload animation on the arms
        gunAnimator.Play("Reload", 0, 0f); // Play the reload animation on the gun
    }

    private void TriggerShootAnimations()
    {
        armsAnimator.Play("Shoot", 0, 0f); // Play the shoot animation on the arms
        gunAnimator.Play("Shoot", 0, 0f); // Play the shoot animation on the gun
        gunShot.Play();
    }

    private int CountBulletsInInventory()
    {
        int totalBullets = 0;

        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == bulletItem)
            {
                totalBullets += itemInSlot.count;
            }
        }

        return totalBullets;
    }

    private void RemoveBulletsFromInventory(int bulletsToRemove)
    {
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == bulletItem)
            {
                int bulletsRemoved = Mathf.Min(bulletsToRemove, itemInSlot.count);
                itemInSlot.count -= bulletsRemoved;
                itemInSlot.RefreshCount();
                bulletsToRemove -= bulletsRemoved;

                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject); // Remove the item from inventory if the count is zero
                }

                if (bulletsToRemove <= 0) break;
            }
        }

        if (bulletsToRemove > 0)
        {
            Debug.LogError($"Not enough bullets to remove {bulletsToRemove}! Check inventory logic.");
        }
    }
    public bool IsReloading => gunData.reloading;
}