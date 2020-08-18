using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
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
