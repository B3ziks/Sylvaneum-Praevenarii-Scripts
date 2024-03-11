using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Needed for the UI event handlers

public class AchievementDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI unlockText;
    private TextMeshProUGUI descriptionPanelText;

    private Achievement achievement;

    public void Setup(Achievement achievement)
    {
        this.achievement = achievement;
        nameText.text = achievement.Name;
        iconImage.sprite = achievement.Icon;
        unlockText.text = "Unlocks: " + achievement.UnlockableCharacterName;
    }

    public void SetDescriptionPanelText(TextMeshProUGUI descriptionText)
    {
        descriptionPanelText = descriptionText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanelText != null && achievement != null)
        {
            descriptionPanelText.text = achievement.Description;
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