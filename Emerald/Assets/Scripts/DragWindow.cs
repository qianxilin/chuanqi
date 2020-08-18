using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (canvas == null)
        {
            Transform testCanvas = transform.parent;
            while (testCanvas != null)
            {
                canvas = testCanvas.GetComponent<Canvas>();
                if (canvas != null)
                    break;
                testCanvas = testCanvas.parent;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.UIDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.UIDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
