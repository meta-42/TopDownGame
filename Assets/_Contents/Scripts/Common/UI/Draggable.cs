using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image draggableImage;

    RectTransform canvas;
    GameObject draggingObject;

    void Awake() {
        canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData) {

        if (canvas == null) return;
        draggingObject = new GameObject("Dragging");
        draggingObject.transform.SetParent(canvas, false);
        draggingObject.transform.SetAsLastSibling();
		
		var image = draggingObject.AddComponent<Image>();
        image.sprite = draggableImage.sprite;
        image.SetNativeSize();
        var group = draggingObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        UpdateDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData eventData) {
		if (draggingObject != null) {
            UpdateDraggedPosition(eventData);
        }
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (draggingObject != null) {
            Destroy(draggingObject);
            draggingObject = null;
        }
	}

    private void UpdateDraggedPosition(PointerEventData eventData) {
        var rt = draggingObject.GetComponent<RectTransform>();
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, eventData.pressEventCamera, out worldPoint)) {
            rt.position = worldPoint;
        }
    }
}
