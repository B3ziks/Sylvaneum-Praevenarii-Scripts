using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour, IPoolMember
{
    PoolMember poolMember;
    protected WeaponBase weapon;
    public float attackArea = 0.5f;
    Vector3 direction;
    [SerializeField] float speed;
    protected int damage = 5;
    protected int numOfHits = 1;
    protected List<IDamageable> enemiesHit = new List<IDamageable>();
    float ttl = 6f;
    public ElementType elementType;
    public float elementalPotency; // The potency or strength of the elemental effect.

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y).normalized;

        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected virtual void Update()
    {
        MoveProjectile();
        if (Time.frameCount % 6 == 0)
        {
            DetectHits();
        }
        CountdownToLive();
    }

    protected virtual void DetectHits()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            if (numOfHits <= 0)
            {
                DeactivateProjectile();
                break;
            }
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !AlreadyHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);
                numOfHits -= 1;
            }
        }

        if (numOfHits <= 0)
        {
            DeactivateProjectile();
        }
    }

    protected virtual bool AlreadyHit(IDamageable enemy)
    {
        return enemiesHit.Contains(enemy);
    }

    private void CountdownToLive()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0f)
        {
            DeactivateProjectile();
        }
    }

    protected virtual void DeactivateProjectile()
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

    public virtual void SetStats(WeaponBase weaponBase)
    {
        weapon = weaponBase;
        speed = weaponBase.weaponData.stats.projectileSpeed;
        damage = weaponBase.GetDamage();
        numOfHits = weaponBase.weaponData.stats.numberOfHits;
        elementalPotency = weaponBase.weaponData.stats.elementalPotency;
        elementType = weaponBase.weaponData.stats.elementType;
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

    public void DisplayDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), worldPosition);
    }

    private void OnEnable()
    {
        ttl = 6f;
        enemiesHit.Clear();
        transform.rotation = Quaternion.identity; // Reset rotation
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DeactivateProjectile();
        }
    }
}