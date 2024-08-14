using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.GetComponent<HmsCalculator>().curHealth = 1;
            player.GetComponent<HmsCalculator>().isAlive = true;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = transform.position + Vector3.up;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
