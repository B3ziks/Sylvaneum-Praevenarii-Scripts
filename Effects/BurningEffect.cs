using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BurningEffect : IEffect
{
    public event UnityAction<bool> OnEffectStateChanged;

    private bool isEffectActive;
    public bool IsEffectActive
    {
        get => isEffectActive;
        set
        {
            isEffectActive = value;
            OnEffectStateChanged?.Invoke(isEffectActive);
        }
    }
}