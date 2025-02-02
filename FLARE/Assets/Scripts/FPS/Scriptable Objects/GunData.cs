using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{

    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public int ammoInventory;

    [Tooltip("In RPM")] public float fireRate;
    public float reloadTime;
    public float reloadTime1;
    public float reloadTime2;
    public float reloadTime3;
    public float reloadTime4;
    public float reloadTime5;

    [HideInInspector] public bool reloading;

}