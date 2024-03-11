using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolMember
{
    protected PoolMember poolMember;
    protected WeaponBase weapon;
    public float attackArea = 0.5f;
    protected Vector3 direction;
    [SerializeField] protected float speed;
    protected int damage = 5;
    protected int numOfHits = 1;
    protected int radius = 0;
    protected float potency = 0f;
    protected List<IDamageable> enemiesHit = new List<IDamageable>();
    protected float ttl = 6f;
    [SerializeField] private bool isHomingModeEnabled = false;
    public static List<Projectile> ActiveProjectiles = new List<Projectile>();
    public float homingTurnSpeed = 15f; // Adjust this value to control the turn speed towards the mouse
    private bool isTargetingEnabled = false;
    private static Vector2 currentTargetingPosition;

    private static Vector2 targetingPosition;
    private TargetComboBehaviour targetComboBehaviour; // Add this

    public void SetTargetingPosition(Vector2 newTargetingPosition)
    {
        currentTargetingPosition = newTargetingPosition;
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y).normalized;

        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void SetTargetComboBehaviour(TargetComboBehaviour tcb)
    {
        targetComboBehaviour = tcb;
    }
    protected virtual void Update()
    {
        if (isHomingModeEnabled)
        {
            UpdateDirectionTowardsMouse();
        }

        if (isTargetingEnabled && targetComboBehaviour != null)
        {
            currentTargetingPosition = targetComboBehaviour.GetCurrentTargetPosition();
            Vector2 newDirection = (currentTargetingPosition - (Vector2)transform.position).normalized;
            direction = newDirection;
        }

        Move();

        if (Time.frameCount % 6 == 0)
        {
            HitDetection();
        }
        TimerToLive();
    }
    private void UpdateDirectionTowardsMouse()
    {
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newDirection = (targetPosition - (Vector2)transform.position).normalized;
        float step = homingTurnSpeed * Time.deltaTime;
        direction = Vector2.MoveTowards(direction, newDirection, step).normalized;
    }

    public virtual void SetPartialStats(WeaponBase weaponBase, float fraction)
    {
        weapon = weaponBase;
        speed = weaponBase.weaponData.stats.projectileSpeed * fraction;
        damage = Mathf.RoundToInt(weaponBase.GetDamage() * fraction);
        enemiesHit.Clear();
    }
    public virtual void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            if (numOfHits <= 0)
            {
                DestroyProjectile(); // Destroy or return the projectile when no hits remain
                break;
            }
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);
                numOfHits -= 1;
            }
        }

        // Additional check outside of loop in case numofHits is zero to start with.
        if (numOfHits <= 0)
        {
            DestroyProjectile();
        }
    }

    //
    private Vector2 CalculateMouseDirection()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        return ((mouseWorldPosition - transform.position).normalized);
    }
    public void EnableHomingMode()
    {
        isHomingModeEnabled = true;
    }

    public void DisableHomingMode()
    {
        isHomingModeEnabled = false;
       // direction = transform.up; // Continue in the last direction it was facing
    }
    // This method is used in WeaponBase to get all projectiles
    public static List<Projectile> GetAllProjectiles()
    {
        return ActiveProjectiles;
    }
    //
    private void UpdateDirectionTowardsTarget()
    {
        // Homing towards the target position instead of the mouse
        Vector2 newDirection = (targetingPosition - (Vector2)transform.position).normalized;
        float step = homingTurnSpeed * Time.deltaTime;
        direction = Vector2.MoveTowards(direction, newDirection, step).normalized;
    }
    public void ResetTargetingPosition()
    {
        currentTargetingPosition = Vector2.zero; // Reset the targeting position
    }
    public void EnableTargeting()
    {
        isTargetingEnabled = true;
    }

    public void DisableTargeting()
    {
        isTargetingEnabled = false;
       // direction = transform.up; // Continue in the last direction it was facing

    }
    //
    protected bool CheckRepeatHit(IDamageable enemy)
    {
        return enemiesHit.Contains(enemy);
    }

    protected void TimerToLive()
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
    public virtual void SetStats(WeaponBase weaponBase)
    {
        weapon = weaponBase;
        speed = weaponBase.weaponData.stats.projectileSpeed;
        damage = weaponBase.GetDamage();
        numOfHits = weaponBase.weaponData.stats.numberOfHits;
        radius = weaponBase.weaponData.stats.explosionRadius;
        potency = weaponBase.weaponData.stats.elementalPotency;

        enemiesHit.Clear();
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    protected void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public virtual void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(weapon.weaponData.stats.elementType);
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }

    protected Color GetMessageColor(ElementType elementType)
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
    void OnDisable()
    {
        ActiveProjectiles.Remove(this);
        // Other cleanup code...
    }
    protected virtual void OnEnable()
    {
        ActiveProjectiles.Add(this);

        ttl = 6f;
        enemiesHit.Clear();
        transform.rotation = Quaternion.identity; // Reset rotation
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
       // UnityEngine.Debug.Log($"{name} collided with {other.gameObject.name}");

        if (other.CompareTag("Obstacle"))
        {
            DestroyProjectile();
        }
    }
}