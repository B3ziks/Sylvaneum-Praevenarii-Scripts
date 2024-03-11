using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    WeaponUpgrade,
    ItemUpgrade,
    WeaponGet,
    ItemGet
}
public enum UpgradeStatType
{
    WeaponGet,
    Damage,
    TimeToAttack,
    NumberOfAttacks, 
    NumberOfHits,
    MaxNumberOfSummonedInstances,
    MaxRicochets,
    Armor,
    hpRegenRate,
    movementSpeed,
    magicBarrier,
    critChance,
    critMultiplier,
    ExplosionRadius,
    projectileSpeed,
    summonmovementSpeed,
    attackRange,
    // Add other stat types as needed
}

[CreateAssetMenu]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType;
    public string Name;
    public string description;
    public Sprite icon;

    public WeaponData weaponData;
    public WeaponStats weaponUpgradeStats;

    public Item item;
    public ItemStats itemStats;

    public WeaponType weaponType;
    public bool isComboUpgrade;

    //
    public int upgradeLevel;
    public int maxUpgradeLevel;  // Add this property

    public UpgradeStatType upgradeStatType; // Add this field to specify what stat this upgrade affects

}
