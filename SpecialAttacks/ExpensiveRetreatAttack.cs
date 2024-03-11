using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/ExpensiveRetreatAttack")]
public class ExpensiveRetreatAttack : SpecialAttack
{
    public PoolObjectData coinPrefab; // Assign the coin prefab in the inspector
    public float recoilForce = 4000f; // Force applied for recoil
    public float coinSpeed = 10f; // Speed of the coin towards the player

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        Rigidbody2D bossRigidbody = executor.GetMonoBehaviour().GetComponent<Rigidbody2D>();
        if (bossRigidbody == null)
        {
            Debug.LogError("Rigidbody2D not found on the boss!");
            return;
        }

        // Calculate recoil direction to be the same as the boss's current facing direction
        Vector2 recoilDirection = executor.GetMonoBehaviour().transform.right.normalized;

        // Start recoil coroutine
        executor.GetMonoBehaviour().StartCoroutine(RecoilAndShootCoin(bossRigidbody, recoilDirection, executor.GetPlayerTransform().position, poolManager));
    }

    private IEnumerator RecoilAndShootCoin(Rigidbody2D bossRigidbody, Vector2 recoilDirection, Vector3 playerPosition, PoolManager poolManager)
    {
        // Apply recoil force to the boss
        bossRigidbody.AddForce(recoilDirection * recoilForce, ForceMode2D.Force);

        // Wait for a frame to allow for the force to take effect
        yield return null;

        // Shoot 3 coins in rapid succession
        for (int i = 0; i < 3; i++)
        {
            GameObject coin = poolManager.GetObject(coinPrefab);
            if (coin != null)
            {
                // Set the coin's position to the boss's current position
                coin.transform.position = bossRigidbody.position;

                // Calculate the direction towards the player
                Vector2 directionToPlayer = (playerPosition - coin.transform.position).normalized;

                // Get the coin's Rigidbody2D and set its velocity towards the player
                Rigidbody2D coinRigidbody = coin.GetComponent<Rigidbody2D>();
                if (coinRigidbody != null)
                {
                    coinRigidbody.velocity = directionToPlayer * coinSpeed;
                }
                coin.SetActive(true);
            }

            // Wait a short time before shooting the next coin
            yield return new WaitForSeconds(0.25f); // Adjust this value to control the rapidness of succession
        }

        // Optionally, reset boss velocity after recoil
        yield return new WaitForSeconds(0.1f); // Adjust the wait time as needed
        bossRigidbody.velocity = Vector2.zero;
    }
}