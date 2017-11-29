using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeActionState : StateMachineBehaviour {

    [Tooltip("开始产生伤害的时间（normalizedTime）")]
    public float startDamageTime = 0.05f;
    [Tooltip("结束产生伤害的时间（normalizedTime）")]
    public float endDamageTime = 0.9f;

    [Tooltip("退出状态时，是否清空输入池")]
    public bool resetTrigger;

    private bool isActive;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime % 1 >= startDamageTime && stateInfo.normalizedTime % 1 <= endDamageTime && !isActive) {
            isActive = true;
            ActiveDamage(animator, true);
        } else if (stateInfo.normalizedTime % 1 > endDamageTime && isActive) {
            isActive = false;
            ActiveDamage(animator, false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (isActive) {
            isActive = false;
            ActiveDamage(animator, false);
        }

        if (resetTrigger)
            animator.ResetTrigger("Melee");
    }

    void ActiveDamage(Animator animator, bool value) {
        var melee = animator.GetComponent<Character>();
        if (melee)
            melee.SetActiveMelee(value);
    }
}
