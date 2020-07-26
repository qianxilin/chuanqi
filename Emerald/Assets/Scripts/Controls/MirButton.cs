using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MirButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Button button;
    private Image image;
    public Sprite neutralButton;
    public Sprite hoverButton;
    public Sprite downButton;
    public UnityEvent onclick;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverButton;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = neutralButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = downButton;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.sprite = neutralButton;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onclick == null) return;
        onclick.Invoke();
    }
}
