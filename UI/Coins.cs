using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [SerializeField] DataContainer data;
    [SerializeField] StageCoinsContainer stageCoinsContainer;
    [SerializeField] TMPro.TextMeshProUGUI coinsCountText;
    public DataPersistenceManager dataPersistenceManager;
    public int StageCoins => stageCoinsContainer.coins; // Property to get the current stage coins

    [SerializeField] private AchievementManager achievementManager; // Add reference to AchievementManager

    private void OnDisable()
    {
        ResetStageCoins();
    }
    public void Add(int count)
    {
        stageCoinsContainer.coins += count;
        data.coins += count;
        coinsCountText.text = "Coins:" + stageCoinsContainer.coins.ToString();

        CheckCoinAchievements(count); // Check for coin-related achievements

        GameData tempGameData = new GameData();
        data.SaveData(ref tempGameData);
        DataPersistenceManager.instance.SaveGame(); // Assuming you have a static instance in DataPersistenceManager
    }

    public void ResetStageCoins()
    {
        stageCoinsContainer.coins = 0;
        coinsCountText.text = "Coins: 0";
        UnityEngine.Debug.Log("Stage coins reset to 0");
    }

    public void CheckCoinAchievements(int addedCoins)
    {
        // Debugging: Checking if objects are null.
        UnityEngine.Debug.Log("Checking coins...");
        UnityEngine.Debug.Log($"Achievement Manager: {achievementManager != null}");
        if (achievementManager == null) return;  // Added null check to prevent further errors if it's null.

        UnityEngine.Debug.Log($"Flag from Achievement Manager: {achievementManager.GetFlag("Rich") != null}");

        // Assuming achievementManager is assigned via the inspector or elsewhere.
        Flag richFlag = achievementManager.GetFlag("Rich");
        if (richFlag != null)
        {
            if (data.coins + addedCoins >= 50 && !richFlag.state)
            {
                richFlag.state = true;
                achievementManager.CheckAchievements();
                data.UnlockAchievement("Rich");

            }
        }
        else
        {
            UnityEngine.Debug.LogError("Rich flag is null. Please check the flag name and data.");
        }
    }
    // Method to remove coins
    public void Remove(int count)
    {
        int coinsAfterRemoval = Mathf.Max(0, stageCoinsContainer.coins - count);
        int coinsRemoved = stageCoinsContainer.coins - coinsAfterRemoval;

        stageCoinsContainer.coins = coinsAfterRemoval;
        data.coins -= coinsRemoved;
        coinsCountText.text = "Coins:" + stageCoinsContainer.coins.ToString();

        // Assuming you want to save after removing coins
        GameData tempGameData = new GameData();
        data.SaveData(ref tempGameData);
        DataPersistenceManager.instance.SaveGame(); // Save the game data
    }
}

