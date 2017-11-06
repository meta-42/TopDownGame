using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour {

    CanvasGroup canvas;
    public bool isFading;
    public float fadeDuration = 1f;

    void Start () {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1;
	}

    public IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        canvas.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(canvas.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(canvas.alpha, finalAlpha))
        {
            //逐步逼近finalAlpha
            canvas.alpha = Mathf.MoveTowards(canvas.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            yield return null;
        }

        isFading = false;
        canvas.blocksRaycasts = false;
    }
}
