using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharacterImage;
    [SerializeField] private TextMeshProUGUI dialogueTextBox;
    [SerializeField] private List<DialogueData> dialogueDataList;
    [SerializeField] private StageData currentStageData;  // Reference to current stage data
    [SerializeField] private DataContainer dataContainer; // Reference to your data container

    private DialogueActor leftActor;
    private DialogueActor rightActor;
    private Queue<Dialogue> dialogueQueue;
    private string currentStageName;
    private bool isCutsceneActive = false;
    private bool hasStartedCutscene = false; // Ensures cutscene is only started once

    public bool HasSeenCutscene
    {
        get => PlayerPrefs.GetInt(currentStageName + "_HasSeenCutscene", 0) == 1;
        set => PlayerPrefs.SetInt(currentStageName + "_HasSeenCutscene", value ? 1 : 0);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => StagesManager.instance.CurrentStageData != null);
        InitCutsceneManager();
    }

    private void InitCutsceneManager()
    {
        gameObject.SetActive(false); // Hide the panel initially

        if (StagesManager.instance == null)
        {
            Debug.LogError("StagesManager instance is null.");
            return;
        }

        currentStageData = StagesManager.instance.CurrentStageData;
        if (currentStageData == null)
        {
            Debug.LogError("CurrentStageData is null. Unable to set currentStageName.");
            return;
        }
        currentStageName = currentStageData.stageID;

        if (dataContainer == null)
        {
            Debug.LogError("DataContainer reference is null.");
            return;
        }

        leftActor = ScriptableObject.CreateInstance<DialogueActor>();
        leftActor.actorName = dataContainer.selectedCharacter.Name;
        leftActor.actorSprite = dataContainer.selectedCharacterActor.actorSprite;
        rightActor = currentStageData.bossActor;

        if (leftCharacterImage == null || rightCharacterImage == null)
        {
            Debug.LogError("Character Image references are null.");
            return;
        }

        leftCharacterImage.sprite = leftActor.actorSprite;
        rightCharacterImage.sprite = rightActor.actorSprite;

        var dialogues = GetDialogueDataForCurrentStage();
        if (dialogues == null || dialogues.Count == 0)
        {
            dialogues = GetDefaultDialogues();
        }

        if (dialogues == null || dialogues.Count == 0)
        {
            Debug.LogError("No dialogues available for this stage, including defaults.");
            return;
        }

        Debug.Log($"Initializing dialogues for stage: {currentStageName}.");
        dialogueQueue = new Queue<Dialogue>(dialogues);

        StartCutscene();
    }

    public void StartCutscene()
    {
        if (hasStartedCutscene || dialogueQueue == null || dialogueQueue.Count == 0) return;

        isCutsceneActive = true;
        dialogueTextBox.text = "Cutscene started!";
        DisplayNextDialogue();

        hasStartedCutscene = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isCutsceneActive)
        {
            DisplayNextDialogue();
        }
    }

    public void DisplayNextDialogue()
    {
        if (dialogueQueue == null || dialogueQueue.Count == 0)
        {
            EndCutscene();
            return;
        }

        Dialogue currentDialogue = dialogueQueue.Dequeue();
        DialogueActor actorToUse = null;
        string textColor = "";

        switch (currentDialogue.role)
        {
            case DialogueRole.Character:
                actorToUse = leftActor;
                textColor = "<color=blue>";
                break;
            case DialogueRole.Enemy:
                actorToUse = rightActor;
                textColor = "<color=red>";
                break;
        }

        if (actorToUse != null)
        {
            dialogueTextBox.text = $"{textColor}{actorToUse.actorName}:</color> {currentDialogue.text}";
        }
        else
        {
            Debug.LogError("Actor not found for the dialogue role.");
        }
    }

    public void EndCutscene()
    {
        isCutsceneActive = false;
        HasSeenCutscene = true;
    }

    private List<Dialogue> GetDialogueDataForCurrentStage()
    {
        var matchedData = dialogueDataList.Find(data => data.stageName.Equals(currentStageName));
        List<Dialogue> characterSpecificDialogues = matchedData?.characterDialogues
            .Find(cd => cd.characterName == dataContainer.selectedCharacter.Name)?.dialogues;

        return characterSpecificDialogues ?? GetDefaultDialogues();
    }

    private List<Dialogue> GetDefaultDialogues()
    {
        var matchedData = dialogueDataList.Find(data => data.stageName.Equals(currentStageName));
        return matchedData?.dialogues;
    }

    private void OnEnable()
    {
        if (currentStageData != null && !hasStartedCutscene)
        {
            StartCutscene();
        }
    }
}