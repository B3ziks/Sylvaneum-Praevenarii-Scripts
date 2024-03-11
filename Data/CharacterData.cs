    using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public string Name;
    public GameObject spritePrefab;
    public WeaponData startingWeapon;
    public bool IsUnlocked; // This will track whether the character is unlocked
    public Sprite characterIcon; // A sprite for the character’s icon
 //   public string UnlockAchievementName; // The name of the achievement that unlocks this character
    public string Description;
    public CharacterTraits traits;
    public DialogueActor characterActor; // Actor for this character

}
[Serializable]
public class CharacterTraits
{
    public int extraHp;
    public int extraArmor;
    public float extraSpeed;
    public float extraHpRegen;
    //... other traits
}

public static class GameDataStartingWeapon
{
    public static WeaponData SelectedStartingWeapon { get; set; }
}