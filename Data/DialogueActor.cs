using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueActor", menuName = "Dialogues/DialogueActor")]
public class DialogueActor : ScriptableObject
{
    public string actorName;
    public Sprite actorSprite;
}