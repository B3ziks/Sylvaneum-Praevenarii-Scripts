using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OptionsContainer : ScriptableObject
{
    [Header("Audio Settings")]
    public float soundVolume;
    public float musicVolume;

    [Header("Display Settings")]
    public string resolution; // "1920x1024" format
    
    [HideInInspector]
    public FullscreenMode _fullscreenMode; 
    public string fullscreenMode 
    {
        get { return _fullscreenMode.ToString(); }
        set { _fullscreenMode = EnumStringToFullscreenMode(value); }
    }

    [HideInInspector]
    public GraphicQuality _graphicQuality;
    public string graphicQuality
    {
        get { return _graphicQuality.ToString(); }
        set { _graphicQuality = EnumStringToGraphicQuality(value); }
    }

    [HideInInspector]
    public ShadowQuality _shadowQuality;
    public string shadowQuality
    {
        get { return _shadowQuality.ToString(); }
        set { _shadowQuality = EnumStringToShadowQuality(value); }
    }

    private FullscreenMode EnumStringToFullscreenMode(string value)
    {
        return (FullscreenMode) Enum.Parse(typeof(FullscreenMode), value);
    }

    private GraphicQuality EnumStringToGraphicQuality(string value)
    {
        return (GraphicQuality) Enum.Parse(typeof(GraphicQuality), value);
    }

    private ShadowQuality EnumStringToShadowQuality(string value)
    {
        return (ShadowQuality) Enum.Parse(typeof(ShadowQuality), value);
    }
}

public enum FullscreenMode
{
    Fullscreen,
    Windowed,
    Borderless
}

public enum GraphicQuality
{
    High,
    Medium,
    Low
}

public enum ShadowQuality
{
    High,
    Medium,
    Low
}