using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovProjectile : Projectile
{
    [SerializeField]
    private PoolObjectData firePoolData;

    public float explosionRadius; // radius of explosion

    private Vector3 targetPosition;
    private PoolManager poolManager;

    // This is already present in Projectile. We just need to override it.
    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        explosionRadius = weaponBase.weaponData.stats.explosionRadius;
    }

    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
        SetTargetPosition();
    }

    protected void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            Ignite();
        }
    }

    public override void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        bool hasIgnited = false;
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                // Instead of destroying the Molotov when it hits an enemy,
                // we ignite it, dealing damage to all enemies in the radius.
                if (!hasIgnited)
                {
                    Ignite();
                    hasIgnited = true;
                }
            }
        }
    }

    private void SetTargetPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
    }

    private void Ignite()
    {
        // Damage and post damage to enemies
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);
                PostDamage(damage, c.transform.position);
            }
        }

        // Spawn fire effect
        if (poolManager && firePoolData)
        {
            GameObject fireEffect = poolManager.GetObject(firePoolData);
            if (fireEffect)
            {
                fireEffect.transform.position = transform.position;
                fireEffect.transform.rotation = Quaternion.identity;
                FlameOnEnemy flameScript = fireEffect.GetComponent<FlameOnEnemy>();
                if (flameScript != null)
                {
                    flameScript.damage = this.damage; // Transferring damage value to the flame script
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get fire effect from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or firePoolData is not set.");
        }

        DestroyProjectile();
    }

    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            Ignite();
        }
    }
}