using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform weaponObjectContainer;
    [SerializeField] PoolManager poolManager;
    [SerializeField] private DataContainer dataContainer;
    [SerializeField] List<WeaponData> weaponDataList; // From WeaponDataManager

    List<WeaponBase> weapons;
    Character character;
    //combo
    private ComboManager comboManager; // You'll need a reference to the ComboManager
    private float lastRapidFireTime = 0f; // Tracks the last time rapid fire was activated
    private float rapidFireCooldown = 10f; // Cooldown duration in seconds
    private float rapidFireDuration = 3f; // Duration of rapid fire mode
    private Dictionary<WeaponBase, float> originalAttackTimes = new Dictionary<WeaponBase, float>(); // Stores the original attack times
    private float rapidFireAttackTime = 0.1f; // The attack time during rapid fire mode
    private bool isRangedComboActive = false;
    public bool isRapidFireActive = false;
    private Queue<Action> upgradeQueue = new Queue<Action>();
    // New combo variables
    private float lastProjectileSpeedComboTime = 0f; // Tracks the last time the combo was activated
    private float projectileSpeedComboCooldown = 10f; // Cooldown duration in seconds
    private float projectileSpeedComboDuration = 5f; // Duration of the projectile speed increase mode
    private Dictionary<WeaponBase, float> originalProjectileSpeeds = new Dictionary<WeaponBase, float>(); // Stores the original projectile speeds
    private float increasedProjectileSpeed = 20f; // The new projectile speed during the combo
    [SerializeField] private bool isProjectileSpeedComboActive = false;
    public bool isProjectileSpeedActive = false;

    private void Awake()
    {
        weapons = new List<WeaponBase>();
        character = GetComponent<Character>();
        comboManager = FindObjectOfType<ComboManager>(); // Get the ComboManager reference

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isRangedComboActive)
        {
            if (Time.time >= lastRapidFireTime + rapidFireCooldown)
            {
                StartCoroutine(EnableRapidFireCoroutine());
                lastRapidFireTime = Time.time; // Update the last activation time
            }
            else
            {
                UnityEngine.Debug.Log("Rapid fire is on cooldown.");
            }
        }
        //
       
    }

    private void Start()
    {
        InitializeAllWeapons();
        ApplyPersistentUpgradesToAllWeapons();

    }
    
    //
    public void InitializeAllWeapons()
    {
        foreach (var weaponData in weaponDataList)
        {
            weaponData.InitializeWeapon();
        }
    }

    private void ApplyPersistentUpgradesToAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            ApplyPersistentUpgradesToWeapon(weapon);
        }
    }
    public UpgradeData GetUpgradeDataForLevel(int level, WeaponData weaponData)
    {
        if (level >= 0 && level < weaponData.upgrades.Count)
        {
            return weaponData.upgrades[level];
        }
        return null;
    }

    public void SaveWeaponUpgrade(WeaponData weaponData)
    {
        PlayerPrefs.SetInt(weaponData.Name + "_UpgradeLevel", weaponData.stats.upgradeLevel);
    }

    public void UpgradeWeapon(WeaponData weaponData)
    {
        if (weaponData.stats.upgradeLevel < weaponData.upgrades.Count)
        {
            UpgradeData upgradeToApply = GetUpgradeDataForLevel(weaponData.stats.upgradeLevel, weaponData);
            weaponData.stats.ApplyUpgrade(upgradeToApply, weaponData.weaponType);
            SaveWeaponUpgrade(weaponData);
        }
        else
        {
           UnityEngine.Debug.Log($"{weaponData.Name} is already at max upgrade level.");
        }
    }

    public void AddWeapon(WeaponData weaponData)
    {
        weaponData.InitializeWeapon();

        GameObject weaponGameObject = Instantiate(weaponData.weaponBasePrefab, weaponObjectContainer);
        WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();

        weaponBase.SetData(weaponData);
        weaponBase.SetPoolManager(poolManager);
        ApplyPersistentUpgradesToWeapon(weaponBase);

        weapons.Add(weaponBase);
        weaponBase.AddOwnerCharacter(character);  // Ensure the WeaponBase class has this method defined.
    }

    public void ApplyPersistentUpgradesToWeapon(WeaponBase weaponBase)
    {
        if (dataContainer != null)
        {
            foreach (var upgrade in dataContainer.upgrades)
            {
                switch (upgrade.persistentUpgrades)
                {
                    case PlayerPersistentUpgrades.Damage:
                        weaponBase.weaponData.stats.damage += (int)(dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.Damage) * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Damage));
                        break;
                    case PlayerPersistentUpgrades.AttackSpeed:
                        weaponBase.weaponData.stats.timeToAttack += dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.AttackSpeed) * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.AttackSpeed);
                        break;
                    case PlayerPersistentUpgrades.CritChance:
                        weaponBase.weaponData.stats.critStrikeChance += dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.CritChance) * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.CritChance);
                        break;
                    case PlayerPersistentUpgrades.CritMultiplier:
                        weaponBase.weaponData.stats.critStrikeMultiplier += dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.CritMultiplier) * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.CritMultiplier);
                        break;
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("DataContainer is not available.");
        }
    }

    // Ensure there is a method in WeaponBase that accepts UpgradeData and performs the upgrade.
    internal void UpgradeWeapon(UpgradeData upgradeData)
    {
        // Define the upgrade action
        Action upgradeAction = () => {
            WeaponBase weaponToUpgrade = weapons.Find(w => w.weaponData == upgradeData.weaponData);
            if (weaponToUpgrade != null)
            {
                weaponToUpgrade.Upgrade(upgradeData);
                ApplyPersistentUpgradesToWeapon(weaponToUpgrade);
            }
        };

        // Queue the upgrade action if rapid fire is active, otherwise perform the upgrade immediately
        QueueUpgrade(upgradeAction);
    }

    //rapid fire combo

    public void QueueUpgrade(Action upgradeAction)
    {
        if (isRapidFireActive)
        {
            upgradeQueue.Enqueue(upgradeAction);
        }
        else
        {
            upgradeAction.Invoke();
        }
    }

    private void ApplyPendingUpgrades()
    {
        while (upgradeQueue.Count > 0)
        {
            Action upgradeAction = upgradeQueue.Dequeue();
            upgradeAction.Invoke();
        }
    }

    public void ActivateRangedCombo(bool state)
    {
        // This will only set the flag once and will never reset it
        if (!isRangedComboActive) // Make sure this only happens once
        {
            isRangedComboActive = state;
        }
    }
 
    public void EnableRapidFireMode()
    {
        // Only enable rapid fire if not on cooldown and if the combo is active
        if (isRangedComboActive && Time.time >= lastRapidFireTime + rapidFireCooldown)
        {
            StartCoroutine(EnableRapidFireCoroutine());
            lastRapidFireTime = Time.time; // Update the last activation time
        }
        else if (!isRangedComboActive)
        {
            // If the combo has never been activated, do not enable rapid fire
            UnityEngine.Debug.Log("Ranged combo has not been activated.");
        }
        else
        {
            // If the combo is active but on cooldown, inform the user
            UnityEngine.Debug.Log("Rapid fire is on cooldown.");
        }
    }

    private IEnumerator EnableRapidFireCoroutine()
    {
        isRapidFireActive = true; // Indicate that rapid fire is active

        // Store the original attack times and set the new attack time
        foreach (var weapon in weapons)
        {
            originalAttackTimes[weapon] = weapon.weaponData.stats.timeToAttack;
            weapon.weaponData.stats.timeToAttack = rapidFireAttackTime;
        }

        // Wait for the rapid fire duration
        yield return new WaitForSeconds(rapidFireDuration);

        // Restore the original attack times
        foreach (var weapon in weapons)
        {
            if (originalAttackTimes.TryGetValue(weapon, out var originalTime))
            {
                weapon.weaponData.stats.timeToAttack = originalTime;
            }
        }

        isRapidFireActive = false; // Indicate that rapid fire has ended
        ApplyPendingUpgrades(); // Apply any upgrades that were queued during rapid fire
        originalAttackTimes.Clear(); // Clear the dictionary for next use
    }
    public void ClearWeapons()
    {
        if (weapons.Count > 1) // Check if there are more than one weapon
        {
            // Start from index 1, assuming the first weapon is the starting one
            for (int i = 1; i < weapons.Count; i++)
            {
                if (weapons[i] != null)
                {
                    Destroy(weapons[i].gameObject); // Destroy the weapon GameObject
                }
            }

            // Keep only the first weapon in the list
            weapons.RemoveRange(1, weapons.Count - 1);

            // Reset the first weapon's stats to base and reapply persistent upgrades
            if (weapons.Count > 0 && weapons[0] != null)
            {
                weapons[0].ResetToBaseStats();
                ApplyPersistentUpgradesToWeapon(weapons[0]);
            }
        }
        // Additional cleanup if needed
    }
     // Method to temporarily enhance all weapons speed
    public void EnhanceAllWeaponsSpeedTemporarily()
    {
        foreach (var weapon in weapons)
        {
            float originalTimeToAttack = weapon.weaponData.stats.timeToAttack;
            weapon.weaponData.stats.timeToAttack *= 0.9f; // Reduce timeToAttack by 10%
            StartCoroutine(ResetWeaponTimeToAttack(weapon, originalTimeToAttack));
        }
    }

    // Coroutine to reset the weapon's timeToAttack after a duration
    private IEnumerator ResetWeaponTimeToAttack(WeaponBase weapon, float originalTimeToAttack)
    {
        yield return new WaitForSeconds(5f); // Duration of enhancement
        weapon.weaponData.stats.timeToAttack = originalTimeToAttack;
    }
    // Method to temporarily enhance all weapons damage
    public void EnhanceAllWeaponsDamageTemporarily()
    {
        foreach (var weapon in weapons)
        {
            int originalDamage = weapon.weaponData.stats.damage;
            int increasedDamage = Mathf.RoundToInt(originalDamage * 1.25f); // Increase damage by 25% and round to nearest integer
            weapon.weaponData.stats.damage = increasedDamage;
            StartCoroutine(ResetWeaponTimeToAttack(weapon, originalDamage));
        }
    }
    // Coroutine to reset the weapon's damage after a duration
    private IEnumerator ResetWeaponDamage(WeaponBase weapon, int originalDamage)
    {
        yield return new WaitForSeconds(5f); // Duration of enhancement
        weapon.weaponData.stats.damage = originalDamage;
    }
    // Method to temporarily enhance all weapons explosion radius
    public void EnhanceAllWeaponsRadiusTemporarily()
    {
        foreach (var weapon in weapons)
        {
            int originalExplosionRadius = weapon.weaponData.stats.explosionRadius;
            weapon.weaponData.stats.explosionRadius += 1; // Increase explosion radius by 1
            StartCoroutine(ResetExplosionRadius(weapon, originalExplosionRadius));
        }
    }
    // Coroutine to reset the weapon's explosion radius after a duration
    private IEnumerator ResetExplosionRadius(WeaponBase weapon, int originalExplosionRadius)
    {
        yield return new WaitForSeconds(5f); // Duration of enhancement
        weapon.weaponData.stats.explosionRadius = originalExplosionRadius;
    }
    // Method to temporarily enhance all weapons number of attacks
    public void EnhanceAllWeaponsAttacksNumberTemporarily()
    {
        foreach (var weapon in weapons)
        {
            int originalNumberOfAttacks = weapon.weaponData.stats.numberOfAttacks;
            weapon.weaponData.stats.numberOfAttacks += 1; // Increase explosion radius by 1
            StartCoroutine(ResetAttacksNumber(weapon, originalNumberOfAttacks));
        }
    }
    // Coroutine to reset the weapon's explosion radius after a duration
    private IEnumerator ResetAttacksNumber(WeaponBase weapon, int originalNumberOfAttacks)
    {
        yield return new WaitForSeconds(5f); // Duration of enhancement
        weapon.weaponData.stats.numberOfAttacks = originalNumberOfAttacks;
    }
}