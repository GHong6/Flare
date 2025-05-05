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

        // Trigger the reload animations
        TriggerReloadAnimations();

        // Get the length of the current reload animation
        string reloadAnimationName = $"Reload{7 - Mathf.Clamp(gunData.currentAmmo + 1, 1, 6)}";
        float animationLength = GetAnimationClipLength(armsAnimator, reloadAnimationName);
        Debug.Log(reloadAnimationName);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Calculate how many bullets need to be reloaded
        int bulletsNeeded = gunData.magSize - gunData.currentAmmo;
        int bulletsAvailable = CountBulletsInInventory();

        int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);

        if (bulletsToReload > 0)
        {
            RemoveBulletsFromInventory(bulletsToReload);
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
                //Ray ray = new Ray(cam.position, cam.forward);
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                Debug.DrawRay(ray.origin, ray.direction * gunData.maxDistance, Color.green, 1f);

                //if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, gunData.maxDistance))
                //{
                //    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                //    damageable?.TakeDamage(gunData.damage);
                //}

                if (Physics.Raycast(ray, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    Debug.Log($"Hit: {hitInfo.transform.name} at {hitInfo.point}");

                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    if (damageable == null)
                    {
                        Debug.LogWarning("Hit object does not implement IDamageable!");
                    }
                    else
                    {
                        damageable.TakeDamage(gunData.damage);
                    }
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
        switch (gunData.currentAmmo)
        {
            case 0:
                armsAnimator.Play("Reload6", 0, 0f); // Play the reload animation on the arms
                gunAnimator.Play("Reload6", 0, 0f); // Play the reload animation on the gun
                break;

            case 1:
                armsAnimator.Play("Reload5", 0, 0f);
                gunAnimator.Play("Reload5", 0, 0f);
                break;

            case 2:
                armsAnimator.Play("Reload4", 0, 0f);
                gunAnimator.Play("Reload44", 0, 0f);
                break;

            case 3:
                armsAnimator.Play("Reload3", 0, 0f);
                gunAnimator.Play("Reload3", 0, 0f);
                break;

            case 4:
                armsAnimator.Play("Reload2", 0, 0f);
                gunAnimator.Play("Reload2", 0, 0f);
                break;

            case 5:
                armsAnimator.Play("Reload1", 0, 0f);
                gunAnimator.Play("Reload1", 0, 0f);
                break;

            default:
                Debug.LogWarning("No matching reload animation for current ammo count: " + gunData.currentAmmo);
                break;
        }
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

    private float GetAnimationClipLength(Animator animator, string clipName)
    {
        // Access the RuntimeAnimatorController
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        // Search for the clip with the matching name
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation clip '{clipName}' not found!");
        return 0f; // Fallback in case the animation clip isn't found
    }
}