using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MagicTargetingWeapon : WeaponBase
{
    private static Vector2 targetingPosition;
    private bool isTargetIndicatorActive = false; // Add this at the class level
    private float timeSinceLastAttack = 0f; // Timer to track time since last attack

    protected override void Awake()
    {
        base.Awake();
        targetingComboCooldown = weaponData.stats.timeToAttack;
        // Additional initialization if needed
    }
    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= weaponData.stats.timeToAttack)
        {
            if (IsEliteOrBossEnemyPresent())
            {
                StartCoroutine(ActivateMagicTargetingCombo());
            }
            timeSinceLastAttack = 0f;
        }
    }
    private bool IsEliteOrBossEnemyPresent()
    {
        var enemies = FindObjectsOfType<Enemy>(); // Assuming you have an Enemy class
        return enemies.Any(e => e.enemyData.isElite || e.enemyData.isBoss);
    }
    public override void Attack()
    {
        // Implement attack logic specific to MagicTargetingWeapon
        if (IsEliteOrBossEnemyPresent() && !isInTargetingComboCooldown)
        {
            StartCoroutine(ActivateMagicTargetingCombo());
        }
    }

    //targeting system combo
    private IEnumerator ActivateMagicTargetingCombo()
    {
        // Check if an elite or boss enemy is present
        Enemy targetEnemy = FindNearestEliteOrBossEnemy();
        if (targetEnemy == null)
        {
            yield break; // Exit if no target enemy is found
        }

        // Activate the targeting system
        isTargetIndicatorActive = true;
        targetingPosition = targetEnemy.transform.position;
        if (targetingIndicatorInstance == null)
        {
            targetingIndicatorInstance = Instantiate(targetingIndicatorPrefab, targetingPosition, Quaternion.identity);
        }
        else
        {
            targetingIndicatorInstance.transform.position = targetingPosition;
            targetingIndicatorInstance.SetActive(true);
        }

        // Wait for the targeting indicator to be active before proceeding
        yield return new WaitUntil(() => targetingIndicatorInstance.activeInHierarchy);

        // Make projectiles home in on the targeting indicator
        List<Projectile> projectiles = Projectile.GetAllProjectiles();
        foreach (var projectile in projectiles)
        {
            if (projectile != null)
            {
                projectile.SetTargetingPosition(targetingPosition);
                projectile.EnableTargeting();
            }
        }

        // Wait for the combo duration
        yield return new WaitForSeconds(targetingComboDuration);

        // Deactivate the targeting system and reset projectiles
        if (targetingIndicatorInstance != null)
        {
            targetingIndicatorInstance.SetActive(false);
        }
        isTargetIndicatorActive = false;
        foreach (var projectile in projectiles.Where(p => p != null))
        {
            projectile.DisableTargeting();
            projectile.ResetTargetingPosition();
        }

        // Wait for the remaining cooldown before the next attack can be initiated
        float remainingCooldown = weaponData.stats.timeToAttack - targetingComboDuration;
        yield return new WaitForSeconds(remainingCooldown);
    }
    private Enemy FindNearestEliteOrBossEnemy()
    {
        var enemies = FindObjectsOfType<Enemy>();
        var eliteOrBossEnemies = enemies.Where(e => e.enemyData.isElite || e.enemyData.isBoss); // Use enemyData here

        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (var enemy in eliteOrBossEnemies)
        {
            float distance = Vector2.Distance(playerPosition, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
    //

    // ... Other methods and logic related to magic targeting ...
}