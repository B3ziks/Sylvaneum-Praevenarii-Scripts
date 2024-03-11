using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Giant Web Attack")]
public class GiantWebAttack : SpecialAttack
{
    [SerializeField] private PoolObjectData giantWebData;
    public float webDuration = 5f;  // Duration for how long the web stays

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not set!");
            return;
        }

        executor.GetMonoBehaviour().StartCoroutine(SpawnWeb(poolManager, playerTransform.position));
    }

    private IEnumerator SpawnWeb(PoolManager poolManager, Vector3 targetPosition)
    {
        GameObject web = poolManager.GetObject(giantWebData);
        if (web != null)
        {
            web.transform.position = targetPosition;
            web.SetActive(true);
            yield return new WaitForSeconds(webDuration);
            web.GetComponent<PoolMember>()?.ReturnToPool(); // Deactivate the web after duration
        }
    }
}