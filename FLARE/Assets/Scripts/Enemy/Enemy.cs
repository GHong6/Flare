using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private PlayerMovementAdvanced1 playerMovement;
    private Vector3 lastKnownPos;
    private bool investigating = false;


    [Header("Sight & Hearing Settings")]
    public float sightDistance = 10f;
    public float fieldOfView = 95f;
    public float eyeHeight;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }

    public Vector3 LastKnownPos { get => lastKnownPos; set => lastKnownPos = value; }

    public Path path;
    //public GameObject debugsphere;

    [Header("Sound")]
    [SerializeField] public AudioSource gunShot;


    [Header("WeaponValue")]
    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    [HideInInspector]
    public bool isShooting = false;

    [SerializeField]
    private string currentState;



    void Start()
    {
        gunShot = GetComponent<AudioSource>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovementAdvanced1>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || playerMovement == null)
            return;

        bool canSeePlayer = CanSeePlayer();
        bool isPlayerRunning = playerMovement.state == PlayerMovementAdvanced1.MovementState.sprinting;

        // If the player is sprinting inside sight radius, update last known position
        if (canSeePlayer && isPlayerRunning)
        {
            lastKnownPos = player.transform.position;
            investigating = true;
        }

        if (investigating)
        {
            InvestigateLastKnownPosition();
        }
    }


    private void InvestigateLastKnownPosition()
    {
        if (agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(lastKnownPos);
        }

        // Stop investigating after reaching the last known position
        if (Vector3.Distance(transform.position, lastKnownPos) < 1f)
        {
            investigating = false;
        }
    }


    //public bool CanSeePlayer()
    //{
    //    if(player != null)
    //    {
    //        if(Vector3.Distance(transform.position,player.transform.position) < sightDistance)
    //        {
    //            Vector3 targetDiraction = player.transform.position - transform.position - (Vector3.up * eyeHight);
    //            float angleToPlayer = Vector3.Angle(targetDiraction, transform.position);
    //            if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
    //            {
    //                Ray ray = new Ray(transform.position + (Vector3.up * eyeHight), targetDiraction);
    //                RaycastHit hitInfo = new RaycastHit();
    //                if (Physics.Raycast(ray, out hitInfo, sightDistance))
    //                {
    //                    if(hitInfo.transform.gameObject == player)
    //                    {
    //                        Debug.DrawRay(ray.origin, ray.direction * sightDistance);
    //                        return true;
    //                    }
    //                }
    //                Debug.DrawRay(ray.origin, ray.direction * sightDistance);
    //            }
    //        }
    //    }
    //    return false;
    //}

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > sightDistance)
            return false;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > fieldOfView / 2)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up * eyeHeight, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, sightDistance))
        {
            if (hitInfo.transform.gameObject == player)
            {
                Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.red);
                return true;
            }
        }

        return false;
    }


    void OnDrawGizmos()
    {
        // Draw sight radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        // Draw field of view
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, transform.position + leftBoundary * sightDistance + Vector3.up * eyeHeight);
        Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, transform.position + rightBoundary * sightDistance + Vector3.up * eyeHeight);

        // Draw player's position if visible
        if (player != null && CanSeePlayer())   
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, player.transform.position);
        }
    }

}
