using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerAbove : MonoBehaviour
{
    private GameObject Player;
    private GameObject CameraFollow;
    private Vector3 offset = new Vector3(0, 100, 0);
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        CameraFollow = GameObject.FindGameObjectWithTag("CinemachineTarget");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position + offset;
        transform.rotation = Quaternion.Euler(90, CameraFollow.transform.eulerAngles.y, 0);
    }
}
