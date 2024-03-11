using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public enum PlayerPersistentUpgrades
{
    HP,
    HPRegen,
    Damage,
    Armor,
    AttackSpeed,
    MovementSpeed,
    GoldGain,
    Dash,
    Reroll,
    CritChance,
    CritMultiplier,
}


[Serializable]
public class PlayerUpgrades
{
    public PlayerPersistentUpgrades persistentUpgrades;
    public int level = 0;
    public int max_level = 10;
    public int costToUpgrade = 100;
    public float increasingValue = 1;
    public string description;
}
[Serializable]
public class ComboDiscoveryState
{
    public ComboType comboType;
    public bool isDiscovered;
}


[CreateAssetMenu]
public class DataContainer : ScriptableObject, IDataPersistence
{
    public int coins;

    // public List<bool> stageCompletion;

    public List<PlayerUpgrades> upgrades;

    public CharacterData selectedCharacter;
    public List<string> completedStages;
    //
    public List<string> achievements = new List<string>();
    //
    [SerializeField] private Difficulty difficulty = Difficulty.Normal;
    public event Action OnSelectedCharacterChanged;
    public DialogueActor selectedCharacterActor;
    public DialogueActor SelectedCharacterActor { get; private set; }
    // Add a field to store the high score for the endless mode
    public int endlessHighScore;
    // Use a string to reference the sprite instead of storing the sprite itself
    public string profilePictureName;
    public string profileName;
    // Add a list to track discovered combos.
    public List<ComboDiscoveryState> discoveredCombos;
    //
    // [SerializeField] private DefeatedEnemiesData defeatedEnemiesData; // Reference to DefeatedEnemiesData
    // New field to track character selections
    public Dictionary<string, int> characterSelectionCounts = new Dictionary<string, int>();


    public Difficulty Difficulty
    {
        get => difficulty;
        set => difficulty = value;
    }

