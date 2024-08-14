using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ApplePrefab;
    [SerializeField] private float respawnTime;
    [SerializeField] private float timeSincePicked;

    // Start is called before the first frame update
    void Start()
    {
        timeSincePicked = 0;
        if (transform.childCount == 0)
        {
            GameObject apple = Instantiate(ApplePrefab, transform);
            apple.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            timeSincePicked += Time.deltaTime;
            if (timeSincePicked >= respawnTime)
            {
                GameObject apple = Instantiate(ApplePrefab, transform);
                apple.GetComponent<Rigidbody>().useGravity = false;
                timeSincePicked = 0;
            }
        }
    }
}
