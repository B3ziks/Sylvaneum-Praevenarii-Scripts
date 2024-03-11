using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePoolBehavior : MonoBehaviour
{
    private int damage;
    private float duration = 5f; // Duration the pool exists

    void Start()
    {
        // Optionally, destroy the pool after a set duration
        Destroy(gameObject, duration);
    }

    public void ConfigurePool(int damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            Character playerCharacter = other.GetComponent<Character>();
            if (playerCharacter != null)
            {
                playerCharacter.TakeDamage(damage);
            }
        }
    }
}