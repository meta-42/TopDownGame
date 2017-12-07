using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : UIPanel<MainMenuPanel> {

    public Button startButton;
    public Button quitButton;

    public string startScene = "Game";

    private void OnEnable() {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable() {
        startButton.onClick.RemoveListener(StartGame);
        quitButton.onClick.RemoveListener(QuitGame);
    }

    void StartGame() {
        GameController.Instance.LoadScene(startScene);
        Hide();
    }

    void QuitGame() {
        Application.Quit();
    }
}
