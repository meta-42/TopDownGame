using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Laser : RayVisual {

    public string tagName = "Player";
    new public ParticleSystem particleSystem;
    ParticleSystem.ShapeModule shape;

    void Start() {
        if (particleSystem != null) {
            shape = particleSystem.shape;
        }

        onHit.AddListener(OnHit);
        onLengthChange.AddListener(OnLengthChange);
    }

    private void OnDestroy()
    {
        onHit.RemoveListener(OnHit);
        onLengthChange.RemoveListener(OnLengthChange);
    }

    void OnHit(RaycastHit hitInfo)
    {
        if (hitInfo.collider.CompareTag(tagName))
        {
            GameController.player.Die();
        }
    }

    void OnLengthChange(Vector3 end)
    {
        if (particleSystem != null)
        {
            shape.radius = Mathf.Lerp(shape.radius, end.magnitude / 2, Time.deltaTime);
            particleSystem.transform.localPosition = Vector3.Lerp(particleSystem.transform.localPosition, end * 0.5f, Time.deltaTime);
        }
    }
}
