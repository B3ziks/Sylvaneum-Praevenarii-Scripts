using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Game/Location Data")]
public class LocationData : ScriptableObject
{
    public string locationName;
    public Sprite locationImage;
    public string shortDescription;
}