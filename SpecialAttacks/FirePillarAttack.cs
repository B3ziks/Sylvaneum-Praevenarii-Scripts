using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Fire Pillar")]
public class FirePillarAttack : SpecialAttack
{
    [SerializeField] public PoolObjectData circleData;
    [SerializeField] public PoolObjectData flameObjectData;
    public float pillarHeight = 3f;
    public float pillarWidth = 3f;
    public float pillarDelay = 2f;
    public float circleDuration = 5f;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player Transform is not set!");
            return;
        }

        // Create the circle instance
        GameObject circleInstance = poolManager.GetObject(circleData);
        Vector3 circlePosition = playerTransform.position;
        circlePosition.z = 1f; // Set the circle's z-position to 1
        circleInstance.transform.position = circlePosition;
        circleInstance.transform.rotation = Quaternion.identity;

        executor.GetMonoBehaviour().StartCoroutine(CreatePillarAfterDelay(playerTransform, poolManager, executor, circleInstance));
    }

    private IEnumerator CreatePillarAfterDelay(Transform playerTransform, PoolManager poolManager, ISpecialAttackExecutor executor, GameObject circleInstance)
    {
        yield return new WaitForSeconds(pillarDelay);

        int gridCountX = Mathf.CeilToInt(pillarWidth);
        int gridCountY = Mathf.CeilToInt(pillarHeight);
        float xOffset = pillarWidth / gridCountX;
        float yOffset = pillarHeight / gridCountY;

        for (int x = 0; x < gridCountX; x++)
        {
            for (int y = 0; y < gridCountY; y++)
            {
                float xPos = circleInstance.transform.position.x + (x * xOffset) - (pillarWidth * 0.5f);
                float yPos = circleInstance.transform.position.y + (y * yOffset) - (pillarHeight * 0.5f);
                Vector3 position = new Vector3(xPos, yPos, 0f); // Ensure the flames have z=0
                SpawnFlame(position, poolManager, executor);
            }
        }

        yield return new WaitForSeconds(circleDuration);
        circleInstance.GetComponent<PoolMember>()?.ReturnToPool();
    }

    private bool IsPositionOccupied(Vector3 position, float threshold = 0.5f)
    {
        FlameOnCharacter[] flames = GameObject.FindObjectsOfType<FlameOnCharacter>();

        foreach (FlameOnCharacter flame in flames)
        {
            if (Vector3.Distance(flame.transform.position, position) < threshold)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnFlame(Vector3 position, PoolManager poolManager, ISpecialAttackExecutor executor)
    {
        if (!IsPositionOccupied(position))
        {
            GameObject flame = poolManager.GetObject(flameObjectData);
            flame.transform.position = position;
            flame.transform.rotation = Quaternion.identity;

            FlameOnCharacter flameScript = flame.GetComponent<FlameOnCharacter>();
            if (flameScript != null)
            {
                // Optionally perform flame-specific setup here
            }
            else
            {
                UnityEngine.Debug.LogError("Flame script not found on flame object!");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Attempted to spawn a flame on top of another flame.");
        }
    }
}