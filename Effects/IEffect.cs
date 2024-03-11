using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;  // Added namespace for UnityEvent.

public interface IEffect
{
    event UnityAction<bool> OnEffectStateChanged;
    bool IsEffectActive { get; }
}