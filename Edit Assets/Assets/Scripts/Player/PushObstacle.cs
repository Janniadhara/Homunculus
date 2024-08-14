using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObstacle : MonoBehaviour
{
    [SerializeField] private float pushStrength;
    [SerializeField] private float playerWeight;
    private MoveAndLook MoveAndLookScript;
    private void Start()
    {
        MoveAndLookScript = GetComponent<MoveAndLook>();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;
        if (rigidbody != null)
        {
            Vector3 pushDirection = hit.gameObject.transform.position - transform.position;
            pushDirection.y = 0;
            pushDirection.Normalize();
            if (hit.transform.CompareTag("Item"))
            {
                if (MoveAndLookScript.isMoving)
                {
                    float pushForce = (1 - rigidbody.mass / playerWeight) * pushStrength;
                    rigidbody.AddForceAtPosition(pushDirection * pushForce, transform.position, ForceMode.Force);
                }
                else
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
            else if (hit.collider.CompareTag("Obstacle")) //still work to do
            {
                if (transform.position.y - hit.transform.position.y < 0)
                {
                    float pushForce = (1 - rigidbody.mass / playerWeight) * pushStrength;
                    rigidbody.AddForceAtPosition(pushDirection * pushForce, transform.position, ForceMode.Force);
                }
                else
                {
                    rigidbody.AddForceAtPosition(Vector3.down * playerWeight, hit.transform.position, ForceMode.Force);
                }
            }
        }
    }
}
