using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

public class FoxBomberProjectile : MonoBehaviour, IPoolMember
{
    PoolMember poolMember;
    WeaponBase weapon;
    public float attackArea = 0.5f;
    Vector3 direction;
    [SerializeField] float speed;
    int damage = 5;
    int numOfHits = 1;
    List<IDamageable> enemiesHit = new List<IDamageable>();
    float ttl = 6f;
    private PoolManager poolManager;
    public float explosionRadius; // radius of explosion

    [SerializeField]
    private PoolObjectData explosionEffectPoolData;

    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y).normalized;

        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        MoveProjectile();
        if (Time.frameCount % 6 == 0)
        {
            DetectHits();
        }
        CountdownToLive();
    }

    private void DetectHits()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        bool hasExploded = false;
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                // Instead of destroying the grenade when it hits an enemy,
                // we explode it, dealing damage to all enemies in the radius.
                if (!hasExploded)
                {
                    Explode();
                    hasExploded = true;
                }
            }
        }
    }
    private void Explode()
    {
        // Damage and post damage to enemies
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy)) // Reuse the CheckRepeatHit method from base class
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy); // Assuming `enemiesHit` is protected in base class
                PostDamage(damage, c.transform.position);
            }
        }

        // Spawn explosion effect
        if (poolManager && explosionEffectPoolData)
        {
            GameObject explosionEffect = poolManager.GetObject(explosionEffectPoolData);
            if (explosionEffect)
            {
                explosionEffect.transform.position = transform.position;
                explosionEffect.transform.rotation = Quaternion.identity;
                explosionEffect.transform.localScale = Vector3.one * explosionRadius;  // Scaling to the diameter
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get explosion effect from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or explosionEffectPoolData is not set.");
        }

        DestroyProjectile();
    }
    private bool AlreadyHit(IDamageable enemy)
    {
        return enemiesHit.Contains(enemy);
    }

    protected bool CheckRepeatHit(IDamageable enemy)
    {
        return enemiesHit.Contains(enemy);
    }

    protected void CountdownToLive()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0f)
        {
            DestroyProjectile();
        }
    }

    protected virtual void DestroyProjectile()
    {
        if (poolMember == null)
        {
            Destroy(gameObject);
        }
        else
        {
            poolMember.ReturnToPool();
        }
    }
    private void DeactivateProjectile()
    {
        if (poolMember == null)
        {
            Destroy(gameObject);
        }
        else
        {
            poolMember.ReturnToPool();
        }
    }

    public void SetStats(WeaponBase weaponBase)
    {
        weapon = weaponBase;
        speed = weaponBase.weaponData.stats.projectileSpeed;
        damage = weaponBase.GetDamage();
        numOfHits = weaponBase.weaponData.stats.numberOfHits;
        explosionRadius = weaponBase.weaponData.stats.explosionRadius;

        enemiesHit.Clear();
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    private void MoveProjectile()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    public virtual void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(weapon.weaponData.stats.elementType);
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }

    private Color GetMessageColor(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return new Color(1f, 0.8f, 0f, 1f); // Brighter orange
            case ElementType.Poison:
                return Color.green;
            case ElementType.Ice:
                return Color.cyan;
            case ElementType.Lightning:
                return Color.yellow;
            default:
                return new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray for default
        }
    }


    private void OnEnable()
    {
        ttl = 6f;
        enemiesHit.Clear();
        transform.rotation = Quaternion.identity; // Reset rotation
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DeactivateProjectile();
        }
    }
}