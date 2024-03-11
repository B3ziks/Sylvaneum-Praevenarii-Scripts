using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ElementalEffectsEnemyManager : MonoBehaviour
{
    // Reference to the Enemy script
    private Enemy enemy;

    // Elemental effect data instances
    private EffectData burningEffectData;
    private EffectData poisonEffectData;
    private EffectData iceEffectData;
    private EffectData lightningEffectData;
    // ... other effects

    // Prefabs
    public GameObject firePrefab;
    public GameObject poisonPrefab;
    public GameObject icePrefab;
    public GameObject lightningPrefab;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            UnityEngine.Debug.LogError("Enemy component not found!");
        }

        // Initialize the EffectData instances
        burningEffectData = new EffectData(firePrefab, transform);
        poisonEffectData = new EffectData(poisonPrefab, transform);
        iceEffectData = new EffectData(icePrefab, transform);
        lightningEffectData = new EffectData(lightningPrefab, transform);
        // ... initialize other effects
    }

    private void Update()
    {
        // Process effects if they are active
        burningEffectData.ProcessEffect(enemy);
        poisonEffectData.ProcessEffect(enemy);
        iceEffectData.ProcessEffect(enemy);
        lightningEffectData.ProcessEffect(enemy);
        // ... process other effects
    }

    // Method to start burning effect
    public void StartBurning(float duration, int damageOverTime)
    {
        // Assuming no additional effects for burning
        burningEffectData.StartEffect(enemy, duration, damageOverTime);
    }

    // Method to start poisoning effect
    public void StartPoisoning(float duration, int damageOverTime, int armorReduction)
    {
        poisonEffectData.StartEffect(enemy, duration, damageOverTime, armorReduction, 0, 0,
            apply: (e) => { e.ApplyPoison(armorReduction); },
            revert: (e) => { e.RevertPoison(); });
    }

    // Method to start ice effect (slowing)
    public void StartSlowing(float duration, int damageOverTime, float movementSpeedReduction)
    {
        iceEffectData.StartEffect(enemy, duration, damageOverTime, 0, movementSpeedReduction, 0,
            apply: (e) => { e.ApplySlowing(movementSpeedReduction); },
            revert: (e) => { e.RevertSlowing(); });
    }

    // Method to start lightning effect (reducing HP regen)
    public void StartIncreasingDamageReceived(float duration, int damageOverTime, float damageReceivedIncrease)
    {
        lightningEffectData.StartEffect(enemy, duration, damageOverTime, 0, 0, damageReceivedIncrease,
            apply: (e) => { e.ApplyDamageReceivedIncreasing(damageReceivedIncrease); },
            revert: (e) => { e.RevertDamageReceivedIncreasing(); });
    }

    // Method to clean up all effects
    public void CleanupEffects()
    {
        // Stop effects directly
        burningEffectData.StopEffect(enemy);
        poisonEffectData.StopEffect(enemy);
        iceEffectData.StopEffect(enemy);
        lightningEffectData.StopEffect(enemy);
        // ... clean up other effects
    }

}