using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int coins;
    public List<PlayerUpgrades> upgrades;
   // public CharacterData selectedCharacter;
    // Adding stage completion data
    public List<string> completedStages;
    public Dictionary<string, bool> savedFlags = new Dictionary<string, bool>();

    //achievements
    public List<string> achievements; // Names of all unlocked characters
    //endless stage
    public int endlessHighScore;
    public string profilePictureName;
    public string profileName;
    public List<ComboDiscoveryState> discoveredCombos;
    //
   // public List<EnemyDefeatData> defeatedEnemies; // Add this line

}

