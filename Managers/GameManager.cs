using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DataContainer dataContainerAsset;
    // Flag to track the game state
    public bool IsGamePausedOrStopped { get; private set; } = false;
    public bool IsRestarting { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    public Transform playerTransform;

    void Start()
    {
        if (dataContainerAsset != null)
        {
            DataPersistenceManager.instance.RegisterDataPersistenceObject(dataContainerAsset);
        }
    }

    // Additional methods to control the game state
    public void PauseOrStopGame()
    {

        IsGamePausedOrStopped = true;
    }

    public void ResumeOrStartGame()
    {
        IsGamePausedOrStopped = false;
    }
    // Method to set the restarting state
    public void SetRestartingState(bool state)
    {
        IsRestarting = state;
    }
    
}