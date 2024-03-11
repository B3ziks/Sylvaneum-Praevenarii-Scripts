using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPlantsTotem : MonoBehaviour
{
    public PoolObjectData plantPoolData;  // Pool object for the exploding plant prefab
    public float spawnRadius = 5f;  // Radius around the totem to spawn plants
    public float plantTTL = 3f;  // Time-to-live for each plant
    public float damageRadius = 2f;  // Damage radius of the exploding plant
    public WeaponBase weaponBaseReference;
    private PoolManager poolManager;
    private float spawnTimer;

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        spawnTimer = weaponBaseReference.weaponData.stats.timeToAttack;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnPlant();
            spawnTimer = weaponBaseReference.weaponData.stats.timeToAttack;
        }
    }

    private void SpawnPlant()
    {
        Vector2 spawnPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * spawnRadius;
        GameObject plant = poolManager.GetObject(plantPoolData);
        plant.transform.position = spawnPosition;
        plant.SetActive(true);

        // Initialize the plant with TTL and damage
        ExplodingPlantPlayer plantScript = plant.GetComponent<ExplodingPlantPlayer>();
        if (plantScript != null)
        {
            plantScript.Initialize(plantTTL, damageRadius, weaponBaseReference.GetDamage());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
