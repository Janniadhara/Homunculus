using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestResource : MonoBehaviour
{
    [SerializeField] private float regrowTime;
    [SerializeField] private string ressourceTag;
    private float timeSinceLastHarvest;
    private bool canHarvest;

    [SerializeField] private AttachedItem attachedItem;
    [SerializeField] private ParticleSystem droppedItemParticles;
    [SerializeField] private bool dropItems;
    private GameObject player;
    private SphereCollider sphereCollider;
    void Start()
    {
        transform.tag = ressourceTag;
        transform.GetChild(0).gameObject.SetActive(true);
        timeSinceLastHarvest = 0;
        canHarvest = true;
        player = GameObject.FindGameObjectWithTag("Player");
        sphereCollider = transform.GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (!canHarvest)
        {
            timeSinceLastHarvest += Time.deltaTime;
            if (timeSinceLastHarvest > regrowTime)
            {
                Regrow();
            }
        }
    }
    public void Harvest()
    {
        transform.tag = "Untagged";
        transform.GetChild(0).gameObject.SetActive(false);
        timeSinceLastHarvest = 0;
        canHarvest = false;
        transform.GetComponent<Outline>().enabled = false;
        if (!dropItems)
        {
            EventsManager.Instance.pickUpItemEvent.ItemPickUp(attachedItem.item, attachedItem.amount);
        }
        else
        {
            for (int i = 0; i < attachedItem.amount; i++)
            {
                GameObject itemPrefab = Instantiate(attachedItem.item.Prefab, transform.position + ((i+1) * 0.1f * Vector3.forward) + Vector3.up * 1.4f - Vector3.forward, transform.rotation);
                ParticleSystem itemParticle = droppedItemParticles;
                Instantiate(itemParticle, itemPrefab.transform, false);
            }
        }
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }
    }
    private void Regrow()
    {
        transform.tag = ressourceTag;
        transform.GetChild(0).gameObject.SetActive(true);
        timeSinceLastHarvest = 0;
        canHarvest = true;
        if (sphereCollider != null)
        {
            sphereCollider.enabled = true;
        }
    }
}
