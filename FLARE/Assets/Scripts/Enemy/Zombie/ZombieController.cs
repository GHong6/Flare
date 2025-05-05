using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
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
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        //transform.LookAt(target);

        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
    }

    private void GetReferance()
    {

    }
}