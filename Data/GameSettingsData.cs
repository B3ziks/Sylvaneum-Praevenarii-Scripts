using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettingsData
{
    public float soundVolume;
    public float musicVolume;
    public string resolution;  // e.g., "1920x1024"
    public string fullscreenMode;
    public string graphicQuality;  // e.g., "Low", "Medium", "High"
    public string shadowQuality;  // e.g., "Low", "Medium", "High"
}