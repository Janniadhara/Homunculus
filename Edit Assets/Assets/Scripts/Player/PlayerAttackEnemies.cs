using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerAttackEnemies : MonoBehaviour
{
    [Header("PlayerEquipment")]
    [SerializeField] private EquipmentObject PlayerEquipment;
    [SerializeField] private WeaponItem[] WeaponItems;
    [SerializeField] private Transform WeaponHolsterSlot;
    [SerializeField] private Transform WeaponHandSlot;
    public GameObject CurrentWeapon;

    private bool canMoveBody;
    private bool canTakeWeapon;
    private bool hasDrawnWeapon;
    private bool canAttack;

    [SerializeField] private GameObject CameraTarget;
    [SerializeField] private GameObject RaycastOriginOrigin;
    // Start is called before the first frame update
    void OnEnable()
    {
        EventsManager.Instance.playerStateEvent.onCanMove += CanPlayerMove;
        EventsManager.Instance.playerStateEvent.onCanTakeWeapon += CheckCanTakeWeapon;
        EventsManager.Instance.playerStateEvent.onCanAttack += CanPlayerAttack;

        EventsManager.Instance.playerStateEvent.onTakeWeaponInHand += TakeWeapon;
        EventsManager.Instance.playerStateEvent.onPutWeaponAway += PutWeaponAway;

        EventsManager.Instance.damageEvents.onAnimalInHitbox += DamageAnimal;
    }

    // Update is called once per frame
    void Update()
    {
        //take weapon or put away
        if (Input.GetKeyDown(KeyCode.Y) && canMoveBody && canTakeWeapon)
        {
            hasDrawnWeapon = !hasDrawnWeapon;
            if (hasDrawnWeapon)
            {
                canMoveBody = false;
                canTakeWeapon = false;
                canAttack = false;
                AnimatorOverrideController weaponAnimations =
                    (PlayerEquipment.Weapon as WeaponItem).weaponAnimations;
                EventsManager.Instance.playerStateEvent.DrawWeapon(weaponAnimations);
                EventsManager.Instance.playerStateEvent.CanMove(false);
            }
            else
            {
                canMoveBody = false;
                canTakeWeapon = false;
                canAttack = false;
                EventsManager.Instance.playerStateEvent.SheathWeapon();
                EventsManager.Instance.playerStateEvent.CanMove(false);
            }
        }
        //attacking
        if (Input.GetKeyDown(KeyCode.Mouse0) && canMoveBody && canAttack)
        {
            canMoveBody = false;
            canTakeWeapon = false;
            canAttack = false;
            EventsManager.Instance.playerStateEvent.AttackEnemy();
            //InventoryManager.Instance.AttackWithEquipedWeapon();
            EventsManager.Instance.playerStateEvent.CanMove(false);
            RotateTowardsLookDirection();
        }
        //items swapping for now
        if (Input.GetKeyDown(KeyCode.Alpha1) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[0];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[1];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[2];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[3];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[4];
            InventoryManager.Instance.EquipWeapon();
        }
    }
    private void TakeWeapon()
    {
        CurrentWeapon = WeaponHolsterSlot.transform.GetChild(0).gameObject;
        CurrentWeapon.transform.SetParent(WeaponHandSlot, false);
    }
    private void PutWeaponAway()
    {
        CurrentWeapon.transform.SetParent(WeaponHolsterSlot, false);
    }
    private void RotateTowardsLookDirection()
    {
        float rayEulerY = RaycastOriginOrigin.transform.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0, rayEulerY, 0);
        RaycastOriginOrigin.transform.localRotation = Quaternion.Euler(
            RaycastOriginOrigin.transform.localEulerAngles.x, -transform.eulerAngles.y + rayEulerY, 0);
    }
    private void CanPlayerMove(bool canMove)
    {
        this.canMoveBody = canMove;
    }
    private void CheckCanTakeWeapon(bool canTakeWeapon)
    {
        this.canTakeWeapon = canTakeWeapon;
    }
    private void CanPlayerAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }
    private void DamageAnimal(GameObject animal)
    {
        EventsManager.Instance.damageEvents.DamageAnimal(
            animal, 
            ((PlayerEquipment.Weapon) as WeaponItem).damageValue
            );
    }
}
