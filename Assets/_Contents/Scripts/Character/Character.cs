using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour , IDamageable {
    public AudioClip dieSound;

    public Weapon defaultWeapon;
    public Transform weaponSocket;

    public float health = 0f;
    public float stamina = 0f;

    public float dashStamina = 30f;
    public float aimingStamina = 30f;

    public float maxHealth = 100f;
    public float healthRecovery = 0f;
    public float maxStamina = 200f;
    public float staminaRecovery = 1.2f;

    [HideInInspector]
    public float currentStaminaRecoveryDelay;
    [HideInInspector]
    public float currentHealthRecoveryDelay;

    //状态
    protected bool isCrouching = false;
    protected bool isGrounded = false;
    protected bool isDead = false;
    protected bool isAiming = false;

    [Range(0.1f,3)]
    [SerializeField]
    protected float speed = 3;

    [Range(1f, 1000)]
    [SerializeField]
    protected float angularSpeed = 360;

    [SerializeField]
    protected float groundCheckDistance = 0.2f;

    protected Rigidbody rigid;
    protected Animator anim;
    protected CapsuleCollider capsule;
    protected float turnAmount;
    protected float forwardAmount;
    protected float rightAmount;
    protected Vector3 velocity;
    protected Vector3 moveRaw;
    protected Vector3 groundNormal;
    protected float defaultCapsuleHeight;
    protected Vector3 defaultCapsuleCenter;
    protected Weapon equippedWeapon;

    protected AnimatorStateInfo baseLayerInfo;
    protected AnimatorStateInfo underBodyInfo;
    protected AnimatorStateInfo upperBodyInfo;
    protected AnimatorStateInfo rightArmInfo;
    protected AnimatorStateInfo leftArmInfo;
    protected AnimatorStateInfo fullBodyInfo;


    #region Cycle

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        defaultCapsuleHeight = capsule.height;
        defaultCapsuleCenter = capsule.center;
        rigid.drag = 8;
        rigid.mass = 30;

        health = maxHealth;
        stamina = maxStamina;

        SpawnDefaultWeapon();
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            UpdateControl();
        }

        CheckHealth();
        CheckStamina();

        StaminaRecovery();
        HealthRecovery();

        CheckGroundStatus();

        UpdateWeapon();
        UpdateMovement();
        UpdateAnimator();
    }

    protected virtual void UpdateWeapon() {
        equippedWeapon.gameObject.SetActive(isAiming);
        equippedWeapon.OnAiming(isAiming);
    }

    protected virtual void UpdateControl() { }

    protected virtual void UpdateMovement()
    {
        if (isAiming)
        {
            velocity = (transform.forward * forwardAmount + transform.right * rightAmount) * speed;
        }
        else
        {
            transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);
            velocity = transform.forward * forwardAmount * speed;
        }

        if (isCrouching || isAiming) velocity *= 0.5f;
        velocity.y = rigid.velocity.y;
        rigid.velocity = velocity;

    }

    protected virtual void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Right", rightAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Speed", velocity.magnitude, 0.01f, Time.deltaTime);
        anim.SetBool("Crouch", isCrouching);
        anim.SetBool("OnGround", isGrounded);
        anim.SetBool("Aiming", isAiming);
    }

    protected virtual void OnAnimatorIK(int layerIndex)
    {
        if (isAiming)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, equippedWeapon.leftHandIK.position);
        }
    }

    #endregion

    #region Public

    public virtual bool Fire(bool continuously) {
        if (!isAiming) return false;

        if (equippedWeapon == null)
            return false;

        bool attackWasSuccessful;

        if (continuously)
            attackWasSuccessful = equippedWeapon.OnAttackContinuously();
        else
            attackWasSuccessful = equippedWeapon.OnAttackOnce();


        if (attackWasSuccessful) {
            anim.SetTrigger("Fire");
        }

        return attackWasSuccessful;
    }

    public virtual void EquipWeapon(Weapon Weapon) {
        if (Weapon) {
            SetCurrentWeapon(Weapon, equippedWeapon);
        }
    }

    public virtual void UnEquipWeapon(Weapon Weapon) {
        if (Weapon && Weapon == equippedWeapon) {
            SetCurrentWeapon(null, Weapon);
        }
    }

    public virtual void Movement(Vector3 move) {

        //当模大于1时，要进行归一化，防止在斜方向移动时，移动速度加快
        if (move.magnitude > 1f) move.Normalize();
        //存下原本的移动输入
        moveRaw = move;

        //将move从世界空间转向本地空间
        move = transform.InverseTransformDirection(move);
        //将move投影在地板的2D平面上
        move = Vector3.ProjectOnPlane(move, groundNormal);
        //返回值为x轴和一个（在零点起始，在(x, y)结束）的2D向量的之间夹角
        turnAmount = Mathf.Atan2(move.x, move.z);
        rightAmount = move.x;
        forwardAmount = move.z;

    }

    public virtual void Crouching(bool crouch) {
        if (isGrounded && !isAiming && crouch ) {
            if (isCrouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            isCrouching = true;
        } else {
            //限制头顶有遮挡时，必须蹲下
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * 0.5f, Vector3.up);
            float crouchRayLength = defaultCapsuleHeight - capsule.radius * 0.5f;
            if (Physics.SphereCast(crouchRay, capsule.radius * 0.5f, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                isCrouching = true;
                return;
            }
            capsule.height = defaultCapsuleHeight;
            capsule.center = defaultCapsuleCenter;
            isCrouching = false;
        }
    }

    public virtual void Aiming(bool aiming) {
        if (isGrounded && !isCrouching && aiming) {
            isAiming = true;
        } else {
            isAiming = false;
        }
    }

    public virtual void Die() {
        if (isDead) return;
        isDead = true;
        Movement(Vector3.zero);
        anim.Play("Die");
        capsule.height = 0.2f;
        capsule.center = new Vector3(0, 0.3f, 0);
        AudioSource.PlayClipAtPoint(dieSound, transform.position);
    }

    #endregion

    #region Interface

    public virtual void TakeDamage(DamageEventData damageData) {
        if (damageData == null) return;
        if (health <= 0) return;

        health += damageData.delta;
        if(health > 0)
        {
            anim.Play("Hit");
        }
    }

    #endregion

    #region Private

    void SpawnDefaultWeapon()
    {
        if (defaultWeapon)
        {
            Weapon NewWeapon = Instantiate(defaultWeapon, weaponSocket.position, weaponSocket.rotation);
            NewWeapon.transform.parent = weaponSocket.transform;
            NewWeapon.name = transform.name + "_" + NewWeapon.name;

            EquipWeapon(NewWeapon);
        }
    }

    void CheckGroundStatus() {
        RaycastHit hitInfo;

        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance)) {
            groundNormal = hitInfo.normal;
            isGrounded = true;
        } else {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    void SetCurrentWeapon(Weapon NewWeapon, Weapon LastWeapon /*= NULL*/) {
        Weapon LocalLastWeapon = null;

        if (LastWeapon != null) {
            LocalLastWeapon = LastWeapon;
        } else if (NewWeapon != equippedWeapon) {
            LocalLastWeapon = equippedWeapon;
        }

        if (LocalLastWeapon) {
            LocalLastWeapon.OnUnEquip();
        }

        equippedWeapon = NewWeapon;

        if (NewWeapon) {

            NewWeapon.OnEquip();
        }
    }


    void CheckHealth()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    void HealthRecovery()
    {
        if (health <= 0 || healthRecovery == 0) return;
        if (currentHealthRecoveryDelay > 0)
        {
            currentHealthRecoveryDelay -= Time.deltaTime;
        }
        else
        {
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            if (health < maxHealth)
            {
                health = Mathf.Lerp(health, maxHealth, healthRecovery * Time.deltaTime);
            }
        }
    }

    void CheckStamina()
    {
        if (isAiming)
        {
            currentStaminaRecoveryDelay = 0.25f;
            ReduceStamina(aimingStamina, true);
        }
    }

    public void StaminaRecovery()
    {
        if (currentStaminaRecoveryDelay > 0)
        {
            currentStaminaRecoveryDelay -= Time.deltaTime;
        }
        else
        {
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
            if (stamina < maxStamina)
            {
                stamina += staminaRecovery;
            }

        }
    }


    public void ReduceStamina(float value, bool accumulative)
    {

        if (accumulative)
        {
            stamina = stamina - value * Time.deltaTime;
        }
        else
        {
            stamina -= value;
        }
        if (stamina < 0)
        {
            stamina = 0;
        }
    }

    #endregion

    #region Anim

    public int baseLayer { get { return anim.GetLayerIndex("Base Layer"); } }
    public int underBodyLayer { get { return anim.GetLayerIndex("UnderBody"); } }
    public int rightArmLayer { get { return anim.GetLayerIndex("RightArm"); } }
    public int leftArmLayer { get { return anim.GetLayerIndex("LeftArm"); } }
    public int upperBodyLayer { get { return anim.GetLayerIndex("UpperBody"); } }
    public int fullbodyLayer { get { return anim.GetLayerIndex("FullBody"); } }

    public virtual void RefreshAnimatorState()
    {
        if (anim == null || !anim.enabled) return;
        baseLayerInfo = anim.GetCurrentAnimatorStateInfo(baseLayer);
        underBodyInfo = anim.GetCurrentAnimatorStateInfo(underBodyLayer);
        rightArmInfo = anim.GetCurrentAnimatorStateInfo(rightArmLayer);
        leftArmInfo = anim.GetCurrentAnimatorStateInfo(leftArmLayer);
        upperBodyInfo = anim.GetCurrentAnimatorStateInfo(upperBodyLayer);
        fullBodyInfo = anim.GetCurrentAnimatorStateInfo(fullbodyLayer);
    }

    public bool InAnimatorStateWithTag(string tag)
    {
        if (anim == null) return false;
        RefreshAnimatorState();
        if (baseLayerInfo.IsTag(tag)) return true;
        if (underBodyInfo.IsTag(tag)) return true;
        if (rightArmInfo.IsTag(tag)) return true;
        if (leftArmInfo.IsTag(tag)) return true;
        if (upperBodyInfo.IsTag(tag)) return true;
        if (fullBodyInfo.IsTag(tag)) return true;
        return false;
    }

    public bool InAnimatorStateWithName(string name)
    {
        RefreshAnimatorState();
        if (anim == null) return false;
        if (baseLayerInfo.IsName(name)) return true;
        if (underBodyInfo.IsName(name)) return true;
        if (rightArmInfo.IsName(name)) return true;
        if (leftArmInfo.IsName(name)) return true;
        if (upperBodyInfo.IsName(name)) return true;
        if (fullBodyInfo.IsName(name)) return true;
        return false;
    }

    public virtual void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
    {
        if (anim.isMatchingTarget || anim.IsInTransition(0))
            return;

        float normalizeTime = Mathf.Repeat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

        if (normalizeTime > normalisedEndTime)
            return;

        anim.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
    }
#endregion

}
