using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitscanWeapon : ShootWeapon
{
    public enum FireMode
    {
        //半自动
        SemiAuto,
        //全自动
        FullAuto
    }

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


        float spread = spreadAim;
        RaycastHit hitInfo;

        var firePos = new Vector3(user.transform.position.x,
            user.transform.position.y + user.GetComponent<CapsuleCollider>().height / 2,
            user.transform.position.z);


        Ray ray = new Ray(firePos, transform.forward);
        Vector3 spreadVector = user.transform.TransformVector(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f));
        ray.direction = Quaternion.Euler(spreadVector) * ray.direction;

        if (debugVisual) {
            Debug.DrawLine(firePos, firePos + user.transform.forward * distanceMax, Color.red, 1100);

        }

        if (Physics.Raycast(ray, out hitInfo, distanceMax, damageMask, QueryTriggerInteraction.Ignore)) {
            if (hitInfo.collider.gameObject == user) {
                return;
            }

            float impulse = hitImpact.GetImpulseAtDistance(hitInfo.distance, distanceMax);
            float damage = hitImpact.GetDamageAtDistance(hitInfo.distance, distanceMax);
            SpawnHitEffect(hitInfo);
            SpawnHitSound(hitInfo);
            var damageable = hitInfo.collider.GetComponent<IDamageable>();
            if (damageable != null) {
                var damageData = new DamageEventData(-damage, user, hitInfo.point, ray.direction, impulse);
                damageable.TakeDamage(damageData);
            } else if (hitInfo.rigidbody) {
                hitInfo.rigidbody.AddForceAtPosition(ray.direction * impulse, hitInfo.point, ForceMode.Impulse);
            }

        }

        if (tracer) {
            var temp = Instantiate(tracer, transform.position, Quaternion.LookRotation(ray.direction));
            Destroy(temp, 1);
        }

    }

    protected void SpawnHitEffect(RaycastHit hit) {
        var effect = hitImpact.GetHitEffect(hit);
        if (effect) {
            GameObject spawnedDecal = GameObject.Instantiate(effect, hit.point, Quaternion.LookRotation(hit.normal));
            spawnedDecal.transform.SetParent(hit.collider.transform);
        }
    }

    protected void SpawnHitSound(RaycastHit hit) {
        var sound = hitImpact.GetHitSound(hit);
        if (sound) {
            AudioSource.PlayClipAtPoint(sound, hit.point);
        }
    }

}
