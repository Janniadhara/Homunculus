using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWildlife : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private bool isSneaking;
    [SerializeField] private float normalRange;
    [SerializeField] private float sneakingRange;
    [SerializeField] private List<GameObject> animals = new List<GameObject>();
    private void OnEnable()
    {
        EventsManager.Instance.playerStateEvent.onPlayerSneaking += CheckSneaking;
    }
    private void OnDisable()
    {
        EventsManager.Instance.playerStateEvent.onPlayerSneaking -= CheckSneaking;
    }
    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {
        if (isSneaking)
        {
            sphereCollider.radius = sneakingRange;
        }
        else
        {
            sphereCollider.radius = normalRange;
        }
        if (Input.GetKeyDown(KeyCode.X)) 
        {
            foreach (GameObject go in animals)
            {
                EventsManager.Instance.damageEvents.DamageAnimal(go, 10);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        EventsManager.Instance.detectPlayerEvent.PlayerInRange(other.gameObject, gameObject);
        if (other.CompareTag("Animal"))
        {
            animals.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        EventsManager.Instance.detectPlayerEvent.PlayerOutRange(other.gameObject);
        if (other.CompareTag("Animal"))
        {
            animals.Remove(other.gameObject);
        }
    }
    private void CheckSneaking(bool sneaking)
    {
        isSneaking = sneaking;
    }
}
