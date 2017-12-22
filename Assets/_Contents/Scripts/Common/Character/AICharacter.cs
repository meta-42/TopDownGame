using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Sensor))]
public class AICharacter : Character {

    protected NavMeshAgent agent;
    protected Sensor sensor;


    protected override void Start() {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<Sensor>();

        rigid.constraints = RigidbodyConstraints.None |
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ;
    }

    protected override void UpdateControl(){
        //AI的移动由Agent处理，这里只做更新状态
        Movement(agent.velocity);
    }

    protected override void UpdateMovement() {
        //AI的移动由Agent处理，这里只做更新状态
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
        velocity = agent.velocity;
    }

    public override void Die() {
        base.Die();
        if (agent)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }



}
