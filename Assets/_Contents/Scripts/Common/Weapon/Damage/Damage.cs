using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable {
    void TakeDamage(DamageEventData damageData);
}

public class DamageEventData {

    public float delta { get; set; }

    public Character attacker { get; private set; }

    public Vector3 hitPoint { get; private set; }

    public Vector3 hitDirection { get; private set; }

    public float hitImpulse { get; private set; }

    public DamageEventData(float rDelta, Character rAttacker = null, Vector3 rHitPoint = default(Vector3), Vector3 rHitDirection = default(Vector3), float rHitImpulse = 0f) {
        delta = rDelta;
        attacker = rAttacker;
        hitPoint = rHitPoint;
        hitDirection = rHitDirection;
        hitImpulse = rHitImpulse;
    }
}
