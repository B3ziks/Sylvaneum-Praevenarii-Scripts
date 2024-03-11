using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameRestartManager : MonoBehaviour
{
    [SerializeField] private DataContainer dataContainer; // Reference to DataContainer
    [SerializeField] private TimerUI timerUI; // Reference to TimerUI
    [SerializeField] private PlayerMove playerController; // Reference to the PlayerController
    [SerializeField] private PoolSystemManager poolSystemManager;
    [SerializeField] private ComboManager comboManager; // Add this reference
    private PauseManager pauseManager;
    [SerializeField] private GameObject panel; // The MainMenuPanel

    //
    [Header("Fade Screen")]
    [SerializeField] private Image fadeScreenImage;
    [SerializeField] private float fadeDuration = 1.5f;
    public static bool IsRestarting { get; private set; }


    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }
    // Call this method to restart the game level
    public void RestartGameLevel()
    {
        ResetGameTimer();
        ResetStageProgress(); // Reset stage progress here
        ClearEnemies();
        ResetCoins();
        ResetPlayerPosition();
        ReloadGameplayScene();
        // Clear equipped weapons
        Level.Instance.ClearEquippedWeapons();
        Level.Instance.ResetRerolls();
        Level.Instance.ResetUpgradeLists();
        Level.Instance.ResetLevelAndExperience();
        // Reset stage events
        StageEventManager.Instance.ResetStageEvents();
        EnemiesManager.Instance.ResetBossHealthBar();
        // Deactivate all messages
        MessageSystem.instance.DeactivateAllMessages();
        // Reset enemy spawns
        EnemiesManager.Instance.ResetEnemySpawns();
        //clearing all totems
        TotemSpawnerBase[] totemSpawners = FindObjectsOfType<TotemSpawnerBase>();
        foreach (var spawner in totemSpawners)
        {
            spawner.ClearTotems();
        }
        //clearing all summons
        foreach (var summoner in FindObjectsOfType<SummonerSpawnerBase>())
        {
            summoner.ClearSummon();
        }
        //clearing all summons
        foreach (var engineer in FindObjectsOfType<TurretSpawnerBase>())
        {
            engineer.ClearTurrets();
        }
        // Clear all pools
        PoolManager poolManager = FindObjectOfType<PoolManager>();
        // Deactivate all pooled objects
        if (poolManager != null)
        {
            poolManager.DeactivateAllPooledObjects();
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager reference is not set in GameRestartManager.");
        }
        if (poolSystemManager != null)
        {
            poolSystemManager.DeactivateAllPooledObjects();
        }
        else
        {
            UnityEngine.Debug.LogError("PoolSystemManager reference is not set in GameRestartManager.");
        }
        // Restart the music
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.RestartMusic();
        }
        else
        {
            UnityEngine.Debug.LogError("MusicManager reference is not set in GameRestartManager.");
        }
        // Get a reference to UpgradePanelManager
        UpgradePanelManager upgradePanelManager = FindObjectOfType<UpgradePanelManager>();
        if (upgradePanelManager != null)
        {
            upgradePanelManager.ResetRerolls();
        }
        else
        {
            UnityEngine.Debug.LogError("UpgradePanelManager not found in the scene.");
        }
        // Reset the combo state
        if (comboManager != null)
        {
            comboManager.ResetComboState();
        }
        else
        {
            UnityEngine.Debug.LogError("ComboManager reference is not set in GameRestartManager.");
        }
    }

    private void ResetGameTimer()
    {
        StageTime stageTime = FindObjectOfType<StageTime>();
        if (StageTime.instance != null)
        {
            StageTime.instance.time = 0f;
            TimerUI timerUI = FindObjectOfType<TimerUI>();
            if (timerUI != null)
            {
                timerUI.UpdateTime(StageTime.instance.time);
            }
            else
            {
                UnityEngine.Debug.LogError("TimerUI is not assigned.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("StageTime is not found in the scene.");
        }
    }
    private void ClearEnemies()
    {
        EnemiesManager enemiesManager = FindObjectOfType<EnemiesManager>();
        if (enemiesManager != null)
        {
            enemiesManager.DeactivateAllEnemies();
        }
    }
    private void ResetCoins()
    {
        Coins coinsComponent = FindObjectOfType<Coins>();
        if (coinsComponent != null)
        {
            coinsComponent.ResetStageCoins();
        }
    }
    private void ResetPlayerPosition()
    {
        if (playerController != null)
        {
            playerController.transform.position = Vector3.zero; // Reset player position to (0,0,0)
            // Additional reset logic for the player (like health, status, etc.) if needed
        }
    }
    private void ReloadGameplayScene()
    {
        if (StagesManager.instance != null && StagesManager.instance.CurrentStageData != null)
        {
            string gameplaySceneName = StagesManager.instance.CurrentStageData.stageID;
            // Unload the current gameplay scene
            SceneManager.UnloadSceneAsync(gameplaySceneName);
            // Load the gameplay scene additively, keeping the "essential" scene loaded
            SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Additive);
        }
        else
        {
            UnityEngine.Debug.LogError("Gameplay scene not found or StagesManager is not properly set.");
        }
    }

    public IEnumerator FadeToBlackAndBack(Action onFullyFaded = null)
    {
        IsRestarting = true;

        // Fade to black
        yield return StartCoroutine(FadeScreen(1.0f));
        onFullyFaded?.Invoke();
        // Fade back to clear
        yield return StartCoroutine(FadeScreen(0.0f));

        IsRestarting = false;
    }

    private IEnumerator FadeScreen(float targetAlpha)
    {
        float speed = Mathf.Abs(fadeScreenImage.color.a - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeScreenImage.color.a, targetAlpha))
        {
            float newAlpha = Mathf.MoveTowards(fadeScreenImage.color.a, targetAlpha, speed * Time.deltaTime);
            fadeScreenImage.color = new Color(fadeScreenImage.color.r, fadeScreenImage.color.g, fadeScreenImage.color.b, newAlpha);
            yield return null;
        }
    }
    // Call this method to restart the game level with fade effect
    public void RestartGameLevelWithFade()
    {
        pauseManager.UnPauseGame();
        panel.SetActive(false);

        StartCoroutine(FadeToBlackAndBack(() => {
            GameManager.instance.SetRestartingState(true);
            RestartGameLevel();
            GameManager.instance.SetRestartingState(false);
        }));
    }
    // Call this method to reset the stage progress when restarting the game level
    private void ResetStageProgress()
    {
        // Find the StageProgress component and reset its progress
        StageProgress stageProgress = FindObjectOfType<StageProgress>();
        if (stageProgress != null)
        {
            stageProgress.ResetProgress();
        }
        else
        {
            UnityEngine.Debug.LogError("StageProgress component not found in the scene.");
        }
    }
}