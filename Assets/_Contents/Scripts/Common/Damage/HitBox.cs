using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    [HideInInspector]
    public MeleeWeapon weapon;

    [HideInInspector]
    public Collider trigger;

    bool CheckTrigger(Collider other) {
        return ((weapon != null && (weapon.user == null || other.gameObject != weapon.user.gameObject)));
    }

    void Start() {
        trigger = GetComponent<Collider>();
        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        if (trigger) {
            trigger.isTrigger = true;
            trigger.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (CheckTrigger(other)) {
            if (weapon != null) {
                weapon.OnHit(this, other);
            }
        }
    }

    void OnDrawGizmos() {

        trigger = gameObject.GetComponent<Collider>();

        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        Color color = Color.red;
        color.a = 0.6f;
        Gizmos.color = color;
        if (!Application.isPlaying && trigger && !trigger.enabled) trigger.enabled = true;
        if (trigger && trigger.enabled) {
            if (trigger as BoxCollider) {
                BoxCollider box = trigger as BoxCollider;

                var sizeX = transform.lossyScale.x * box.size.x;
                var sizeY = transform.lossyScale.y * box.size.y;
                var sizeZ = transform.lossyScale.z * box.size.z;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}

