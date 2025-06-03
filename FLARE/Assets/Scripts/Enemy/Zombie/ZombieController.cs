using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 3;
    private float timeOfLastAttack = 0;
    private bool hasStopped = false;

    private NavMeshAgent agent = null;
    private Animator anim = null;
    private ZombieStats stats = null;
    

    [SerializeField] private Transform target;


    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();

        GetReferance();
    }

    void Update()
    {
        MoveToTarget();
        
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
        //anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);
        RotateToTarget();

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget <= agent.stoppingDistance)
        {

            //anim.SetFloat("Speed", Of);
            if (!hasStopped)
            {
                hasStopped = true;
                timeOfLastAttack = Time.time;
            }
            //attack
            //charaterstats 

            if (Time.time >= timeOfLastAttack + stats.attackSpeed)
            {
                timeOfLastAttack = Time.time;
                PlayerHealth targetStats = target.GetComponent<PlayerHealth>();
                AttackTarget(targetStats);
                Debug.Log("Zombie attacks!");

            }
        }
        else
        {
            if (hasStopped)
            {
                hasStopped = false;
            }
        }
    }



    private void RotateToTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // Ignore vertical difference

        if (direction.sqrMagnitude > 0.001f) // Prevent errors from zero-length vectors
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }

    private void AttackTarget(PlayerHealth statsToDamage)
    {
        //anim.SetTrigger("attack");
        statsToDamage.TakeDamage(stats.damage);
    }

    private void GetReferance()
    {
        agent = GetComponent<NavMeshAgent>();
        //anim = GetComponentInChild<Animator>();
        stats = GetComponent<ZombieStats>();
    }
}