using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour , IDamageable {
    
    [SerializeField]
    protected Transform weaponSocket;
    [SerializeField]
    protected float speed = 3;
    [SerializeField]
    protected float angularSpeed = 360;
    [SerializeField]
    protected float groundCheckDistance = 0.2f;
    [SerializeField]
    int[] defaultInventory = null;

    public float health = 0f;
    public float stamina = 0f;
    public float maxHealth = 100f;
    public float healthRecovery = 0f;
    public float maxStamina = 200f;
    public float staminaRecovery = 1.2f;
    public List<Weapon> inventory = new List<Weapon>();

    protected float currentStaminaRecoveryDelay;
    protected float currentHealthRecoveryDelay;
    protected bool isGrounded = false;
    protected bool isDead = false;
    protected Rigidbody rigid;
    protected Animator anim;
    protected CapsuleCollider capsule;
    protected float turnAmount;
    protected float forwardAmount;
    protected float rightAmount;
    protected Vector3 velocity;
    protected Vector3 moveRaw;
    protected Vector3 groundNormal;
    protected Weapon currentWeapon;


    #region Property
    public bool isPlayer {
        get{
            if(this.GetType() == typeof(PlayerCharacter)){
                return true;
            }else{
                return false;
            }
        }
    }

    public int baseLayer { get { return anim.GetLayerIndex("Base Layer"); } }
    public int underBodyLayer { get { return anim.GetLayerIndex("Under Body"); } }
    public int rightArmLayer { get { return anim.GetLayerIndex("Right Arm"); } }
    public int leftArmLayer { get { return anim.GetLayerIndex("Left Arm"); } }
    public int upperBodyLayer { get { return anim.GetLayerIndex("Upper Body"); } }
    public int fullbodyLayer { get { return anim.GetLayerIndex("Full Body"); } }

    #endregion

    #region Base

    protected virtual void Start()
    {

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        rigid.drag = 5;
        rigid.mass = 30;

        health = maxHealth;
        stamina = maxStamina;

        SpawnDefaultInventory();
    }

    void OnDestroy() {
        ClearInventory();
    }

    protected virtual void Update()
    {
       if (!isDead)
       {
           UpdateControl();
       }

       UpdateStatus();
       UpdateMovement();
       UpdateAnimator();
    }

    protected virtual void FixedUpdate() {
        if (InAnimatorStateWithTag("RootMotion")) return;
        if (!isPlayer) return;
        rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
    }

    protected virtual void UpdateStatus() {
        CheckGround();
        StaminaRecovery();
        HealthRecovery();
    }

    protected virtual void UpdateControl() { }

    protected virtual void UpdateMovement()
    {
        velocity = (transform.forward * forwardAmount + transform.right * rightAmount) * speed;
        velocity.y = rigid.velocity.y;
    }

    protected virtual void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Right", rightAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Speed", velocity.magnitude, 0.01f, Time.deltaTime);
        anim.SetBool("OnGround", isGrounded);
        anim.SetBool("Aiming",(currentWeapon as ShootWeapon) != null);

    }

    #endregion

    #region Die
    public virtual void Die() {
        if (isDead) return;
        isDead = true;
        Movement(Vector3.zero);
        anim.Play("Die");
        capsule.height = 0.1f;
        capsule.center = new Vector3(0, 0.3f, 0);
    }

    #endregion

    #region Locomotion
    
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
    #endregion

    #region Melee

    public virtual void Melee() {

        if (currentWeapon as MeleeWeapon == null) return; 

        anim.SetTrigger("Melee");
    }

    public virtual void SetActiveMelee(bool active) {
        if (currentWeapon as MeleeWeapon == null) return;

        (currentWeapon as MeleeWeapon).SetActiveDamage(active);
    }

    #endregion

    #region Shoot
    
    public virtual bool Shoot(bool continuously) {
        var weapon = (currentWeapon as ShootWeapon);

        if (weapon == null)
            return false;

        bool successful;

        if (continuously)
            successful = weapon.OnAttackContinuously();
        else
            successful = weapon.OnAttackOnce();

        if (successful) {
            anim.SetTrigger("Shoot");
        }

        return successful;
    }
    
    #endregion

    #region Inventory
    
    void AddWeaponToInventory(Weapon weapon) {

        if (!weapon) return;

        inventory.Add(weapon);
        weapon.OnEnterInventory(this);
        if (InventoryPanel.Get()) {
            InventoryPanel.Get().RefreshEquipInventory();
        }
    }

    void RemoveWeaponFromInventory(Weapon weapon) {

        if (!weapon) return;

        inventory.Remove(weapon);
        weapon.OnLeaveInventory();

        if (InventoryPanel.Get()) {
            InventoryPanel.Get().RefreshEquipInventory();
        }

    }

    void ClearInventory() {
        for (int i=0; i < inventory.Count; i++) {
            var item = inventory[i];
            if (!item) continue;
            RemoveWeaponFromInventory(item);
            Destroy(item);
        }
    }

    void SpawnDefaultInventory()
    {
        foreach(var id in defaultInventory) {
            var itemData = DataTable.Get<ItemData>(id);
            var item = Resources.Load<Weapon>("Weapon/" + itemData.name);
            if (!item) return;
            Transform socket = null;
            if (item as ShootWeapon) {
                socket = weaponSocket.Find("ShootSocket");
            } else if(item as MeleeWeapon) {
                socket = weaponSocket.Find("MeleeSocket");
            }
            Weapon newWeapon = Instantiate(item, socket.position, socket.rotation);
            newWeapon.transform.parent = socket.transform;
            AddWeaponToInventory(newWeapon);
        }

        if(inventory.Count > 0) {
            EquipWeapon(inventory[0]);
        }
    }


    #endregion

    #region Weapon

    public void EquipNextWeapon() {
        if(inventory.Count >= 2) {
            var currentIndex = inventory.IndexOf(currentWeapon);
            var nextIndex = (currentIndex + 1) % inventory.Count;
            var nextWeapon = inventory[nextIndex];
            EquipWeapon(nextWeapon);
        }
    }

    public void EquipPrevWeapon() {
        if (inventory.Count >= 2) {
            var currentIndex = inventory.IndexOf(currentWeapon);
            var prevIndex = (currentIndex - 1 + inventory.Count) % inventory.Count;
            var prevWeapon = inventory[prevIndex];
            EquipWeapon(prevWeapon);
        }
    }

    public virtual void EquipWeapon(Weapon newWeapon) {
        if (!newWeapon) return;
        SetCurrentWeapon(newWeapon, currentWeapon);
    }

    public virtual void UnEquipWeapon(Weapon newWeapon) {
        if (newWeapon && newWeapon == currentWeapon) {
            SetCurrentWeapon(null, newWeapon);
        }
    }

    void SetCurrentWeapon(Weapon newWeapon, Weapon lastWeapon) {
        Weapon LocalLastWeapon = null;

        if (lastWeapon != null) {

            LocalLastWeapon = lastWeapon;

        } else if (newWeapon != currentWeapon) {

            LocalLastWeapon = currentWeapon;

        }

        if (LocalLastWeapon) LocalLastWeapon.OnUnEquip();

        currentWeapon = newWeapon;

        if (newWeapon) newWeapon.OnEquip();

    }

    #endregion

    #region Damage

    public virtual void TakeDamage(DamageEventData damageData) {
        if (damageData == null) return;

        if (health > Mathf.Abs(damageData.delta) || damageData.delta > 0) {
            health += damageData.delta;
            anim.Play("Hit");
        } else {
            if (!isDead) Die();
        }
    }

    #endregion

    #region Status
    
    void CheckGround() {
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

    #region Animation

    public bool InAnimatorStateWithTag(string tag)
    {
        if (anim == null) return false;
        if (anim.GetCurrentAnimatorStateInfo(baseLayer).IsTag(tag)) return true;
        if (anim.GetCurrentAnimatorStateInfo(underBodyLayer).IsTag(tag)) return true;
        if (anim.GetCurrentAnimatorStateInfo(rightArmLayer).IsTag(tag)) return true;
        if (anim.GetCurrentAnimatorStateInfo(leftArmLayer).IsTag(tag)) return true;
        if (anim.GetCurrentAnimatorStateInfo(upperBodyLayer).IsTag(tag)) return true;
        if (anim.GetCurrentAnimatorStateInfo(fullbodyLayer).IsTag(tag)) return true;
        return false;
    }

    public bool InAnimatorStateWithName(string name)
    {
        if (anim == null) return false;
        if (anim.GetCurrentAnimatorStateInfo(baseLayer).IsName(name)) return true;
        if (anim.GetCurrentAnimatorStateInfo(underBodyLayer).IsName(name)) return true;
        if (anim.GetCurrentAnimatorStateInfo(rightArmLayer).IsName(name)) return true;
        if (anim.GetCurrentAnimatorStateInfo(leftArmLayer).IsName(name)) return true;
        if (anim.GetCurrentAnimatorStateInfo(upperBodyLayer).IsName(name)) return true;
        if (anim.GetCurrentAnimatorStateInfo(fullbodyLayer).IsName(name)) return true;
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
