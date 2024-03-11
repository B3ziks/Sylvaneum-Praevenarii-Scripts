using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantWeb : MonoBehaviour
{
    public float slowEffect = 0.5f; // How much to slow the player, e.g., 50% speed reduction

    // Dictionary to keep track of original speeds of characters
    private Dictionary<Character, float> originalSpeeds = new Dictionary<Character, float>();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character player = collider.GetComponent<Character>();
        if (player != null && !originalSpeeds.ContainsKey(player))
        {
            // Store the original speed
            originalSpeeds[player] = player.movementSpeed;
            // Apply the slow effect
            player.movementSpeed *= slowEffect;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Character player = collider.GetComponent<Character>();
        if (player != null && originalSpeeds.ContainsKey(player))
        {
            // Reset to the original speed
            player.movementSpeed = originalSpeeds[player];
            // Remove the player from the dictionary
            originalSpeeds.Remove(player);
        }
    }
}