using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStats
{
    public int armor;
    public float hpRegenRate;
    public int hpBoost;
    public float movementSpeedBoost;
    public bool barrierActive;

    public ItemStats Sum(ItemStats other)
    {
        return new ItemStats
        {
            armor = this.armor + other.armor,
            hpRegenRate = this.hpRegenRate + other.hpRegenRate,
            hpBoost = this.hpBoost + other.hpBoost,
            movementSpeedBoost = this.movementSpeedBoost + other.movementSpeedBoost,
            barrierActive = this.barrierActive || other.barrierActive
        };
    }
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string Name;
    public ItemStats stats;
    public List<UpgradeData> upgrades;

    public void Init(string Name)
    {
        this.Name = Name;
        stats = new ItemStats();
        upgrades = new List<UpgradeData>();
    }

    public void Equip(Character character)
    {
        character.armor += stats.armor;
        character.hpRegenerationRate += stats.hpRegenRate;
        character.maxHp += stats.hpBoost;
        character.movementSpeed += stats.movementSpeedBoost;
        Debug.Log($"Equipping item {Name} to character. Barrier active: {stats.barrierActive}");

        if (stats.barrierActive)
        {
            character.barrierActive = true;
        }
    }

   
}