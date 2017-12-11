using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{

    public Transform leftHandIK;
    public Transform rightHandIK;
    public int id;
    [SerializeField]
    protected HitImpact hitImpact;

    public ItemData data;

    public Character owner { get; private set; }

    public bool isEquiped { get; private set; }
    public bool isInInventory { get; private set; }
    void Awake() {
        data = DataTable.Get<ItemData>(id);
    }

    public virtual void OnEquip()
    {
        isEquiped = true;
        gameObject.SetActive(true);
    }

    public virtual void OnUnEquip()
    {
        isEquiped = false;
        gameObject.SetActive(false);
    }

    public void OnEnterInventory(Character newOwner) {
        if(owner != newOwner) {
            owner = newOwner;
            gameObject.SetActive(false);
            isInInventory = true;
            GetComponent<Collider>().enabled = false;
        }
    }

    public void OnLeaveInventory() {
        owner = null;
        gameObject.SetActive(true);
        isInInventory = false;
        GetComponent<Collider>().enabled = true;
    }

}
