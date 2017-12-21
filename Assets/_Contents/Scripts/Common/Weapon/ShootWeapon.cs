using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootWeapon : Weapon {

    public virtual bool OnAttackOnce() { return false; }

    public virtual bool OnAttackContinuously() { return false; }

    public virtual void OnAiming(bool isAiming) { }
}
