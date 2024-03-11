using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboPanel : MonoBehaviour
{
    [Header("Combo UI")]
    [SerializeField] private GameObject comboPanel; // The panel that contains the combo UI
    [SerializeField] private TMP_Text comboTitleText; // TextMeshPro text for "COMBO!"
    [SerializeField] private TMP_Text comboDescriptionText; // TextMeshPro text for combo name and its description
    [SerializeField] private PauseManager pauseManager;

    private void Awake()
    {
        // Ensure PauseManager is assigned.
        if (pauseManager == null)
        {
            UnityEngine.Debug.LogError("PauseManager not assigned in ComboPanelController!");
        }
    }

    public void ShowPanel(string comboName, string comboDescription)
    {
        pauseManager.PauseGame();
        comboTitleText.text = "COMBO!";
        comboDescriptionText.text = comboName + ": " + comboDescription;
        comboPanel.SetActive(true);
    }

    public void HidePanel()
    {
        comboPanel.SetActive(false);
        pauseManager.UnPauseGame();
    }
}