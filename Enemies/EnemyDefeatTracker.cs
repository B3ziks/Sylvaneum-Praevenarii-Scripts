using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefeatTracker : MonoBehaviour
{
    public static EnemyDefeatTracker Instance { get; private set; }

    // Reference to the ScriptableObject that stores the persistent defeat counts
    public DefeatedEnemiesData defeatedEnemiesData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Enemy.EnemyDefeated += OnEnemyDefeated;
    }

    private void OnDisable()
    {
        Enemy.EnemyDefeated -= OnEnemyDefeated;
    }

    private void OnEnemyDefeated(EnemyData enemyData)
    {
        if (enemyData == null) return;

        int enemyId = enemyData.enemyId;
        defeatedEnemiesData.IncrementDefeatCount(enemyId);

        Debug.Log($"Enemy ID {enemyId} defeated. Total defeats: {defeatedEnemiesData.GetDefeatCount(enemyId)}");
    }
}