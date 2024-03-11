using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private GameObject introPanel; // Assign the intro panel in the inspector
    [SerializeField] private StageData currentStageData; // Reference to current stage data
    private PauseManager pauseManager;

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
            UnityEngine.Debug.LogError("CurrentStageData is null. Unable to determine current stage.");
            yield break; // Exit if there is no current stage data
        }

        string currentStageName = currentStageData.stageID;

        // Check if the current stage is "GameplayStage1"
        if (currentStageName == "GameplayStage1")
        {
            InitializeIntro();
        }
    }

    private void InitializeIntro()
    {
        pauseManager.PauseGame();

        // Activate the intro panel
        if (introPanel != null)
        {
            introPanel.SetActive(true);
        }
    }

    // Method to be called by the button on the intro panel
    public void CloseIntroPanel()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }

        pauseManager.UnPauseGame();
        UnityEngine.Debug.Log("Intro panel closed and game unpaused.");
    }
}