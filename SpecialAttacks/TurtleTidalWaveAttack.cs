using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/TurtleTidalWaveAttack")]
public class TurtleTidalWaveAttack : SpecialAttack
{
    public PoolObjectData tidalWaveObjectData;
    public float tidalWaveSpeed;
    public float attackDuration = 3.0f; // Duration of attack in seconds
    public float timeBetweenSpawns = 0.5f; // Time between each tidal wave spawn

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
            // Calculate the spawn position off-screen at the top and bottom of the camera
            Vector2 topSpawnPosition = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 1));
            Vector2 bottomSpawnPosition = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 0));

            // Add offset to ensure the tidal wave spawns off-screen
            topSpawnPosition.y += 1f;
            bottomSpawnPosition.y -= 1f;

            // Spawn tidal waves and set their velocity to move towards the opposite side
            SpawnAndLaunchTidalWave(poolManager, topSpawnPosition, Vector2.down);
            SpawnAndLaunchTidalWave(poolManager, bottomSpawnPosition, Vector2.up);

            // Increment timer and wait for the next spawn
            timer += timeBetweenSpawns;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void SpawnAndLaunchTidalWave(PoolManager poolManager, Vector2 startPosition, Vector2 direction)
    {
        GameObject tidalWave = poolManager.GetObject(tidalWaveObjectData);
        if (tidalWave != null)
        {
            tidalWave.transform.position = startPosition;
            Rigidbody2D tidalWaveRigidbody = tidalWave.GetComponent<Rigidbody2D>();
            if (tidalWaveRigidbody != null)
            {
                tidalWaveRigidbody.velocity = direction * tidalWaveSpeed;
            }

            tidalWave.SetActive(true);
        }
    }
}