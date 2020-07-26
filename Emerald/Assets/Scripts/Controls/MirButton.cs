using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MirButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Button button;
    private AudioSource clickSound;
    private Image image;

    public Sprite NeutralButton;
    public Sprite HoverButton;
    public Sprite DownButton;
    public UnityEvent ClickEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        clickSound = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = HoverButton;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = NeutralButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = DownButton;

        if (clickSound != null)
            clickSound.Play();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.sprite = NeutralButton;
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        if (ClickEvent == null) return;
        ClickEvent.Invoke();
    }
}
