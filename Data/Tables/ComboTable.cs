using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo System/Combo Table")]
public class ComboTable : ScriptableObject
{
    public List<ComboData> combos;

    public ComboData GetComboData(ComboType comboType)
    {
        return combos.Find(combo => combo.comboType == comboType);
    }

    // ... additional methods for managing combos ...
}