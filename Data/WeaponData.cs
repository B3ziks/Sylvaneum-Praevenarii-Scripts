using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]

public class WeaponStats
{
    public int damage;
    public float timeToAttack;
    public int numberOfAttacks;
    public int numberOfHits;
    public float projectileSpeed;
    public float stun;
    public float knockback;
    public float knockbackTimeWeight;
    public ElementType elementType = ElementType.Normal;
    public float elementalPotency;  // This can represent the strength or duration of the effect, depending on the element type
    public int damageOverTime;
    public int upgradeLevel;  // Add this line to track the upgrade level
    public float critStrikeChance = 10f;
    public float critStrikeMultiplier =1f;
    //ranged stats
    public int maxRicochets;
    //melee stats
    public int maxNumberOfSummonedInstances;
    //engineer stats
    public int attackRange;
    //summoner stats
    public int moveSpeed;
    public int explosionRadius;
    public int maxUpgradeLevel;
    // Added a constructor without parameters for initializations
    public WeaponStats() { }

    public WeaponStats(WeaponStats stats)
    {
        this.damage = stats.damage;
        this.timeToAttack = stats.timeToAttack;
        this.numberOfAttacks = stats.numberOfAttacks;
        this.numberOfHits = stats.numberOfHits;
        this.projectileSpeed = stats.projectileSpeed;
        this.stun = stats.stun;
        this.knockback = stats.knockback;
        this.knockbackTimeWeight = stats.knockbackTimeWeight;
        this.maxUpgradeLevel = stats.maxUpgradeLevel;
        this.critStrikeChance = stats.critStrikeChance;
        this.critStrikeMultiplier = stats.critStrikeMultiplier;
        // Including elemental stats
        this.elementType = stats.elementType;
        this.elementalPotency = stats.elementalPotency;
        this.damageOverTime = stats.damageOverTime;
        this.maxRicochets = stats.maxRicochets;
        //melee stats
        //engineer stats
        this.attackRange = stats.attackRange;
        this.maxNumberOfSummonedInstances = stats.maxNumberOfSummonedInstances;
        //summoner stats
        this.moveSpeed = stats.moveSpeed;
        //explosive stats
        this.explosionRadius = stats.explosionRadius;
    }

    internal void Sum(WeaponStats weaponUpgradeStats, WeaponType weaponType)
    {
        this.damage += weaponUpgradeStats.damage;
        this.timeToAttack += weaponUpgradeStats.timeToAttack;
        this.numberOfAttacks += weaponUpgradeStats.numberOfAttacks;
        this.numberOfHits += weaponUpgradeStats.numberOfHits;
        this.maxUpgradeLevel += weaponUpgradeStats.maxUpgradeLevel;
        this.critStrikeChance += weaponUpgradeStats.critStrikeChance;
        this.critStrikeMultiplier += weaponUpgradeStats.critStrikeMultiplier;
        if (weaponType == WeaponType.Ranged)
        {
            this.projectileSpeed += weaponUpgradeStats.projectileSpeed;
            //... any other ranged-only stats
        }
        if (weaponType == WeaponType.Melee)
        {
            this.stun += weaponUpgradeStats.stun;
            //... any other melee-only stats
        }
        this.knockback += weaponUpgradeStats.knockback;
        this.knockbackTimeWeight += weaponUpgradeStats.knockbackTimeWeight;
        // Including elemental stats
        this.elementalPotency += weaponUpgradeStats.elementalPotency;

        this.maxRicochets += weaponUpgradeStats.maxRicochets;
        this.damageOverTime += weaponUpgradeStats.damageOverTime;
        //melee
        this.maxNumberOfSummonedInstances += weaponUpgradeStats.maxNumberOfSummonedInstances;
        //engineer
        this.attackRange += weaponUpgradeStats.attackRange;
        //summoner
        this.moveSpeed += weaponUpgradeStats.moveSpeed;
        //explosives
        this.explosionRadius += weaponUpgradeStats.explosionRadius;
    }

    public void ApplyUpgrade(UpgradeData upgrade, WeaponType weaponType)
    {
        this.Sum(upgrade.weaponUpgradeStats, weaponType);
        // Prevent exceeding the max upgrade level.
        upgradeLevel = Mathf.Min(upgradeLevel + 1, upgrade.maxUpgradeLevel);
    }



}


[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string Name;
    public WeaponStats stats; // currentStats
    public WeaponStats baseStats; // added for storing base values
    public WeaponType weaponType;
    public GameObject weaponBasePrefab;
    public List<UpgradeData> upgrades;
    public Sprite Image;
    public string Description;
    // Initializes weapon at the start of a game session
    public void InitializeWeapon()
    {
        ResetToBaseStats();

        // Then load and apply any saved upgrades.
        LoadAndApplyAllUpgrades();
    }

    // Applies a single upgrade and saves the new level
    public void UpgradeWeapon()
    {
        if (upgrades.Count > 0 && stats.upgradeLevel < upgrades[0].maxUpgradeLevel)
        {
            UpgradeData upgradeToApply = upgrades[stats.upgradeLevel];
            stats.ApplyUpgrade(upgradeToApply, weaponType); // Apply upgrade to currentStats
            SaveUpgradeLevel(); // Save the new upgrade level
        }
    }
    public WeaponStats GetCurrentStatsCopy()
    {
        WeaponStats copiedStats = new WeaponStats(stats); // Assume you have a copy constructor

        // Apply saved upgrades
        int savedLevel = PlayerPrefs.GetInt(Name + "_UpgradeLevel", 0);
        for (int i = 0; i < savedLevel && i < upgrades.Count; i++)
        {
            copiedStats.ApplyUpgrade(upgrades[i], weaponType);
        }

        return copiedStats;
    }

    private void LoadAndApplyAllUpgrades()
    {
        LoadUpgradeLevel();
        ApplyAllUpgrades();
    }
    public void ResetToBaseStats()
    {
        stats = new WeaponStats(baseStats); // Copy base stats to current stats.
    }
    private void ApplyAllUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            for (int i = 0; i < stats.upgradeLevel && i < upgrade.maxUpgradeLevel; i++)
            {
                stats.ApplyUpgrade(upgrade, weaponType);
            }
        }
    }

    private void SaveUpgradeLevel()
    {
        PlayerPrefs.SetInt(Name + "_UpgradeLevel", stats.upgradeLevel);
    }

    private void LoadUpgradeLevel()
    {
        stats.upgradeLevel = PlayerPrefs.GetInt(Name + "_UpgradeLevel", 0);
    }
}


public enum WeaponType
{
    Melee,
    Ranged,
    Magic,
    Summoner,
    Engineer,
    Explosives,
    Totem
    //... any other types
}

public enum ElementType
{
    Normal,
    Fire,
    Poison,
    Ice,
    Lightning
    //... any other types
}