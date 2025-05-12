using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStats : CharacterStats
{

    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;

    [SerializeField] private bool canAttack;


    void Start()
    {
        InitVariables();
    }
    public void DealDamage()
    {
        //damage func

    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    public override void InitVariables()
    {
        maxHealth = 50;
        SetHealthTo(maxHealth);
        isDead = false;

        damage = 33;
        attackSpeed = 1.5f;
        canAttack = true;
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }
}