    private void Awake()
    {
        if (discoveredCombos == null)
        {
            discoveredCombos = new List<ComboDiscoveryState>();
        }
    }

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }

    public void UnlockAchievement(string achievementName)
    {
        if (!achievements.Contains(achievementName))
        {
            achievements.Add(achievementName);
            UnityEngine.Debug.Log($"Achievement {achievementName} unlocked!");

            // Immediately save after updating.

        }
    }

    public PlayerUpgrades GetPlayerUpgrade(PlayerPersistentUpgrades upgradeType)
    {
        // Validate if the upgradeType is within valid range
        if (upgradeType < 0 || (int)upgradeType >= upgrades.Count)
        {
            UnityEngine.Debug.LogError($"UpgradeType {upgradeType} is out of bounds.");
            return null;
        }
        return upgrades[(int)upgradeType];
    }

    public float GetIncreasingValue(PlayerPersistentUpgrades upgradeType)
    {
        PlayerUpgrades upgrade = GetPlayerUpgrade(upgradeType);
        return upgrade != null ? upgrade.increasingValue : 0;
    }

    public int GetUpgradeLevel(PlayerPersistentUpgrades upgradeType)
    {
        PlayerUpgrades upgrade = GetPlayerUpgrade(upgradeType);
        return upgrade != null ? upgrade.level : 0;
    }

    public void SetSelectedCharacter(CharacterData character)
    {
        selectedCharacter = character;
        OnSelectedCharacterChanged?.Invoke();
    }
    public void IncrementCharacterSelection(string characterName, int count)
    {
        if (characterSelectionCounts.ContainsKey(characterName))
        {
            characterSelectionCounts[characterName] += count;
        }
        else
        {
            characterSelectionCounts.Add(characterName, count);
        }

        UnityEngine.Debug.Log($"Incremented {characterName} by {count}. New count: {characterSelectionCounts[characterName]}");
    }

    public List<CharacterSelectionData> GetCharacterSelections()
    {
        return characterSelectionCounts.Select(kvp => new CharacterSelectionData
        {
            characterName = kvp.Key,
            chosenCount = kvp.Value
        }).ToList();
    }
    public string GetMostChosenCharacter()
    {
        if (characterSelectionCounts.Count == 0)
            return "None";

        return characterSelectionCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
    public void SetCharacterSelectionCount(string characterName, int count)
    {
        if (characterSelectionCounts.ContainsKey(characterName))
        {
            characterSelectionCounts[characterName] = count;
        }
        else
        {
            characterSelectionCounts.Add(characterName, count);
        }
    }
    //
    public void SetSelectedCharacterActor(DialogueActor actor)
    {
        SelectedCharacterActor = actor; // This is using the property
                                        // Change it to use the field:
        selectedCharacterActor = actor;
        // Trigger an event or do other related things if needed
    }

    // Method to save and load profile data
    public void SaveProfileData(string name, string pictureName)
    {
        profileName = name;
        profilePictureName = pictureName;
    }

    public void ResetSelectedCharacter()
    {
        selectedCharacter = null;
        OnSelectedCharacterChanged?.Invoke();
    }
    public bool IsCharacterSelected(string characterName)
    {
        return selectedCharacter != null && selectedCharacter.Name == characterName;
    }
    public void UpdateCompletedStages(FlagsTable flagTable)
    {
        // Ensure your completedStages list is initialized
        if (completedStages == null) completedStages = new List<string>();

        // Check if a flag in flagTable indicates a stage has been completed
        foreach (var flag in flagTable.flags)
        {
            // If the flag indicating completion of a stage is set...
            if (flag.state)
            {
                // ... and if that stage is not already recorded as completed in your list...
                if (!completedStages.Contains(flag.Name))
                {
                    // ... record it as completed.
                    completedStages.Add(flag.Name);
                    UnityEngine.Debug.Log($"Stage {flag.Name} completed!");
                }
            }
        }
    }
    public Sprite GetProfilePictureSprite()
    {
        if (!string.IsNullOrEmpty(profilePictureName))
        {
            // Load the sprite from the resources (if stored in a Resources folder)
            return Resources.Load<Sprite>(profilePictureName);
        }
        return null; // Or your default sprite
    }

    // Method to get the high score
    public int GetHighScore()
    {
        return endlessHighScore;
    }
    // Method to save the high score
    public void SaveHighScore(int score)
    {
        endlessHighScore = score;
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
    }

    public void ResetDifficultyToNormal()
    {
        SetDifficulty(Difficulty.Normal);
        UnityEngine.Debug.Log("Difficulty Reset to: Normal");
    }

    public bool IsAchievementUnlocked(string achievementName)
    {
        return achievements.Contains(achievementName);
    }
    public void AddDiscoveredCombo(ComboType comboType)
    {
        // Check if the combo has already been discovered
        if (discoveredCombos.Any(c => c.comboType == comboType))
        {
            return;
        }

        // Add the new combo discovery state
        discoveredCombos.Add(new ComboDiscoveryState { comboType = comboType, isDiscovered = true });
    }

    public void SaveData(ref GameData data)
    {
        //UnityEngine.Debug.Log("Saving data from DataContainer to GameData. Coins from DataContainer: " + this.coins);

        if (data == null)
            data = new GameData();


        data.coins = this.coins;
        data.upgrades = this.upgrades;
       // data.selectedCharacter = this.selectedCharacter;  // Assuming CharacterData has a name field
        data.completedStages = this.completedStages;

        //endless stage
        data.endlessHighScore = this.endlessHighScore;
        // Save unlocked characters
        data.achievements = this.achievements;
        // Save the name/reference to the sprite instead of the sprite object
        data.profileName = this.profileName;
        data.profilePictureName = this.profilePictureName; // Store the name/path
        // Save discovered combos
        data.discoveredCombos = new List<ComboDiscoveryState>();
        foreach (var combo in discoveredCombos)
        {
            data.discoveredCombos.Add(new ComboDiscoveryState
            {
                comboType = combo.comboType,
                isDiscovered = combo.isDiscovered
            });
        }
        // Save defeated enemies
        /*
        data.defeatedEnemies = new List<EnemyDefeatData>();
        foreach (var pair in defeatedEnemiesData.enemyDefeatCounts)
        {
            data.defeatedEnemies.Add(new EnemyDefeatData { enemyId = pair.Key, defeatCount = pair.Value });
        }
        */
        /*
        UnityEngine.Debug.Log("Saving data from DataContainer to GameData. Coins from DataContainer: " + this.coins);
        UnityEngine.Debug.Log("Number of upgrades in DataContainer: " + this.upgrades.Count);
        UnityEngine.Debug.Log($"Achievements: {data.achievements.Count}");
        UnityEngine.Debug.Log($"CompletedStages: {this.completedStages.Count}");
        UnityEngine.Debug.Log($"Achievements in DataContainer: {this.achievements.Count}");
        */

    }

    public void LoadData(GameData data)
    {
        UnityEngine.Debug.Log("Loading data into DataContainer. Coins from GameData: " + data.coins);
        UnityEngine.Debug.Log("Number of upgrades in GameData: " + data.upgrades.Count);


        if (data != null)
        {
            this.coins = data.coins;
            this.upgrades = data.upgrades;
           // this.selectedCharacter = data.selectedCharacter;
            this.completedStages = data.completedStages;
            this.achievements = data.achievements;
            this.endlessHighScore = data.endlessHighScore;
            // Load the profile data
            this.profileName = data.profileName;
            this.profilePictureName = data.profilePictureName; // Get the name/path
            // Load discovered combos
            if (data.discoveredCombos != null)
            {
                discoveredCombos = new List<ComboDiscoveryState>(data.discoveredCombos);
            }
            // Load defeated enemies
            /*
            if (data.defeatedEnemies != null)
            {
                foreach (var enemyDefeat in data.defeatedEnemies)
                {
                    defeatedEnemiesData.enemyDefeatCounts[enemyDefeat.enemyId] = enemyDefeat.defeatCount;
                }
            }
            */

        }
    }
}