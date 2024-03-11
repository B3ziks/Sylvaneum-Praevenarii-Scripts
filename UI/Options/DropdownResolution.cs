using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;  // <- Don't forget this
using TMPro;

public class DropdownResolution : MonoBehaviour
{
    [SerializeField] private OptionsContainer optionsContainer;

    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialize the dropdown value to match the saved resolution
        InitializeDropdownValue();
    }

    private void InitializeDropdownValue()
    {
        string[] resolutionParts = optionsContainer.resolution.Split('x');
        if (resolutionParts.Length == 2)
        {
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);

            for (int i = 0; i < dropdown.options.Count; i++)
            {
                string[] optionParts = dropdown.options[i].text.Split('x');
                if (optionParts.Length == 2)
                {
                    int optionWidth = int.Parse(optionParts[0]);
                    int optionHeight = int.Parse(optionParts[1]);

                    if (optionWidth == width && optionHeight == height)
                    {
                        dropdown.value = i;
                        break;
                    }
                }
            }
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        string[] resolutionParts = dropdown.options[index].text.Split('x');
        if (resolutionParts.Length == 2)
        {
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);
            Screen.SetResolution(width, height, Screen.fullScreen);
            optionsContainer.resolution = dropdown.options[index].text;
        }
    }
}