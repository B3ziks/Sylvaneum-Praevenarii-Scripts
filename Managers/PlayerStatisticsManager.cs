using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PlayerStatisticsManager : MonoBehaviour
{
    [SerializeField] private DefeatedEnemiesData defeatedEnemiesData; // Reference to ScriptableObject
    [SerializeField] private DataContainer dataContainer; // Reference to DataContainer

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/playerStatistics.json";
        LoadDefeatedEnemiesData();
    }

    public void SaveDefeatedEnemiesData()
    {
        PlayerStatisticsData data = new PlayerStatisticsData();
        foreach (var pair in defeatedEnemiesData.enemyDefeatCounts)
        {
            Debug.Log($"Adding to save data - Enemy ID: {pair.Key}, Count: {pair.Value}");
            data.defeatedEnemies.Add(new EnemyDefeatData { enemyId = pair.Key, defeatCount = pair.Value });
        }
        // Add character selections to data
        data.characterSelections = dataContainer.GetCharacterSelections();

        string json = JsonUtility.ToJson(data);
        Debug.Log($"JSON Data being saved: {json}");
        File.WriteAllText(savePath, json);
    }

    public void LoadDefeatedEnemiesData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerStatisticsData data = JsonUtility.FromJson<PlayerStatisticsData>(json);

            defeatedEnemiesData.enemyDefeatCounts.Clear();
            foreach (var enemyDefeat in data.defeatedEnemies)
            {
                defeatedEnemiesData.enemyDefeatCounts[enemyDefeat.enemyId] = enemyDefeat.defeatCount;
            }

            // Load the character selections into DataContainer
            foreach (var characterSelection in data.characterSelections)
            {
                // Directly set the count from the save file
                dataContainer.SetCharacterSelectionCount(characterSelection.characterName, characterSelection.chosenCount);
            }

            // Update the total defeated count after loading
            defeatedEnemiesData.totalDefeatedEnemies = data.defeatedEnemies.Sum(e => e.defeatCount);
        }
    }

    public int GetTotalDefeatedEnemies()
    {
        int total = 0;
        foreach (var enemyDefeat in defeatedEnemiesData.enemyDefeatCounts.Values)
        {
            total += enemyDefeat;
        }
        return total;
    }

    void OnApplicationQuit()
    {
        SaveDefeatedEnemiesData();
    }
    private void OnDisable()
    {
        SaveDefeatedEnemiesData();

    }
}