using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeMovement : MonoBehaviour
{
    private Vector3 NextDestination;
    private NavMeshAgent agent;
    private Animator Animator;
    private Transform Parent;
    private Terrain terrain;
    private float idleSeconds;
    private bool idle;
    private float travelSeconds;
    private bool arrived;
    private CharacterController characterController;
    private float ySpeed;
    public float maxDistance { private get; set; }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Parent = transform.parent;
        terrain = Terrain.activeTerrain;
        arrived = false;
        idle = false;
        idleSeconds = 0;
        travelSeconds = 0;
        GetNextDestination();
        agent.destination = NextDestination;
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterController.isGrounded)
        {
            ySpeed += Time.deltaTime * 2;
            characterController.Move(Vector3.down * ySpeed);
        }
        if (idleSeconds > 0 && idle)
        {
            idleSeconds -= Time.deltaTime;
            agent.isStopped = true;
        }
        if (idleSeconds <= 0)
        {
            idle = false;
            agent.isStopped = false;
            travelSeconds += Time.deltaTime;
        }
        CheckIfArrived();
        Animator.SetFloat("Speed", agent.velocity.magnitude);
    }
    private void CheckIfArrived()
    {
        if (!idle && arrived)
        {
            arrived = false;
            //agent.destination = NextDestination;
            agent.SetDestination(NextDestination);
        }

        float xDif = transform.position.x - NextDestination.x;
        float zDif = transform.position.z - NextDestination.z;
        if ((xDif < 0.05 && zDif < 0.05) || travelSeconds > 40)
        {
            SetIdle();
            arrived = true;
            GetNextDestination();
        }
    }
    private void GetNextDestination()
    {
        float x = Random.Range(-maxDistance, maxDistance + 1);
        float z = Random.Range(-maxDistance, maxDistance + 1);
        NextDestination = new Vector3(Parent.position.x + x, 0, Parent.position.z + z);
        NextDestination.y = terrain.SampleHeight(NextDestination);
        if (Vector3.Distance(transform.parent.position, NextDestination) > maxDistance)
        {
            GetNextDestination();
        }
    }
    private void SetIdle()
    {
        idle = true;
        idleSeconds = Random.Range(5, 11);
        travelSeconds = 0;
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(NextDestination, 0.2f);
    }
}
