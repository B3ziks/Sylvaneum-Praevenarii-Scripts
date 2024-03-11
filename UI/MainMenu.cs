using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel; // The MainMenuPanel
    [SerializeField] private GameObject upgradePanel; // The MainMenuPanel
    [SerializeField] private GameObject comboPanel; // The MainMenuPanel
    [SerializeField] private GameObject defaultMenuPanel; // This is the MainMenuPanel with buttons like "resume", "back to menu", "options".
    [SerializeField] private GameObject optionsPanel; // This is the main OptionsPanel that may have other child panels
    [SerializeField] private GameObject soundOptionsPanel; // The SoundOptions panel which is a child of OptionsPanel
    [SerializeField] private GameObject graphicsOptionsPanel; // The GraphicOptions panel which is a child of OptionsPanel
    [SerializeField] private GameObject introPanel; // The MainMenuPanel
    //
    [SerializeField] private GameObject characterStatsPanel; // The MainMenuPanel
    [SerializeField] private GameObject weaponStatsPanel; // The MainMenuPanel
    [SerializeField] private GameObject gameWinPanel; // The MainMenuPanel
    [SerializeField] private GameObject gameLosePanel; // The MainMenuPanel

    [SerializeField] private List<GameObject> tutorialPanels; // Drag your tutorial panels here in order

    private PauseManager pauseManager;

    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

    void Update()
    {
       // GameRestartManager restartManager = FindObjectOfType<GameRestartManager>();
        if (GameRestartManager.IsRestarting)
        {
            return; // Skip leveling up if the game is restarting
        }

        // Check if any tutorial panel is currently active
        bool isTutorialPanelActive = tutorialPanels.Any(panel => panel.activeInHierarchy);

        // Check if the gameWinPanel or gameLosePanel is active
        bool isGameEndPanelActive = gameWinPanel.activeInHierarchy || gameLosePanel.activeInHierarchy;

        if (!upgradePanel.activeInHierarchy && !introPanel.activeInHierarchy && !isTutorialPanelActive && !isGameEndPanelActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!panel.activeInHierarchy)
                {
                    OpenMenu();
                }
                else
                {
                    CloseMenu();
                }
            }
        }
        else
        {
            return;
        }
    }

    public void CloseMenu()
    {
        ResetMenu();
        pauseManager.UnPauseGame();
        panel.SetActive(false);
    }

    public void OpenMenu()
    {
        pauseManager.PauseGame();
        panel.SetActive(true);
        characterStatsPanel.SetActive(true);
        weaponStatsPanel.SetActive(true);
    }

    // This method resets the menu panels to their default state
    private void ResetMenu()
    {
        defaultMenuPanel.SetActive(true);  // Activate the default main menu panel
        optionsPanel.SetActive(false); // Deactivate the main OptionsPanel
        soundOptionsPanel.SetActive(false); // Deactivate the SoundOptions panel
        graphicsOptionsPanel.SetActive(false); // Deactivate the GraphicOptions panel
        comboPanel.SetActive(false);
    }
}