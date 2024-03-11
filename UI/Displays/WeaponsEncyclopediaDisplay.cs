using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponsEncyclopediaDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    public WeaponData WeaponData => weaponData;

    public WeaponData weaponData;  // Assuming you have a WeaponData class similar to UpgradeData
    private TextMeshProUGUI descriptionPanelText;

    public void Setup(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        weaponImage.sprite = weaponData.Image;
        weaponNameText.text = weaponData.Name;
    }

    public void SetDescriptionPanelText(TextMeshProUGUI descriptionPanelText)
    {
        this.descriptionPanelText = descriptionPanelText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanelText != null && weaponData != null)
        {
            descriptionPanelText.text = weaponData.Description;
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