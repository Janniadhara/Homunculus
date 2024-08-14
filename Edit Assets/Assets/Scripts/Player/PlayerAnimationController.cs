using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator PlayerAnimator;
    private MoveAndLook MoveAndLookScript;
    private HmsCalculator HmsCalculatorScript;
    private FightingStance StanceScript;
    private float playerSpeed;
    private float playerIdle;

    private AttachedItem attachedItem;
    private int mineCount;

    //combat stuff
    [SerializeField] private AnimatorOverrideController defaultAnimator;
    [SerializeField] private AnimatorOverrideController swordAnimator;


    // Start is called before the first frame update
    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        MoveAndLookScript = GetComponentInParent<MoveAndLook>();
        HmsCalculatorScript = GetComponentInParent<HmsCalculator>();
        StanceScript = GetComponentInParent<FightingStance>();

        EventsManager.Instance.pickUpItemEvent.onHarvestPlant += HarvestPlant;
        EventsManager.Instance.pickUpItemEvent.onMineMineral += MineMineral;
        mineCount = 0;

        EventsManager.Instance.playerStateEvent.onDrawWeapon += DrawWeapon;
        EventsManager.Instance.playerStateEvent.onSheathWeapon += SheathWeapon;
        EventsManager.Instance.playerStateEvent.onAttackEnemy += AttackEnemy;

        EventsManager.Instance.playerStateEvent.CanTakeWeapon(true);
        EventsManager.Instance.playerStateEvent.CanAttack(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerSpeed = MoveAndLookScript.maxSpeed switch
        {
            1 => 0,
            2 => 1,
            5 => 2,
            10 => 3,
            _ => (float)1,
        };
        playerIdle = MoveAndLookScript.isSneaking switch
        {
            false => 0,
            true => 1,
        };
        PlayerAnimator.SetBool("b_isMoving", MoveAndLookScript.isMoving);
        PlayerAnimator.SetFloat("blend_Speed", playerSpeed, 0.05f, Time.deltaTime);
        PlayerAnimator.SetFloat("blend_Idle", playerIdle, 0.05f, Time.deltaTime);

        PlayerAnimator.SetBool("b_isGrounded", MoveAndLookScript.isGrounded);
        PlayerAnimator.SetBool("b_isJumping", MoveAndLookScript.isJumping);
        PlayerAnimator.SetBool("b_isFalling", MoveAndLookScript.isFalling);
        PlayerAnimator.SetBool("b_isSliding", MoveAndLookScript.isSliding);
        PlayerAnimator.SetBool("b_isSwimming", MoveAndLookScript.isSwimming);

        PlayerAnimator.SetBool("isAlive", HmsCalculatorScript.isAlive);
    }
    private void HarvestPlant(AttachedItem item)
    {
        PlayerAnimator.SetBool("b_harvest", true);
        attachedItem = item;
        Debug.Log("begin to harvest: " + attachedItem);
    }
    void PlantHarvested()
    {
        PlayerAnimator.SetBool("b_harvest", false);
        EventsManager.Instance.pickUpItemEvent.PlantHarvested(attachedItem);
        Debug.Log("harvested: " + attachedItem);
    }
    private void MineMineral(AttachedItem item)
    {
        PlayerAnimator.SetBool("b_mine", true);
        attachedItem = item;
        Debug.Log("begin to mine: " + attachedItem);
    }
    void MineralMined()
    {
        mineCount++;
        if(mineCount == 3)
        {
            PlayerAnimator.SetBool("b_mine", false);
            EventsManager.Instance.pickUpItemEvent.PlantHarvested(attachedItem);
            Debug.Log("mined: " + attachedItem);
            mineCount = 0;
        }
    }
    private void DrawWeapon(AnimatorOverrideController weaponAnimations)
    {
        PlayerAnimator.runtimeAnimatorController = weaponAnimations;
        PlayerAnimator.Play("Draw Weapon 1");
        PlayerAnimator.SetBool("b_waponDrawn", true);
    }
    private void SheathWeapon()
    {
        PlayerAnimator.Play("Sheath Weapon 1");
        PlayerAnimator.SetBool("b_waponDrawn", false);
    }
    void WeaponDrawn()
    {
        EventsManager.Instance.playerStateEvent.CanMove(true);
        EventsManager.Instance.playerStateEvent.CanTakeWeapon(true);
        EventsManager.Instance.playerStateEvent.CanAttack(true);
    }
    void TakeWeapon()
    {
        EventsManager.Instance.playerStateEvent.TakeWeaponInHand();
    }
    void WeaponSheathed()
    {
        EventsManager.Instance.playerStateEvent.CanMove(true);
        EventsManager.Instance.playerStateEvent.CanTakeWeapon(true);
        EventsManager.Instance.playerStateEvent.CanAttack(false);
        PlayerAnimator.runtimeAnimatorController = defaultAnimator;
    }
    void PutWeaponAway()
    {
        EventsManager.Instance.playerStateEvent.PutWeaponAway();
    }
    private void AttackEnemy()
    {
        PlayerAnimator.Play("Attack Sword");
    }
    void StartAttack()
    {
        EventsManager.Instance.playerStateEvent.BeginAttack();
    }
    void EndAttack()
    {
        EventsManager.Instance.playerStateEvent.EndAttack();
    }
    void EndOfAttackAnimation()
    {
        EventsManager.Instance.playerStateEvent.CanMove(true);
        EventsManager.Instance.playerStateEvent.CanTakeWeapon(true);
        EventsManager.Instance.playerStateEvent.CanAttack(true);
    }
}
