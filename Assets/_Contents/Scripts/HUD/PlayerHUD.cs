using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour {

    public ProgressBar health;
    public ProgressBar stamina;

    void Start () {
        health.minValue = 0;
        health.maxValue = GameController.Instance.player.maxHealth;
        stamina.minValue = 0;
        stamina.maxValue = GameController.Instance.player.maxStamina;
    }

    void Update() {
        health.value = GameController.Instance.player.health;
        stamina.value = GameController.Instance.player.stamina;
    }

}
