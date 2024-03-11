using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingSpiritsWeapon : WeaponBase
{
    public PoolObjectData spiritObjectData;
    public float radius = 2f; // Distance from player
    private Transform playerTransform;
    private List<GameObject> activeSpirits = new List<GameObject>();

    private void Start()
    {
        playerTransform = transform;
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not found!");
            return;
        }

        // Initialize the list of active spirits
        activeSpirits = new List<GameObject>(weaponData.stats.maxNumberOfSummonedInstances);

        // Start the coroutine to manage spirit activation
        StartCoroutine(ManageSpiritActivation());
    }

    private IEnumerator ManageSpiritActivation()
    {
        while (true)
        {
            // Remove any null references from the list (spirits that have been deactivated)
            activeSpirits.RemoveAll(item => item == null);

            // If we haven't reached the max count, try to activate a new spirit
            while (activeSpirits.Count < weaponData.stats.maxNumberOfSummonedInstances)
            {
                ActivateSpirits();
                // Wait for the timeToAttack interval before trying to activate another spirit
                yield return new WaitForSeconds(weaponData.baseStats.timeToAttack);
            }

            yield return null; // Wait for the next frame to reassess
        }
    }

    private void ActivateSpirits()
    {
        // Determine the angle increment based on the number of active spirits to maintain spacing
        float angleIncrement = 2 * Mathf.PI / weaponData.stats.maxNumberOfSummonedInstances;

        // Calculate the angle for the new spirit
        float currentAngle = activeSpirits.Count * angleIncrement;
        Vector3 positionOffset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * radius;
        Vector3 spiritPosition = playerTransform.position + positionOffset;

        GameObject spirit = SpawnProjectile(spiritObjectData, spiritPosition);
        if (spirit != null)
        {
            CirclingSpiritController spiritScript = spirit.GetComponent<CirclingSpiritController>();
            if (spiritScript != null)
            {
                spiritScript.Initialize(playerTransform, radius, poolManager, currentAngle);
                spiritScript.SetDamage(weaponData.stats.damage); // Set the damage
                spiritScript.OnDeactivate += RemoveSpiritFromActiveList; // Subscribe to the OnDeactivate event
                activeSpirits.Add(spirit); // Add to the list of active spirits
            }
            else
            {
                UnityEngine.Debug.LogError("CirclingSpiritController script not found on spirit object!");
            }
        }
    }

    private void RemoveSpiritFromActiveList(GameObject spirit)
    {
        // Remove the spirit from the active list
        activeSpirits.Remove(spirit);
    }

    public override void Attack()
    {
        // This method is kept empty because the coroutine handles the spawning of spirits
    }
}