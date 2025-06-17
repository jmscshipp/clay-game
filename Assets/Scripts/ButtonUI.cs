using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Sprite highlightedSprite;
    [SerializeField]
    private Color highlightedColor;
    [SerializeField]
    private Sprite clickedSprite;
    [SerializeField]
    private Color clickedColor;
    [SerializeField]
    private float clickedColorDuration;
    [SerializeField]
    private UnityEvent OnClick;
    [SerializeField]
    private UnityEvent OnHighlight;

    [SerializeField]
    TMP_Text text;
    [SerializeField]
    TMP_Text text2;
    // used for saving color state after click color duration
    private Color savedColor;
    private bool clicking;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHighlight.Invoke();
        savedColor = highlightedColor;
        if (!clicking)
        {
            text.color = savedColor;
            text2.color = savedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        savedColor = defaultColor;
        if (!clicking)
        {
            text.color = savedColor;
            text2.color = savedColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = clickedColor;
        text2.color = clickedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(ClickEffects());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
    }

    private IEnumerator ClickEffects()
    {
        clicking = true;
        yield return new WaitForSeconds(clickedColorDuration);
        text.color = savedColor;
        text2.color = savedColor;
        clicking = false;
    }
}
