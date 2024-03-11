using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStats
{
    public int hp = 999;
    public int damage = 1;
    public int experience_reward = 400;
    public float moveSpeed = 1f;
    public int armor = 0;
    public float damageReceivedMultiplier = 1f; // 1 = 100%, 1.5 = 150%, etc.
    public float hpRegenerationRate = 1f;
    public float hpRegenerationTimer;
    public float timeToAttack = 1f;
    // Elemental properties
    public ElementType elementType = ElementType.Normal;
    public float elementalPotency; // Represents the strength or duration of the effect, depending on the element type
    public int elementalDamageOverTime;

    public EnemyStats(EnemyStats stats)
    {
        this.hp = stats.hp;
        this.damage = stats.damage;
        this.experience_reward = stats.experience_reward;
        this.moveSpeed = stats.moveSpeed;
        this.armor = stats.armor;
        this.damageReceivedMultiplier = stats.damageReceivedMultiplier;
        this.hpRegenerationRate += stats.hpRegenerationRate;
        this.elementType = stats.elementType;
        this.elementalPotency = stats.elementalPotency;
        this.elementalDamageOverTime = stats.elementalDamageOverTime;
    }

    internal void ApplyProgress(float progress)
    {
        UnityEngine.Debug.Log($"Applying progress {progress} to stats. Original HP: {hp}, Damage: {damage}");
        this.hp = (int)(hp * progress);
        this.damage = (int)(damage * progress);
        UnityEngine.Debug.Log($"New HP: {hp}, Damage: {damage}");
    }

}


[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public string Name;
    public int enemyId; // Unique identifier for each type of enemy
    public LocationData enemyLocation; // Replace the string 'Location' field

    public PoolObjectData poolObjectData;
    public EnemyStats stats;

    // New projectile-related fields
    public PoolObjectData projectilePoolData;
    public Vector3 projectileSpawnOffset = Vector3.zero; // Offset for spawning the projectile relative to the enemy position

    // Boss data
    public bool isBoss; // Whether this enemy is a boss or not
    public BossData bossData; // Contains boss-specific data, only populated if 'isBoss' is true

    // Elite data
    public bool isElite; // Whether this enemy is elite or not
    public BossData eliteData; // Using the BossData structure for elite as well, only populated if 'isElite' is true
    //ui
    public Sprite image;
    public string description;
    private EnemyStats defaultStats;

    private void OnEnable()
    {
        defaultStats = new EnemyStats(stats); // Store a copy of the initial stats

    }

    public void ResetStats()
    {
        stats = new EnemyStats(defaultStats); // Reset to default stats
    }
}

