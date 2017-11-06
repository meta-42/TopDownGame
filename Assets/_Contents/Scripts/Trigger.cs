using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour {

    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public string tagName = "Player";

    public bool isRepeat = false;
    private bool isTriggered = false;

    //highlight
    [ColorUsage(false,true,0,2,0,2)]
    public Color highlightColor;
    public Renderer[] renders;
    private string colorName = "_EmissionColor";

    //animate
    public Animator[] animates;
    private string animateEnterName = "TriggerEnter";
    private string animateExitName = "TriggerExit";

    //sound
    public AudioSource[] sounds;

    //active
    public GameObject[] activeObjects;


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagName)) return;



        if (!isRepeat && isTriggered) return;

        isTriggered = true;

        foreach (var obj in activeObjects) {
            obj.SetActive(true);
        }

        foreach (var render in renders)
        {
            render.material.SetColor(colorName, highlightColor);
        }

        foreach (var animate in animates)
        {
            animate.SetBool(animateEnterName, true);
        }

        foreach (var sound in sounds)
        {
            sound.Play();
        }


        OnEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagName))
            return;

        foreach (Renderer render in renders)
        {
            render.material.SetColor(colorName, Color.black);
        }

        foreach (var animate in animates)
        {
            animate.SetBool(animateEnterName, false);
        }

        OnExit.Invoke();
    }
}
