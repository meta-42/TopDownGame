using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 处理伤害修正，冲击修正， 受击效果
/// </summary>
[Serializable]
public class HitImpact {

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

    [SerializeField]
    public GameObject metalHitEffect;
    [SerializeField]
    public GameObject sandHitEffect;
    [SerializeField]
    public GameObject stoneHitEffect;
    [SerializeField]
    public GameObject waterLeakEffect;
    [SerializeField]
    public GameObject woodHitEffect;
    [SerializeField]
    public GameObject[] fleshHitEffects;

    [SerializeField]
    public AudioClip[] defaultHitSound;

    public AudioClip GetHitSound(PhysicMaterial pm) {
        if (pm != null) {
            string materialName = pm.name;
            switch (materialName) {
                case "Metal":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "Sand":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "Stone":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "WaterLeak":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "Wood":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "Meat":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
                case "Character":
                    return defaultHitSound[UnityEngine.Random.Range(0, defaultHitSound.Length)];
            }
        }
        return null;
    }

    public AudioClip GetHitSound(RaycastHit hit) {
        return GetHitSound(hit.collider.sharedMaterial);
    }


    public GameObject GetHitEffect(PhysicMaterial pm) {
        if (pm != null) {
            string materialName = pm.name;
            switch (materialName) {
                case "Metal":
                    return metalHitEffect;
                case "Sand":
                    return sandHitEffect;
                case "Stone":
                    return stoneHitEffect;
                case "WaterLeak":
                    return waterLeakEffect;
                case "Wood":
                    return woodHitEffect;
                case "Meat":
                    return fleshHitEffects[UnityEngine.Random.Range(0, fleshHitEffects.Length)];
                case "Character":
                    return fleshHitEffects[UnityEngine.Random.Range(0, fleshHitEffects.Length)];
            }
        }
        return null;
    }

    public GameObject GetHitEffect(RaycastHit hit) {
        return GetHitEffect(hit.collider.sharedMaterial);
    }

    public float GetDamage() {
        return damageMax;
    }

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

