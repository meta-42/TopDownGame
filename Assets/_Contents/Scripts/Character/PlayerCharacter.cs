using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class PlayerCharacter : Character
{
    bool canControl = true;

    public GameObject AimTargetVisual;



    private void UpdateAimTargetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000f, 9))
        {
            Vector3 FinalPos = new Vector3(hit.point.x, 0, hit.point.z);

            AimTargetVisual.transform.position = FinalPos;
            AimTargetVisual.transform.LookAt(transform.position);

        }

        if (isAiming)
        {
            transform.LookAt(AimTargetVisual.transform.position);
        }
    }


    protected override void Start() {
        base.Start();

        rigid.constraints = RigidbodyConstraints.None | 
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        EquipWeapon(equippedWeapon);
    }

    protected override void UpdateControl() {
        base.UpdateControl();

        UpdateAimTargetPos();

        if (!canControl) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = v * Vector3.forward + h * Vector3.right;
        Movement(move);
        Crouching(Input.GetKey(KeyCode.C));
        Aiming(Input.GetButton("Fire2"));

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            Vector3 dashVelocity = transform.forward * 100;
            rigid.AddForce(dashVelocity, ForceMode.VelocityChange);
        }

        if(Input.GetButton("Fire1") && isAiming) {
            Fire(true);
        }

    }




    public void EndPlay() {
        canControl = false;
        Movement(Vector3.zero);
        StartCoroutine(OnRestartLevel());
    }

    IEnumerator OnRestartLevel() {
        yield return new WaitForSeconds(3);
        GameController.Instance.ReloadScene();
    }
}

