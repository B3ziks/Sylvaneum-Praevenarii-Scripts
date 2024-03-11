using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinManager : MonoBehaviour
{
    private PauseManager pauseManager;

    private void Awake()
    {
        pauseManager = FindObjectOfType<PauseManager>();

    }

    private void OnEnable()
    {
        Enemy.BossKilled += OnBossKilled;
    }

    private void OnDisable()
    {
        Enemy.BossKilled -= OnBossKilled;
    }

    private void OnBossKilled()
    {
        UnityEngine.Debug.Log("Boss killed event triggered!");
            GameWinPanel.Instance.gameObject.SetActive(true);
            pauseManager?.PauseGame();

    }
}