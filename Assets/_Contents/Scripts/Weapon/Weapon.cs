using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WeaponType {
    Melee,
    Shoot,
}


public abstract class Weapon : MonoBehaviour
{
    public Transform leftHandIK;
    public Transform rightHandIK;

    [HideInInspector]
    public WeaponType type;

    [SerializeField]
    protected HitImpact hitImpact;


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

    public virtual void OnEquip()
    {
        isEquiped = true;
    }

    public virtual void OnUnEquip()
    {
        isEquiped = false;
    }


}
