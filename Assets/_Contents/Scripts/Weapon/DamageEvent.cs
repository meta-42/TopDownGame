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

/// <summary>
/// 处理伤害修正，冲击修正（根据距离）
/// </summary>
[Serializable]
public class RayImpact {
    [Range(0f, 1000f)]
    [SerializeField]
    private float damageMax = 15f;

    [Range(0f, 1000f)]
    [SerializeField]
    private float impulseMax = 15f;

    [SerializeField]
    private AnimationCurve distanceCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(0.8f, 0.5f),
        new Keyframe(1f, 0f));


    public float GetDamageAtDistance(float distance, float maxDistance) {
        return ApplyCurveToValue(damageMax, distance, maxDistance);
    }

    public float GetImpulseAtDistance(float distance, float maxDistance) {
        return ApplyCurveToValue(impulseMax, distance, maxDistance);
    }

    private float ApplyCurveToValue(float value, float distance, float maxDistance) {
        float maxDistanceAbsolute = Mathf.Abs(maxDistance);
        float distanceClamped = Mathf.Clamp(distance, 0f, maxDistanceAbsolute);

        return value * distanceCurve.Evaluate(distanceClamped / maxDistanceAbsolute);
    }
}

