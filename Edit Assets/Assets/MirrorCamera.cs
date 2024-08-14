using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, Player.transform.position.y, transform.position.z);
        Vector3 PlayerEyes = Player.transform.position + Vector3.up * 0.7f;
        Vector3 origin = transform.position;
        Vector3 direction = -origin + PlayerEyes;
        direction.z *= -1;
        //direction.Normalize();
        Debug.DrawRay(origin, direction, Color.magenta);

        direction.Normalize();
        Quaternion toRotationMovement = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotationMovement, 500 * Time.deltaTime);

    }
}
