using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefeatedEnemiesData", menuName = "Defeated Enemies Data")]
public class DefeatedEnemiesData : ScriptableObject
{
    public Dictionary<int, int> enemyDefeatCounts = new Dictionary<int, int>();
    public int totalDefeatedEnemies = 0; // New field

    public void IncrementDefeatCount(int enemyId)
    {
        if (enemyDefeatCounts.ContainsKey(enemyId))
        {
            enemyDefeatCounts[enemyId]++;
        }
        else
        {
            enemyDefeatCounts.Add(enemyId, 1);
        }
        totalDefeatedEnemies++; // Increment total count
    }

    public int GetDefeatCount(int enemyId)
    {
        if (enemyDefeatCounts.TryGetValue(enemyId, out int count))
        {
            return count;
        }
        return 0;
    }
}