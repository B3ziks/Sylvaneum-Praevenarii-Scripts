using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagesManager : MonoBehaviour
{
    public static StagesManager instance;

    public StageData CurrentStageData { get; private set; }
    private void Awake()
    {
        // Singleton pattern to keep the game manager persistent across scenes
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentStageData(StageData stageData)
    { 
        UnityEngine.Debug.Log($"Setting current stage data: {stageData}");
        CurrentStageData = stageData;
    }
}
