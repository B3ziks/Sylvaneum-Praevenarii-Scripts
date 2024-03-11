using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSettingsOptions : MonoBehaviour
{
   [SerializeField] private GameObject panel;

    private GameSettingsManager gameSettingsManager;
    
    private void Awake()
    {
        gameSettingsManager = FindObjectOfType<GameSettingsManager>();
        
        if (!gameSettingsManager)
        {
            UnityEngine.Debug.LogError("No GameSettingsManager found in the scene!");
        }
    }

    public void CloseMenu()
    {
        panel.SetActive(false);
        
        // If gameSettingsManager is available, save the settings
        if (gameSettingsManager)
        {
            gameSettingsManager.SaveSettings();
        }
    }

    public void OpenMenu()
    {
        panel.SetActive(true);

        // If gameSettingsManager is available, load the settings
        if (gameSettingsManager)
        {
            gameSettingsManager.LoadSettings();
        }
    }
}