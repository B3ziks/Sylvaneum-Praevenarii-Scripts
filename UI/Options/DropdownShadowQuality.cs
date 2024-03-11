using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Import TextMesh Pro namespace
using System.Diagnostics;
using System;

public class DropdownShadowQuality : MonoBehaviour
{
    [SerializeField] private OptionsContainer optionsContainer;

    private void Awake()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set the current dropdown value to the previously saved shadow quality
        dropdown.value = (int)Enum.Parse(typeof(ShadowQuality), optionsContainer.shadowQuality);
    }

    private void OnDropdownValueChanged(int index)
    {
        optionsContainer.shadowQuality = ((ShadowQuality)index).ToString();
    }
}