using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TidalWaveBehaviour : MonoBehaviour
{
    public int damage = 50; // Damage value

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Character playerCharacter = other.GetComponent<Character>();
            if (playerCharacter != null)
            {
                playerCharacter.TakeDamage(damage);
                // Optionally, apply additional effects if needed
            }

        }
    }
}
