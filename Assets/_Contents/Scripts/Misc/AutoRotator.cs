using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotator : MonoBehaviour {

    public float speed;
    public Vector3 axis;

	void Update () {
        //var e = transform.rotation.eulerAngles + direction * speed;

        transform.Rotate(axis, speed* Time.deltaTime);

    }
}
