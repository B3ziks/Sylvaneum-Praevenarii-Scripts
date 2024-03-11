using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BuffAura : Aura
{
    public BuffAuras auraType; // Assign this in the inspector for each different aura prefab

    // Store the original stats when buffs are applied
    private Dictionary<Enemy, Dictionary<BuffAuras, float>> originalStats = new Dictionary<Enemy, Dictionary<BuffAuras, float>>();

    private void OnTriggerStay2D(Collider2D collision)
    {
       // UnityEngine.Debug.Log("Stay Triggered");

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !originalStats.ContainsKey(enemy))
        {
            StoreOriginalStat(enemy);
            ApplyAuraEffect(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //UnityEngine.Debug.Log("Exit Triggered");

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            RemoveAuraEffect(collision);
        }
    }

    private void StoreOriginalStat(Enemy enemy)
    {
        if (!originalStats.ContainsKey(enemy))
        {
            originalStats[enemy] = new Dictionary<BuffAuras, float>();
        }

        switch (auraType)
        {
            case BuffAuras.DamageBuff:
                originalStats[enemy][BuffAuras.DamageBuff] = enemy.stats.damage;
                break;
            case BuffAuras.AttackSpeedBuff:
                originalStats[enemy][BuffAuras.AttackSpeedBuff] = enemy.stats.timeToAttack;
                break;
                // Handle other buffs similarly...
        }

       // UnityEngine.Debug.Log($"Stored original {auraType} for enemy: {originalStats[enemy][auraType]}");
    }

    protected override void ApplyAuraEffect(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.ApplyBuff(auraType);
      //  UnityEngine.Debug.Log($"Enemy's {auraType} after buff: {GetValueAfterBuff(enemy)}");
    }

    private float GetValueAfterBuff(Enemy enemy)
    {
        switch (auraType)
        {
            case BuffAuras.DamageBuff:
                return enemy.stats.damage;
            case BuffAuras.AttackSpeedBuff:
                return enemy.stats.timeToAttack;
                // Handle other buffs similarly...
        }
        return 0f;
    }

    protected override void RemoveAuraEffect(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        if (originalStats.ContainsKey(enemy) && originalStats[enemy].ContainsKey(auraType))
        {
            enemy.RemoveBuff(auraType, originalStats[enemy][auraType]);
            originalStats[enemy].Remove(auraType);
            if (originalStats[enemy].Count == 0)
            {
                originalStats.Remove(enemy);
            }
        }
    }
}

public enum BuffAuras
{
    DamageBuff,
    AttackSpeedBuff,
    ArmorBuff,
    HPRegenBuff
}