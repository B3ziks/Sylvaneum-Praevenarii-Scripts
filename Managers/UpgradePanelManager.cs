using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] UpgradeDescriptionPanel upgradeDescriptionPanel;

    PauseManager pauseManager;
    [SerializeField] List<UpgradeButton> upgradeButtons;
    //reroll
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollText; // Add this to reference your TMPro text
    [SerializeField] private int totalRerolls = 3; // set this to however many rerolls you want to give per game
    [SerializeField] DataContainer dataContainer; // Reference to your DataContainer script


    Level characterLevel; // Change Level to PlayerLevel

    int selectedUpgradeID;
    List<UpgradeData> upgradeData;

    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
        characterLevel = GameManager.instance.playerTransform.GetComponent<Level>(); // Change Level to PlayerLevel
    }

    private void Start()
    {
        rerollButton.onClick.RemoveListener(RerollUpgrades);
        rerollButton.onClick.AddListener(RerollUpgrades);

        HideButtons();
        selectedUpgradeID = -1;
        LoadInitialRerolls();

    }

    private void LoadInitialRerolls()
    {
        if (dataContainer != null)
        {
            // Apply Reroll upgrade
            ApplyPersistentUpgrade();
            UpdateRerollText(); // Update the reroll text to display the correct amount
        }
        else
        {
            Debug.LogError("DataContainer script not found in the scene.");
        }
    }

    public void OpenPanel(List<UpgradeData> upgradeDatas)
    {
        Clean();
        pauseManager.PauseGame();
        panel.SetActive(true);

        this.upgradeData = upgradeDatas;

        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].Set(upgradeDatas[i]);
        }

        ShowDescription();
        upgradeDescriptionPanel.Set(upgradeDatas);
    }

    public void Clean()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].Clean();
        }
    }

    public void Upgrade(int pressedButtonID)
    {
        characterLevel.Upgrade(pressedButtonID); // Change Upgrade to ApplyUpgrade
        ClosePanel();
        HideDescription();
    }

    private void HideDescription()
    {
        upgradeDescriptionPanel.gameObject.SetActive(false);
    }

    private void ShowDescription()
    {
        upgradeDescriptionPanel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        selectedUpgradeID = -1;
        HideButtons();
        upgradeDescriptionPanel.Set(new List<UpgradeData>());
        pauseManager.UnPauseGame();
        panel.SetActive(false);
    }

    private void HideButtons()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }

    public void RerollUpgrades()
    {
        if (totalRerolls <= 0)
        {
            return; // Exit if no rerolls are left
        }

        characterLevel.RerollUpgrades();

        totalRerolls--;
        UpdateRerollText(); // Update reroll text every time you reroll

        if (totalRerolls == 0)
        {
            rerollButton.interactable = false; // Disable button if no rerolls are left
        }
    }
    public void ApplyPersistentUpgrade()
    {
        if (dataContainer != null)
        {
            float rerollValue = dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.Reroll);
            totalRerolls =+ 3 + (int)(rerollValue * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Reroll));
        }

    }
    private void UpdateRerollText()
    {
        rerollText.text = "Rerolls left: " + totalRerolls;
    }
    public void ResetRerolls()
    {
        if (dataContainer != null)
        {
            float rerollValue = dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.Reroll);
            totalRerolls = 3 + (int)(rerollValue * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Reroll));
        }
        UpdateRerollText();
        rerollButton.interactable = true; // Make sure the reroll button is re-enabled
    }

}