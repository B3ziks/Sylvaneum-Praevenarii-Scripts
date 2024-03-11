using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro support

public class EnemiesSpawnGroup
{
    public EnemyData enemyData;
    public int count;
    public bool isBoss;
    public bool isElite;
    public float repeatTimer;
    public float timeBetweenSpawn;
    public int repeatCount;

    public EnemiesSpawnGroup(EnemyData enemyData, int count, bool isBoss, bool isElite)
    {
        this.enemyData = enemyData;
        this.count = count;
        this.isBoss = isBoss;
        this.isElite = isElite;
    }

    public void SetRepeatSpawn(float timeBetweenSpawns, int repeatCount)
    {
        this.timeBetweenSpawn = timeBetweenSpawns;
        this.repeatCount = repeatCount;
        repeatTimer = timeBetweenSpawn;
    }
}

public class EnemiesManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] StageProgress stageProgress;
    //[SerializeField] GameObject enemy;
    [SerializeField] PoolManager poolManager;
    //[SerializeField] GameObject enemyAnimation;
    [SerializeField] Vector2 spawnArea;
    //[SerializeField] float spawnTimer;
    //[SerializeField] GameObject player;
    GameObject player;
    //float timer;
    public DataContainer dataContainer;

    List<Enemy> bossEnemiesList;
    List<Enemy> eliteEnemiesList;
    private List<Enemy> activeEnemies = new List<Enemy>();

    int totalBossHealth;
    int currentBossHealth;
    [SerializeField] Slider bossHealthBar;
    [SerializeField] private TextMeshProUGUI bossNameText; // TextMeshProUGUI for displaying boss's name

    List<EnemiesSpawnGroup> enemiesSpawnGroupList;
    List<EnemiesSpawnGroup> repeatedSpawnGroupList;
    [SerializeField] LayerMask obstacleLayer; // Layer mask for the obstacles in your game
    public static EnemiesManager Instance { get; private set; }

    int spawnPerFrame = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    private void Start()
    {
        player = GameManager.instance.playerTransform.gameObject;
        bossHealthBar = FindObjectOfType<BossHPBar>(true).GetComponent<Slider>();
        //stageProgress = FindObjectOfType<StageProgress>();
    }
   

    private void Update()
    {
        float currentProgress = stageProgress.Progress;

        ProcessSpawn();
        ProcessRepeatedSpawnGroups();
        UpdateBossHealth();

    }

    private void ProcessRepeatedSpawnGroups()
    {
        if (repeatedSpawnGroupList == null) { return; }
        for(int i = repeatedSpawnGroupList.Count - 1; i >=0;i--)
        {
            repeatedSpawnGroupList[i].repeatTimer -= Time.deltaTime;
            if (repeatedSpawnGroupList[i].repeatTimer < 0)
            {

                repeatedSpawnGroupList[i].repeatTimer = repeatedSpawnGroupList[i].timeBetweenSpawn;
                AddGroupToSpawn(repeatedSpawnGroupList[i].enemyData, repeatedSpawnGroupList[i].count, repeatedSpawnGroupList[i].isBoss, repeatedSpawnGroupList[i].isElite);
                repeatedSpawnGroupList[i].repeatCount -= 1;

                if (repeatedSpawnGroupList[i].repeatCount <= 0)
                {
                    repeatedSpawnGroupList.RemoveAt(i);
                }
            }
        }
    }

    private void ProcessSpawn()
    {
        if (enemiesSpawnGroupList == null){ return; }

        for (int i = 0; i < spawnPerFrame; i++)
        {
            if (enemiesSpawnGroupList.Count > 0)
            {
                if (enemiesSpawnGroupList[0].count <=0) { return; }
                SpawnEnemy(enemiesSpawnGroupList[0].enemyData, enemiesSpawnGroupList[0].isBoss, enemiesSpawnGroupList[0].isElite);
                enemiesSpawnGroupList[0].count -= 1;

                if (enemiesSpawnGroupList[0].count <= 0)
                {
                    enemiesSpawnGroupList.RemoveAt(0);
                }
            }
        }
    }

    private void UpdateBossHealth()
    {
        if(bossEnemiesList == null) { return; }
        if(bossEnemiesList.Count == 0) { return; }

        currentBossHealth = 0;
        for(int i=0; i< bossEnemiesList.Count; i++)
        {
            if(bossEnemiesList[i] == null) { continue; }
            currentBossHealth += bossEnemiesList[i].stats.hp;
        }

        bossHealthBar.value = currentBossHealth;

        if(currentBossHealth <= 0)
        {
            bossHealthBar.gameObject.SetActive(false);
            bossEnemiesList.Clear();
        }
    }

    public void SpawnReinforcement(Vector3 position, EnemyData enemyToSpawn)
    {
        GameObject enemy = poolManager.GetObject(enemyToSpawn.poolObjectData);

        if (enemy == null)
        {
            UnityEngine.Debug.LogWarning("No available objects in the pool for: " + enemyToSpawn.poolObjectData.name);

            // Create a new enemy object if the pool is empty and use it
            enemy = Instantiate(enemyToSpawn.poolObjectData.originalPrefab);
        }

        // Set the parent to "---ENEMIES---"
        Transform enemiesParent = GameObject.Find("---ENEMIES---").transform;
        if (enemiesParent != null)
        {
            enemy.transform.SetParent(enemiesParent);
        }
        else
        {
            UnityEngine.Debug.LogWarning("---ENEMIES--- GameObject not found in the scene!");
        }

        enemy.transform.position = position;
        enemy.SetActive(true); // Activate the enemy if it was retrieved from the pool

        // Spawning main object
        Enemy newEnemyComponent = enemy.GetComponent<Enemy>();
        newEnemyComponent.enemyData = enemyToSpawn;
        newEnemyComponent.SetTarget(player);
        newEnemyComponent.SetStats(enemyToSpawn.stats);
        newEnemyComponent.UpdateStatsForProgress(stageProgress.Progress);
        newEnemyComponent.UpdateStatsForDifficulty(dataContainer.GetDifficulty());
    }

    public void AddGroupToSpawn(EnemyData enemyToSpawn, int count, bool isBoss, bool isElite)
    {
        // This method might be called frequently, so let's ensure there is no memory allocation when not necessary
        if (enemiesSpawnGroupList == null)
            enemiesSpawnGroupList = new List<EnemiesSpawnGroup>();

        enemiesSpawnGroupList.Add(new EnemiesSpawnGroup(enemyToSpawn, count, isBoss, isElite));
    }

    public void SpawnEnemy(EnemyData enemyToSpawn, bool isBoss, bool isElite)
    {
        Vector3 position;
        int attempts = 10; // Maximum number of attempts to find a valid position

        // Attempt to find a valid position
        do
        {
            position = UtilityTools.GenerateRandomPositionSquarePattern(spawnArea) + player.transform.position;
            attempts--;
        }
        while (IsPositionOnObstacle(position) && attempts > 0);

        // If we couldn't find a valid position after all attempts, exit the method
        if (attempts <= 0)
        {
            UnityEngine.Debug.LogWarning("Couldn't find a valid spawn position for enemy.");
            return;
        }

        // Spawning main object
        GameObject newEnemy = poolManager.GetObject(enemyToSpawn.poolObjectData);
        newEnemy.transform.position = position;

        Enemy newEnemyComponent = newEnemy.GetComponent<Enemy>();

        // Assign the EnemyData to the Enemy script
        newEnemyComponent.enemyData = enemyToSpawn;

        newEnemyComponent.SetTarget(player);
        newEnemyComponent.SetStats(enemyToSpawn.stats);
        newEnemyComponent.UpdateStatsForProgress(stageProgress.Progress);

        // Update stats based on the current difficulty
        newEnemyComponent.UpdateStatsForDifficulty(dataContainer.GetDifficulty());

        if (isBoss)
        {
            BossController bossController = newEnemy.GetComponent<BossController>();
            if (bossController != null)
            {
                bossController.SetBossData(enemyToSpawn);
            }
            SpawnBossEnemy(newEnemyComponent);
        }
        if (isElite)
        {
            EliteEnemyController eliteEnemyController = newEnemy.GetComponent<EliteEnemyController>();
            if (eliteEnemyController != null)
            {
                eliteEnemyController.SetEliteEnemyData(enemyToSpawn);
            }
            SpawnEliteEnemy(newEnemyComponent);
        }
        newEnemy.transform.parent = transform;
        // Add the new enemy to the active enemies list
        activeEnemies.Add(newEnemyComponent);
    }

    private void SpawnBossEnemy(Enemy newBoss)
    {
        if (bossEnemiesList == null)
        {
            bossEnemiesList = new List<Enemy>();
        }

        bossEnemiesList.Add(newBoss);
        totalBossHealth += newBoss.stats.hp;

        bossHealthBar.gameObject.SetActive(true);
        bossHealthBar.maxValue = totalBossHealth;

        // Update the boss's name in the UI
        if (bossNameText != null && newBoss.enemyData.isBoss)
        {
            bossNameText.text = newBoss.enemyData.Name; // Set the boss's name
            bossNameText.gameObject.SetActive(true); // Ensure the text object is active
        }
    }

    private void SpawnEliteEnemy(Enemy newElite)
    {
        UnityEngine.Debug.Log("Spawning an Elite Enemy!");

        if (eliteEnemiesList == null)
        {
            eliteEnemiesList = new List<Enemy>();
        }

        eliteEnemiesList.Add(newElite);

    }

    public void AddRepeatedSpawn(StageEvent stageEvent, bool isBoss, bool isElite)
    {
        EnemiesSpawnGroup repeatSpawnGroup = new EnemiesSpawnGroup(stageEvent.enemyToSpawn, stageEvent.count, isBoss, isElite);
        repeatSpawnGroup.SetRepeatSpawn(stageEvent.repeatEverySeconds, stageEvent.repeatCount);

        if (repeatedSpawnGroupList == null)
        {
            repeatedSpawnGroupList = new List<EnemiesSpawnGroup>();
        }

        repeatedSpawnGroupList.Add(repeatSpawnGroup);
    }

    private bool IsPositionOnObstacle(Vector3 position)
    {
        float checkRadius = 0.5f; // You can adjust this based on the size of your enemy and obstacles
        return Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);
    }
    public void ResetBossHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
            bossHealthBar.value = 0;  // Reset the value to 0
            currentBossHealth = 0;    // Reset the current boss health
            totalBossHealth = 0;      // Reset the total boss health
        }
        // Also hide the boss name text when resetting the boss health bar
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }

        // Clear the boss enemies list
        if (bossEnemiesList != null)
        {
            bossEnemiesList.Clear();
        }
    }
    public void ResetEnemySpawns()
    {
        if (enemiesSpawnGroupList != null)
        {
            enemiesSpawnGroupList.Clear();
        }

        if (repeatedSpawnGroupList != null)
        {
            repeatedSpawnGroupList.Clear();
        }

        // Assuming you have a method or a way to deactivate all currently active enemies
        if (activeEnemies != null)
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.ReturnToPool(); // Deactivate and return the enemy to the pool
                }
            }
            activeEnemies.Clear();
        }
    }

    public void DeactivateAllEnemies()
    {
        // Create a temporary list to avoid modifying the collection during iteration
        List<Enemy> enemiesToDeactivate = new List<Enemy>(activeEnemies);
        foreach (var enemy in enemiesToDeactivate)
        {
            if (enemy != null)
            {
                enemy.ReturnToPool(); // Deactivate and return the enemy to the pool
            }
        }

        activeEnemies.Clear(); // Clear the list of active enemies after deactivation
    }
    public void RemoveEnemyFromActiveList(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }
}
