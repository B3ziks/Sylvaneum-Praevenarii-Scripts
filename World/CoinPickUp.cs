using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour, IPickUpObject
{
    [SerializeField] int count;

    public void OnPickUp(Character character)
    {
        if (character == null || character.coins == null)
        {
            UnityEngine.Debug.LogError("Character or Character.coins is null!");
            return;
        }

        // Calculate the total coins to add, considering the gold gain upgrade
        int totalCoins = count + Mathf.FloorToInt(count * character.goldGain);
        character.coins.Add(totalCoins);
    }

}
