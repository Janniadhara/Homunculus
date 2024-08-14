using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(SphereCollider))]
public class ZombieMovement : MonoBehaviour
{
    [Header("Basic Movement")]
    [SerializeField] private WayPoints[] wayPoints;
    [SerializeField] private Transform startingWaypoint;
    [SerializeField] private bool waitAtEachWaypoint;
    private NavMeshAgent agent;
    private Animator npcAnimator;
    private Transform curWaypoint;

    [Header("Idle Config")]
    [SerializeField] private float idleTime;
    private float isIdleTime;

    //[Header("States")]
    private bool idle;
    private bool followWaypoints;
    private bool playerInRange;

    [Header("Player Aggro Config")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float attackRange;
    private SphereCollider detectionSphere;
    private Transform curTarget;
    public float distance;
    private bool enemyInRange;
    private bool followEnemy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcAnimator = GetComponent<Animator>();
        detectionSphere = GetComponent<SphereCollider>();
        if (wayPoints != null)
        {
            if (startingWaypoint != null)
            {
                curWaypoint = startingWaypoint;
            }
            else
            {
                curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            }
        }
        isIdleTime = 0;
        wayPoints[0].StartFollowing();
        agent.destination = curWaypoint.position;

        detectionSphere.radius = detectionRange;
    }

    void Update()
    {
        //stick to ground
        if (followWaypoints)
        {
            idle = false;
            agent.isStopped = false;
            agent.speed = 0.5f;
            npcAnimator.SetBool("b_isMoving", true);
            npcAnimator.SetFloat("f_moveSpeed", 0);

            CheckDistanceToWaypoint();
            agent.destination = curWaypoint.position;

            if (wayPoints[0].WaitBevoreFollowing())
            {
                followWaypoints = false;
                idle = true;
            }
            //npcAnimator.SetFloat("f_moveSpeed", agent.velocity.magnitude);
        }
        else if (idle)
        {
            npcAnimator.SetBool("b_isMoving", false);
            if (isIdleTime < idleTime)
            {
                isIdleTime += Time.deltaTime;
                agent.isStopped = true;
            }
            else
            {
                followWaypoints = true;
                idle = false;
                isIdleTime = 0;
                wayPoints[0].StartFollowing();
            }
        }
        else if (followEnemy)
        {
            agent.speed = 1;
            agent.SetDestination(curTarget.position);
            //npcAnimator.SetFloat("f_moveSpeed", agent.velocity.magnitude);
            npcAnimator.SetFloat("f_moveSpeed", 1);
            npcAnimator.SetBool("b_isMoving", true);
        }
        if (enemyInRange)
        {
            distance = Vector3.Distance(transform.position, curTarget.position);
            if (distance <= aggroRange)
            {
                followWaypoints = false;
                idle = false;
                isIdleTime = 0;
                followEnemy = true;
                agent.isStopped = false;
            }
            if (distance <= attackRange)
            {
                npcAnimator.SetTrigger("t_attackEnemy");
            }
            if (distance > detectionRange)
            {
                distance = 0;
                followEnemy = false;
            }
        }
        else
        {
            followWaypoints = true;
        }
        npcAnimator.SetBool("b_followEnemy", followEnemy);
    }
    private void CheckDistanceToWaypoint()
    {
        if (Vector3.Distance(transform.position, curWaypoint.position) < 0.4)
        {
            curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            if (waitAtEachWaypoint)
            {
                followWaypoints = false;
                idle = true;
            }
        }
    }
    private void MoveToWaypoint(Vector3 destination)
    {
        //Vector3 npsPos = transform.position;
        //Vector3 movementDirection = new Vector3(
        //    destination.x - npsPos.x,
        //    0,
        //    destination.z - npsPos.z
        //    );
        //rotae towards movement direction
        //Quaternion rotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        //move forward
        //Vector3 velocity = transform.forward * moveSpeed;
        //velocity.y += ySpeed;
        //npcController.Move(velocity * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curTarget = other.transform;
            enemyInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curTarget = null;
            enemyInRange = false;
        }
    }
}
