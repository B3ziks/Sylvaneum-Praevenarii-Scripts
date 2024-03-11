using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Snowballs Attack")]
public class SnowballsAttack : SpecialAttack
{
    public PoolObjectData snowballObjectData;
    public float snowballSpeed;
    public float attackDuration = 3.0f; // Duration of attack in seconds
    public float timeBetweenSpawns = 0.5f; // Time between each snowball spawn

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            return;
        }

        // Start coroutine to handle the attack sequence
        executor.GetMonoBehaviour().StartCoroutine(AttackSequence(poolManager));
    }

    private IEnumerator AttackSequence(PoolManager poolManager)
    {
        Camera mainCamera = Camera.main;
        float timer = 0f;

        while (timer < attackDuration)
        {
            // Calculate the spawn position off-screen to the left and right of the camera
            Vector2 leftSpawnPosition = mainCamera.ViewportToWorldPoint(new Vector2(0, 0.5f));
            Vector2 rightSpawnPosition = mainCamera.ViewportToWorldPoint(new Vector2(1, 0.5f));

            // Add offset to ensure the snowball spawns off-screen
            leftSpawnPosition.x -= 1f;
            rightSpawnPosition.x += 1f;

            // Spawn snowballs and set their velocity to move towards the opposite side
            SpawnAndLaunchSnowball(poolManager, leftSpawnPosition, Vector2.right);
            SpawnAndLaunchSnowball(poolManager, rightSpawnPosition, Vector2.left);

            // Increment timer and wait for the next spawn
            timer += timeBetweenSpawns;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void SpawnAndLaunchSnowball(PoolManager poolManager, Vector2 startPosition, Vector2 direction)
    {
        GameObject snowball = poolManager.GetObject(snowballObjectData);
        if (snowball != null)
        {
            snowball.transform.position = startPosition;
            Rigidbody2D snowballRigidbody = snowball.GetComponent<Rigidbody2D>();
            if (snowballRigidbody != null)
            {
                snowballRigidbody.velocity = direction * snowballSpeed;
            }

            snowball.SetActive(true);
        }
    }
}