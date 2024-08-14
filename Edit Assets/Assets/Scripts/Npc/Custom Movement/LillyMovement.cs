using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LillyMovement : MonoBehaviour
{
    private CharacterController npcController;
    private Animator npcAnimator;
    [SerializeField] private WayPoints[] wayPoints;
    private Transform curWaypoint;
    [SerializeField] private float moveSpeed;
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
            if (Vector3.Distance(transform.position, curWaypoint.position) < 0.4)
            {
                curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            }
            moveSpeed = 2;
            MoveToWaypoint(curWaypoint.position);

            if (wayPoints[0].WaitBevoreFollowing())
            {
                followWaypoints = false;
                idle = true;
            }
            npcAnimator.SetFloat("f_Speed", 0);
            npcAnimator.SetBool("b_isMoving", true);
        }
        else if (!followWaypoints && idle)
        {
            if (isIdleTime < idleTime)
            {
                isIdleTime += Time.deltaTime;
                npcAnimator.SetBool("b_isMoving", false);
                npcAnimator.SetBool("b_isFarming", true);
            }
            else
            {
                followWaypoints = true;
                idle = false;
                npcAnimator.SetBool("b_isFarming", false);
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
        Vector3 velocity = transform.forward * moveSpeed;
        velocity = SmoothSlope(velocity);
        velocity.y += ySpeed;
        npcController.Move(velocity * Time.deltaTime);
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
