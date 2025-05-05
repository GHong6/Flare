using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private string debugState;

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private PlayerMovementAdvanced playerMovement;
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

    public bool isShooting = false;



    [SerializeField]
    private string currentState;

    void Start()
    {
        gunShot = GetComponent<AudioSource>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 720f; // Increase for faster turning (default is ~120-200)

        stateMachine.Initialise();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovementAdvanced>();
    }

    void Update()
    {
        DetectPlayerActions(); // Always check if player is running or shooting

        if (player == null || playerMovement == null)
            return;

        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            agent.isStopped = true;  // Stop walking
            if (!playerDetected)
            {
                suspicionTimer += Time.deltaTime;
                if (suspicionTimer >= 1f)
                {
                    playerDetected = true;
                    AttackPlayer();
                    debugState = "Attack";
                }
            }
        }
        else
        {
            agent.isStopped = false; // Resume walking when not seeing the player
            suspicionTimer = 0f;
            debugState = "Patrolling";
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
                debugState = "normal";
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

    public void DetectPlayerActions()
{
        bool isPlayerRunning = playerMovement.state == PlayerMovementAdvanced.MovementState.sprinting;
        bool isPlayerShooting = playerMovement.IsShooting;

        if ((isPlayerRunning || isPlayerShooting) && Vector3.Distance(transform.position, player.transform.position) <= sightDistance)
    {
        lastKnownPos = player.transform.position;
        alertMode = true;
        stateMachine.ChangeState(new SearchState());  // Start investigating
    }
}


}
