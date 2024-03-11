using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Fist Of God Attack")]
public class FistOfGodAttack : SpecialAttack
{
    public PoolObjectData fistOfGodPrefab; // Assign this in the inspector
    public float attackDelay = 1.0f; // Delay before the fist spawns above the player
    private Transform playerTransform;
    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        playerTransform = FindPlayerTransform();
        if (playerTransform != null)
        {
            Vector3 strikePosition = playerTransform.position + Vector3.up * 3.0f; // 3 units above the player
            executor.GetMonoBehaviour().StartCoroutine(SpawnFistAfterDelay(poolManager, strikePosition));
        }
    }

    private IEnumerator SpawnFistAfterDelay(PoolManager poolManager, Vector3 strikePosition)
    {
        yield return new WaitForSeconds(attackDelay);

        GameObject fistOfGodGO = poolManager.GetObject(fistOfGodPrefab);
        if (fistOfGodGO != null)
        {
            FistOfGod fistOfGod = fistOfGodGO.GetComponent<FistOfGod>();
            if (fistOfGod != null)
            {
                fistOfGodGO.transform.position = strikePosition; // Start above the target
                fistOfGodGO.SetActive(true);
                fistOfGod.Initialize(playerTransform.position); // Pass the player's position as the target
            }
            else
            {
                Debug.LogError("FistOfGod component not found on the Fist Of God object!");
            }
        }
        else
        {
            Debug.LogError("Fist Of God object not available in pool!");
        }
    }

    private Transform FindPlayerTransform()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        return playerGameObject != null ? playerGameObject.transform : null;
    }
}