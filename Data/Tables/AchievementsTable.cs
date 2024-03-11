using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement
{
    public string Name; // Name of the achievement
    public string Description; // Description to be displayed in UI
    public string FlagName; // Corresponding flag to monitor
    public string UnlockableCharacterName; // Name of character to unlock
    public Sprite Icon; // new field for the icon sprite

    // ... any other relevant properties...
}

[CreateAssetMenu(menuName = "DataTables/AchievementsTable")]
public class AchievementsTable : ScriptableObject
{
    public List<Achievement> achievements;

    // ... methods to check achievements, etc. ...
}