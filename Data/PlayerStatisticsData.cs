using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatisticsData
{
    public List<EnemyDefeatData> defeatedEnemies = new List<EnemyDefeatData>();
    public List<CharacterSelectionData> characterSelections = new List<CharacterSelectionData>();


}

[Serializable]
public class EnemyDefeatData
{
    public int enemyId;
    public int defeatCount;
}

[Serializable]
public class CharacterSelectionData
{
    public string characterName;
    public int chosenCount;
}