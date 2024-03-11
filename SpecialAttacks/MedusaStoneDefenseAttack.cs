using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/MedusaStoneDefenseAttack")]
public class MedusaStoneDefenseAttack : SpecialAttack
{
    public PoolObjectData stoneFigurePoolData;
    public float defenseDuration;
    public int defenseStacks = 3;
    public float circleRadius = 3f;
    public float rotationSpeed = 1f;
    private GameObject[] stoneFigures;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        EnemyMedusa enemyMedusa = executor.GetMonoBehaviour().GetComponent<EnemyMedusa>();
        if (enemyMedusa == null)
        {
            Debug.LogError("EnemyMedusa component not found on the executor!");
            return;
        }

        // Activate defense with the specified number of stacks
        enemyMedusa.ActivateDefense(defenseStacks);

        // Initialize stone figures array based on the number of defense stacks
        stoneFigures = new GameObject[defenseStacks];

        // Start a coroutine to manage the stone figures
        executor.GetMonoBehaviour().StartCoroutine(HandleStoneFigures(executor, poolManager, defenseStacks));
    }

    private IEnumerator HandleStoneFigures(ISpecialAttackExecutor executor, PoolManager poolManager, int initialStacks)
    {
        // Create and position stone figures
        for (int i = 0; i < initialStacks; i++)
        {
            stoneFigures[i] = poolManager.GetObject(stoneFigurePoolData);
            if (stoneFigures[i] != null)
            {
                stoneFigures[i].transform.position = executor.transform.position;
                stoneFigures[i].SetActive(true);
                // Start coroutine to circle the figure around Medusa
                executor.GetMonoBehaviour().StartCoroutine(CircleFigure(stoneFigures[i], executor, i));
            }
        }

        // Continuously check for the current number of defense stacks
        EnemyMedusa enemyMedusa = executor.GetMonoBehaviour().GetComponent<EnemyMedusa>();
        int previousStackCount = initialStacks;

        while (enemyMedusa != null && enemyMedusa.CurrentDefenseStacks > 0)
        {
            if (previousStackCount != enemyMedusa.CurrentDefenseStacks)
            {
                // If the stack count has changed, deactivate the last figure
                DeactivateLastFigure(previousStackCount - 1);
                previousStackCount = enemyMedusa.CurrentDefenseStacks;
            }
            yield return null;
        }

        // When all stacks are lost, deactivate and return all figures to pool
        DeactivateAllFigures();
    }

    private void DeactivateLastFigure(int index)
    {
        if (index >= 0 && index < stoneFigures.Length && stoneFigures[index] != null)
        {
            stoneFigures[index].SetActive(false);
            stoneFigures[index].GetComponent<PoolMember>()?.ReturnToPool();
            stoneFigures[index] = null;
        }
    }

    private void DeactivateAllFigures()
    {
        for (int i = 0; i < stoneFigures.Length; i++)
        {
            if (stoneFigures[i] != null)
            {
                stoneFigures[i].SetActive(false);
                stoneFigures[i].GetComponent<PoolMember>()?.ReturnToPool();
                stoneFigures[i] = null;
            }
        }
    }

    private IEnumerator CircleFigure(GameObject figure, ISpecialAttackExecutor executor, int index)
    {
        float angle = index * (360f / defenseStacks);
        Vector3 initialPositionOffset = CalculatePositionOffset(index, circleRadius);

        while (figure != null && figure.activeSelf)
        {
            angle += rotationSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;

            // Calculate the offset based on the initial position offset and the current angle
            Vector3 offset = new Vector3(
                Mathf.Cos(radians) * initialPositionOffset.x - Mathf.Sin(radians) * initialPositionOffset.y,
                Mathf.Sin(radians) * initialPositionOffset.x + Mathf.Cos(radians) * initialPositionOffset.y,
                0
            );

            figure.transform.position = executor.transform.position + offset;
            yield return null;
        }
    }

    private Vector3 CalculatePositionOffset(int index, float radius)
    {
        float angle = index * (360f / defenseStacks) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
    }
}