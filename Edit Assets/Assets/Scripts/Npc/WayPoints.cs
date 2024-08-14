using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private List<Transform> WayPointTransform;
    [SerializeField] private bool loop;
    [SerializeField] private bool random;
    [SerializeField] private bool waitAtEnd;
    private Transform NextWaypoint;
    private bool forward = true;
    private bool wait;

    [Header("Gizmo")]
    [Range(0, 5)]
    [SerializeField] private float gizmoRadius;
    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }
        //looping
        if (loop && !random)
        {
            if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
            {
                return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
            }
            else
            {
                return transform.GetChild(0);
            }
        }
        //first to last and back
        else if (!loop && !random)
        {
            if (forward && currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
            {
                return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
            }
            else if (forward && currentWaypoint.GetSiblingIndex() == transform.childCount - 1)
            {
                if (waitAtEnd)
                {
                    StopFollowing();
                }
                forward = false;
                return transform.GetChild(currentWaypoint.GetSiblingIndex() - 1);
            }
            if (!forward && currentWaypoint.GetSiblingIndex() > 0)
            {
                return transform.GetChild(currentWaypoint.GetSiblingIndex() - 1);
            }
            else if (!forward && currentWaypoint.GetSiblingIndex() == 0)
            {
                if (waitAtEnd)
                {
                    StopFollowing();
                }
                forward = true;
                return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
            }
        }
        //random location
        else if (!loop && random)
        {
            if (waitAtEnd)
            {
                StopFollowing();
            }
            int randomChild = Random.Range(0, transform.childCount);
            return transform.GetChild(randomChild);
        }
        return transform.GetChild(0);
    }
    public Transform GetRandomWaypoint()
    {
        return transform.GetChild(Random.Range(0, transform.childCount - 1));
    }
    public bool WaitBevoreFollowing()
    {
        return wait;
    }
    public int GetCurrentWaypointIndex(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = transform.GetChild(0);
        }
        return currentWaypoint.GetSiblingIndex();
    }
    public void StartFollowing()
    {
        wait = false;
    }
    private void StopFollowing()
    {
        wait = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (Transform t in transform) 
        {
            Gizmos.DrawWireSphere(t.position, gizmoRadius);
        }
        Gizmos.color = Color.red;
        if (!random)
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawLine(transform.position, transform.GetChild(i).position);
            }
        }
        if (loop)
        {
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
        }
    }
}
