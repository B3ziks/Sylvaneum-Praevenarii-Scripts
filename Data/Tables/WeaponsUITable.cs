using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/WeaponsUITable")]
public class WeaponsUITable : ScriptableObject
{
    public List<WeaponData> weapons;

    // ... any methods for filtering enemies by type, etc. ...
}