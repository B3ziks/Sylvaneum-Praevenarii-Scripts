using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/SpiderCocoonAttack")]
public class SpiderCocoonAttack : SpecialAttack
{
    public float stunDuration = 2.0f; // Duration of the stun effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        // Activate the cocoon GameObject which is a child of the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform cocoonTransform = player.transform.Find("Cocoon");
        if (cocoonTransform != null)
        {
            cocoonTransform.gameObject.SetActive(true);
            SpiderCocoon spiderCocoon = cocoonTransform.GetComponent<SpiderCocoon>();
            if (spiderCocoon != null)
            {
                spiderCocoon.ActivateStun(stunDuration);
            }
        }
    }
}