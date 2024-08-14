using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float underwaterDrag; //3f
    [SerializeField] private float underwaterAngularDrag; //1f
    [SerializeField] private float airDrag; //0f
    [SerializeField] private float airAngularDrag; //0.05f
    private float floatingForce; //15f
    [SerializeField] private float waterSlowingMultiplier = 0.8f;
    [SerializeField] private float waterFloatingMultiplier = 0.05f;
    private Rigidbody rb;
    private bool underwater;
    private Transform waterSurfaceObject;
    private float diff;

    public float totalFloatingForce;
    public float rbYVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rbYVelocity = rb.velocity.y;
        floatingForce = rb.mass * 100;
        if (waterSurfaceObject != null)
        {
            diff = transform.position.y - waterSurfaceObject.position.y - 0.5f;
        }
        if (diff < 0)
        {
            if (rbYVelocity < 0)
            {
                totalFloatingForce = Mathf.Lerp(totalFloatingForce, floatingForce, Time.deltaTime * waterSlowingMultiplier);
            }
            else
            {
                totalFloatingForce = Mathf.Lerp(totalFloatingForce, floatingForce, Time.deltaTime * waterFloatingMultiplier);
            }
            rb.AddForceAtPosition(Vector3.up * totalFloatingForce * Mathf.Abs(diff), transform.position, ForceMode.Force);
            if (!underwater)
            {
                underwater = true;
                SwitchState(true);
            }
        }
        else if (underwater)
        {
            totalFloatingForce = 0;
            underwater = false;
            SwitchState(false);
        }
    }
    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            rb.drag = underwaterDrag;
            rb.angularDrag = underwaterAngularDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
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
