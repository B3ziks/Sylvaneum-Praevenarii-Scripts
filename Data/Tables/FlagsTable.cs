using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Flag
{
    public string Name;
    public bool state;
}

[CreateAssetMenu(menuName = "DataTables/FlagsTable")]
public class FlagsTable : ScriptableObject, IDataPersistence
{
    public List<Flag> flags;

    public Flag GetFlag(string nameOfFlag)
    {
        return flags.Find(x => x.Name == nameOfFlag);
    }

    public void SaveData(ref GameData data)
    {
        foreach (var flag in flags)
        {
            data.savedFlags[flag.Name] = flag.state;
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var savedFlag in data.savedFlags)
        {
            Flag matchingFlag = GetFlag(savedFlag.Key);
            if (matchingFlag != null)
            {
                matchingFlag.state = savedFlag.Value;
            }
        }
    }
}
