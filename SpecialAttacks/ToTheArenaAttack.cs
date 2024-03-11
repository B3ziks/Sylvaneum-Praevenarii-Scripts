using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/To The Arena")]
public class ToTheArenaAttack : SpecialAttack
{
    public PoolObjectData pillarObjectData;
    public float radius = 10f;
    public int numberOfPillars;
    public float obstacleCheckRadius = 3f;  // Adjust this value based on your game's scale

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        // Assuming the player has the tag "Player". Fetch the player's transform.
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Calculate the midpoint between the player and the boss.
        Vector3 midPoint = (playerTransform.position + executor.transform.position) / 2;

        float angleStep = 360f / numberOfPillars;
        for (int i = 0; i < numberOfPillars; i++)
        {
            float angleInRad = i * angleStep * Mathf.Deg2Rad;
            // Inside the for loop:
            Vector3 position = new Vector3(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad), 0) * radius + midPoint;

            // Adjust the Z position based on the Y position
            position.z = position.y * 0.01f;

            // Check for obstacles around the position where the pillar will be placed
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, obstacleCheckRadius);
            foreach (Collider2D hitCollider in hitColliders)
            {
                // You might want to tag your obstacles and check for that tag here for more precision
                if (hitCollider.CompareTag("Obstacle"))
                {
                    Destroy(hitCollider.gameObject);
                }
            }

            GameObject pillar = poolManager.GetObject(pillarObjectData);
            pillar.transform.position = position;
            pillar.transform.rotation = Quaternion.identity;
        }
    }

}