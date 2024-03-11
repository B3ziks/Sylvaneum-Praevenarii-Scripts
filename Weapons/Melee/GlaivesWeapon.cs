using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlaivesWeapon : WeaponBase
{
    public PoolObjectData glaiveObjectData;
    public float radius; // distance from player
    public int numberOfGlaives;
    private Transform playerTransform;
    private List<GameObject> activeGlaives = new List<GameObject>();
    private float attackTimer;

    private void Start()
    {
        playerTransform = transform;
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not found!");
            return;
        }
    }

    private void Update()
    {
        // Attack timer
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackTimer = weaponData.stats.timeToAttack;
            Attack();
        }
    }

    public override void Attack()
    {
        // Check for max number of glaives before activating
        if (activeGlaives.Count < weaponData.stats.maxNumberOfSummonedInstances)
        {
            ActivateGlaives();
        }
    }

    public void ActivateGlaives()
    {
        float angleStep = 360f / numberOfGlaives;
        for (int i = 0; i < numberOfGlaives; i++)
        {
            float angleInRad = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad), 0) * radius + playerTransform.position;

            GameObject glaive = SpawnProjectile(glaiveObjectData, position);
            activeGlaives.Add(glaive); // Add to the list of active glaives

            glaive.transform.position = new Vector3(glaive.transform.position.x, glaive.transform.position.y, 0); // Set Z to 0

            GlaiveController glaiveScript = glaive.GetComponent<GlaiveController>();
            if (glaiveScript != null)
            {
                glaiveScript.Initialize(playerTransform, radius);
                glaiveScript.damage = weaponData.stats.damage;  // Set the damage
            }
            else
            {
                UnityEngine.Debug.LogError("GlaiveController script not found on glaive object!");
            }
        }
    }
}