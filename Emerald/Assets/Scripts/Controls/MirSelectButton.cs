using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MirSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private AudioSource clickSound;
    private Image image;
    private bool Selected;

    public Sprite NeutralImage;
    public Sprite HoverImage;
    public Sprite DownImage;
    public Sprite SelectImage;
    public UnityEvent ClickEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        clickSound = GetComponent<AudioSource>();
    }

    public void Select(bool select)
    {
        Selected = select;
        image.sprite = GetNeutralButton();
    }

    public Sprite GetNeutralButton()
    {
        return Selected ? SelectImage : NeutralImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Selected) return;
        image.sprite = HoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Selected) return;
        image.sprite = GetNeutralButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = DownImage;        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.sprite = GetNeutralButton();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            clickSound.Play();

        if (ClickEvent == null) return;
        ClickEvent.Invoke();
    }
}
