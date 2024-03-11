using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterIconDisplay : MonoBehaviour
{
    [SerializeField] private Image characterIconImage;
    [SerializeField] private TMPro.TextMeshProUGUI nameText; // Ensure this is linked in the inspector

    [SerializeField] private Button selectButton;
    [SerializeField] private Button detailsButton; // New button for showing details

    [SerializeField] private TextMeshProUGUI unlockStatusText;
    private CharacterSelectionPanel characterSelectionPanel; // Reference to CharacterSelectionPanel

    private CharacterData characterData;
    public DataContainer dataContainer; // reference to your DataContainer

    private static CharacterIconDisplay selectedCharacterDisplay; // Store the currently selected character display
    private CharacterDetailsPanel detailsPanelInstance; // Field to store the current instance.

    public static bool CharacterSelected { get; private set; } = false;
    // Define the CharacterSelectedEvent
    public delegate void CharacterSelectedHandler(bool characterSelected);
    public static event CharacterSelectedHandler CharacterSelectedEvent;
    // Additional reference needed:


    private void Awake()
    {
        characterSelectionPanel = FindObjectOfType<CharacterSelectionPanel>();

        // Setup details button to show character details
        detailsButton.onClick.RemoveAllListeners();
        detailsButton.onClick.AddListener(DisplayCharacterDetails);

        // Setup select button to only select the character
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCharacterSelected);
    }

    public void Setup(CharacterData characterData)
    {
        this.characterData = characterData;
        UpdateDisplay();
    }

    private void DisplayCharacterDetails()
    {
        if (characterData.IsUnlocked)
        {
            if (characterSelectionPanel != null && characterSelectionPanel.characterDetailsPanelPrefab != null)
            {
                // Destroy the previous instance if it exists
                if (detailsPanelInstance != null)
                {
                    Destroy(detailsPanelInstance.gameObject);
                }

                // Instantiate the CharacterDetailsPanel prefab as a child of the CharacterSelectionPanel
                detailsPanelInstance = Instantiate(characterSelectionPanel.characterDetailsPanelPrefab, characterSelectionPanel.transform);

                // Populate the details
                detailsPanelInstance.UpdateTraits(characterData);

                // Find and setup the Select button within the details panel
                Button selectButtonInDetailsPanel = detailsPanelInstance.selectButton; // Assuming there's a public reference to the button in the details panel script
                selectButtonInDetailsPanel.onClick.RemoveAllListeners();
                selectButtonInDetailsPanel.onClick.AddListener(OnCharacterSelected); // This should now bind the select action to the button within the details panel

                // Set the details panel to active
                detailsPanelInstance.gameObject.SetActive(true);
            }
            else
            {
                UnityEngine.Debug.LogError("CharacterSelectionPanel or characterDetailsPanelPrefab is null.");
            }
        }
    }
    private void UpdateDisplay()
    {
        if (characterData != null)
        {
            characterIconImage.sprite = characterData.characterIcon;
            selectButton.interactable = characterData.IsUnlocked;
            nameText.text = dataContainer.IsCharacterSelected(characterData.Name) ? "Selected" : characterData.Name;
            unlockStatusText.text = characterData.IsUnlocked ? "Unlocked" : "Locked";
            unlockStatusText.color = characterData.IsUnlocked ? Color.green : Color.red;
        }
    }
    public void CloseDetailsPanel()
    {
        UnityEngine.Debug.Log("Attempting to close Details Panel");
        if (detailsPanelInstance != null)
        {
            detailsPanelInstance.ClosePanel();
            UnityEngine.Debug.Log("Details Panel Closed");
        }
        else
        {
            UnityEngine.Debug.Log("No detailsPanelInstance found to close");
        }
    }
    public static void ResetCharacterSelection()
    {
        if (selectedCharacterDisplay != null)
        {
            selectedCharacterDisplay.DeselectCharacter();
            selectedCharacterDisplay = null;
            CharacterSelected = false;

        }
    }
    public void OnCharacterSelected()
    {
        if (selectedCharacterDisplay != null && selectedCharacterDisplay != this)
        {
            selectedCharacterDisplay.DeselectCharacter();
        }

        CharacterSelected = true;
        nameText.text = "Selected";
        selectedCharacterDisplay = this;
        SetSelectedCharacter();

        if (CharacterSelectedEvent != null)
        {
            CharacterSelectedEvent(true);
        }

        // Close the details panel after selection
        CloseDetailsPanel();
    }

    // Deselect this character
    private void DeselectCharacter()
    {
        CharacterSelected = false;
        UpdateDisplay();
    }

    // Deselect this character
    public static void DeselectAllCharacters()
    {
        if (selectedCharacterDisplay != null)
        {
            selectedCharacterDisplay.DeselectCharacter();
            selectedCharacterDisplay = null;
            CharacterSelected = false;
        }
    }


    public void SetSelectedCharacter()
    {
        if (dataContainer == null)
        {
            return;
        }

        if (characterData == null)
        {
            return;
        }
        else
        {
            dataContainer.SetSelectedCharacter(characterData);
            dataContainer.SetSelectedCharacterActor(characterData.characterActor);
            GameDataStartingWeapon.SelectedStartingWeapon = characterData.startingWeapon;
        }

        // Assuming GameDataStartingWeapon is a static class with a static property.
        if (GameDataStartingWeapon.SelectedStartingWeapon == null)
        {
            return;
        }

       
    }

    private void OnEnable()
    {
        // Subscribe to the selected character change event and update the display
        dataContainer.OnSelectedCharacterChanged += UpdateDisplay;

        // Reset the character selection on enable
        ResetCharacterSelection();
        dataContainer.SetSelectedCharacter(null);

        // Update the display to ensure the UI reflects the current state
        UpdateDisplay();
    }

    private void OnDisable()
    {
        dataContainer.OnSelectedCharacterChanged -= UpdateDisplay;
    }
}