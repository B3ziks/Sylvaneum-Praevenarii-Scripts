using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySettingsController : MonoBehaviour
{
    [SerializeField] private OptionsContainer optionsContainer;

    public void OnResolutionChange(string selectedResolution)
    {
        optionsContainer.resolution = selectedResolution;
        string[] dimensions = selectedResolution.Split('x');
        if (dimensions.Length == 2)
        {
            int width, height;
            if (int.TryParse(dimensions[0], out width) && int.TryParse(dimensions[1], out height))
            {
                // Apply the resolution here.
                Screen.SetResolution(width, height, (UnityEngine.FullScreenMode)Enum.Parse(typeof(FullscreenMode), optionsContainer.fullscreenMode));
            }
        }
    }

    public void OnFullscreenModeChange(int index)
    {
        optionsContainer.fullscreenMode = ((FullscreenMode)index).ToString();
        Screen.fullScreenMode = (UnityEngine.FullScreenMode)index;  // Directly using Unity's FullScreenMode enum.
    }
}