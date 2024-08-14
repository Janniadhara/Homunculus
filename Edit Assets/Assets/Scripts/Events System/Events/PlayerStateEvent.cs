using System;
using UnityEngine;

public class PlayerStateEvent
{
    public event Action<bool> onPlayerSneaking;
    public void PlayerSneaking(bool sneaking)
    {
        if (onPlayerSneaking != null)
        {
            onPlayerSneaking(sneaking);
        }
    }
    public event Action<bool> onCanMove;
    public void CanMove(bool canMove)
    {
        if (onCanMove != null)
        {
            onCanMove(canMove);
        }
    }
    public event Action<bool> onCanTakeWeapon;
    public void CanTakeWeapon(bool canTakeWeapon)
    {
        if (onCanTakeWeapon != null)
        {
            onCanTakeWeapon(canTakeWeapon);
        }
    }
    public event Action<bool> onCanAttack;
    public void CanAttack(bool canAttack)
    {
        if (onCanAttack != null)
        {
            onCanAttack(canAttack);
        }
    }
    public event Action<AnimatorOverrideController> onDrawWeapon;
    public void DrawWeapon(AnimatorOverrideController weaponAnimations)
    {
        if (onDrawWeapon != null)
        {
            onDrawWeapon(weaponAnimations);
        }
    }
    public event Action onTakeWeaponInHand;
    public void TakeWeaponInHand()
    {
        if (onTakeWeaponInHand != null)
        {
            onTakeWeaponInHand();
        }
    }
    public event Action onSheathWeapon;
    public void SheathWeapon()
    {
        if (onSheathWeapon != null)
        {
            onSheathWeapon();
        }
    }
    public event Action onPutWeaponAway;
    public void PutWeaponAway()
    {
        if (onPutWeaponAway != null)
        {
            onPutWeaponAway();
        }
    }
    public event Action onAttackEnemy;
    public void AttackEnemy()
    {
        if (onAttackEnemy != null)
        {
            onAttackEnemy();
        }
    }
    public event Action onBeginAttack;
    public void BeginAttack()
    {
        if (onBeginAttack != null)
        {
            onBeginAttack();
        }
    }
    public event Action onEndAttack;
    public void EndAttack()
    {
        if (onEndAttack != null)
        {
            onEndAttack();
        }
    }
}
