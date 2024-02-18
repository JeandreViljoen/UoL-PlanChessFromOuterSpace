using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler ,IPointerEnterHandler, IPointerExitHandler
{
    public event Action<PointerEventData> OnMouseDown;
    public event Action<PointerEventData> OnMouseUp;
    public event Action<PointerEventData> OnMouseEnter;
    public event Action<PointerEventData> OnMouseExit;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Clicked: {gameObject.name}");
        OnMouseDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseUp?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(eventData);
    }
}
