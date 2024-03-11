using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public AchievementsTable achievementsTable;
    public FlagsTable flagsTable;
    public CharactersTable charactersTable;
    public DataContainer dataContainer; // Reference to DataContainer.


    // Method to get a specific flag
    public Flag GetFlag(string flagName)
    {
        Flag foundFlag = flagsTable.GetFlag(flagName);
        if (foundFlag == null)
            UnityEngine.Debug.LogError($"No flag found with the name: {flagName}");
        return foundFlag;
    }


    public void CheckAchievements()
    {
        foreach (var achievement in achievementsTable.achievements)
        {
            Flag correspondingFlag = flagsTable.GetFlag(achievement.FlagName);
            if (correspondingFlag != null && correspondingFlag.state)
            {
                UnityEngine.Debug.Log($"Flag {correspondingFlag.Name} is True. Unlocking {achievement.UnlockableCharacterName} and adding achievement {achievement.Name} to DataContainer.");

                UnlockAssociatedCharacter(achievement.UnlockableCharacterName);

                // Add achievement to DataContainer
                if (dataContainer != null)
                {
                    if (dataContainer.achievements == null)
                    {
                        dataContainer.achievements = new List<string>();
                    }

                    if (!dataContainer.achievements.Contains(achievement.Name))
                    {
                        dataContainer.achievements.Add(achievement.Name);
                        UnityEngine.Debug.Log($"{achievement.Name} added to DataContainer achievements list. List Count: {dataContainer.achievements.Count}");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("DataContainer is null!");
                }
            }
            else if (correspondingFlag == null)
            {
                UnityEngine.Debug.LogError($"Flag {achievement.FlagName} not found!");
            }
            else
            {
                UnityEngine.Debug.Log($"Flag {correspondingFlag.Name} is False. No action taken.");
            }
        }
    }



    private void UnlockAssociatedCharacter(string characterName)
    {
        CharacterData charToUnlock = charactersTable.characters.Find(c => c.Name == characterName);
        if (charToUnlock != null && !charToUnlock.IsUnlocked)
        {
            charToUnlock.IsUnlocked = true;
            NotifyCharacterUnlock(charToUnlock);
        }
    }

    private void NotifyCharacterUnlock(CharacterData character)
    {
        // Handle character unlock notification logic here.
        UnityEngine.Debug.Log("Unlocked:" + character);
    }
}