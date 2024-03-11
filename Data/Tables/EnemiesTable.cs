using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/EnemiesTable")]
public class EnemiesTable : ScriptableObject
{
    public List<EnemyData> enemies;

    // ... any methods for filtering enemies by type, etc. ...
}