using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour , IDamageable {

    public UnityEvent onDead;
    public AudioClip dieSound;
    public Weapon equippedWeapon;

    //状态
    protected bool isCrouching = false;
    protected bool isGrounded = false;
    protected bool isDead = false;
    protected bool isAiming = false;

    [Range(0.1f,5)]
    [SerializeField]
    protected float speed = 5;

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
    protected Vector3 velocity;
    protected Vector3 moveRaw;
    protected Vector3 groundNormal;
    protected float defaultCapsuleHeight;
    protected Vector3 defaultCapsuleCenter;

    #region Public

    public bool Fire(bool continuously) {
        if (!isAiming) return false;

        if (equippedWeapon == null)
            return false;

        bool attackWasSuccessful;

        if (continuously)
            attackWasSuccessful = equippedWeapon.AttackContinuouslyHandle();
        else
            attackWasSuccessful = equippedWeapon.AttackOnceHandle();


        if (attackWasSuccessful) {
            anim.SetTrigger("Fire");
        }

        return attackWasSuccessful;
    }

    public void EquipWeapon(Weapon Weapon) {
        if (Weapon) {
            SetCurrentWeapon(Weapon, equippedWeapon);
        }
    }

    public void UnEquipWeapon(Weapon Weapon) {
        if (Weapon && Weapon == equippedWeapon) {
            SetCurrentWeapon(null, Weapon);
        }
    }

    public void Movement(Vector3 move) {

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
        forwardAmount = move.z;
    }

    public void Crouching(bool crouch) {
        if (isGrounded && crouch) {
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

    public void Aiming(bool aiming) {
        if (isGrounded && aiming) {
            equippedWeapon.gameObject.SetActive(true);
            isAiming = true;
        } else {
            equippedWeapon.gameObject.SetActive(false);
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
        onDead.Invoke();
    }

    #endregion

    #region Interface

    public virtual void TakeDamage(DamageEventData damageData) {
        Die();
    }

    #endregion

    #region Private

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

    #endregion

    #region Cycle

    protected virtual void Start() {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        defaultCapsuleHeight = capsule.height;
        defaultCapsuleCenter = capsule.center;
        rigid.drag = 8;
        rigid.mass = 30;
    }

    protected virtual void Update() {
        if (!isDead) {
            UpdateControl();
        }
        CheckGroundStatus();
        UpdateMovement();
        UpdateAnimator();
    }

    protected virtual void UpdateControl() { }

    protected virtual void UpdateMovement() {
        //转向控制
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);

        velocity = Vector3.zero;

        var a = moveRaw.normalized;
        var b = transform.forward.normalized;
        if (Vector3.Dot(a, b) >= 0.9f) {
            //移动控制
            velocity = transform.forward * forwardAmount * speed;
            if (isCrouching || isAiming) velocity *= 0.5f;
            velocity.y = rigid.velocity.y;
            rigid.velocity = velocity;
        }


    }

    protected virtual void UpdateAnimator() {
        anim.SetFloat("Forward", velocity.magnitude, 0.01f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", isCrouching);
        anim.SetBool("OnGround", isGrounded);
        anim.SetBool("Aiming", isAiming);
    }
    #endregion
}
