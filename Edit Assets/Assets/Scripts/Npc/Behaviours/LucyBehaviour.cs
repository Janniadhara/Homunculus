using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LucyBehaviour : MonoBehaviour
{
    private CharacterController npcController;
    private NavMeshAgent agent;
    private Animator npcAnimator;
    private Terrain terrain;
    [SerializeField] QuestInfo questInfo;
    [Header("Waypoints Config")]
    [SerializeField] private WayPoints[] wayPoints;
    private float maxDistanceToPlayer;
    public Transform curWaypoint;

    [Header("Idle Config")]
    [SerializeField] private float idleTime;
    private float isIdleTime;

    [Header("Dialogue")]
    public bool isTalkingToPlayer;

    [Header("States")]
    [SerializeField] private bool idle;
    [SerializeField] private bool followWaypoints;
    public bool canWalk;

    private GameObject Player;
    public float test;
    public int testint;
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onStartQuest += GoToMom;
        EventsManager.Instance.questEvents.onQuestStateChange += QuestStateChanged;
        EventsManager.Instance.dialogueEvent.onStartDialogue += CheckTalkingToMe;
        EventsManager.Instance.dialogueEvent.onFinishDialogue += FinishTalking;
    }
    private void OnDisable()
    {
        EventsManager.Instance.questEvents.onStartQuest -= GoToMom;
        EventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChanged;
        EventsManager.Instance.dialogueEvent.onStartDialogue += CheckTalkingToMe;
        EventsManager.Instance.dialogueEvent.onFinishDialogue += FinishTalking;
    }
    // Start is called before the first frame update
    void Start()
    {
        npcController = GetComponent<CharacterController>();
        //agent = GetComponent<NavMeshAgent>();
        npcAnimator = GetComponent<Animator>();
        terrain = Terrain.activeTerrain;
        Player = GameObject.FindGameObjectWithTag("Player");
        if (wayPoints.Length != 0)
        {
            curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
            testint = wayPoints[0].GetCurrentWaypointIndex(curWaypoint);
            wayPoints[0].StartFollowing();
        }
        isIdleTime = 0;
        maxDistanceToPlayer = 10;
        canWalk = true;
        isTalkingToPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {

        //if follow waypoints dont use agent
        if (followWaypoints && !idle && !isTalkingToPlayer)
        {
            //check distance to player
            Vector3 distanceToPlayer = Player.transform.position - gameObject.transform.position;
            float distance = distanceToPlayer.magnitude;
            test = distance;
            if (distance > maxDistanceToPlayer)
            {
                canWalk = false;
            }
            else
            {
                canWalk = true;
            }
            if (Vector3.Distance(transform.position, curWaypoint.position) < 0.6)
            {
                curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
                testint = wayPoints[0].GetCurrentWaypointIndex(curWaypoint);
            }
            if (wayPoints[0].WaitBevoreFollowing())
            {
                followWaypoints = false;
                idle = true;
            }
            MoveToWaypoint(curWaypoint.position);
            npcAnimator.SetFloat("f_Speed", 0);
            npcAnimator.SetBool("b_IsMoving", canWalk);
        }
        else if (!followWaypoints && idle)
        {
            npcAnimator.SetFloat("f_Speed", 0);
            npcAnimator.SetBool("b_IsMoving", false);
        }
        if (isTalkingToPlayer)
        {
            FacePlayer();
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
    private void FacePlayer()
    {
        //rotate towards the player to face them
        Vector3 playerPos = Player.transform.position;
        Vector3 npcPos = transform.position;
        Vector3 nTop = new Vector3(playerPos.x - npcPos.x, 0.0f, playerPos.z - npcPos.z);
        Quaternion rotation = Quaternion.LookRotation(nTop);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
    }
    void GoToMom(string questId)
    {
        if (questId == questInfo.id)
        {
            followWaypoints = true;
            idle = false;
        }
    }
    void QuestStateChanged(Quest quest)
    {
        if (quest.info.id == questInfo.id)
        {
            if (quest.state == QuestState.CAN_START)
            {
                followWaypoints = false;
                idle = true;
            }
            else if (quest.state == QuestState.IN_PROGRESS)
            {
                followWaypoints = true;
                idle = false;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                if (wayPoints[0].GetCurrentWaypointIndex(curWaypoint) == 0)
                {
                    followWaypoints = false;
                    idle = true;
                }
                else
                {
                    followWaypoints = true;
                    idle = false;
                    curWaypoint = wayPoints[0].GetNextWaypoint(curWaypoint);
                    wayPoints[0].StartFollowing();
                    maxDistanceToPlayer = 10000;
                }
            }
        }
    }
    private void CheckTalkingToMe(TextAsset npcJson, string npcName)
    {
        if (npcName == transform.name)
        {
            isTalkingToPlayer = true;
            npcAnimator.SetBool("b_IsMoving", false);
        }
    }
    private void FinishTalking(string npcName)
    {
        if (npcName == transform.name)
        {
            isTalkingToPlayer = false;
        }
    }
}
