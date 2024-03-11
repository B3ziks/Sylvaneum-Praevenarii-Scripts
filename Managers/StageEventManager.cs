using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    EnemiesManager enemiesManager;
    PlayerWinManager playerWin;
    public static StageEventManager Instance { get; private set; }

    StageTime stageTime;
    int eventIndexer;

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void Start()
    {
        UnityEngine.Debug.Log($"Setting StagesManager CurrentStageData with: {stageData}");

        playerWin = FindObjectOfType<PlayerWinManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        StagesManager.instance.SetCurrentStageData(stageData);
    }

    private void Update()
    {
        if (eventIndexer >= stageData.stageEvents.Count) return;

        if (stageTime.time > stageData.stageEvents[eventIndexer].time)
        {
            ProcessStageEvent();
            eventIndexer++;
        }
    }

    private void ProcessStageEvent()
    {
        switch (stageData.stageEvents[eventIndexer].eventType)
        {
            case StageEventType.SpawnEnemy:
                SpawnEnemy(false, false);
                break;
            case StageEventType.WinStage:
                WinStage();
                break;
            case StageEventType.SpawnEnemyBoss:
                SpawnEnemy(true, false);
                break;
            case StageEventType.SpawnEnemyElite:
                SpawnEnemy(false, true);
                break;
        }
        UnityEngine.Debug.Log(stageData.stageEvents[eventIndexer].message);
    }
    // New method to reset the stage events
    public void ResetStageEvents()
    {
        eventIndexer = 0;
        stageTime.ResetTimer();
        // Reset any other relevant state or timers related to stage events
    }

    private void SpawnEnemyBoss()
    {
        //enemiesManager.SpawnEnemy(stageData.stageEvents[eventIndexer].enemyToSpawn, true);
        SpawnEnemy(true, false);
    }
    private void SpawnEnemyElite()
    {
        //enemiesManager.SpawnEnemy(stageData.stageEvents[eventIndexer].enemyToSpawn, true);
        SpawnEnemy(false, true);
    }
    private void WinStage()
    {
        playerWin.Win(stageData.stageID);
    }

    private void SpawnEnemy(bool bossEnemy, bool eliteEnemy)
    {
        StageEvent currentEvent = stageData.stageEvents[eventIndexer];
        enemiesManager.AddGroupToSpawn(currentEvent.enemyToSpawn, currentEvent.count, bossEnemy, eliteEnemy);

        if (currentEvent.isRepeatedEvent == true)
        {
            enemiesManager.AddRepeatedSpawn(currentEvent, bossEnemy, eliteEnemy); // Added eliteEnemy parameter
        }
    }

    private void SpawnObject()
    {
        Vector3 positionToSpawn = GameManager.instance.playerTransform.position;
        positionToSpawn += UtilityTools.GenerateRandomPositionSquarePattern(new Vector2(5f, 5f));

        SpawnManager.instance.SpawnObject(
            positionToSpawn,
            stageData.stageEvents[eventIndexer].objectToSpawn
        );
    }
}
