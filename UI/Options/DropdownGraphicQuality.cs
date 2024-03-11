using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Import TextMesh Pro namespace
using System;

public class DropdownGraphicQuality : MonoBehaviour
{
    [SerializeField] private OptionsContainer optionsContainer;

    private void Awake()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set the current dropdown value to the previously saved graphic quality
        dropdown.value = (int)Enum.Parse(typeof(GraphicQuality), optionsContainer.graphicQuality);
    }

    private void OnDropdownValueChanged(int index)
    {
        optionsContainer.graphicQuality = ((GraphicQuality)index).ToString();
    }
}