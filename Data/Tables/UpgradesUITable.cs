using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/UpgradesUITable")]
public class UpgradesUITable : ScriptableObject
{
    public List<UpgradeData> upgrades;

    // ... any methods for filtering enemies by type, etc. ...
}