using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class MainMenuProfileManager : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text profileNameText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private GameObject editPanel;
    [SerializeField] private TMP_InputField editNameInputField;
    [SerializeField] private DataContainer dataContainer;
    [SerializeField] private CharactersTable charactersTable;
    [SerializeField] private GameObject imagesChoicePanel; // Parent GameObject with GridLayoutGroup
    [SerializeField] private Color selectedBorderColor = Color.yellow; // Color for selected border
    [SerializeField] private Color defaultBorderColor = Color.white; // Color for default border
    [SerializeField] private Sprite defaultProfileSprite; // Assign this in the inspector
    [SerializeField] private string defaultProfileName = "Player"; // Assign this if needed
    [SerializeField] private Button saveButton;

    private List<Button> characterButtons = new List<Button>(); // To keep track of all buttons
    private Button selectedButton; // Currently selected button
    private CharacterData selectedCharacterData; // Currently selected character
    private Sprite originalProfileSprite;
    private string originalProfileName;

    private void Start()
    {
        originalProfileName = dataContainer.profileName ?? defaultProfileName;
        // Use the method to get the Sprite from the name
        originalProfileSprite = dataContainer.GetProfilePictureSprite() ?? defaultProfileSprite;

        LoadProfileData();
        LoadHighScore();
        editPanel.SetActive(false);
        PopulateCharacterIcons();
        // Initialize the input field's listener for text change
        editNameInputField.onValueChanged.AddListener(ValidateInput);

        // Validate initially to set the correct state of the Save button
        ValidateInput(editNameInputField.text);
    }

    public void OpenEditPanel()
    {
        // Store the original profile data before changes
        originalProfileName = profileNameText.text;
        originalProfileSprite = profileImage.sprite;

        // Initialize the edit panel with the original data
        editNameInputField.text = originalProfileName;

        // Open the edit panel
        editPanel.SetActive(true);
    }

    private void ValidateInput(string input)
    {
        // Regular expression for validation
        Regex validNameRegex = new Regex(@"^[a-zA-Z0-9_-]{0,16}$"); // Limits to 16 characters

        // Enable or disable the Save button based on the input validation
        saveButton.interactable = validNameRegex.IsMatch(input) && !string.IsNullOrEmpty(input);
    }
    public void SaveProfile()
    {
        // Save the new profile name
        string newName = editNameInputField.text;
        profileNameText.text = newName;
        dataContainer.profileName = newName;

        // Save the selected character's icon name if one has been selected
        if (selectedCharacterData != null)
        {
            // Use the character's name to save the reference to the icon
            dataContainer.SaveProfileData(newName, selectedCharacterData.Name);
            // Update the profile image immediately with the selected sprite
            profileImage.sprite = selectedCharacterData.characterIcon;
        }

        // Hide the edit panel
        editPanel.SetActive(false);
    }

    private void LoadProfileData()
    {
        // Use the method in DataContainer to get the Sprite from the stored name
        profileImage.sprite = dataContainer.GetProfilePictureSprite() ?? defaultProfileSprite;
        profileNameText.text = !string.IsNullOrEmpty(dataContainer.profileName) ? dataContainer.profileName : defaultProfileName;
    }

    private void LoadHighScore()
    {
        int highScore = dataContainer.GetHighScore();
        highscoreText.text = "" + highScore.ToString();
    }

    private void PopulateCharacterIcons()
    {
        foreach (CharacterData character in charactersTable.characters)
        {
            GameObject buttonObj = new GameObject(character.Name + "Button", typeof(Image), typeof(Button));
            buttonObj.transform.SetParent(imagesChoicePanel.transform, false);
            Button button = buttonObj.GetComponent<Button>();
            Image buttonImage = buttonObj.GetComponent<Image>();
            buttonImage.sprite = character.characterIcon;
            buttonImage.color = defaultBorderColor; // Default color without selection

            // Add a click event listener to the button
            button.onClick.AddListener(() => {
                SelectCharacterIcon(character, button);
            });

            // Save the button in the list for future reference
            characterButtons.Add(button);
        }
    }
    public void BackWithoutSaving()
    {
        // Revert to the original profile data
        profileNameText.text = originalProfileName;
        profileImage.sprite = originalProfileSprite;

        // If a character icon was selected, reset its border color
        if (selectedButton != null)
        {
            selectedButton.GetComponent<Image>().color = defaultBorderColor;
        }

        // Hide the edit panel
        editPanel.SetActive(false);
    }
    private void SelectCharacterIcon(CharacterData character, Button button)
    {
        // Deselect the previous button if any
        if (selectedButton != null)
        {
            selectedButton.GetComponent<Image>().color = defaultBorderColor;
        }

        // Select the new button
        selectedButton = button;
        selectedButton.GetComponent<Image>().color = selectedBorderColor;

        // Update the selected character data
        selectedCharacterData = character;
    }

}