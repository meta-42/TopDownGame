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

    protected override void UpdateControl() {
        base.UpdateControl();

        UpdateAimTarget();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = v * Vector3.forward + h * Vector3.right;
        Movement(move);

        if (currentWeapon as MeleeWeapon) {
            if (Input.GetButtonDown("Fire1")) {
                Melee();
            }
        }

        if (currentWeapon as ShootWeapon) {
            if (Input.GetButton("Fire1")) {
                Shoot(true);
            }
            if (Input.GetButtonDown("Fire1")) {
                Shoot(false);
            }
        }

        if(!InAnimatorStateWithTag("Attack")){
            if (Input.GetAxis("Mouse ScrollWheel") < -0.1) {
                EquipPrevWeapon();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0.1) {
                EquipNextWeapon();
            }
        }


        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (InventoryPanel.Get() != null && !InventoryPanel.Get().gameObject.activeSelf) {
                InventoryPanel.Show();
                Cursor.visible = true;
            } else {
                InventoryPanel.Hide();
                Cursor.visible = false;
            } 
        }
    }

    protected override void UpdateMovement() {
        base.UpdateMovement();
        transform.LookAt(aimTarget.transform.position, Vector3.up);
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

