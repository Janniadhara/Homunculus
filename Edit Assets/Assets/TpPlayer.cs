using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpPlayer : MonoBehaviour
{
    public Transform TpTransform;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.SetPositionAndRotation(TpTransform.position, TpTransform.rotation);
            other.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            other.transform.SetPositionAndRotation(TpTransform.position, TpTransform.rotation);
        }
    }
}
