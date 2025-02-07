using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private PlayerMovementAdvanced1 playerMovement;
    private Vector3 lastKnownPos;
    private bool investigating = false;
    private bool alertMode = false;
    private float alertTimer = 0f;
    private float suspicionTimer = 0f;
    private bool playerDetected = false;
    private bool instantAttack = false;

    [Header("Sight & Hearing Settings")]
    public float sightDistance = 10f;
    public float fieldOfView = 95f;
    public float eyeHeight;
    public float alertDuration = 60f;

    [Header("Weapon Settings")]
    public Transform gunBarrel;
    public float fireRate;
    public int minShots = 2;
    public int maxShots = 6;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }

    public Vector3 LastKnownPos { get => lastKnownPos; set => lastKnownPos = value; }

    public Path path;
    //public GameObject debugsphere;

    [Header("Sound")]
    [SerializeField] public AudioSource gunShot;


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

    void Update()
    {
        if (player == null || playerMovement == null)
            return;

        bool canSeePlayer = CanSeePlayer();
        bool isPlayerRunning = playerMovement.state == PlayerMovementAdvanced1.MovementState.sprinting;

        // If the player is seen, handle suspicion and attack
        if (canSeePlayer)
        {
            if (!playerDetected)
            {
                suspicionTimer += Time.deltaTime;
                if (suspicionTimer >= 1f)
                {
                    playerDetected = true;
                    AttackPlayer();
                }
            }
        }
        else
        {
            suspicionTimer = 0f;
        }

        // If the player runs within sight range, update lastKnownPos
        if (isPlayerRunning && Vector3.Distance(transform.position, player.transform.position) <= sightDistance)
        {
            lastKnownPos = player.transform.position;
            alertMode = true;
            stateMachine.ChangeState(new SearchState());  // Start investigating
        }

        // If the player shoots, update lastKnownPos and investigate
        if (playerMovement.IsShooting) // Assume IsShooting is a bool in PlayerMovementAdvanced1
        {
            lastKnownPos = player.transform.position; // Change this if you have a separate shoot position
            alertMode = true;
            stateMachine.ChangeState(new SearchState());  // Start investigating
        }

        // Handle Alert Timer
        if (alertMode)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= alertDuration)
            {
                alertMode = false;
                alertTimer = 0f;
                instantAttack = false;
            }
        }
    }

    private void AttackPlayer()
    {
        StartCoroutine(FireShots());
    }

    private IEnumerator FireShots()
    {
        int shots = Random.Range(minShots, maxShots);
        for (int i = 0; i < shots; i++)
        {
            Debug.Log("Enemy fires at player!");
            yield return new WaitForSeconds(fireRate);
        }
        lastKnownPos = player.transform.position;
        agent.SetDestination(lastKnownPos);
    }

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
}
