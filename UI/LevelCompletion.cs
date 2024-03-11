using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] float timeToCompleteLevel;
    StageTime stageTime;
    PauseManager pauseManager;
    [SerializeField] GameWinPanel levelCompletePanel;

    private bool levelCompleted = false;

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
        pauseManager = FindObjectOfType<PauseManager>();
        levelCompletePanel = FindObjectOfType<GameWinPanel>(true);

        Enemy.BossKilled += OnBossKilled;
    }

    private void OnDestroy()
    {
        Enemy.BossKilled -= OnBossKilled;
    }

    private void Update()
    {

    }

    private void OnBossKilled()
    {
        if (!levelCompleted)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        levelCompleted = true;
        pauseManager.PauseGame();
        levelCompletePanel.gameObject.SetActive(true);
    }
}