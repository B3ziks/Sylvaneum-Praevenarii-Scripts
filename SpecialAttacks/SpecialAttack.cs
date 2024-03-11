using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAttack : ScriptableObject
{
    public string attackName;
    public int damage;
    public float cooldown;
    public float initialDelay;

    // Updated parameter type to ISpecialAttackExecutor.
    public abstract void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager);
}