using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspProjectile : MonoBehaviour, IPoolMember
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
                Transform enemyTransform = ((UnityEngine.Component)enemy).transform; // Cast IDamageable to Component to access transform
                Vector3 enemyPosition = enemyTransform.position;
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                PostDamage(damage, enemyPosition);
                enemiesHit.Add(enemy);
                numOfHits -= 1;
            }
        }

        if (numOfHits <= 0)
        {
            DeactivateProjectile();
        }
    }

    private bool AlreadyHit(IDamageable enemy)
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