using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/CoinStealAttack")]
public class CoinStealAttack : SpecialAttack
{
    public int coinsToSteal = 5; // The number of coins to steal

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Assuming the player has a Coins component attached
        Coins playerCoins = executor.GetPlayerTransform().GetComponent<Coins>();
        if (playerCoins == null)
        {
            Debug.LogError("Coins component not found on the player!");
            return;
        }

        // Steal coins and handle the case where the player doesn't have enough
        int actualCoinsToSteal = Mathf.Min(coinsToSteal, playerCoins.StageCoins);
        playerCoins.Remove(actualCoinsToSteal);

        // Log or display a message that coins were stolen
        Debug.Log($"Stole {actualCoinsToSteal} coins from the player.");
    }
}