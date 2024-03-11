using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltProjectile : Projectile, IPoolMember
{
    private int maxRicochets; // The max number of times the projectile can ricochet.
    private int currentRicochets = 0;
    private LineRenderer lineRenderer;
    private List<Vector3> lightningPath = new List<Vector3>();

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 3f;
        lineRenderer.endWidth = 3f;
    }
    public void SetLineSprite(Sprite sprite)
    {
        if (lineRenderer.material != null && sprite != null)
        {
            lineRenderer.material.mainTexture = sprite.texture;
        }
    }
    public override void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                lightningPath.Add(c.transform.position); // Store the hit position
                UpdateLightningEffect();

                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);

                if (currentRicochets < maxRicochets)
                {
                    currentRicochets++;
                    Ricochet();
                    return;
                }
                else
                {
                    DestroyProjectile();
                    return;
                }
            }
        }
    }

    private void UpdateLightningEffect()
    {
        lineRenderer.positionCount = lightningPath.Count;
        lineRenderer.SetPositions(lightningPath.ToArray());
    }

    private void Ricochet()
    {
        if (maxRicochets <= 0)
        {
            DestroyProjectile();
            return;
        }

        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 5f);
        float closestDistance = float.MaxValue;
        IDamageable closestEnemy = null;
        Transform closestEnemyTransform = null;

        foreach (Collider2D c in potentialTargets)
        {
            IDamageable potentialEnemy = c.GetComponent<IDamageable>();
            if (potentialEnemy != null && !CheckRepeatHit(potentialEnemy))
            {
                float currentDistance = Vector2.Distance(transform.position, c.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemy = potentialEnemy;
                    closestEnemyTransform = c.transform;
                }
            }
        }

        if (closestEnemy != null && closestEnemyTransform != null)
        {
            lightningPath.Add(closestEnemyTransform.position); // Store the next position for ricochet
            UpdateLightningEffect();

            Vector2 directionToEnemy = (closestEnemyTransform.position - transform.position).normalized;
            SetDirection(directionToEnemy.x, directionToEnemy.y);
            maxRicochets--;
        }
        else
        {
            DestroyProjectile();
        }
    }

    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        maxRicochets = weaponBase.weaponData.stats.numberOfHits;
    }

    private void OnEnable()
    {
        base.OnEnable();
        lightningPath.Clear(); // Clear the path when the object is reused
        UpdateLightningEffect();
        currentRicochets = 0;
    }
}