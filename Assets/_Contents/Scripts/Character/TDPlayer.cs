using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDPlayer : PlayerCharacter {
    protected override void Start() {
        base.Start();
        Cursor.visible = false;
    }

	protected override void UpdateControl() {
        base.UpdateControl();

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
            if (Input.GetKeyUp(KeyCode.Q)) {
                EquipPrevWeapon();
            }

            if (Input.GetKeyUp(KeyCode.E)) {
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
        var aimPos  = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,10));
        aimPos.y = transform.position.y;
        transform.LookAt(aimPos);
    }
}
