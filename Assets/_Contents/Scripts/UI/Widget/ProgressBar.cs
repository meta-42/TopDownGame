using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour {

    public RectTransform fill;
    public RectTransform flow;
    public float minValue;
    public float maxValue;
    public float flowSpeed;
    public float value;

    Vector3 flowScale = Vector3.one;

	void Update () {
        //fill
        var v = Mathf.Clamp(value, minValue, maxValue);
        var step = 1f / maxValue;
        fill.localScale = new Vector3(step * v, 1f, 1f);

        //flow
        flowScale = Vector3.Lerp(flowScale, fill.localScale, Time.deltaTime * flowSpeed);
        flow.localScale = flowScale;
    }
}
