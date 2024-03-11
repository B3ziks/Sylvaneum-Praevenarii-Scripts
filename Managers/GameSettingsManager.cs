using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    [SerializeField] private OptionsContainer optionsContainer;

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/gameSettings.json";

        // Check if the settings file exists
        if (System.IO.File.Exists(savePath))
        {
            // Load the existing settings
            LoadSettings();
        }
        else
        {
            // Set default values and save
            SetDefaultValues();
            SaveSettings();
        }
    }

    private void SetDefaultValues()
    {
        optionsContainer.soundVolume = 0.5f;  // example default value
        optionsContainer.musicVolume = 0.5f;  // example default value
        optionsContainer.resolution = "1920x1080";
        optionsContainer.fullscreenMode = "Fullscreen";
        optionsContainer.graphicQuality = "Medium";
        optionsContainer.shadowQuality = "Medium";
    }

    public void SaveSettings()
    {
        GameSettingsData data = new GameSettingsData
        {
            soundVolume = optionsContainer.soundVolume,
            musicVolume = optionsContainer.musicVolume,
            resolution = optionsContainer.resolution,
            fullscreenMode = optionsContainer.fullscreenMode,  // this is already a string
            graphicQuality = optionsContainer.graphicQuality,  // renamed field
            shadowQuality = optionsContainer.shadowQuality     // renamed field
        };

        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            GameSettingsData data = JsonUtility.FromJson<GameSettingsData>(json);

            optionsContainer.soundVolume = data.soundVolume;
            optionsContainer.musicVolume = data.musicVolume;
            optionsContainer.resolution = data.resolution;
            optionsContainer.fullscreenMode = data.fullscreenMode;
            optionsContainer.graphicQuality = data.graphicQuality;  // renamed field
            optionsContainer.shadowQuality = data.shadowQuality;    // renamed field
        }
    }
}