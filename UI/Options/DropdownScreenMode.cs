using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro components
using System;


public class DropdownScreenMode : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private OptionsContainer optionsContainer;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialize the dropdown value to match the saved fullscreen mode
        SetDropdownToCurrentMode();
    }

    private void SetDropdownToCurrentMode()
    {
        FullscreenMode currentMode = optionsContainer._fullscreenMode;
        dropdown.value = (int)currentMode;
        ApplyScreenMode(currentMode);
    }

    private void OnDropdownValueChanged(int index)
    {
        FullscreenMode selectedMode = (FullscreenMode)index;
        optionsContainer._fullscreenMode = selectedMode;
        ApplyScreenMode(selectedMode);
    }

    private void ApplyScreenMode(FullscreenMode mode)
    {
        int width, height;
        bool isFullscreen = false;

        // Parse the saved resolution for windowed mode
        ParseResolution(optionsContainer.resolution, out width, out height);

        switch (mode)
        {
            case FullscreenMode.Fullscreen:
                isFullscreen = true;
                break;
            case FullscreenMode.Windowed:
                isFullscreen = false;
                break;
            case FullscreenMode.Borderless:
                width = Screen.currentResolution.width;
                height = Screen.currentResolution.height;
                isFullscreen = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        Screen.SetResolution(width, height, isFullscreen);
    }

    private void ParseResolution(string resolution, out int width, out int height)
    {
        string[] dimensions = resolution.Split('x');
        if (dimensions.Length == 2 && int.TryParse(dimensions[0], out width) && int.TryParse(dimensions[1], out height))
        {
            return;
        }

        Debug.LogError("Invalid resolution format.");
        width = 800; // Default width
        height = 600; // Default height
    }
}