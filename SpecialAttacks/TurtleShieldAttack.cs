using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/TurtleShieldAttack")]
public class TurtleShieldAttack : SpecialAttack
{
    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        EnemyTurtle enemyTurtle = executor.GetMonoBehaviour().GetComponent<EnemyTurtle>();
        if (enemyTurtle != null)
        {
            enemyTurtle.ActivateShield(); // Activate the shield on the enemy turtle
        }
        else
        {
            Debug.LogError("EnemyTurtle component not found on the executor!");
        }
    }
}