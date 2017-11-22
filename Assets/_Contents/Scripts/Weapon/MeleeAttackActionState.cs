using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackActionState : StateMachineBehaviour {


    [Tooltip("开始产生伤害的时间（normalizedTime）")]
    public float startDamageTime = 0.05f;
    [Tooltip("结束产生伤害的时间（normalizedTime）")]
    public float endDamageTime = 0.9f;
    [Tooltip("允许移动/转向时的时间（normalizedTime）")]
    public float allowMovementTime = 0.9f;

    [Tooltip("是否要重置连击，连击最后一击时勾选此项")]
    public bool resetAttackTrigger;

    private bool isActive;
    private bool isAttacking;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        isAttacking = true;
        //animator.GetComponent<Character>().OnEnableAttack();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime % 1 >= startDamageTime && stateInfo.normalizedTime % 1 <= endDamageTime && !isActive) {
            isActive = true;
            ActiveDamage(animator, true);
        } else if (stateInfo.normalizedTime % 1 > endDamageTime && isActive) {
            isActive = false;
            ActiveDamage(animator, false);
        }

        if (stateInfo.normalizedTime % 1 > allowMovementTime && isAttacking) {
            isAttacking = false;
            //animator.GetComponent<Character>().OnDisableAttack();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (isActive) {
            isActive = false;
            ActiveDamage(animator, false);
        }

        isAttacking = false;
        //animator.GetComponent<Character>().OnDisableAttack();

        if (resetAttackTrigger)
            animator.GetComponent<Character>().ResetAttack();
    }

    void ActiveDamage(Animator animator, bool value) {
        var melee = animator.GetComponent<Character>();
        if (melee)
            melee.SetActiveAttack(value);
    }
}
