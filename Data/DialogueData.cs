using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogues/New DialogueData")]
public class DialogueData : ScriptableObject
{
    public string stageName;  // Identifier to determine which stage this dialogue belongs to
    public List<Dialogue> dialogues;
    public List<CharacterDialogue> characterDialogues; // Character-specific dialogues

}

public enum DialogueRole
{
    Character,
    Enemy,
    //... any other roles
}

[Serializable]
public class CharacterDialogue
{
    public string characterName;
    public List<Dialogue> dialogues;
}

[Serializable]
public class Dialogue
{
    public DialogueRole role;
    public string text;
}