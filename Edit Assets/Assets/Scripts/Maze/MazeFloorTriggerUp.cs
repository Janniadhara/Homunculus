using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFloorTriggerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //disable prev
        transform.GetComponentInParent<MazeFloor>().DisablePrevFloor();
        //enable next
        transform.GetComponentInParent<MazeFloor>().EnableNextFloor();
    }
}
