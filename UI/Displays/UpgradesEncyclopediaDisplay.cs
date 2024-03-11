using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradesEncyclopediaDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image upgradeImage;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    public UpgradeData UpgradeData => upgradeData;

    public UpgradeData upgradeData;
    private TextMeshProUGUI descriptionPanelText;

    public void Setup(UpgradeData upgradeData)
    {
        this.upgradeData = upgradeData;
        upgradeImage.sprite = upgradeData.icon;
        upgradeNameText.text = upgradeData.Name;
    }

    public void SetDescriptionPanelText(TextMeshProUGUI descriptionPanelText)
    {
        this.descriptionPanelText = descriptionPanelText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanelText != null && upgradeData != null)
        {
            descriptionPanelText.text = upgradeData.description;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanelText != null)
        {
            descriptionPanelText.text = "";
        }
    }
}