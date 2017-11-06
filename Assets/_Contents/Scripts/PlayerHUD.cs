using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour {

    public GameObject tipsWin;
    public GameObject tipsLose;

    void Start () {
        tipsWin.SetActive(false);
        tipsLose.SetActive(false);
    }

    public void ShowTipsWin() {
        tipsWin.SetActive(true);
    }

    public void ShowTipsLose() {
        tipsLose.SetActive(true);
    }

}
