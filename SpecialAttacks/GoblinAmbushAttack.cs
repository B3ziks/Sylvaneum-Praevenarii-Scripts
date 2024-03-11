using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Goblin Ambush Attack")]
public class GoblinAmbushAttack : SpecialAttack
{
    public PoolObjectData daggerObjectData;
    public float daggerSpeed;
    public float attackDuration = 1.0f; // Duration of attack in seconds

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            return;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        Camera mainCamera = Camera.main;
        Vector3 topCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
        Vector3 bottomCorner = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));

        // Start coroutine to handle the attack sequence
        executor.GetMonoBehaviour().StartCoroutine(AttackSequence(poolManager, playerTransform.position, topCorner, bottomCorner));
    }

    private IEnumerator AttackSequence(PoolManager poolManager, Vector3 playerPosition, Vector3 topCorner, Vector3 bottomCorner)
    {
        float timer = 0f;
        while (timer < attackDuration)
        {
            SpawnAndLaunchDagger(poolManager, topCorner, playerPosition);
            SpawnAndLaunchDagger(poolManager, bottomCorner, playerPosition);

            timer += 0.5f; // Increment timer by the wait time
            yield return new WaitForSeconds(0.5f); // Wait between spawns, adjust as needed
        }
    }

    private void SpawnAndLaunchDagger(PoolManager poolManager, Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject dagger = poolManager.GetObject(daggerObjectData);
        if (dagger != null)
        {
            dagger.transform.position = startPosition;
            dagger.transform.rotation = Quaternion.LookRotation(Vector3.forward, targetPosition - startPosition);

            Rigidbody2D daggerRigidbody = dagger.GetComponent<Rigidbody2D>();
            if (daggerRigidbody != null)
            {
                Vector2 direction = (targetPosition - startPosition).normalized;
                daggerRigidbody.velocity = direction * daggerSpeed;
            }

            dagger.SetActive(true);
        }
    }
}