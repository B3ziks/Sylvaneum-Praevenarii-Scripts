using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoSlotMachineProjectile : MonoBehaviour, IPoolMember
{
    public float projectileSpeed = 5f;
    public int damage = 10; // Static damage value
    public float ttl = 6f;  // Time To Live

    public float elementalPotency = 5f; // Static elemental potency
    public float elementalDamageOverTime = 2f; // Static elemental damage over time

    PoolMember poolMember;
    protected Transform playerTransform;
    protected float timeAlive;
    protected Rigidbody2D rb;
    private bool shouldSetVelocity;

    protected void OnEnable()
    {
        rb = rb ?? GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        shouldSetVelocity = true;
        timeAlive = 0;
    }

    protected virtual void Update()
    {
        if (shouldSetVelocity)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * projectileSpeed;
            shouldSetVelocity = false;

            SetDirection(direction.x, direction.y); // Set the rotation after determining the direction
        }

        timeAlive += Time.deltaTime;
        if (timeAlive >= ttl)
        {
            DeactivateProjectile();
        }
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Character playerCharacter = other.GetComponent<Character>();
            playerCharacter.TakeDamage(damage);

            ElementalEffectManager effectManager = playerCharacter.GetComponent<ElementalEffectManager>();
            if (effectManager != null)
            {
                effectManager.ApplyElementalEffect(ElementType.Poison, elementalPotency, elementalDamageOverTime);
            }

            DeactivateProjectile();
        }
        else if (other.CompareTag("Obstacle"))
        {
            DeactivateProjectile();
        }
    }

    public void ResetProjectile()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    protected virtual void DeactivateProjectile()
    {
        if (poolMember != null)
        {
            ResetProjectile();
            poolMember.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}