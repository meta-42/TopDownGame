using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class PlayerCharacter : Character
{
    public GameObject defaultAimTarget;

    GameObject aimTarget;
    Transform aimTargetPos;

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
            anim.SetIKPosition(AvatarIKGoal.LeftHand, equippedWeapon.leftHandIK.position);
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
        Aiming(Input.GetButton("Fire3"));

        if (Input.GetButtonDown("Fire2")) {
            var targetPos = aimTarget.transform.position;
            var dir = (targetPos - transform.position).normalized;
            var distance = (targetPos - transform.position).magnitude;

            //transform.position = aimTarget.transform.position;
            //Vector3 dashVelocity = transform.forward * 100;
            Vector3 dashVelocity = Vector3.Scale(dir, distance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigid.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * rigid.drag + 1)) / -Time.deltaTime)));
            rigid.AddForce(dashVelocity, ForceMode.VelocityChange);
            transform.LookAt(targetPos);
        }

        if (isAiming) {
            if (Input.GetButton("Fire1")) {
                Fire(true);
            }
            if (Input.GetButtonDown("Fire1")) {
                Fire(false);
            }
        }


    }

    protected override void UpdateMovement() {
        base.UpdateMovement();
        if (isAiming) {
            transform.LookAt(aimTarget.transform.position);
            equippedWeapon.transform.parent.LookAt(aimTargetPos.position, Vector3.up);
        }
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(OnRestartLevel());
    }

    void SpawnDefaultAimTarget() {
        if (defaultAimTarget) {
            Cursor.visible = false;
            aimTarget = Instantiate(defaultAimTarget);
            aimTargetPos = aimTarget.transform.Find("AimPos");
        }
    }

    void UpdateAimTarget() {
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

