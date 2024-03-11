using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI traitsProsText;
    [SerializeField] private TextMeshProUGUI traitsConsText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI startingWeaponText;
    [SerializeField] public Button selectButton;


    public void UpdateTraits(CharacterData characterData)
    {
        // Update the character image (if you have one)
        characterImage.sprite = characterData.characterIcon;

        // Update the character's name
        nameText.text = characterData.Name;

        // Clear the traits text initially
        traitsProsText.text = "";
        traitsConsText.text = "";

        // Access the traits from CharacterData's traits object
        CharacterTraits traits = characterData.traits;

        if (traits.extraHp > 0)
        {
            traitsProsText.text += $"+{traits.extraHp} HP\n";
        }
        else if (traits.extraHp < 0)
        {
            traitsConsText.text += $"{traits.extraHp} HP\n";
        }

        if (traits.extraArmor > 0)
        {
            traitsProsText.text += $"+{traits.extraArmor} Armor\n";
        }
        else if (traits.extraArmor < 0)
        {
            traitsConsText.text += $"{traits.extraArmor} Armor\n";
        }

        if (traits.extraSpeed > 0)
        {
            traitsProsText.text += $"+{traits.extraSpeed} Speed\n";
        }
        else if (traits.extraSpeed < 0)
        {
            traitsConsText.text += $"{traits.extraSpeed} Speed\n";
        }

        // Update the character's description
        descriptionText.text = characterData.Description;

        // Update the character's starting weapon
        startingWeaponText.text = "Starting Weapon: " + characterData.startingWeapon.Name;


    }
    public void SetSelectButtonAction(Action onSelectAction)
    {
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelectAction());
    }
    public void ClosePanel()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);

    }
}