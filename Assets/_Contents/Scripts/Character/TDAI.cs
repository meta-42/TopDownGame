using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDAI : AICharacter {

	public WaypointGroup waypointGroup;
    int currentIndex = 0;

    protected override void Start() {
        base.Start();
    }

	protected override void UpdateControl() {
        UpdatePatrol();
        base.UpdateControl();
    }
    void UpdatePatrol() {

        var points = waypointGroup.waypoints;

        var targetPos = points[currentIndex].transform.position;

        if (!agent.SetDestination(targetPos)) {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) {
            currentIndex = (currentIndex + 1) % points.Count;
        }
    }
}
