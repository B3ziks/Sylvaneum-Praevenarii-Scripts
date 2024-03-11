using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Poison Cloud")]
public class PoisonCloudAttack : SpecialAttack
{
    public PoolObjectData poisonObjectData;
    public float followSpeed = 2f;  // Speed at which the cloud follows the player.

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

        GameObject poisonCloud = poolManager.GetObject(poisonObjectData);

        if (poisonCloud == null)
        {
            UnityEngine.Debug.LogError("Poison object not available in pool!");
            return;
        }

        poisonCloud.transform.position = executor.transform.position;
        poisonCloud.transform.rotation = Quaternion.identity;
        poisonCloud.SetActive(true);

        PoisonCloudBehaviour cloudScript = poisonCloud.GetComponent<PoisonCloudBehaviour>();
        if (cloudScript == null)
        {
            cloudScript = poisonCloud.AddComponent<PoisonCloudBehaviour>();
        }
        cloudScript.Initialize(followSpeed, playerTransform);
    }
}

public class PoisonCloudBehaviour : MonoBehaviour
{
    private float followSpeed;
    private Transform target;

    public void Initialize(float speed, Transform playerTransform)
    {
        this.followSpeed = speed;
        this.target = playerTransform;
    }

    void Update()
    {
        if (target != null)
        {
            // Move the cloud towards the player
            transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);
        }
    }
}