using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeWaypoint : MonoBehaviour
{
    public bool drawGizmo;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(transform);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}
