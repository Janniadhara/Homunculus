using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUnderwater : MonoBehaviour
{
    private SphereCollider col;
    private Transform waterSurfaceObject;
    private float diff;
    [SerializeField] private GameObject underWaterPlane;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();
        underWaterPlane.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (waterSurfaceObject != null)
        {
            diff = transform.position.y - waterSurfaceObject.position.y - 0.5f;
        }
        else
        {
            underWaterPlane.SetActive(false);
        }
        if (diff < 0)
        {
            underWaterPlane.SetActive(true);
        }
        else
        {
            underWaterPlane.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterSurfaceObject = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterSurfaceObject = null;
        }
    }
}
