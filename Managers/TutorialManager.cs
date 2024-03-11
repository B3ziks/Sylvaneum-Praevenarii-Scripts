using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialPanels; // Drag your tutorial panels here in order
    [SerializeField] private StageData currentStageData;      // Reference to current stage data
    private string currentStageName;
    private int currentPanelIndex = 0;
    PauseManager pauseManager;

    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => StagesManager.instance.CurrentStageData != null);
        currentStageData = StagesManager.instance.CurrentStageData;

        if (currentStageData == null)
        {
            UnityEngine.Debug.LogError("CurrentStageData is null. Unable to set currentStageName.");
            yield break; // Exit if there is no current stage data
        }

        currentStageName = currentStageData.stageID;

        // Check if the current stage is the tutorial stage
        if (currentStageName == "GameplayStage0")
        {
            InitializeTutorial();
        }
    }

    private void InitializeTutorial()
    {
        pauseManager.PauseGame();

        // Ensure all panels are initially set to inactive
        foreach (var panel in tutorialPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // Activate the first panel
        if (tutorialPanels.Count > 0)
        {
            tutorialPanels[currentPanelIndex].SetActive(true);
        }
    }

    public void ShowNextPanel()
    {
        // Deactivate the current panel
        if (currentPanelIndex < tutorialPanels.Count)
        {
            tutorialPanels[currentPanelIndex].SetActive(false);
            currentPanelIndex++;

            // Activate the next panel if it exists
            if (currentPanelIndex < tutorialPanels.Count)
            {
                tutorialPanels[currentPanelIndex].SetActive(true);
            }
            else
            {
                // Optional: Call a function when the tutorial is finished
                TutorialFinished();
            }
        }
    }
    // Method to be called when the skip button is pressed
    public void SkipTutorial()
    {
        currentPanelIndex = tutorialPanels.Count; // Set the index to the end
        TutorialFinished();
    }

    private void TutorialFinished()
    {
        pauseManager.UnPauseGame();

        // Deactivate all panels
        foreach (var panel in tutorialPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        UnityEngine.Debug.Log("Tutorial finished.");
    }
}