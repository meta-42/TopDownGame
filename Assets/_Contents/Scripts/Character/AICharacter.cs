using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AICharacter : Character {

    NavMeshAgent agent;

    protected override void Start() {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        rigid.constraints = RigidbodyConstraints.None |
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ;
    }

    protected override void UpdateControl() {
        base.UpdateControl();
        Movement(agent.velocity);
    }

    protected override void UpdateMovement() {
        //转向控制
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);

        //移动控制
        speed = agent.speed;
        angularSpeed = agent.angularSpeed;
        velocity = agent.velocity;
    }


}
