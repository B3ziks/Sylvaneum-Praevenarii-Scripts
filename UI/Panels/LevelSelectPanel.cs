using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private List<SelectStageButton> stageSelectButtons;
    [SerializeField] private FlagsTable flagsTable;
    [SerializeField] private DataContainer dataContainer;
    [SerializeField] private LevelDetail levelDetail; // Reference to the LevelDetail script
    [SerializeField] private GameObject levelDetailPanel; // Reference to the LevelDetail script
    [SerializeField] private TMP_Text hintText; // Reference to the TextMeshPro text component

    public static LevelSelectPanel Instance { get; private set; } // Singleton instance
    public StageData SelectedStageData { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StageButtonClick(StageData stageData)
    {
        levelDetailPanel.SetActive(true);  // Ensure the LevelDetail Panel is active and visible
        levelDetail.UpdateDetails(stageData);
        SelectedStageData = stageData;  // Store selected stage data
        hintText.gameObject.SetActive(false); // Hide the "Choose stage from list ->" text
    }

    private void UpdateButtons()
    {
        foreach (var stageButton in stageSelectButtons)
        {
            bool isUnlocked = IsStageUnlocked(stageButton);
            stageButton.gameObject.SetActive(isUnlocked);
        }
    }

    private bool IsStageUnlocked(SelectStageButton stageButton)
    {
        List<string> requiredStages = stageButton.stageData.stageCompletionToUnlock;

        // If no completion is required to unlock, the stage is unlocked by default
        if (requiredStages == null || requiredStages.Count == 0)
            return true;

        // Check against the completedStages in DataContainer
        foreach (string stageID in requiredStages)
        {
            if (!dataContainer.completedStages.Contains(stageID))
                return false;
        }

        return true;
    }

    private void OnEnable()
    {
        UpdateButtons();
        hintText.gameObject.SetActive(true); // Display the "Choose stage from list ->" text again
    }
}