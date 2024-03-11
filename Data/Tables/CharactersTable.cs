using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataTables/CharactersTable")]
public class CharactersTable : ScriptableObject
{
    public List<CharacterData> characters = new List<CharacterData>();

    public void UnlockCharacter(string characterName)
    {
        CharacterData charToUnlock = characters.Find(c => c.Name == characterName);
        if (charToUnlock != null)
        {
            UnityEngine.Debug.Log($"Unlocking {charToUnlock.Name}...");
            charToUnlock.IsUnlocked = true;
        }
        else
        {
            UnityEngine.Debug.Log($"{characterName} not found in charactersTable!");
        }
    }
}