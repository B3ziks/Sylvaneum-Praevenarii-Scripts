using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public PlayerMove playerMove;
    public WeaponData weaponData;

    protected float timer;
    protected Vector2 vectorOfAttack;
    protected PoolManager poolManager;

    protected Character owner; // A field to store the owner character
    public float rotationSpeed = 5f;  // Adjusts how fast the weapon rotates. 
    public bool shouldRotateTowardsMouse = true;
    // Homing missile mode variables
    public bool isHomingMissileModeActive = false;
    public float homingMissileDuration = 5f;
    public float homingMissileCooldown = 10f;
    private bool isInHomingMissileCooldown = false;
    public bool isComboActive = false;
    // Variables for MagicTargetingCombo
    public GameObject targetingIndicatorPrefab; // Assign this in the inspector
    protected GameObject targetingIndicatorInstance;
    public bool isTargetingComboActivated = false; // This should be set by the ComboManager
    protected float targetingComboDuration = 3.0f; // Duration of the combo
    protected float targetingComboCooldown = 10.0f; // Cooldown duration
    protected bool isInTargetingComboCooldown = false;
    protected static Vector2 targetingPosition;
    protected bool isSceneLoaded;
    //
    private MagicTargetingWeapon magicTargetingWeapon; // New field

    protected virtual void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        // Get the MagicTargetingWeapon component from this GameObject or its children
        magicTargetingWeapon = GetComponentInChildren<MagicTargetingWeapon>();
    }
    private void Update()
    {
        if (GameManager.instance.IsGamePausedOrStopped) // Ensure this property exists in your GameManager
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Attack();
            timer = weaponData.stats.timeToAttack;
        }
        if (shouldRotateTowardsMouse && !isHomingMissileModeActive)
        {
            UpdateVectorOfAttack();
        }

        // New input check for activating homing missile mode
        if (Input.GetKeyDown(KeyCode.R) && !isInHomingMissileCooldown && isComboActive)
        {
            StartCoroutine(ActivateHomingMissileMode());
        }
       
    }


    // New Coroutine for homing missile mode
    private IEnumerator ActivateHomingMissileMode()
    {
        isHomingMissileModeActive = true;
        List<Projectile> projectiles = Projectile.GetAllProjectiles();

        // Enable homing mode in each projectile
        foreach (var projectile in projectiles)
        {
            projectile.EnableHomingMode();
        }

        yield return new WaitForSeconds(homingMissileDuration);

        // Disable homing mode after the duration ends
        // This also ensures that any projectiles that are still active have their homing mode disabled
        foreach (var projectile in projectiles.Where(p => p != null))
        {
            projectile.DisableHomingMode();
        }

        isHomingMissileModeActive = false;
        isInHomingMissileCooldown = true;
        yield return new WaitForSeconds(homingMissileCooldown);
        isInHomingMissileCooldown = false;
    }
   
   
    //
    public void ApplyDamage(Collider2D[] colliders)
    {
        int damage = GetDamage();
        foreach (var collider in colliders)
        {
            IDamageable entity = collider.GetComponent<IDamageable>();
            if (entity != null)
            {
                ApplyDamage(collider.transform.position, damage, entity);
            }
        }
    }

    public void ApplyDamage(Vector3 position, int damage, IDamageable entity)
    {
        PostDamage(damage, position);
        entity.TakeDamage(damage);

        // Apply elemental and other additional effects
        ApplyAdditionalEffects(entity, position);

    }

    public void ApplyAdditionalEffects(IDamageable entity, Vector3 enemyPosition)
    {
        entity.Stun(weaponData.stats.stun);
        entity.Knockback((enemyPosition - transform.position).normalized,
                          weaponData.stats.knockback,
                          weaponData.stats.knockbackTimeWeight);
       // UnityEngine.Debug.Log($"Applying Knockback. Direction: {(enemyPosition - transform.position).normalized}, Force: {weaponData.stats.knockback}, Time Weight: {weaponData.stats.knockbackTimeWeight}");

        // Applying elemental effects based on weapon's elemental type
        entity.ApplyElementalEffect(weaponData.stats.elementType, weaponData.stats.elementalPotency, weaponData.stats.damageOverTime);
    }
    private void ApplyAllUpgrades()
    {
        foreach (var upgrade in weaponData.upgrades)
        {
            for (int i = 0; i < weaponData.baseStats.upgradeLevel && i < upgrade.maxUpgradeLevel; i++)
            {
                weaponData.stats.ApplyUpgrade(upgrade, weaponData.weaponType);
            }
        }
    }

    public virtual void SetData(WeaponData wd)
    {
        weaponData = wd;
        weaponData.stats = new WeaponStats(wd.baseStats);
        foreach (var upgrade in weaponData.upgrades)
        {
            for (int i = 0; i < weaponData.baseStats.upgradeLevel && i < upgrade.maxUpgradeLevel; i++)
            {
                weaponData.stats.ApplyUpgrade(upgrade, weaponData.weaponType);
            }
        }
    }

    public void AddOwnerCharacter(Character character)
    {
        this.owner = character;
    }

    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    public abstract void Attack();

    public int GetDamage()
    {
        // Start with base damage
        int damage = weaponData.stats.damage;

        // Determine if this hit is a critical strike
        if (UnityEngine.Random.value < weaponData.stats.critStrikeChance)
        {
            // Apply critical strike multiplier
            damage = Mathf.RoundToInt(damage * weaponData.stats.critStrikeMultiplier);
            // Optionally, you can add a way to notify the player that a critical hit occurred
          //  UnityEngine.Debug.Log("Critical Hit!");
        }

        return damage;
    }

    public virtual void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(weaponData.stats.elementType);
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

    public void Upgrade(UpgradeData upgradeData)
    {
        weaponData.stats.Sum(upgradeData.weaponUpgradeStats, weaponData.weaponType);
    }

    private void UpdateVectorOfAttack()
    {
        vectorOfAttack = CalculateMouseDirection();
        FaceTowards(vectorOfAttack + (Vector2)transform.position);
    }

    public Vector2 CalculateMouseDirection()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        return ((mouseWorldPosition - transform.position).normalized);
    }

    // Additional method to handle rotation towards a target
    private void FaceTowards(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Debug logs
        // UnityEngine.Debug.Log($"Target Position: {targetPosition}, Calculated Angle: {angle}, Current Rotation: {transform.rotation.eulerAngles.z}");
    }

    public virtual GameObject SpawnProjectile(PoolObjectData poolObjectData, Vector3 position)
    {
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = position;

        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetDirection(vectorOfAttack.x, vectorOfAttack.y);
            projectile.SetStats(this);
            // Set the TargetComboBehaviour instance
            if (TargetComboBehaviour.Instance != null)
            {
                projectile.SetTargetComboBehaviour(TargetComboBehaviour.Instance);
            }
        }

        return projectileGO;
    }
    public void ResetToBaseStats()
    {
        weaponData.stats = new WeaponStats(weaponData.baseStats);
    }
}