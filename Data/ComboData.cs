using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewComboData", menuName = "Combo System/Combo Data")]
public class ComboData : ScriptableObject
{
    public ComboType comboType;
    public string comboName;
    public string comboDescription;
    public bool isDiscovered;
    public Sprite Icon; // new field for the icon sprite

}