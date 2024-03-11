using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Poison Pool")]
public class PoisonPoolAttack : SpecialAttack
{
    public PoolObjectData poisonPoolObjectData;
    public float poolDuration = 5f;
    public int damagePerSecond = 2;
    public float poolStartDelay = 2f; // New field for the delay.

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player not found!");
            return;
        }

        // Store player's current position
        Vector3 playerPosition = playerTransform.position;

        executor.GetMonoBehaviour().StartCoroutine(StartPoisonPoolAfterDelay(playerPosition, poolManager));
    }

    private IEnumerator StartPoisonPoolAfterDelay(Vector3 position, PoolManager poolManager)
    {
        yield return new WaitForSeconds(poolStartDelay);

        GameObject poisonPool = poolManager.GetObject(poisonPoolObjectData);

        if (poisonPool == null)
        {
            UnityEngine.Debug.LogError("Poison object not available in pool!");
            yield break;  // Exit the coroutine
        }

        poisonPool.transform.position = new Vector3(position.x, position.y, 1f); // Set to previously recorded player position and adjust the z value
        poisonPool.transform.rotation = Quaternion.identity;
        poisonPool.SetActive(true);

        PoisonPoolBehaviour poolScript = poisonPool.AddComponent<PoisonPoolBehaviour>();
        poolScript.Initialize(poolDuration, damagePerSecond);
    }
}

public class PoisonPoolBehaviour : MonoBehaviour
{
    private float poolDuration;
    private int damagePerSecond;
    private bool playerInside = false;
    private bool poolIsActive = false; // New field to check if pool has started damaging.

    public void Initialize(float duration, int dps)
    {
        this.poolDuration = duration;
        this.damagePerSecond = dps;
        StartCoroutine(ActivatePoolAfterDelay());
        StartCoroutine(PoolDuration());
    }

    private IEnumerator ActivatePoolAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        poolIsActive = true;
        while (playerInside && poolIsActive)
        {
            Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            if (player)
            {
                player.TakeDamage(damagePerSecond);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator PoolDuration()
    {
        yield return new WaitForSeconds(poolDuration);
        Destroy(this.gameObject);  // Destroy the pool after the specified duration
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}