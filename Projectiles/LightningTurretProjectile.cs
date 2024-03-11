using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTurretProjectile : MonoBehaviour, IPoolMember
{
    PoolMember poolMember;
    WeaponBase weapon;
    public float attackArea = 0.5f;
    Vector3 direction;
    [SerializeField] float speed;
    int damage = 5;
    int numOfHits = 1;
    int maxRicochets;
    int currentRicochets = 0;
    List<IDamageable> enemiesHit = new List<IDamageable>();
    float ttl = 6f;
    LineRenderer lineRenderer;
    List<Vector3> lightningPath = new List<Vector3>();
    private Transform currentTarget = null;
    private IDamageable lastHit = null;  // Store the last hit target

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y).normalized;

        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 3f;
        lineRenderer.endWidth = 3f;
    }

    void Update()
    {
        MoveProjectile();
        if (Time.frameCount % 6 == 0)
        {
            HitDetection();
        }
        CountdownToLive();
    }

    private void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                lightningPath.Add(c.transform.position);
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
                    DeactivateProjectile();
                    return;
                }
            }
        }
    }

    private void Ricochet()
    {
        if (maxRicochets <= 0)
        {
            DeactivateProjectile();
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
            lightningPath.Add(closestEnemyTransform.position);
            UpdateLightningEffect();

            Vector2 directionToEnemy = (closestEnemyTransform.position - transform.position).normalized;
            SetDirection(directionToEnemy.x, directionToEnemy.y);
            maxRicochets--;
        }
        else
        {
            DeactivateProjectile();
        }
    }

    private void UpdateLightningEffect()
    {
        lineRenderer.positionCount = lightningPath.Count;
        lineRenderer.SetPositions(lightningPath.ToArray());
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
        maxRicochets = weaponBase.weaponData.stats.maxRicochets;
        enemiesHit.Clear();
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    private void MoveProjectile()
    {
        if (currentTarget != null)
        {
            Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
            transform.position += dirToTarget * speed * Time.deltaTime;

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget < 0.1f)
            {
                currentTarget = null;
            }
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void SetLineSprite(Sprite sprite)
    {
        if (lineRenderer.material != null && sprite != null)
        {
            lineRenderer.material.mainTexture = sprite.texture;
        }
    }

    protected bool CheckRepeatHit(IDamageable enemy)
    {
        return enemiesHit.Contains(enemy);
    }

    public void DisplayDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), worldPosition);
    }

    private void OnEnable()
    {
        ttl = 6f;
        enemiesHit.Clear();
        currentRicochets = 0;
        transform.rotation = Quaternion.identity;
        lightningPath.Clear();
        UpdateLightningEffect();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DeactivateProjectile();
        }
    }
}