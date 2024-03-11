using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Summon Reinforcements")]
public class SummonReinforcementsAttack : SpecialAttack
{
    public EnemyData reinforcementData;
    public int numberOfReinforcements;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        EnemiesManager enemiesManager = executor.GetEnemiesManager();
        if (enemiesManager == null)
        {
            UnityEngine.Debug.LogError("EnemiesManager is not set!");
            return;
        }

        for (int i = 0; i < numberOfReinforcements; i++)
        {
            Vector3 positionToSpawn = executor.transform.position;
            positionToSpawn += UtilityTools.GenerateRandomPositionSquarePattern(new Vector2(5f, 5f));

            enemiesManager.SpawnReinforcement(positionToSpawn, reinforcementData);
        }
    }
}