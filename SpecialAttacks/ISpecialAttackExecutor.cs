using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecialAttackExecutor
{
    // This method provides the attacker's position in world space.
    Transform transform { get; }

    // To get the player's position or perform other player-related activities.
    Transform GetPlayerTransform();

    // To access the pooling system.
    PoolManager GetPoolManager();
    MonoBehaviour GetMonoBehaviour(); // This method will allow access to MonoBehaviour methods
    EnemyData GetEnemyData();  // Assuming EnemyData is the type of enemyData.
    EnemiesManager GetEnemiesManager();
}