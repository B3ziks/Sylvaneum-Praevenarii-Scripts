using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum Difficulty
{
    Normal,
    Hard,
    Insane
}

public class LevelDetail : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI levelNameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI locationText;
    [SerializeField] private Image levelImage;
    [SerializeField] private TextMeshProUGUI difficultyText; // Add this line for difficulty text field

    public DifficultyManager difficultyManager; // Reference to the DifficultyManager

    private void Start()
    {
        difficultyManager = DifficultyManager.Instance;

        if (difficultyManager != null)
        {
            UpdateDifficultyText();
            difficultyManager.OnDifficultyChanged.AddListener(UpdateDifficultyText);
        }
        else
        {
            UnityEngine.Debug.LogError("DifficultyManager not set in LevelDetail.");
        }
    }

    private void OnDestroy()
    {
        difficultyManager.OnDifficultyChanged.RemoveListener(UpdateDifficultyText);
    }

    public void UpdateDetails(StageData stageData)
    {
        levelNameText.text = stageData.stageName;
        descriptionText.text = stageData.stageDescription;
        locationText.text = stageData.stageLocation.locationName;
        levelImage.sprite = stageData.stageThumbnail;

        UpdateDifficultyText(); // Update the difficulty text field here as well

        gameObject.SetActive(true);
    }


    public void UpdateDifficultyText()
    {
        if (difficultyManager != null)
        {
            Difficulty currentDifficulty = difficultyManager.GetCurrentDifficulty();
            difficultyText.text = "" + currentDifficulty.ToString();
            UnityEngine.Debug.Log("LevelDetail: Updating difficulty text to " + currentDifficulty);
        }
    }
}
