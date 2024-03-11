using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class SummaryPanelWon : MonoBehaviour
{
    [SerializeField] private GameObject upgradePrefab;  // Prefab with Image and Text component to display upgrade
    [SerializeField] private Transform contentPanel;    // Parent transform where upgrade UI elements will be instantiated
    private Level level;               // Reference to the Level script
    [SerializeField] private TextMeshProUGUI headerText;           // Text component to display header "your acquired upgrades"


    void Start()
    {
           level = Level.Instance; // Get the instance of Level
         if (level == null)
          {
              UnityEngine.Debug.LogError("Level instance is null.");
               return;
          }

        DisplayAcquiredUpgrades();
    }
    void DisplayAcquiredUpgrades()
    {
        headerText.text = "Your acquired upgrades";

        // Instantiate and display each acquired upgrade
        foreach (UpgradeData upgrade in level.AcquiredUpgrades)
        {
            GameObject upgradeUI = Instantiate(upgradePrefab, contentPanel);
            upgradeUI.GetComponentInChildren<Image>().sprite = upgrade.icon;
        }
    }
}