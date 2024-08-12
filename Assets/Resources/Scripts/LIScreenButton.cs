using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LIScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject colouredButton;

    private void Start()
    {
        colouredButton.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        colouredButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        colouredButton.SetActive(false);
    }
}
