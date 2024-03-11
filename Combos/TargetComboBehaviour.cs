using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetComboBehaviour : MonoBehaviour
{

    private Transform currentTarget;
    public static TargetComboBehaviour Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        // Find all enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            // If the enemy matches the targetEnemyData and is elite or boss
            if (enemy.enemyData ==  (enemy.enemyData.isElite || enemy.enemyData.isBoss))
            {
                AttachToEnemy(enemy.transform);
                break;
            }
        }
    }
    public Vector2 GetCurrentTargetPosition()
    {
        return currentTarget != null ? (Vector2)currentTarget.position : Vector2.zero;
    }

    private void AttachToEnemy(Transform enemyTransform)
    {
        // Set the current target to the enemy's transform
        currentTarget = enemyTransform;
        // Position the targeting prefab above the enemy (customize as needed)
        transform.position = enemyTransform.position + Vector3.up * CalculateEffectYOffset(enemyTransform);
        // You can also set the transform.parent if you want the target to follow the enemy's movement
        transform.SetParent(enemyTransform);
    }

    private void OnDisable()
    {
        // Cleanup code when the object is disabled
        currentTarget = null;
        if (GameManager.instance != null)
        {
            GameManager.instance.StartCoroutine(DetachFromEnemyNextFrame());
        }
    }

    private IEnumerator DetachFromEnemyNextFrame()
    {
        yield return new WaitForEndOfFrame();

        // Check if this script instance is still valid
        if (this != null && gameObject != null && gameObject.activeInHierarchy && transform.parent != null)
        {
            transform.SetParent(null);
        }

        currentTarget = null;
    }


    private float CalculateEffectYOffset(Transform enemyTransform)
    {
        // Calculate how far above the enemy the effect should appear
        var spriteRenderer = enemyTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Adjust the value to set the effect above the sprite
            return spriteRenderer.bounds.extents.y + 0.5f;
        }
        return 1f; // Default offset if no SpriteRenderer is found
    }
}