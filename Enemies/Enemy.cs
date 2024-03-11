using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour, IDamageable, IPoolMember
{
    // Start is called before the first frame update
    Transform targetDestination;
    GameObject targetGameobject;
    Character targetCharacter;

    public Rigidbody2D rgdbd2d;

    [SerializeField] public EnemyStats stats;
    //testing purposes
    public EnemyData enemyData;

    float stunned;
    Vector3 knockbackVector;
    float knockbackForce;
    float knockbackTimeWeight;
    PoolMember poolMember;

    [SerializeField] private ElementalEffectsEnemyManager effectsManager;
    //events
    public static event Action BossKilled;
    private EnemiesManager enemiesManager;

    private bool canAttack = true;  // New flag to control attack availability
    private float attackCooldown = 0.5f;  // New field to define attack cooldown duration
    private float flipThreshold = 0.1f;
    // New field to keep track of currently applied buffs
    private List<BuffAuras> activeBuffs = new List<BuffAuras>();
    private bool isBuffed = false; // New flag to check if the enemy is buffed

    //pathfinding
    private bool avoiding = false; // Flag to track whether enemy is currently avoiding
    private Vector3 avoidanceDirection; // Direction to move in while avoiding
    private float avoidanceTime = 0.5f; // Time to move in the avoidance direction
    private float currentAvoidanceTime = 0; // Track how long enemy has been moving in the avoidance direction
    private float avoidanceRayLength = 2f; // Length of the raycasts
    private float avoidanceRayAngle = 30f; // Angle between the center ray and the side rays
    private float maxAvoidanceTime = 2.0f; // adjust this time as needed
    public LayerMask obstacleLayer; // assign this in the inspector
    private float originalMoveSpeed;
    private int originalArmorValue;
    private bool isArmorReduced = false;
    private bool isSlowed = false;
    private bool isDamageReceivedIncreased = false; // Flag to check if damage multiplier is already increased
    private float originalDamageMultiplier = 1f; // Default value
    //
    public GameObject effectsContainer; // Assign this in the inspector to the "Effects" child
    public GameObject damageBuffPrefab; // Assign this in the inspector
    public GameObject attackSpeedBuffPrefab; // Assign this in the inspector

    private GameObject damageBuffInstance;
    private GameObject attackSpeedBuffInstance;
    //
    private float checkStuckInterval = 1.0f; // Time interval to check if stuck
    private float lastStuckCheckTime = 0f; // Keep track of last stuck check
     //
    public static event Action<EnemyData> EnemyDefeated;
    public GameObject catchingAnimation;

    //
    void Start() 
    {
        // Get the ElementalEffectsEnemyManager component from the current GameObject
        enemiesManager = FindObjectOfType<EnemiesManager>();

        effectsManager = GetComponent<ElementalEffectsEnemyManager>();
        if (effectsManager == null)
        {
            // Optionally handle the case where the effects manager is not found
           UnityEngine.Debug.LogError("ElementalEffectsEnemyManager not found on the enemy!");
        }
    }
    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        //targetGameobject = targetDestination.gameObject;
    }

    public void SetTarget(GameObject target)
    {
        targetGameobject = target;
        targetDestination = target.transform;
    }

    internal void UpdateStatsForProgress(float progress)
    {
        stats.ApplyProgress(progress);
    }

    private void FixedUpdate()
    {
        ProcessStun();
        Move(); 
        ApplyKnockback();  // Added this line to apply the knockback effect
        CheckForObstacles();

        // Check if stuck
        if (Time.time > lastStuckCheckTime + checkStuckInterval)
        {
            CheckIfStuck();
            lastStuckCheckTime = Time.time;
        }
    }
    private void CheckIfStuck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, obstacleLayer);
        if (colliders.Length > 0)
        {
            Vector3 safePosition = FindSafePosition();
            if (safePosition != Vector3.zero)
            {
                transform.position = safePosition; // Teleport to safe position
            }
        }
    }

    private Vector3 FindSafePosition()
    {
        float checkDistance = 1.0f;
        int numberOfChecks = 8;
        for (int i = 0; i < numberOfChecks; i++)
        {
            Vector3 direction = Quaternion.Euler(0, 0, (360f / numberOfChecks) * i) * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, obstacleLayer);
            if (hit.collider == null)
            {
                // Found a safe direction, return position in this direction
                return transform.position + direction * checkDistance;
            }
        }
        return Vector3.zero; // No safe position found, return zero vector
    }

    private void ProcessStun()
    {
        if (stunned > 0f)
        {
            //rgdbd2d.velocity = Vector2.zero;
            stunned -= Time.fixedDeltaTime;

        }
    }

    private void Move()
    {
        if (stunned > 0f) { return; }

        Vector2 desiredDirection = (targetDestination.position - transform.position).normalized;

        if (avoiding)
        {
            desiredDirection = avoidanceDirection;
        }

        // Use steering to smoothly turn towards the desired direction
        Vector2 steeringForce = desiredDirection - rgdbd2d.velocity.normalized;
        Vector2 adjustedVelocity = rgdbd2d.velocity + (steeringForce.normalized * stats.moveSpeed);

        // Clamp velocity magnitude to max speed
        rgdbd2d.velocity = Vector2.ClampMagnitude(adjustedVelocity, stats.moveSpeed);

        // Flip the sprite based on desired direction (intent) rather than current velocity
        if (desiredDirection.x < -flipThreshold)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (desiredDirection.x > flipThreshold)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void CheckForObstacles()
    {
        if (avoiding)
        {
            currentAvoidanceTime += Time.fixedDeltaTime;
            if (currentAvoidanceTime > maxAvoidanceTime)
            {
                avoiding = false; // stop avoiding after the max time has passed
                return;
            }
        }
        else
        {
            float rayLength = 2f; // or whatever distance makes sense for your game

            // Central raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (targetDestination.position - transform.position).normalized, rayLength, obstacleLayer);

            // Left and right raycasts at some angles, adjust as necessary
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 20) * (targetDestination.position - transform.position).normalized, rayLength, obstacleLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -20) * (targetDestination.position - transform.position).normalized, rayLength, obstacleLayer);

            if (hit.collider != null)
            {
                // Logic to decide which way to avoid: Compare the distances of hitLeft and hitRight.
                if (hitLeft.distance < hitRight.distance)
                {
                    avoidanceDirection = Vector3.Cross((targetDestination.position - transform.position).normalized, Vector3.forward);
                }
                else
                {
                    avoidanceDirection = Vector3.Cross((targetDestination.position - transform.position).normalized, Vector3.back);
                }

                StartAvoidance();
            }
        }
    }

    private bool RayHitObstacle(Vector3 origin, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, avoidanceRayLength);
        return hit.collider != null && hit.collider.CompareTag("Obstacle");
    }

    private void StartAvoidance()
    {
        avoiding = true;
        // Decide the perpendicular direction (in this case, let's go right of the current direction towards player)
        avoidanceDirection = Vector3.Cross((targetDestination.position - transform.position).normalized, Vector3.forward);
        currentAvoidanceTime = 0; // Reset the current avoidance time
    }

    private Vector3 CalculateMovementVelocity(Vector3 direction)
    {
        return direction * stats.moveSpeed * (stunned > 0f ? 0f : 1f);
    }

    private Vector3 CalculateKnockback()
    {
        if (knockbackTimeWeight > 0f)
        {
            knockbackTimeWeight -= Time.fixedDeltaTime;
        }

        return knockbackVector * knockbackForce * (knockbackTimeWeight > 0f ? 1f : 0f);
    }


    internal void SetStats(EnemyStats stats)
    {
        this.stats = new EnemyStats(stats);
        //throw new NotImplementedException();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == targetGameobject)
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Debug.Log("Attacking the character!");
        if (canAttack)  // Check if enemy can attack
        {
            if (targetCharacter == null)
            {
                targetCharacter = targetGameobject.GetComponent<Character>();
            }
            targetCharacter.TakeDamage(stats.damage);

            StartCoroutine(AttackCooldownRoutine());  // Start cooldown coroutine after attacking
        }
    }
    private void FlipSpriteBasedOnDirection(Vector3 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;  // Disable attacking
        yield return new WaitForSeconds(attackCooldown);  // Wait for cooldown duration
        canAttack = true;  // Enable attacking again
    }

    public void Heal(int health)
    {
        stats.hp += health;

    }
    public virtual void TakeDamage(int damage)
    {
        // Subtract armor from the incoming damage, ensuring it doesn't go below zero
        int damageAfterArmor = Mathf.Max(damage - stats.armor, 0);

        // Adjust damage based on the damage received multiplier
        int adjustedDamage = Mathf.RoundToInt(damageAfterArmor * stats.damageReceivedMultiplier);

        stats.hp -= adjustedDamage;

        if (stats.hp < 1)
        {
            Defeated();
        }
    }

    private void Defeated()
    {
        if (effectsManager != null)
        {
            effectsManager.CleanupEffects();
        }

        targetGameobject.GetComponent<Level>().AddExperience(stats.experience_reward);
        GetComponent<DropOnDestroy>().CheckDrop();

        // Check if the enemy is elite and update the score accordingly.
        int scoreToAdd = enemyData.isElite ? 1000 : 100;
        EndlessStageManager.instance.EnemyKilled(enemyData);  // Corrected method call
        // Check if the enemy is a boss and trigger the BossKilled event.
        // Trigger EnemyDefeated event with enemyData
        EnemyDefeated?.Invoke(enemyData);

        if (enemyData.isBoss)
        {
            if (GameManager.instance.IsRestarting)
            {
                return; // Do not trigger boss defeat if the game is restarting
            }
            else
            {
                catchingAnimation.SetActive(true);
                Animator animator = catchingAnimation.GetComponent<Animator>();
                animator.Play("CatchAnimation");
                StartCoroutine(WaitForAnimation(animator));
            }
        }
        else
        {

            if (poolMember != null)
            {
                poolMember.ReturnToPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

    private IEnumerator WaitForAnimation(Animator animator)
    {
        // Wait for the current animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Now invoke the event after the animation is done
        BossKilled?.Invoke();

        if (poolMember != null)
        {
            poolMember.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReturnToPool()
    {
        enemiesManager?.RemoveEnemyFromActiveList(this);

        ResetEnemyState();

        if (poolMember != null)
        {
            poolMember.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void ResetEnemyState()
    {
        enemyData.ResetStats();

        // Reset any state or properties to their defaults
        // For example, resetting health, status effects, etc.
    }
    //
    public void Stun(float stun)
    {
        stunned = stun;
    }

    public void Knockback(Vector3 vector, float force, float timeWeight)
    {
        knockbackVector = vector;
        knockbackForce += force;
        knockbackTimeWeight = timeWeight;
    }

    private void ApplyKnockback()
    {
        Vector3 knockback = CalculateKnockback();
        rgdbd2d.velocity += (Vector2)knockback;
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    internal void UpdateStatsForDifficulty(Difficulty difficulty)
    {
        float difficultyMultiplier = 1f;

        switch (difficulty)
        {
            case Difficulty.Normal:
                difficultyMultiplier = 1f;
                break;
            case Difficulty.Hard:
                difficultyMultiplier = 2f; // +200%
                break;
            case Difficulty.Insane:
                difficultyMultiplier = 4f; // +400%
                break;
        }

        stats.hp = Mathf.CeilToInt(stats.hp * difficultyMultiplier);
        stats.damage = Mathf.CeilToInt(stats.damage * difficultyMultiplier);
        stats.armor = Mathf.CeilToInt(stats.armor * difficultyMultiplier);
        stats.experience_reward = Mathf.CeilToInt(stats.experience_reward * (difficultyMultiplier/2));

    }
    public void ApplyElementalEffect(ElementType effectType, float duration, int potency)
    {
        // Parameters based on potency
        int armorReduction = potency / 5; // Armor reduction is directly proportional to potency.
        float movementSpeedReduction = duration / 5f; // Adjust the divisor for balance.
        float damageReceivedIncrease = duration / 6f; // HP regen reduction is directly proportional to potency.


        // If the enemy object has been destroyed, don't try to apply any effects.
        if (this == null || !gameObject.activeInHierarchy)
        {
            UnityEngine.Debug.LogWarning("Attempted to apply an effect to an enemy that is no longer active.");
            return;
        }

        switch (effectType)
        {
            case ElementType.Fire:
                effectsManager.StartBurning(duration, potency);
                break;
            case ElementType.Poison:
                // Assuming potency is the damage over time, and armor reduction is another effect of the poison
                effectsManager.StartPoisoning(duration, potency, armorReduction);
                break;
            case ElementType.Ice:
                // Assuming potency is the damage over time, and movement speed reduction is another effect of the ice
                effectsManager.StartSlowing(duration, potency, movementSpeedReduction);
                break;
            case ElementType.Lightning:
                // Assuming potency is the damage over time, and HP regen reduction is the effect of the lightning
                effectsManager.StartIncreasingDamageReceived(duration, potency, damageReceivedIncrease);
                break;
            case ElementType.Normal:
                // Handle normal effect if there is one
                break;
            default:
                UnityEngine.Debug.LogWarning($"Unsupported effect type: {effectType}");
                break;
        }
    }

    // Method to apply the poison effect that reduces armor
    public void ApplyPoison(int armorReduction)
    {
        if (!isArmorReduced)
        {
            // Store the original armor value
            originalArmorValue = stats.armor;

            // Calculate the new armor value
            int newArmorValue = stats.armor - armorReduction;

            // Ensure armor doesn't go below zero
            stats.armor = Mathf.Max(0, newArmorValue);
            isArmorReduced = true;
        }
    }

    // Method to revert the poison effect on armor
    public void RevertPoison()
    {
        if (isArmorReduced)
        {
            // Restore the original armor value
            stats.armor = originalArmorValue;
            isArmorReduced = false;
            // Optional: Log for debugging
            UnityEngine.Debug.Log($"Poison effect reverted. Armor restored to original: {stats.armor}");
        }
    }

    // Method to apply the slowing effect that reduces move speed
    public void ApplySlowing(float movementSpeedReduction)
    {
        if (!isSlowed)
        {
            originalMoveSpeed = stats.moveSpeed; // Store the original speed before applying the effect.
            stats.moveSpeed -= movementSpeedReduction;
            stats.moveSpeed = Mathf.Max(stats.moveSpeed, 0.1f); // Prevent move speed from dropping below 0.1.

            isSlowed = true;

            UnityEngine.Debug.Log($"Slowing applied. Original Speed: {originalMoveSpeed}, Reduced by: {movementSpeedReduction}, New Speed: {stats.moveSpeed}");
        }
    }

    // Method to revert the slowing effect on move speed
    public void RevertSlowing()
    {
        if (isSlowed)
        {
            stats.moveSpeed = originalMoveSpeed; // Restore the original speed.

            isSlowed = false;

            UnityEngine.Debug.Log($"Slowing reverted. Speed restored to original: {stats.moveSpeed}");
        }
    }

    public void ApplyDamageReceivedIncreasing(float damageReceivedIncreasing)
    {
        if (isDamageReceivedIncreased)
        {
            return;
        }

        if (damageReceivedIncreasing <= 0)
        {
            return;
        }

        originalDamageMultiplier = stats.damageReceivedMultiplier; // Store the original multiplier only if it hasn't been increased yet
        stats.damageReceivedMultiplier += damageReceivedIncreasing;
        isDamageReceivedIncreased = true; // Set the flag to true after increasing the multiplier
       // UnityEngine.Debug.Log($"Applied damage increase. Previous multiplier: {originalDamageMultiplier}, Increased by: {damageReceivedIncreasing}, New multiplier: {stats.damageReceivedMultiplier}");
    }

    public void RevertDamageReceivedIncreasing()
    {
        if (!isDamageReceivedIncreased)
        {
            return;
        }

        stats.damageReceivedMultiplier = originalDamageMultiplier; // Revert to the stored original multiplier
        isDamageReceivedIncreased = false; // Reset the flag after reverting the multiplier
       // UnityEngine.Debug.Log($"Reverted damage increase to original multiplier: {originalDamageMultiplier}");

        // Reset the originalDamageMultiplier back to the default value after the revert is done
        originalDamageMultiplier = 1f;
    }

    //buffs
    public void InstantiateBuffEffect(BuffAuras buffType)
    {
        if (effectsContainer == null)
        {
            UnityEngine.Debug.LogError("Effects container is not assigned on " + gameObject.name);
            return;
        }

        GameObject prefabToInstantiate = null;
        // Check if the prefab is already instantiated and just activate it
        // Calculate the offset
        Vector3 effectOffset = new Vector3(0, CalculateEffectYOffset(), 0);

        switch (buffType)
        {
            case BuffAuras.DamageBuff:
                if (damageBuffInstance == null)
                {
                    damageBuffInstance = Instantiate(damageBuffPrefab, effectsContainer.transform);
                }
                damageBuffInstance.transform.localPosition = effectOffset;
                damageBuffInstance.SetActive(true);
                break;
            case BuffAuras.AttackSpeedBuff:
                if (attackSpeedBuffInstance == null)
                {
                    attackSpeedBuffInstance = Instantiate(attackSpeedBuffPrefab, effectsContainer.transform);
                }
                attackSpeedBuffInstance.transform.localPosition = effectOffset;
                attackSpeedBuffInstance.SetActive(true);
                break;
                // Add more cases as needed for different buffs
        }
    

        if (prefabToInstantiate != null)
        {
            GameObject buffEffectInstance = Instantiate(prefabToInstantiate, effectsContainer.transform);
            buffEffectInstance.transform.localPosition = Vector3.zero; // Position it at the container's origin

            // Store the instance for later reference
            switch (buffType)
            {
                case BuffAuras.DamageBuff:
                    damageBuffInstance = buffEffectInstance;
                    break;
                case BuffAuras.AttackSpeedBuff:
                    attackSpeedBuffInstance = buffEffectInstance;
                    break;
                    // Add more cases as needed for different buffs
            }
        }
    }

    private float CalculateEffectYOffset()
    {
        // You could make this a serialized field if you want to adjust it in the inspector for each enemy type.
        float additionalOffset = 0.5f; // This value determines how far above the sprite the effect will appear.

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // The total offset is the size of the sprite plus an additional fixed amount.
            return spriteRenderer.bounds.extents.y + additionalOffset;
        }
        return additionalOffset; // If for some reason the sprite renderer is missing, we still apply the additional offset.
    }

    public void RemoveBuffEffect(BuffAuras buffType)
    {
        GameObject buffEffectInstance = null;
        // Deactivate the prefab instead of destroying it
        switch (buffType)
        {
            case BuffAuras.DamageBuff:
                if (damageBuffInstance != null)
                {
                    damageBuffInstance.SetActive(false);
                }
                break;
            case BuffAuras.AttackSpeedBuff:
                if (attackSpeedBuffInstance != null)
                {
                    attackSpeedBuffInstance.SetActive(false);
                }
                break;
                // Add more cases as needed for different buffs
        }
    }
    public void ApplyBuff(BuffAuras buffType)
    {
        if (activeBuffs.Contains(buffType)) // Check if buff is already active
            return; // If already active, just return

        switch (buffType)
        {
            case BuffAuras.DamageBuff:
                InstantiateBuffEffect(buffType);
                stats.damage = (int)(stats.damage * 1.20f); // 120% damage
                activeBuffs.Add(buffType); // Add buff to active list
                break;
            case BuffAuras.AttackSpeedBuff:
                InstantiateBuffEffect(buffType);
                stats.timeToAttack = (stats.timeToAttack * 0.80f); // 80% of original attack time, so faster attack
                activeBuffs.Add(buffType); // Add buff to active list
                break;
                // Implement other buffs as needed...
        }
    }

    public void RemoveBuff(BuffAuras buffType, float originalValue)
    {
        if (!activeBuffs.Contains(buffType)) // Check if buff is active
            return; // If not active, just return

        switch (buffType)
        {
            case BuffAuras.DamageBuff:
                stats.damage = (int)originalValue;
                RemoveBuffEffect(buffType);
                activeBuffs.Remove(buffType); // Remove buff from active list
                break;
            case BuffAuras.AttackSpeedBuff:
                RemoveBuffEffect(buffType);
                stats.timeToAttack = originalValue;
                activeBuffs.Remove(buffType); // Remove buff from active list
                break;
                // Implement other buffs as needed...
        }
    }
    private void OnDestroy()
    {
        // Ensure all coroutines are stopped when the GameObject is destroyed
        if (effectsManager != null)
        {
            effectsManager.CleanupEffects();
        }
    }
       private void OnDisable()
    {
          // Ensure all coroutines are stopped when the GameObject is destroyed
        if (effectsManager != null)
        {
            effectsManager.CleanupEffects();
        }
    }

}
