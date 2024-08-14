using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter: " + other.name);
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit: " + other.name);
    }
}
