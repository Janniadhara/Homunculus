using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiItemName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ItemDescriptionText;
    [SerializeField] private TextMeshProUGUI ItemDescriptionTextReal;
    void Update()
    {
        //transform.position = Input.mousePosition + new Vector3(-5, 0, 0);
    }

    public void SetItemName(string itemName)
    {
        ItemDescriptionText.text = itemName;
        ItemDescriptionTextReal.text = itemName;
    }
}
