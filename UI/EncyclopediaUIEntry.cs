using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EncyclopediaUIEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string descriptionText; // Set this in the inspector
    public TextMeshProUGUI descriptionDisplay; // Drag your Text or TMPro Text element here

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionDisplay.text = descriptionText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionDisplay.text = ""; // Clear the text or set to a default message
    }
}