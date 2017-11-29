
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameController : SingletonBehaviour<GameController>
{
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;


    public string startScene = "Game";

  
    private AudioSource audioSource2D;

    static PlayerCharacter _player;
    public static PlayerCharacter player {
        get {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
            return _player;
        }
    }

    static Fader _fader;
    public static Fader fader
    {
        get
        {
            if (_fader == null)
                _fader = GameObject.FindObjectOfType<Fader>();
            return _fader;
        }
    }

    public void Play2D(AudioClip clip, float volume)
    {
        if (audioSource2D)
            audioSource2D.PlayOneShot(clip, volume);
    }

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName) {
        if (!fader.isFading) {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator Start() {
        audioSource2D = GetComponent<AudioSource>();

        //加载初始场景并等待加载完成
        yield return StartCoroutine(LoadSceneAndSetActive(startScene));

        //加载完成，淡入
        StartCoroutine(fader.Fade(0f));
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName) {
        //淡出场景
        yield return StartCoroutine(fader.Fade(1f));
        if (BeforeSceneUnload != null)
            BeforeSceneUnload();

        //卸载当前场景
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //加载新场景
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //淡入场景
        yield return StartCoroutine(fader.Fade(0f));
        if (AfterSceneLoad != null)
            AfterSceneLoad();

    }

    private IEnumerator LoadSceneAndSetActive(string sceneName) {

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newLoadedScene);
    }
}
