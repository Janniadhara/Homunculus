using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMovement : MonoBehaviour
{
    private Vector3 NextDestination;
    [SerializeField] private GameObject WaypointPrefab;
    private Transform Parent;
    private Terrain terrain;
    [SerializeField] float speed;
    private float idleSeconds;
    private bool idle;
    private bool arrived;
    // Start is called before the first frame update
    void Start()
    {
        Parent = transform.parent;
        terrain = Terrain.activeTerrain;
        arrived = false;
        SetNextDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDirection = NextDestination - transform.position;
        if (movementDirection.magnitude <= 0.2f)
        {
            SetNextDestination();
        }
        if (NextDestination != null)
        {
            movementDirection.Normalize();
            float movementSpeed = movementDirection.magnitude * speed;
            Vector3 velocity = movementDirection * movementSpeed;
            Quaternion rotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
            transform.position += velocity * Time.deltaTime;
        }
        else if (NextDestination == null)
        {
            SetNextDestination();
        }
    }
    private void SetNextDestination()
    {
        int x = Random.Range(-8, 9);
        int y = Random.Range(1, 9);
        int z = Random.Range(-8, 9);
        NextDestination = new Vector3(Parent.position.x + x, Parent.position.y + y, Parent.position.z + z);
        //Instantiate(WaypointPrefab, NextDestination, transform.rotation);
    }
    private void OnTriggerEnter(Collider other)
    {
        //SetNextDestination();
        //Destroy(other.gameObject);
    }
}
