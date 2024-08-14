using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultNpcMovement : MonoBehaviour
{
    private CharacterController npcController;
    private NavMeshAgent agent;
    private Animator npcAnimator;
    private Terrain terrain;
    [Header("Waypoints Config")]
    [SerializeField] private WayPoints[] wayPoints;
    private Transform curWaypoint;

    [Header("Idle Config")]
    [SerializeField] private float idleTime;
    private float isIdleTime;

    [Header("States")]
    [SerializeField] private bool idle;
    [SerializeField] private bool followWaypoints;

    public float test;

    void Start()
    {
        npcController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        npcAnimator = GetComponent<Animator>();
        terrain = Terrain.activeTerrain;
        if (wayPoints.Length != 0 )
        {
            curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            wayPoints[0].StartFollowing();
        }
        isIdleTime = 0;
    }
    void Update()
    {
        //if follow waypoints dont use agent
        if (followWaypoints && !idle)
        {
            if (Vector3.Distance(transform.position, curWaypoint.position) < 0.6)
            {
                curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            }
            if (wayPoints[0].WaitBevoreFollowing())
            {
                followWaypoints = false;
                idle = true;
            }
            //MoveToWaypoint(curWaypoint.position);
            npcAnimator.SetFloat("f_Speed", 0);
            npcAnimator.SetBool("b_IsMoving", true);
            agent.SetDestination(curWaypoint.position);
            //agent.updateRotation = true;
        }
        else if (!followWaypoints && idle)
        {
            if (isIdleTime < idleTime)
            {
                isIdleTime += Time.deltaTime;
                npcAnimator.SetBool("b_IsMoving", false);
                agent.isStopped = true;
            }
            else
            {
                followWaypoints = true;
                agent.isStopped = false;
                idle = false;
                isIdleTime = 0;
                wayPoints[0].StartFollowing();
            }
        }
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = (transform.forward).normalized;
        Debug.DrawRay(origin, direction * 0.5f, Color.yellow);

        //when looking at something
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 0.5f))
        {
            if (hit.transform.CompareTag("Door"))
            {
                var animator = hit.transform.GetComponent<Animator>();
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Close State left"))
                {
                    animator.SetTrigger("openclose");
                }
            }
        }
    }
    private void MoveToWaypoint(Vector3 destination)
    {
        Vector3 npsPos = transform.position;
        Vector3 movementDirection = new Vector3(
            destination.x - npsPos.x,
            0,
            destination.z - npsPos.z
            );
        //rotae towards movement direction
        Quaternion rotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
        //move forward
        //Vector3 velocity = transform.forward * moveSpeed;
        //velocity = SmoothSlope(velocity);
        //velocity.y += ySpeed;
        //npcController.Move(velocity * Time.deltaTime);
    }
    private Vector3 SmoothSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var adjustVelocity = slopeRotation * velocity;

            if (adjustVelocity.y < 0)
            {
                return adjustVelocity;
            }
        }
        return velocity;
    }
}
