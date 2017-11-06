using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{
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

    public UnityEngine.Events.UnityEvent onEquip;

    public UnityEngine.Events.UnityEvent onUnequip;

    public UnityEngine.Events.UnityEvent onAttack;

    public virtual bool AttackOnceHandle() { return false; }

    public virtual bool AttackContinuouslyHandle() { return false; }

    public virtual void OnEquip()
    {
        isEquiped = true;
        onEquip.Invoke();
    }

    public virtual void OnUnEquip()
    {
        isEquiped = false;
        onUnequip.Invoke();
    }
}
