using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Droppable : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public Image containerImage;
    public Image receivingImage;
    public Color highlightColor;

    Color normalColor;

    public Action<Draggable>  onDrop;

    public void OnEnable() {
        
        if (containerImage != null)
            normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data) {
        containerImage.color = normalColor;

        if (receivingImage == null)
            return;
        var dragging = GetDragging(data);
        if (dragging != null) {
            if (onDrop != null) {
                onDrop.Invoke(dragging);
            }

            receivingImage.sprite = dragging.GetComponent<Image>().sprite;
        }
    }

    public void OnPointerEnter(PointerEventData data) {
        if (containerImage == null)
            return;

        var dragging = GetDragging(data);
        if (dragging != null) {
            containerImage.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData data) {
        if (containerImage == null) return;
        containerImage.color = normalColor;
    }

    private Draggable GetDragging(PointerEventData data) {
        if (!data.pointerDrag) return null;
        var dragging = data.pointerDrag.GetComponent<Draggable>();
        return dragging;
    }
}
