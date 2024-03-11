using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ComboDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;

    private ComboData comboData;
    private TextMeshProUGUI globalDescriptionPanelText;

    public void Setup(ComboData comboData)
    {
        this.comboData = comboData;
        nameText.text = comboData.comboName;
        iconImage.sprite = comboData.Icon;
    }

    public void SetDescriptionPanelText(TextMeshProUGUI descriptionText)
    {
        globalDescriptionPanelText = descriptionText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (globalDescriptionPanelText != null && comboData != null)
        {
            globalDescriptionPanelText.text = comboData.comboDescription;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (globalDescriptionPanelText != null)
        {
            globalDescriptionPanelText.text = "";
        }
    }
}