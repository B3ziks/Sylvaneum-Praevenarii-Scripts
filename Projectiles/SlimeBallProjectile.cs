using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlimeBallProjectile : EnemyProjectile
{
    public PoolObjectData slimePoolData; // Reference to slime pool data
    private PoolManager poolManager;     // Reference to the pool manager
    private Rigidbody2D slimeRb;              // Reference to the Rigidbody2D component
    private bool isDescending = false;   // Flag to track if the projectile is descending

    // Added time out for deactivation
    private float timeOut = 10f;         // Time after which the projectile will be deactivated regardless of position
    private float timeElapsed = 0f;      // Time since the projectile was launched

    protected void OnEnable()
    {
        base.OnEnable();
        poolManager = FindObjectOfType<PoolManager>(); // Find the PoolManager instance
        slimeRb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        timeElapsed = 0f; // Reset the time elapsed
    }

    private void Update()
    {
        // Update the time elapsed
        timeElapsed += Time.deltaTime;

        // If the projectile is going up, set isDescending to false
        if (slimeRb.velocity.y > 0)
        {
            isDescending = false;
        }
        // If the projectile starts going down, set isDescending to true
        else if (slimeRb.velocity.y <= 0)
        {
            isDescending = true;
        }

        // Check if the projectile is descending and is below a certain threshold above the player's y position
        if (isDescending && transform.position.y <= playerTransform.position.y + 0.5f)
        {
            CreateSlimePoolAt(transform.position); // Create the slime pool
            DeactivateProjectile(); // Deactivate the slime ball
            isDescending = false; // Reset the flag
        }

        // If the time elapsed exceeds the timeout, deactivate the projectile
        if (timeElapsed >= timeOut)
        {
            DeactivateProjectile();
        }
    }

    private void CreateSlimePoolAt(Vector3 position)
    {
        if (slimePoolData != null && poolManager != null)
        {
            GameObject slimePool = poolManager.GetObject(slimePoolData);
            if (slimePool != null)
            {
                slimePool.transform.position = position;
                slimePool.SetActive(true);

                SlimePoolBehavior poolBehavior = slimePool.GetComponent<SlimePoolBehavior>();
                if (poolBehavior != null)
                {
                    poolBehavior.ConfigurePool(damage);
                }
            }
        }
        else
        {
            Debug.LogError("SlimePoolData not set or PoolManager not found!");
        }
    }

    protected override void DeactivateProjectile()
    {
        // Ensure the projectile stops all physics interactions
        if (slimeRb != null)
        {
            slimeRb.velocity = Vector2.zero;
            slimeRb.isKinematic = true;
            slimeRb.simulated = false; // This stops all physics interactions
        }

        gameObject.SetActive(false); // Deactivate the slime ball
    }
}