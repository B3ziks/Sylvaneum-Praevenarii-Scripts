using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class CharacterSelectionButton : MonoBehaviour
{
    [SerializeField] private Button selectionButton;
    [SerializeField] private TextMeshProUGUI unlockStatusText;

    private void Start()
    {
        CharacterIconDisplay.ResetCharacterSelection();

        // Subscribe to the CharacterSelectedEvent
        CharacterIconDisplay.CharacterSelectedEvent += HandleCharacterSelected;

        // Initialize the button state based on whether a character is selected
        HandleCharacterSelected(CharacterIconDisplay.CharacterSelected);
    }

    private void OnEnable()
    {
        // Assume CharacterSelected is a static property that determines if a character has been selected
        selectionButton.interactable = CharacterIconDisplay.CharacterSelected;
        CharacterIconDisplay.CharacterSelectedEvent += HandleCharacterSelected;
        HandleCharacterSelected(CharacterIconDisplay.CharacterSelected); // Call to ensure UI is up to date
    }

    private void OnDisable()
    {
        CharacterIconDisplay.CharacterSelectedEvent -= HandleCharacterSelected;
        CharacterIconDisplay.ResetCharacterSelection(); // Reset the selection
    }

    // Handle character selection event
    private void HandleCharacterSelected(bool characterSelected)
    {
        selectionButton.interactable = characterSelected;
        unlockStatusText.text = characterSelected ? "Level Selection" : "Choose Character";
    }
    private void ResetButtonState()
    {
        // Reset the button state to disabled if no character is selected
        if (!CharacterIconDisplay.CharacterSelected)
        {
            selectionButton.interactable = false;
            unlockStatusText.text = "Choose Character";
        }
    }

}