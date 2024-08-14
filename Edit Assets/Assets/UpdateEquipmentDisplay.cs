using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateEquipmentDisplay : MonoBehaviour
{
    [SerializeField] private GameObject HelmetSlot;
    [SerializeField] private GameObject ChestSlot;
    [SerializeField] private GameObject PantstSlot;
    [SerializeField] private GameObject BootsSlot;
    [SerializeField] private GameObject WeaponSlot;
    [SerializeField] private GameObject ShieldSlot;
    [SerializeField] private GameObject RingSlot;

    public EquipmentObject PlayerEquipment;
    public GameObject ItemDisplayPrefab;

    // Update is called once per frame
    void Update()
    {
        if (PlayerEquipment.Helmet != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, HelmetSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Helmet.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Chestplate != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, ChestSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Chestplate.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Pants != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, PantstSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Pants.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Shoes != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, BootsSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Shoes.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Gloves != null) //Slot needed
        {
            var Equipment = Instantiate(ItemDisplayPrefab, transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Gloves.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Weapon != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, WeaponSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Weapon.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Shield != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, ShieldSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Shield.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
        if (PlayerEquipment.Ring != null)
        {
            var Equipment = Instantiate(ItemDisplayPrefab, RingSlot.transform);
            var image = Equipment.transform.Find("ItemImage").GetComponent<Image>();
            image.sprite = PlayerEquipment.Ring.Picture;
            var amount = Equipment.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            amount.text = "";
        }
    }
}
