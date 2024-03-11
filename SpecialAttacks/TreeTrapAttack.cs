using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Tree Trap Attack")]
public class TreeTrapAttack : SpecialAttack
{
    public PoolObjectData treeObjectData; // Data for the tree obstacle
    public float radius = 5f; // Smaller radius around the player
    public int numberOfTrees = 8; // Adjust the number of trees
    public float obstacleCheckRadius = 1f; // Smaller radius for obstacle check

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

        float angleStep = 360f / numberOfTrees;
        for (int i = 0; i < numberOfTrees; i++)
        {
            float angleInRad = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad), 0) * radius + playerTransform.position;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, obstacleCheckRadius);
            bool isPositionClear = true;
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Obstacle") || hitCollider.CompareTag("Player"))
                {
                    isPositionClear = false;
                    break; // If an obstacle or player is found, skip placing a tree here
                }
            }

            if (isPositionClear)
            {
                GameObject tree = poolManager.GetObject(treeObjectData);
                if (tree != null)
                {
                    tree.transform.position = position;
                    tree.transform.rotation = Quaternion.identity;
                }
                else
                {
                    UnityEngine.Debug.LogError("Tree object not available in pool!");
                }
            }
        }
    }
}