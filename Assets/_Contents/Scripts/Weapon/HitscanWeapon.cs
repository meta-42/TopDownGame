﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitscanWeapon : Weapon
{
    public enum FireMode
    {
        //半自动
        SemiAuto,
        //全自动
        FullAuto
    }



    [SerializeField]
    private RayImpact rayImpact;

    [SerializeField]
    private FireMode fireMode;

    [SerializeField]
    private SoundsPlayer fireAudio;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private GameObject tracer;

    [SerializeField]
    private GameObject infrared;

    [SerializeField]
    [Range(0f, 30f)]
    private float spreadAim = 0.95f;

    [SerializeField]
    private float distanceMax = 150f;

    [SerializeField]
    private LayerMask damageMask;

    [SerializeField]
    private int shotsPerMinute = 450;

    [SerializeField]
    private float shotDuration = 0.22f;

    private float timeBetweenShotsMin;
    private float nextTimeCanFire;
    private AudioSource audioSource;

    public bool debugVisual;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        infrared.SetActive(false);
        if (fireMode == FireMode.SemiAuto)
            timeBetweenShotsMin = shotDuration;
        else
            timeBetweenShotsMin = 60f / shotsPerMinute;
    }

    public override bool OnAttackOnce()
    {
        if (Time.time < nextTimeCanFire || !isEquiped)
            return false;

        nextTimeCanFire = Time.time + timeBetweenShotsMin;

        Shoot();

        return true;
    }

    public override bool OnAttackContinuously()
    {
        if (fireMode == FireMode.SemiAuto)
            return false;

        return OnAttackOnce();
    }

    public override void OnAiming(bool isAiming)
    {
        if (isAiming) {
            infrared.SetActive(true);
        } else {
            infrared.SetActive(false);
        }
    }

    protected void Shoot()
    {
        fireAudio.Play(SoundsPlayer.Selection.Randomly, audioSource, 1f);

        if (muzzleFlash)
            muzzleFlash.Play(true);

        DoHitscan();

    }

    protected void DoHitscan()
    {
        float spread = spreadAim; 
        RaycastHit hitInfo;

        var firePos = new Vector3(user.transform.position.x,
            user.transform.position.y + user.GetComponent<CapsuleCollider>().height / 2,
            user.transform.position.z);
        

        Ray ray = new Ray(firePos, user.transform.forward);
        Vector3 spreadVector = user.transform.TransformVector(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f));
        ray.direction = Quaternion.Euler(spreadVector) * ray.direction;

        if (debugVisual) {
            Debug.DrawLine(firePos, firePos + user.transform.forward * distanceMax, Color.red, 1100);

        }

        if (Physics.Raycast(ray, out hitInfo, distanceMax, damageMask, QueryTriggerInteraction.Ignore))
        {
            if(hitInfo.collider.gameObject == user) {
                return;
            }

            float impulse = rayImpact.GetImpulseAtDistance(hitInfo.distance, distanceMax);
            float damage = rayImpact.GetDamageAtDistance(hitInfo.distance, distanceMax);
            var damageable = hitInfo.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                var damageData = new DamageEventData(-damage, user, hitInfo.point, ray.direction, impulse);
                damageable.TakeDamage(damageData);
            }
            else if (hitInfo.rigidbody)
            {
                hitInfo.rigidbody.AddForceAtPosition(ray.direction * impulse, hitInfo.point, ForceMode.Impulse);
            }

        }

        if (tracer) {
            var temp = Instantiate(tracer, transform.position, Quaternion.LookRotation(ray.direction));
            Destroy(temp, 1);
        }
    }
}
