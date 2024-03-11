using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StageEventType
{
    SpawnEnemy,
    SpawnEnemyBoss,
    SpawnEnemyElite,
    SpawnObject,
    WinStage
}

[Serializable]
public class StageEvent
{
    public StageEventType eventType;

    public float time;
    public string message;

    public EnemyData enemyToSpawn;
    public GameObject objectToSpawn;
    public int count;

    public bool isRepeatedEvent;
    public float repeatEverySeconds;
    public int repeatCount;

}

[CreateAssetMenu]
public class StageData : ScriptableObject
{
    [Header("Map Generation")]
    public GameplayStageData gameplayData; // Add this line
    // Existing properties
    public List<StageEvent> stageEvents;
    public string stageID;
    public List<string> stageCompletionToUnlock;

    // New properties to hold stage information to be displayed on UI
    [Header("UI Information")]
    public string stageName; // To display the name of the stage
    public Sprite stageThumbnail; // To display an image representing the stage
    public string stageDescription; // To display some details or lore of the stage
    public LocationData stageLocation; // Replace the string 'stageLocation' field
    public List<string> difficultyLevels; // Possible difficulty levels for future use
    public bool isComingSoon = false;
    // Additional properties for dialogue
    [Header("Dialogue")]
    public DialogueActor bossActor; // Assign through Unity Editor

    // Method to get the stage details in a formatted way. This might be used to update UI.
    public string GetFormattedStageDetails()
    {
        return $"{stageName}\n\n{stageDescription}";
    }
}