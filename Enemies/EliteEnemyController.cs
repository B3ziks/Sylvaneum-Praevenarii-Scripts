using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class EliteEnemyController : MonoBehaviour, IPoolMember, ISpecialAttackExecutor
{
    public EnemyData enemyData;
    public static event Action EliteEnemyKilled;
    public PoolManager poolManager;
    PoolMember poolMember;
    private EnemiesManager enemiesManager;

    private float[] specialAttackCooldowns;

    protected virtual void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        enemiesManager = GameObject.FindObjectOfType<EnemiesManager>();

        if (enemyData != null && enemyData.isElite && enemyData.eliteData != null)
        {
            InitializeSpecialAttacks();
        }
        else
        {
            UnityEngine.Debug.Log("Enemydata is null!");

        }
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    private void InitializeSpecialAttacks()
    {
        specialAttackCooldowns = new float[enemyData.eliteData.specialAttacks.Count];
        for (int i = 0; i < specialAttackCooldowns.Length; i++)
        {
            specialAttackCooldowns[i] = 0f;
        }
    }

    public EnemiesManager GetEnemiesManager()
    {
        return enemiesManager;
    }

    protected virtual void Update()
    {
        if (enemyData != null && enemyData.isElite)
        {
            HandleSpecialAttacks();
        }
    }

    public void SetEliteEnemyData(EnemyData data)
    {
        enemyData = data;

        if (enemyData != null && enemyData.isElite && enemyData.eliteData != null)
        {
            InitializeSpecialAttacks();
        }
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    private void HandleSpecialAttacks()
    {
        for (int i = 0; i < enemyData.eliteData.specialAttacks.Count; i++)
        {
            if (specialAttackCooldowns[i] <= 0)
            {
                ExecuteSpecialAttack(enemyData.eliteData.specialAttacks[i]);
                specialAttackCooldowns[i] = enemyData.eliteData.specialAttacks[i].cooldown;
            }
            else
            {
                specialAttackCooldowns[i] -= Time.deltaTime;
            }
        }
    }

    public static void TriggerEliteEnemyKilled()
    {
        EliteEnemyKilled?.Invoke();
    }

    protected virtual void ExecuteSpecialAttack(SpecialAttack attack)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        attack.ExecuteAttack(this, poolManager);
    }

    public Transform GetPlayerTransform()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }

    public PoolManager GetPoolManager()
    {
        return poolManager;
    }

    public MonoBehaviour GetMonoBehaviour()
    {
        return this;
    }
}