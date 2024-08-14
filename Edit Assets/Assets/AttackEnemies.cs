using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemies : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool waponDrawn;
    [SerializeField] private AnimatorOverrideController defaultAnimator;
    [SerializeField] private AnimatorOverrideController swordAnimator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        waponDrawn = false;
        animator.runtimeAnimatorController = defaultAnimator;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            waponDrawn = !waponDrawn;
            if (waponDrawn)
            {
                animator.Play("Draw Weapon 1");
                animator.runtimeAnimatorController = swordAnimator;
            }
            else
            {
                animator.Play("Sheath Weapon 1");
                animator.runtimeAnimatorController = defaultAnimator;
            }
            //animator.SetTrigger("t_drawWeapon");
            animator.SetBool("b_waponDrawn", waponDrawn);
        }
        if (Input.GetKeyDown(KeyCode.Q) && waponDrawn)
        {
            animator.SetTrigger("t_Attack");
        }
    }
    void StartAttack()
    {
        EventsManager.Instance.playerStateEvent.BeginAttack();
    }
    void EndAttack()
    {
        EventsManager.Instance.playerStateEvent.EndAttack();
    }
}
