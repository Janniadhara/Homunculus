using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeManager : MonoBehaviour
{
    [Range(0f, 20f)]
    [SerializeField] private float range;
    private void OnDrawGizmos()
    {
        Vector3 vector3 = new Vector3(range, range, range);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, vector3 * 2);
    }
}
