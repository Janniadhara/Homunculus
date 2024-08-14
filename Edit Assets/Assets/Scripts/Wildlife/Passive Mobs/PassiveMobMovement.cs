using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PassiveMobMovement : MonoBehaviour
{
    [Header("Wander around")]
    [SerializeField] private float minIdleSeconds;
    [SerializeField] private float maxIdleSeconds;
    [SerializeField] private float maxTravelSeconds; //for now if model is stuck, it isn't for eternity
    private Vector3 NextDestination;
    private float actualIdleTime;
    private float idleTime;
    private bool idle;
    private float travelSeconds;
    private bool hasArrived;
    public float MaxDistance { private get; set; } //get from parent object

    [Header("Interacting with player")]
    [SerializeField] private bool isScaredOfPlayer;
    private bool playerInRange;
    private GameObject player;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform parent;
    private Terrain terrain;
    //staying on ground
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
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        parent = transform.parent;
        terrain = Terrain.activeTerrain;
        hasArrived = false;
        idle = false;
        idleTime = 0;
        travelSeconds = 0;
        GetNextDestination();
        agent.destination = NextDestination;
    }
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
        if (isScaredOfPlayer && playerInRange)
        {
            idle = false;
            agent.isStopped = false;
            FleeDestination();
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
            CheckIfArrived();
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        if (isScaredOfPlayer)
        {
            animator.SetBool("b_playerDetected", playerInRange);
        }
    }
    private void CheckIfArrived()
    {
        if (!idle && hasArrived)
        {
            hasArrived = false;
            //agent.destination = NextDestination;
            agent.SetDestination(NextDestination);
        }

        float xDif = transform.position.x - NextDestination.x;
        float zDif = transform.position.z - NextDestination.z;
        if ((xDif < 0.05 && zDif < 0.05) || travelSeconds > 40)
        {
            SetIdle();
            hasArrived = true;
            GetNextDestination();
        }
    }
    private void GetNextDestination()
    {
        float x = Random.Range(-MaxDistance, MaxDistance);
        float z = Random.Range(-MaxDistance, MaxDistance);
        NextDestination = new Vector3(parent.position.x + x, 0, parent.position.z + z);
        NextDestination.y = terrain.SampleHeight(NextDestination);
        if (Vector3.Distance(transform.parent.position, NextDestination) > MaxDistance)
        {
            GetNextDestination();
        }
    }
    private void FleeDestination()
    {
        NextDestination = transform.position + (transform.position - player.transform.position);
        NextDestination.y = terrain.SampleHeight(NextDestination);
        agent.SetDestination(NextDestination);
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
