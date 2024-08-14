using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoMovement : MonoBehaviour
{
    private CharacterController npcController;
    private Animator npcAnimator;
    [SerializeField] private WayPoints[] wayPoints;
    private Transform curWaypoint;
    [Header("Speeds")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    private float ySpeed;

    [Header("Idle Config")]
    [SerializeField] private float idleTime;
    private float isIdleTime;

    [Header("States")]
    [SerializeField] private bool idle;
    [SerializeField] private bool followWaypoints;
    void Start()
    {
        npcController = GetComponent<CharacterController>();
        npcAnimator = GetComponent<Animator>();
        if (wayPoints != null)
        {
            curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
        }
        isIdleTime = 0;
        wayPoints[0].StartFollowing();
    }

    void Update()
    {
        //stick to ground
        ySpeed = -1;
        npcController.Move(Vector3.up * ySpeed * Time.deltaTime);
        if (followWaypoints && !idle)
        {
            if (Vector3.Distance(transform.position, curWaypoint.position) < 1)
            {
                curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            }
            MoveToWaypoint(curWaypoint.position);

            if (wayPoints[0].WaitBevoreFollowing())
            {
                followWaypoints = false;
                idle = true;
            }
            //npcAnimator.SetFloat("f_Speed", 0);
            npcAnimator.SetBool("b_isMoving", true);
        }
        else if (!followWaypoints && idle)
        {
            if (isIdleTime < idleTime)
            {
                isIdleTime += Time.deltaTime;
                npcAnimator.SetBool("b_isMoving", false);
            }
            else
            {
                followWaypoints = true;
                idle = false;
                isIdleTime = 0;
                wayPoints[0].StartFollowing();
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        //move forward
        Vector3 velocity = transform.forward * moveSpeed;
        velocity.y += ySpeed;
        npcController.Move(velocity * Time.deltaTime);
    }
}
