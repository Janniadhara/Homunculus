using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFloorTriggerDown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //enable prev
        transform.GetComponentInParent<MazeFloor>().EnablePrevFloor();
        //disable next
        transform.GetComponentInParent<MazeFloor>().DisableNextFloor();
    }
}
