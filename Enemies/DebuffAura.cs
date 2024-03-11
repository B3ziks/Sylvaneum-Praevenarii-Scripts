using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAura : Aura
{
    public DebuffAuras auraType; // Assign this in the inspector for each different debuff aura prefab

    // Store the original stats when debuffs are applied
    private Dictionary<Character, Dictionary<DebuffAuras, float>> originalStats = new Dictionary<Character, Dictionary<DebuffAuras, float>>();

    private void OnTriggerStay2D(Collider2D collision)
    {
       // UnityEngine.Debug.Log("Stay Triggered");

        Character player = collision.GetComponent<Character>();
        if (player != null && !originalStats.ContainsKey(player))
        {
            StoreOriginalStat(player);
            ApplyAuraEffect(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      //  UnityEngine.Debug.Log("Exit Triggered");

        Character player = collision.GetComponent<Character>();
        if (player != null)
        {
            RemoveAuraEffect(collision);
        }
    }

    private void StoreOriginalStat(Character player)
    {
        if (!originalStats.ContainsKey(player))
        {
            originalStats[player] = new Dictionary<DebuffAuras, float>();
        }

        switch (auraType)
        {
            case DebuffAuras.MovementSlowAura:
                originalStats[player][DebuffAuras.MovementSlowAura] = player.movementSpeed;
                break;
            case DebuffAuras.HPRegenerationRateSlowAura:
                originalStats[player][DebuffAuras.HPRegenerationRateSlowAura] = player.hpRegenerationRate;
                break;
            case DebuffAuras.ReduceArmorAura:
                originalStats[player][DebuffAuras.ReduceArmorAura] = player.armor;
                break;
                // Handle other debuffs similarly...
        }

        //UnityEngine.Debug.Log($"Stored original {auraType} for player: {originalStats[player][auraType]}");
    }

    protected override void ApplyAuraEffect(Collider2D collision)
    {
        Character player = collision.GetComponent<Character>();
        if (player == null) return;

        player.ApplyDebuff(auraType);
       // UnityEngine.Debug.Log($"Player's {auraType} after debuff: {GetValueAfterDebuff(player)}");
    }

    private float GetValueAfterDebuff(Character player)
    {
        switch (auraType)
        {
            case DebuffAuras.MovementSlowAura:
                return player.movementSpeed;
            case DebuffAuras.HPRegenerationRateSlowAura:
                return player.hpRegenerationRate;
            case DebuffAuras.ReduceArmorAura:
                return player.armor;
            default:
                break;
                // Handle other debuffs similarly...
        }
        return 0f;
    }

    protected override void RemoveAuraEffect(Collider2D collision)
    {
        Character player = collision.GetComponent<Character>();
        if (player == null) return;

        if (originalStats.ContainsKey(player) && originalStats[player].ContainsKey(auraType))
        {
            player.RemoveDebuff(auraType, originalStats[player][auraType]);
            originalStats[player].Remove(auraType);
            if (originalStats[player].Count == 0)
            {
                originalStats.Remove(player);
            }
        }
    }
}

public enum DebuffAuras
{
    HPRegenerationRateSlowAura,
    MovementSlowAura,
    ReduceArmorAura
}