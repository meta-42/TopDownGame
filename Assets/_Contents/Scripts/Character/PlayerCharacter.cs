using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class PlayerCharacter : Character
{
    public GameObject defaultAimTarget;

    GameObject aimTarget;
    Transform aimTargetPos;

    public float dashStamina = 30f;
    public float dashDistance = 5f;

    protected override void Start() {
        base.Start();

        SpawnDefaultAimTarget();
        rigid.constraints = RigidbodyConstraints.None | 
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

    }

    protected virtual void OnAnimatorIK(int layerIndex) {
        if (isAiming) {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, equippedShootWeapon.leftHandIK.position);
        }
    }

    protected override void UpdateControl() {
        base.UpdateControl();

        UpdateAimTarget();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = v * Vector3.forward + h * Vector3.right;
        Movement(move);
        Crouching(Input.GetKey(KeyCode.C));
        Aiming(Input.GetButton("Fire2"));

        if (Input.GetButton("Fire1")) {
            Fire(true);
        }

        if (Input.GetButtonDown("Fire1")) {
            if (isAiming) {
                Fire(false);
            } else {
                Melee();
            }

        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Dash();
        }
    }

    protected override void UpdateMovement() {
        base.UpdateMovement();
        if (isAiming) {
            transform.LookAt(aimTarget.transform.position, Vector3.up);
            equippedShootWeapon.transform.parent.LookAt(aimTargetPos.transform.position, Vector3.up);
        }
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(OnRestartLevel());
    }

    void Dash() {
        if (isAiming) return;
        if (stamina < dashStamina) return;
        if (moveRaw.magnitude <= 0) return;

        currentStaminaRecoveryDelay = 1f;
        ReduceStamina(dashStamina, false);

        Vector3 dashVelocity = Vector3.Scale(moveRaw, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigid.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * rigid.drag + 1)) / -Time.deltaTime)));
        rigid.AddForce(dashVelocity, ForceMode.VelocityChange);
        anim.Play("Dash");
    }

    void SpawnDefaultAimTarget() {
        if (defaultAimTarget) {
            Cursor.visible = false;
            aimTarget = Instantiate(defaultAimTarget);
            aimTargetPos = aimTarget.transform.Find("AimPos");
        }
    }

    void UpdateAimTarget() {

        if (!isAiming) {
            aimTarget.SetActive(false);
            return;
        }

        aimTarget.SetActive(true);
        if (!aimTarget) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000f, 9)) {
            Vector3 FinalPos = new Vector3(hit.point.x, 0, hit.point.z);

            aimTarget.transform.position = FinalPos;
            aimTarget.transform.LookAt(transform.position);
        }
    }

    IEnumerator OnRestartLevel() {
        yield return new WaitForSeconds(3);
        GameController.Instance.ReloadScene();
    }
}

