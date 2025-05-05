using System.Collections;
using System.Collections.Generic;
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
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }
}