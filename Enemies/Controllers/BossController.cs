using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossController : MonoBehaviour, IPoolMember, ISpecialAttackExecutor
{
    public EnemyData enemyData;
    public static event Action BossKilled;  // Declare the event
    public PoolManager poolManager; // Added field to set PoolManager
    PoolMember poolMember;
    private EnemiesManager enemiesManager;

    private float[] specialAttackCooldowns;
    //
    private bool isCasting = false; // New field to check if currently casting an attack
    private float castingDelay = 4.0f; // The delay between casting attacks


    protected virtual void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        enemiesManager = GameObject.FindObjectOfType<EnemiesManager>();

        if (enemyData.isBoss && enemyData.bossData != null)
        {
            InitializeSpecialAttacks();
        }
    }
    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }
    private void InitializeSpecialAttacks()
    {
        specialAttackCooldowns = new float[enemyData.bossData.specialAttacks.Count];
        for (int i = 0; i < specialAttackCooldowns.Length; i++)
        {
            // Set the cooldown to the initial delay if it's the first time the attack is being used
            specialAttackCooldowns[i] = enemyData.bossData.specialAttacks[i].initialDelay > 0 ?
                                        enemyData.bossData.specialAttacks[i].initialDelay :
                                        enemyData.bossData.specialAttacks[i].cooldown;
        }
    }
    public EnemiesManager GetEnemiesManager()
    {
        return enemiesManager;
    }
    protected virtual void Update()
    {
        if (enemyData.isBoss)
        {
            HandleSpecialAttacks();
        }
    }

    public void SetBossData(EnemyData data)
    {
        enemyData = data;

        if (enemyData.isBoss && enemyData.bossData != null)
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
        if (isCasting) return; // If currently casting, do not initiate another attack

        for (int i = 0; i < enemyData.bossData.specialAttacks.Count; i++)
        {
            if (specialAttackCooldowns[i] <= 0)
            {
                StartCoroutine(CastSpecialAttackWithDelay(enemyData.bossData.specialAttacks[i], castingDelay));
                specialAttackCooldowns[i] = enemyData.bossData.specialAttacks[i].cooldown + castingDelay; // Add delay to cooldown
                break; // Break to prevent multiple attacks from casting at the same time
            }
            else
            {
                specialAttackCooldowns[i] -= Time.deltaTime;
            }
        }
    }
    private IEnumerator CastSpecialAttackWithDelay(SpecialAttack attack, float delay)
    {
        isCasting = true; // Set the casting flag to true

        // Optional: Post a "casting" message if you want to notify about the casting start
       // PostDamage("Casting " + attack.attackName, transform.position + Vector3.up * 2f);

        yield return new WaitForSeconds(delay); // Wait for the specified casting delay

        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            isCasting = false; // Reset the casting flag
            yield break;
        }

        ExecuteSpecialAttack(attack);
        isCasting = false; // Reset the casting flag after the attack is executed
    }
    public static void TriggerBossKilled()
    {
        BossKilled?.Invoke();
    }
    private void ExecuteSpecialAttack(SpecialAttack attack)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }
        // Post the special attack name as a message
        Vector3 messagePosition = transform.position + new Vector3(0, 2f, 0); // Adjust the Y offset as needed
        MessageSystem.instance.PostSpecialAttackMessage(attack.attackName, messagePosition);

        attack.ExecuteAttack(this, poolManager); // Adjusted to send PoolManager as a parameter
    }

    // Method to get the player's transform.
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
        return this; // return the current MonoBehaviour instance
    }
    // This method displays a message at the boss's position.
    public virtual void PostDamage(string name, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(name, targetPosition);
    }

}
