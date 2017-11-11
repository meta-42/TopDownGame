using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {

    public bool debugVisual;

    public float sightDistance = 10;

    public float sightLineNum = 30;

    public float sightHeight = 1.5f;

    public float sightAngle = 90f;

    public LayerMask layerMask;

    public string targetTag = "Player";

    [HideInInspector]
    public Transform target;


    void Update () {
        FieldOfView();
    }

    void FieldOfView()
    {
        target = null;

        var sightStart = new Vector3(transform.position.x,
            transform.position.y + sightHeight,
            transform.position.z) ;

        var forwardLeft = Quaternion.Euler(0, -(sightAngle/2), 0) * transform.forward * sightDistance;

        for (int i = 0; i <= sightLineNum; i++)
        {
            var step = sightAngle / sightLineNum;
            var v = Quaternion.Euler(0, step * i, 0) * forwardLeft;
            var sightEnd = sightStart + v;

            RaycastHit hit;
            if (Physics.Linecast(sightStart, sightEnd, out hit, layerMask))
            {
                sightEnd = hit.point;

                if (hit.transform.CompareTag(targetTag))
                {
                    target = hit.transform;
                }
            }

            if (debugVisual)
            {
                Debug.DrawLine(sightStart, sightEnd, Color.red);
            }
        }
    }
}
