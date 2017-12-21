﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {

    public List<HitBox> hitBoxes;
    public bool debugVisual;

    private Dictionary<HitBox, List<GameObject>> hitObjctCache;
    private bool canApplyDamage;


    protected virtual void Start() {
        hitObjctCache = new Dictionary<HitBox, List<GameObject>>();

        if (hitBoxes.Count > 0) {
            foreach (HitBox hitBox in hitBoxes) {
                hitBox.weapon = this;
                hitObjctCache.Add(hitBox, new List<GameObject>());
            }
        } else {
            this.enabled = false;
        }
    }

    public virtual void SetActiveDamage(bool value) {
        canApplyDamage = value;
        for (int i = 0; i < hitBoxes.Count; i++) {
            var hitCollider = hitBoxes[i];
            hitCollider.trigger.enabled = value;
            if (value == false && hitObjctCache != null)
                hitObjctCache[hitCollider].Clear();
        }
    }

    public virtual void OnHit(HitBox hitBox, Collider other) {
        if (canApplyDamage && 
            !hitObjctCache[hitBox].Contains(other.gameObject) &&
            (owner != null && other.gameObject != owner.gameObject)) {

            hitObjctCache[hitBox].Add(other.gameObject);

            SpawnHitEffect(other);
            SpawnHitSound(other);

            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                var damageData = new DamageEventData(-hitImpact.GetDamage(), owner);
                damageable.TakeDamage(damageData);

            }
        }
    }

    protected void SpawnHitEffect(Collider other) {

        var effect = hitImpact.GetHitEffect(other.sharedMaterial);

        if (effect) {
            var dir = (owner.transform.position - other.transform.position).normalized;
            GameObject spawnedDecal = GameObject.Instantiate(effect, other.transform.position, Quaternion.LookRotation(dir));
            spawnedDecal.transform.SetParent(other.transform);
        }
    }

    protected void SpawnHitSound(Collider other) {

        var sound = hitImpact.GetHitSound(other.sharedMaterial);
        if (sound) {
            AudioSource.PlayClipAtPoint(sound, other.transform.position);
        }
    }
}

