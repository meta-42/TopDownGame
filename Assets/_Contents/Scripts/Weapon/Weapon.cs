using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{
    public Transform leftHandIK;

    private Character _user;
    public Character user
    {
        get
        {
            if (_user == null)
                _user = GetComponent<Character>();
            if (!_user)
                _user = GetComponentInParent<Character>();
            return _user;
        }
    }

    public bool isEquiped { get; private set; }


    public virtual bool OnAttackOnce() { return false; }

    public virtual bool OnAttackContinuously() { return false; }

    public virtual void OnAiming(bool isAiming) { }

    public virtual void OnEquip()
    {
        isEquiped = true;
    }

    public virtual void OnUnEquip()
    {
        isEquiped = false;
    }
}
