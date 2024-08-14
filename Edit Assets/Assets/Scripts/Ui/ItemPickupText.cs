using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupText : MonoBehaviour
{
    [SerializeField] private GameObject ItemTextPrefab;
    // Start is called before the first frame update
    void Start()
    {
        EventsManager.Instance.pickUpItemEvent.onItemPickUp += ItemPickedUp;

        //remove children on start
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ItemPickedUp(ItemObject item, int amount)
    {
        GameObject prefab = Instantiate(ItemTextPrefab, transform, false);
        Text itemText = prefab.GetComponent<Text>();
        itemText.text = amount.ToString() + "x " + item.itemName;
    }
}
