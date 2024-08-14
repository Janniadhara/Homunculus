using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSlimeManager : MonoBehaviour
{
    [SerializeField] private float spawnRange;
    [SerializeField] private float maxDistanceFromParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistanceFromParent);
    }
}
