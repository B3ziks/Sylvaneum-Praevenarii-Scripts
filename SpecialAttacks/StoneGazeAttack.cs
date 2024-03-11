using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/StoneGazeAttack")]
public class StoneGazeAttack : SpecialAttack
{
    public float stunDuration = 2.0f; // Duration of the stun effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        StoneGaze stoneGaze = player.GetComponentInChildren<StoneGaze>();
        if (stoneGaze != null)
        {
            stoneGaze.ActivateEffect(stunDuration);
        }
        else
        {
            Debug.LogError("StoneGaze component not found on the player!");
        }
    }
}