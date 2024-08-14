using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggressiveMobMovement : MonoBehaviour
{
    public bool useTerrain { private get; set; }
    [Header("Patrolling")]
    private Transform currentWaypoint;
    public WayPoints waypoints { private get; set; }
    public bool followWaypoints { private get; set; }
    [SerializeField] private bool waitAtEnd;
    [SerializeField] private bool waitAtEveryPoint;

    [Header("Wander around")]
    [SerializeField] private float minIdleSeconds;
    [SerializeField] private float maxIdleSeconds;
    private float actualIdleTime;
    private float idleTime;
    [SerializeField] private float maxTravelSeconds; //for now if model is stuck, it isn't for eternity
    public float travelSeconds;
    private Vector3 NextDestination;
    private bool idle;
    private bool hasArrived;
    public float MaxDistance { private get; set; } //get from parent object
    [Header("Interacting with player")]
    public bool playerInRange;
    private GameObject player;
    private bool followPlayer;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackDelay;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform Parent;
    private Terrain terrain;

    private CharacterController characterController;
    private float ySpeed;

    private void OnEnable()
    {
        EventsManager.Instance.detectPlayerEvent.onPlayerInRange += PlayerInRange;
        EventsManager.Instance.detectPlayerEvent.onPlayerOutRange += PlayerOutRange;
    }
    private void OnDisable()
    {
        EventsManager.Instance.detectPlayerEvent.onPlayerInRange -= PlayerInRange;
        EventsManager.Instance.detectPlayerEvent.onPlayerOutRange -= PlayerOutRange;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Parent = transform.parent;
        terrain = Terrain.activeTerrain;
        hasArrived = false;
        idle = false;
        idleTime = 0;
        travelSeconds = 0;
        if (followWaypoints)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            NextDestination = currentWaypoint.position;
        }
        else
        {
            GetNextDestination();
        }
        agent.destination = NextDestination;
    }

    // Update is called once per frame
    void Update()
    {
        //make model stick to ground
        if (!characterController.isGrounded)
        {
            ySpeed = 10;
            characterController.Move(Vector3.down * ySpeed);
        }
        else
        {
            ySpeed = 0;
        }
        if (playerInRange)
        {
            followPlayer = true;
        }
        if (followPlayer)
        {
            agent.SetDestination(player.transform.position);
            if (Vector3.Distance(transform.position, player.transform.position) > 15)
            {
                followPlayer = false;
            }
            if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            {
                agent.isStopped = true;
                animator.SetBool("b_attack", true);
                animator.SetFloat("Speed", 0);
                //AttackPlayer();
            }
            else
            {
                agent.isStopped = false;
                animator.SetBool("b_attack", false);
            }
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            if (idle)
            {
                idleTime += Time.deltaTime;
                agent.isStopped = true;
            }
            if (idleTime >= actualIdleTime)
            {
                idle = false;
                agent.isStopped = false;
                travelSeconds += Time.deltaTime;
            }
            if (CheckIfArrived() && !idle)
            {
                if (followWaypoints)
                {
                    if (waitAtEveryPoint)
                    {
                        SetIdle();
                    }
                    travelSeconds = 0;
                    currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                    NextDestination = currentWaypoint.position;
                }
                else
                {
                    SetIdle();
                    GetNextDestination();
                }
                agent.SetDestination(NextDestination);
            }
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        animator.SetBool("b_playerDetected", followPlayer);
    }
    private bool CheckIfArrived()
    {
        float xDif = transform.position.x - NextDestination.x;
        float zDif = transform.position.z - NextDestination.z;
        /*if ((xDif < 0.4 && zDif < 0.4) || travelSeconds > maxTravelSeconds)
        {
            return true;
        }*/
        if (Vector3.Distance(transform.position, NextDestination) < 0.5 || travelSeconds > maxTravelSeconds)
        {
            return true;
        }
        return false;
    }
    private void GetNextDestination()
    {
        float x = Random.Range(-MaxDistance, MaxDistance + 1);
        float z = Random.Range(-MaxDistance, MaxDistance + 1);
        NextDestination = new Vector3(Parent.position.x + x, 0, Parent.position.z + z);
        if (useTerrain)
        {
            NextDestination.y = terrain.SampleHeight(NextDestination);
        }
        else
        {
            NextDestination.y = Parent.position.y;
        }
        if (Vector3.Distance(transform.parent.position, NextDestination) > MaxDistance)
        {
            GetNextDestination();
        }
    }
    private void SetIdle()
    {
        idle = true;
        idleTime = 0;
        actualIdleTime = Random.Range(minIdleSeconds, maxIdleSeconds);
        travelSeconds = 0;
    }
    private void PlayerInRange(GameObject wildlife, GameObject player)
    {
        if (wildlife == gameObject)
        {
            playerInRange = true;
            this.player = player;
        }
    }
    private void PlayerOutRange(GameObject wildlife)
    {
        if (wildlife == gameObject)
        {
            playerInRange = false;
        }
    }
}
