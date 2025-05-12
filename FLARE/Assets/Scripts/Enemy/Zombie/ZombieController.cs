using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim = null;

    [SerializeField] private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            Debug.LogWarning("Target not assigned to ZombieController!");
        }
    }

    void Update()
    {
        MoveToTarget();
        
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
        anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);
        RotateToTarget();

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget <= agent.stoppingDistance)
        {

            //anim.SetFloat("Speed", Of);
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


    private void GetReferance()
    {

    }
}