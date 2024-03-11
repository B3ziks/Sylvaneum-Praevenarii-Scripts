using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ExperienceBar experienceBar;
    [SerializeField] private UpgradePanelManager upgradePanel;
    [SerializeField] private DataContainer data;
    [SerializeField] private AchievementManager achievementManager;

    [Header("Upgrades")]
    [SerializeField] private List<UpgradeData> upgradesAvailableOnStart;
    private List<UpgradeData> selectedUpgrades = new List<UpgradeData>();
    public List<UpgradeData> AcquiredUpgrades { get; private set; } = new List<UpgradeData>();
    [SerializeField] private List<UpgradeData> weaponGetUpgrades = new List<UpgradeData>();
    [SerializeField] private List<UpgradeData> itemGetUpgrades = new List<UpgradeData>();
    [SerializeField] private List<UpgradeData> weaponUpgradeUpgrades = new List<UpgradeData>();
    [SerializeField] private List<UpgradeData> itemUpgradeUpgrades = new List<UpgradeData>();
    private WeaponManager weaponManager;
    private PassiveItems passiveItems;

    public static Level Instance { get; private set; }

    private int level = 1;
    private int experience = 0;
    // Initial Level and Experience
    private int initialLevel;
    private int initialExperience;

    private const int EXPERIENCE_MULTIPLIER = 1000;
    [SerializeField] public List<WeaponData> equippedWeapons = new List<WeaponData>();
    [SerializeField] public List<Item> equippedItems = new List<Item>();

    private List<UpgradeData> previousComboUpgrades = new List<UpgradeData>();
    private ComboManager comboManager;
    private List<ComboType> comboActivated = new List<ComboType>();

    [Header("Rerolls")]
    [SerializeField] private int totalRerollsAvailable = 3;  // Define how many rerolls you want for the entire game
    private int rerollsUsed = 0;  // Keep track of rerolls used
    //intial list
    private List<UpgradeData> initialWeaponGetUpgrades;
    private List<UpgradeData> initialItemGetUpgrades;
    private List<UpgradeData> initialWeaponUpgradeUpgrades;
    private List<UpgradeData> initialItemUpgradeUpgrades;
    private Dictionary<UpgradeData, int> initialUpgradeLevels = new Dictionary<UpgradeData, int>();

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        comboManager = GetComponent<ComboManager>();
        SyncWeaponsWithComboManager();

        weaponManager = GetComponent<WeaponManager>();
        passiveItems = GetComponent<PassiveItems>();
        // Store the initial state


        StoreInitialState();
    }

    private int ExperienceToLevelUp => level * EXPERIENCE_MULTIPLIER;

    private void Start()
    {
        if (GameDataStartingWeapon.SelectedStartingWeapon != null)
        {
            equippedWeapons.Add(GameDataStartingWeapon.SelectedStartingWeapon);
        }
        experienceBar.SetLevelText(level);
        experienceBar.UpdateExperienceSlider(experience, ExperienceToLevelUp);
        ApplyPersistentUpgrade();
    }

    private void StoreInitialState()
    {
        initialWeaponGetUpgrades = new List<UpgradeData>(weaponGetUpgrades);
        initialItemGetUpgrades = new List<UpgradeData>(itemGetUpgrades);
        initialWeaponUpgradeUpgrades = new List<UpgradeData>(weaponUpgradeUpgrades);
        initialItemUpgradeUpgrades = new List<UpgradeData>(itemUpgradeUpgrades);

        StoreInitialUpgradeLevels(weaponGetUpgrades);
        StoreInitialUpgradeLevels(itemGetUpgrades);
        StoreInitialUpgradeLevels(weaponUpgradeUpgrades);
        StoreInitialUpgradeLevels(itemUpgradeUpgrades);
        //initial level and experience
        initialLevel = level;
        initialExperience = experience;
    }
    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
        experienceBar.UpdateExperienceSlider(experience, ExperienceToLevelUp);
    }

    private void CheckLevelUp()
    {
        int maxIterations = 10;
        int iterations = 0;

        while (experience >= ExperienceToLevelUp && iterations < maxIterations)
        {
            LevelUp();
            iterations++;
        }

        if (iterations >= maxIterations)
        {
            UnityEngine.Debug.LogWarning("Potential infinite loop detected during level up.");
        }
    }

    private void LevelUp()
    {
      //  GameRestartManager restartManager = FindObjectOfType<GameRestartManager>();
        if (GameRestartManager.IsRestarting)
        {
            return; // Skip leveling up if the game is restarting
        }
        experience -= ExperienceToLevelUp;
        level++;
        experienceBar.SetLevelText(level);

        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(3));
        upgradePanel.OpenPanel(selectedUpgrades);

        //achievements
        CheckLevelAchievements();
    }

    private void CheckLevelAchievements()
    {
        if (level < 5) return;

        Flag smartFlag = achievementManager?.GetFlag("Smart");
        if (smartFlag != null && !smartFlag.state)
        {
            smartFlag.state = true;
            achievementManager?.CheckAchievements();
            data.UnlockAchievement("Smart");
        }
        else if (smartFlag == null)
        {
            UnityEngine.Debug.LogError("Smart flag is missing.");
        }
    }

    public void Upgrade(int selectedUpgradeId)
    {
        UpgradeData selectedUpgrade = selectedUpgrades[selectedUpgradeId];

        switch (selectedUpgrade.upgradeType)
        {
            case UpgradeType.WeaponGet:
                if (equippedWeapons.Count < 4) // Only add the weapon if there are less than 4
                {
                    weaponGetUpgrades.Remove(selectedUpgrade);
                    EquipWeapon(selectedUpgrade.weaponData);
                }
                break;

            case UpgradeType.ItemGet:
                itemGetUpgrades.Remove(selectedUpgrade);
                EquipItem(selectedUpgrade.item); // Equip the item
                break;

            case UpgradeType.WeaponUpgrade:
                weaponUpgradeUpgrades.Remove(selectedUpgrade);
                weaponManager.UpgradeWeapon(selectedUpgrade);  // Pass UpgradeData directly
                break;

            case UpgradeType.ItemUpgrade:
                itemUpgradeUpgrades.Remove(selectedUpgrade);
                passiveItems.UpgradeItem(selectedUpgrade); // Pass UpgradeData directly
                break;
        }
        AcquiredUpgrades.Add(selectedUpgrade);
    }



    private void ShuffleUpgrades(List<UpgradeData> listToShuffle)
    {
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            int randIndex = UnityEngine.Random.Range(0, i + 1);
            UpgradeData temp = listToShuffle[i];
            listToShuffle[i] = listToShuffle[randIndex];
            listToShuffle[randIndex] = temp;
        }
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        // Shuffle each list separately.
        ShuffleUpgrades(weaponGetUpgrades);
        ShuffleUpgrades(itemGetUpgrades);
        ShuffleUpgrades(weaponUpgradeUpgrades);
        ShuffleUpgrades(itemUpgradeUpgrades);

        // Existing code for creating dictionaries remains unchanged
        Dictionary<WeaponType, int> weaponTypeOccurrences = equippedWeapons
            .Select(weapon => weapon.weaponType)
            .GroupBy(type => type)
            .ToDictionary(group => group.Key, group => group.Count());

        int representedWeightFactor = 5;

        Dictionary<UpgradeStatType, int> highestLevelsPerStatType = AcquiredUpgrades
            .GroupBy(u => u.upgradeStatType)
            .ToDictionary(group => group.Key, group => group.Max(u => u.upgradeLevel));

        // Adjusted function to determine if the player can acquire the upgrade
        Func<UpgradeData, bool> canAcquireUpgrade = (upgrade) =>
        {
            // If it's a WeaponGet upgrade, and the player has less than 4 weapons, they can acquire it.
            if (upgrade.upgradeType == UpgradeType.WeaponGet && equippedWeapons.Count < 4 && !equippedWeapons.Any(w => w.Name == upgrade.weaponData.Name))
            {
                return true;
            }

            // If it's a WeaponUpgrade, ensure the player has the weapon.
            if (upgrade.upgradeType == UpgradeType.WeaponUpgrade && !equippedWeapons.Any(w => w.Name == upgrade.weaponData.Name))
            {
                return false; // Can't acquire upgrades for weapons not owned
            }

            // If it's an ItemGet or ItemUpgrade, check if the player has the item
            if ((upgrade.upgradeType == UpgradeType.ItemGet || upgrade.upgradeType == UpgradeType.ItemUpgrade) && !equippedItems.Contains(upgrade.item))
            {
                return false; // Can't acquire upgrades for items not owned
            }

            // For WeaponUpgrade and ItemUpgrade, check if the player can get the next level
            var existingUpgradeForWeaponOrItem = AcquiredUpgrades
                .Where(u => u.weaponData == upgrade.weaponData && u.upgradeStatType == upgrade.upgradeStatType)
                .OrderByDescending(u => u.upgradeLevel)
                .FirstOrDefault();

            if (existingUpgradeForWeaponOrItem == null)
            {
                return upgrade.upgradeLevel == 0; // Allow level 0 if no upgrades have been acquired for this weapon/stat type
            }

            return upgrade.upgradeLevel == existingUpgradeForWeaponOrItem.upgradeLevel + 1; // Allow the next level upgrade
        };
        // Combine weaponGetUpgrades with weighting, but only for weapons the player does not have
        List<UpgradeData> eligibleWeaponGets = weaponGetUpgrades
            .Where(u => equippedWeapons.Count < 4 && !equippedWeapons.Contains(u.weaponData))
            .Select(u => new {
                Upgrade = u,
                Weight = weaponTypeOccurrences.TryGetValue(u.weaponData.weaponType, out int occ) ? occ * representedWeightFactor : 1
            })
            .OrderByDescending(u => u.Weight)
            .SelectMany(u => Enumerable.Repeat(u.Upgrade, u.Weight))
            .ToList();

        // Filter and weight weapon upgrades
        List<UpgradeData> eligibleWeaponUpgrades = weaponUpgradeUpgrades
            .Where(canAcquireUpgrade)
            .ToList();

        // Filter and include 'ItemGet' upgrades only if the player doesn't have the item yet
        List<UpgradeData> eligibleItemGets = itemGetUpgrades
            .Where(u => !equippedItems.Any(i => i.Name == u.item.Name))
            .ToList();
        List<UpgradeData> eligibleItemUpgrades = itemUpgradeUpgrades.Where(canAcquireUpgrade).ToList();


        // Combine all eligible upgrades
        var allEligibleUpgrades = eligibleWeaponGets
            .Concat(eligibleWeaponUpgrades)
            .Concat(eligibleItemGets)
            .Concat(eligibleItemUpgrades)
            .ToList();

        // Shuffle the final list before selecting
        ShuffleUpgrades(allEligibleUpgrades);

        // Custom DistinctBy method to ensure uniqueness
        List<UpgradeData> GetDistinctUpgrades(IEnumerable<UpgradeData> upgrades, Func<UpgradeData, string> keySelector)
        {
            var uniqueUpgrades = new HashSet<string>();
            var distinctUpgrades = new List<UpgradeData>();
            foreach (var upgrade in upgrades)
            {
                string key = keySelector(upgrade);
                if (uniqueUpgrades.Add(key))
                {
                    distinctUpgrades.Add(upgrade);
                }
            }
            return distinctUpgrades;
        }

        // Prioritize WeaponGet upgrades if the player has less than 4 weapons
        var selectedWeaponGets = GetDistinctUpgrades(allEligibleUpgrades.Where(u => u.upgradeType == UpgradeType.WeaponGet),
                                                     u => u.weaponData.Name)
                                 .Take(4 - equippedWeapons.Count)
                                 .ToList();

        int remainingSlots = Mathf.Max(0, count - selectedWeaponGets.Count);
      var remainingUpgrades = GetDistinctUpgrades(allEligibleUpgrades.Except(selectedWeaponGets),
    u => {
        switch (u.upgradeType)
        {
            case UpgradeType.WeaponGet:
            case UpgradeType.WeaponUpgrade:
                return u.weaponData != null ? u.weaponData.Name : null;
            case UpgradeType.ItemGet:
            case UpgradeType.ItemUpgrade:
                return u.item != null ? u.item.Name : null;
            default:
                return null; // Or handle other cases as needed
        }
    })
    .Take(remainingSlots)
    .ToList();


        // Combine selected upgrades
        selectedWeaponGets.AddRange(remainingUpgrades);
        return selectedWeaponGets.GetRange(0, Mathf.Min(count, selectedWeaponGets.Count));
    }

    private bool CanAcquireUpgrade(UpgradeData upgrade)
    {
        if (upgrade.upgradeType == UpgradeType.WeaponGet)
        {
            return equippedWeapons.Count < 4 && !equippedWeapons.Any(w => w.Name == upgrade.weaponData.Name);
        }
        if (upgrade.upgradeType == UpgradeType.ItemGet)
        {
            return !equippedItems.Any(i => i.Name == upgrade.item.Name);
        }
        if (upgrade.upgradeType == UpgradeType.WeaponUpgrade || upgrade.upgradeType == UpgradeType.ItemUpgrade)
        {
            // Check if the player owns the weapon/item
            bool ownsWeaponOrItem = (upgrade.upgradeType == UpgradeType.WeaponUpgrade && equippedWeapons.Any(w => w.Name == upgrade.weaponData.Name)) ||
                                    (upgrade.upgradeType == UpgradeType.ItemUpgrade && equippedItems.Any(i => i.Name == upgrade.item.Name));
            if (!ownsWeaponOrItem) return false;

            // Check if the upgrade level is one more than the highest level the player has acquired
            var existingUpgrade = AcquiredUpgrades.Where(u => u.Name == upgrade.Name && u.upgradeStatType == upgrade.upgradeStatType)
                                                  .OrderByDescending(u => u.upgradeLevel)
                                                  .FirstOrDefault();
            int nextLevel = existingUpgrade != null ? existingUpgrade.upgradeLevel + 1 : 0;
            return upgrade.upgradeLevel == nextLevel;
        }
        return false;
    }

    private List<UpgradeData> FilterUpgradesByLevel(List<UpgradeData> upgrades, UpgradeType type)
    {
        // Create a dictionary to hold the highest level of each upgrade type the player has acquired.
        var highestLevels = new Dictionary<string, int>();
        foreach (var upgrade in AcquiredUpgrades.Where(u => u.upgradeType == type))
        {
            if (highestLevels.ContainsKey(upgrade.Name))
            {
                if (upgrade.upgradeLevel > highestLevels[upgrade.Name])
                {
                    highestLevels[upgrade.Name] = upgrade.upgradeLevel;
                }
            }
            else
            {
                highestLevels.Add(upgrade.Name, upgrade.upgradeLevel);
            }
        }

        // Filter the upgrades to only include the next level that the player does not yet have.
        return upgrades.Where(upgrade =>
            (!highestLevels.ContainsKey(upgrade.Name) && upgrade.upgradeLevel == 0) ||
            (highestLevels.TryGetValue(upgrade.Name, out var currentLevel) && upgrade.upgradeLevel == currentLevel + 1)
        ).ToList();
    }

    public void EquipItem(Item itemToEquip)
    {
        if (equippedItems.Contains(itemToEquip)) return; // Return if item is already equipped

        equippedItems.Add(itemToEquip);

        // Update the PassiveItems list
        PassiveItems passiveItemsComponent = GetComponent<PassiveItems>();
        if (passiveItemsComponent != null)
        {
            if (!passiveItemsComponent.items.Contains(itemToEquip))
            {
                passiveItemsComponent.items.Add(itemToEquip);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Could not find PassiveItems component.");
        }
    }

    // Modify the EquipWeapon method to sync after equipping a weapon
    public void EquipWeapon(WeaponData weaponToEquip)
    {
        if (equippedWeapons.Count >= 4) return; // Return if 4 weapons are already equipped

        if (!equippedWeapons.Contains(weaponToEquip))
        {
            equippedWeapons.Add(weaponToEquip);
            weaponManager.AddWeapon(weaponToEquip);
            SyncWeaponsWithComboManager();
        }
    }

    // Modified logic to upgrade a weapon
    private void UpgradeWeapon(UpgradeData upgradeData)
    {
        WeaponData weaponToUpgrade = upgradeData.weaponData;
        if (equippedWeapons.Contains(weaponToUpgrade))
        {
            weaponManager.UpgradeWeapon(upgradeData); // Passing UpgradeData directly
        }
    }

    // Modified logic to upgrade an item
    private void UpgradeItem(UpgradeData upgradeData)
    {
        Item itemToUpgrade = upgradeData.item;
        if (equippedItems.Contains(itemToUpgrade))
        {
            passiveItems.UpgradeItem(upgradeData); // Passing UpgradeData directly
        }
    }
    // Create a method to synchronize the weapons list with ComboManager
    private void SyncWeaponsWithComboManager()
    {
        UnityEngine.Debug.Log("Synchronizing weapons with ComboManager...");
        comboManager.SetWeaponDataList(equippedWeapons);
    }
    
    public void RerollUpgrades()
    {
        if (rerollsUsed >= totalRerollsAvailable)
        {
            return; // Exit if reroll has already been used this level-up or if max rerolls are used up
        }
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(3));
        upgradePanel.OpenPanel(selectedUpgrades); // Reopen the panel with new upgrades

        rerollsUsed++;  // Increase the count of rerolls used
    }
    public void ClearEquippedWeapons()
    {
        if (equippedWeapons.Count > 1) // Check if there are more than one weapon
        {
            // Remove all weapons except the first one
            equippedWeapons.RemoveRange(1, equippedWeapons.Count - 1);

            // Clear all weapons in the WeaponManager except the first one
            weaponManager.ClearWeapons();

            // Update ComboManager if it relies on equipped weapons
            SyncWeaponsWithComboManager();
        }
        // If there's only one or no weapon, do nothing
    }
    public void ResetRerolls()
    {
        if (data != null)
        {
            float rerollValue = data.GetIncreasingValue(PlayerPersistentUpgrades.Reroll);
            totalRerollsAvailable = 3 + (int)(rerollValue * data.GetUpgradeLevel(PlayerPersistentUpgrades.Reroll));
        }
        rerollsUsed = 0; // Also reset the rerolls used
    }
    public void ApplyPersistentUpgrade()
    {
        if (data != null)
        {
            float rerollValue = data.GetIncreasingValue(PlayerPersistentUpgrades.Reroll);
            totalRerollsAvailable =+ 3 + (int)(rerollValue * data.GetUpgradeLevel(PlayerPersistentUpgrades.Reroll));
        }

    }
    private void StoreInitialUpgradeLevels(List<UpgradeData> upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            if (!initialUpgradeLevels.ContainsKey(upgrade))
            {
                initialUpgradeLevels.Add(upgrade, upgrade.upgradeLevel);
            }
        }
    }

    public void ResetUpgradeLists()
    {
        weaponGetUpgrades = new List<UpgradeData>(initialWeaponGetUpgrades);
        itemGetUpgrades = new List<UpgradeData>(initialItemGetUpgrades);
        weaponUpgradeUpgrades = new List<UpgradeData>(initialWeaponUpgradeUpgrades);
        itemUpgradeUpgrades = new List<UpgradeData>(initialItemUpgradeUpgrades);

        foreach (var upgrade in weaponGetUpgrades.Concat(itemGetUpgrades).Concat(weaponUpgradeUpgrades).Concat(itemUpgradeUpgrades))
        {
            if (initialUpgradeLevels.ContainsKey(upgrade))
            {
                upgrade.upgradeLevel = initialUpgradeLevels[upgrade];
            }
        }
        ResetUpgradeData();
    }

    public void ResetUpgradeData()
    {
        foreach (var upgrade in initialUpgradeLevels.Keys)
        {
            if (upgrade != null)
            {
                upgrade.upgradeLevel = initialUpgradeLevels[upgrade];
            }
        }

        // Reset the list of acquired upgrades
        AcquiredUpgrades.Clear();
    }

    public void ResetLevelAndExperience()
    {
        level = initialLevel;
        experience = initialExperience;
        experienceBar.SetLevelText(level);
        experienceBar.UpdateExperienceSlider(experience, ExperienceToLevelUp);
    }

}