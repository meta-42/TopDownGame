using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnHit : UnityEvent<RaycastHit> { }
public class OnLengthChange : UnityEvent<Vector3> { }

[RequireComponent(typeof(LineRenderer))]
public class RayVisual : MonoBehaviour {

    public float maxLength = 50f;
    LineRenderer lineRenderer;
    Vector3 endPoint;
    public OnHit onHit = new OnHit();
    public OnLengthChange onLengthChange = new OnLengthChange();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        RaycastHit hit;
        if (lineRenderer.useWorldSpace)
        {
            endPoint = transform.position + transform.forward * maxLength;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxLength))
            {

                endPoint = hit.point;
                onHit.Invoke(hit);
            }
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPoint);
        }
        else
        {
            endPoint = transform.forward * maxLength;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxLength))
            {

                endPoint = transform.InverseTransformPoint(hit.point);
                onHit.Invoke(hit);
            }
            lineRenderer.SetPosition(1, endPoint);
        }

        onLengthChange.Invoke(endPoint);
    }
}
