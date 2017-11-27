using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour {

    public GameObject tipsWin;
    public GameObject tipsLose;

    public ProgressBar health;
    public ProgressBar stamina;

    void Start () {
        tipsWin.SetActive(false);
        tipsLose.SetActive(false);
        health.minValue = 0;
        health.maxValue = GameController.player.maxHealth;
        stamina.minValue = 0;
        stamina.maxValue = GameController.player.maxStamina;
    }

    void Update() {
        health.value = GameController.player.health;
        stamina.value = GameController.player.stamina;
    }

    public void ShowTipsWin() {
        tipsWin.SetActive(true);
    }

    public void ShowTipsLose() {
        tipsLose.SetActive(true);
    }

}
