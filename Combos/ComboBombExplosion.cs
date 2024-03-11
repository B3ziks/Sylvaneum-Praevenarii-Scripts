using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboBombExplosion : MonoBehaviour, IPoolMember
{
    public float delay = 3f; // Time in seconds before the bomb explodes
    public float blastRadius = 5f; // The radius of the explosion effect
    public int explosionDamage = 50; // The damage the explosion deals
    public Vector2 dropOffset = new Vector2(0, 10); // The offset from where the bomb will start dropping
    [SerializeField]
    private PoolObjectData explosionEffectPoolData; // Assign this with the prefab data in the inspector

    private PoolManager poolManager;
    protected PoolMember poolMember;

    private float countdown;
    private bool hasExploded = false;
    private Rigidbody2D rb; // Reference to the bomb's Rigidbody2D

    void Start()
    {
        poolManager = FindObjectOfType<PoolManager>(); // Find the PoolManager in the scene
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        // Set the bomb to be kinematic until it's time to drop it
        rb.isKinematic = true;
    }
    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    void OnEnable()
    {
        countdown = delay;
        hasExploded = false;

        // Position the bomb at an offset above the target drop position
        Vector2 targetDropPosition = (Vector2)transform.position + dropOffset;
        transform.position = targetDropPosition;

        // Start the bomb falling after a short delay, could be instant or delayed based on your design
        Invoke(nameof(StartFalling), 0.5f);
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true; // Ensure the bomb doesn't explode more than once
        }
    }

    void StartFalling()
    {
        rb.isKinematic = false; // Let the bomb fall
    }

    void Explode()
    {
        // Show explosion effect here
        GameObject explosionEffect = poolManager.GetObject(explosionEffectPoolData);
        if (explosionEffect)
        {
            explosionEffect.transform.position = transform.position;
            explosionEffect.transform.rotation = Quaternion.identity;
            explosionEffect.transform.localScale = Vector3.one * blastRadius * 2; // Adjust the scale if necessary

            explosionEffect.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("ComboBombExplosion: Failed to get explosion effect from pool.");
        }
        // Detect all colliders within the blast radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        foreach (Collider2D nearbyObject in colliders)
        {
            // Apply damage here, if the object has a component that can take damage
            var damageable = nearbyObject.GetComponent<IDamageable>(); // Assuming you have an IDamageable interface or similar setup
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
                PostDamage(explosionDamage, nearbyObject.transform.position);
            }
        }

        // Deactivate or destroy the bomb after explosion
        poolMember.ReturnToPool();

        // Destroy(gameObject);
    }

    public virtual void PostDamage(int damage, Vector3 targetPosition)
    {
        // Implement your message system here. 
        // MessageSystem.instance.PostMessage(damage.ToString(), targetPosition);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}