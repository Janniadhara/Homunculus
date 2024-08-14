using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingStance : MonoBehaviour
{
    private MoveAndLook MoveAndLookScript;

    public EquipmentObject PlayerEquipment;
    public GameObject WeaponHandSlot;
    private ItemObject Weapon;

    public bool fighting;
    public GameObject Sword;
    public bool atk;
    public bool isAttacking;
    public float atkSpeed = 2.3f;
    public float atkDuration = 1.3f;

    //[SerializeField] private float atkDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        MoveAndLookScript = GetComponent<MoveAndLook>();
        fighting = false;
        atk = false;
        isAttacking = false;
        Weapon = PlayerEquipment.Weapon;
        Sword = Weapon.Prefab;

        atkSpeed = 2.3f;
        atkDuration = 1.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (atkSpeed >= 2.2 && MoveAndLookScript.isGrounded)
        {
            atk = true;
        }
        else
        {
            atk = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!fighting)
            {
                Sword = Instantiate(Sword, WeaponHandSlot.transform);
                Sword.GetComponent<MeshCollider>().isTrigger = true;
                Sword.GetComponent<MeshCollider>().enabled = false;
                Sword.GetComponent<Rigidbody>().useGravity = false;
                fighting = true;
            }
            else if (fighting && atkSpeed >= 2.3f && atkDuration >= 1.3f)         
            {
                atk = true;
            }
        }

        if (atk)
        {
            atk = false;
            isAttacking = true;
            Sword.GetComponent<MeshCollider>().enabled = true;
        }

        if (isAttacking)
        {
            isAttacking = false;
        }
        if (!isAttacking)
        {
            atkSpeed += Time.deltaTime;
            atkDuration += Time.deltaTime;
        }
        

        if (atkSpeed <= 0)
        {
            Sword.GetComponent<MeshCollider>().enabled = false;
            atkSpeed = 2.3f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (!fighting)
            {
                var item = Instantiate(Weapon.Prefab, WeaponHandSlot.transform);
                item.GetComponent<MeshCollider>().isTrigger = true;
                item.GetComponent<MeshCollider>().enabled = false;
                item.GetComponent<Rigidbody>().useGravity = false;
                fighting = true;
            }
            else
            {
                Destroy(WeaponHandSlot.transform.GetChild(0).gameObject);
                Sword = Weapon.Prefab;
                fighting = false;
            }
        }
    }
}
