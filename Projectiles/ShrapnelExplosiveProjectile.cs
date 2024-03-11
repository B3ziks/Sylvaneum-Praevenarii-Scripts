using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelExplosiveProjectile : Projectile
{
    [SerializeField] private PoolObjectData shrapnelPoolData;
    public float shrapnelRange = 10f; // Distance the shrapnel will travel
    private PoolManager poolManager;

    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Call base method to handle default behavior
        base.OnTriggerEnter2D(other);

        // Check for collision with an enemy
        if (other.CompareTag("Enemy"))
        {
            Vector2 hitDirection = (other.transform.position - transform.position).normalized;
            Explode(hitDirection);
        }
    }

    protected void Explode(Vector2 hitDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, hitDirection, shrapnelRange);
        if (hit.collider != null)
        {
            SpawnShrapnel(transform.position, hit.point);
        }

        DestroyProjectile();
    }

    private void SpawnShrapnel(Vector2 origin, Vector2 target)
    {
        GameObject shrapnel = poolManager.GetObject(shrapnelPoolData);
        if (shrapnel != null)
        {
            shrapnel.transform.position = origin;
            Vector2 direction = (target - origin).normalized;
            shrapnel.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            shrapnel.SetActive(true);

            ClaymoreShrapnelController shrapnelController = shrapnel.GetComponent<ClaymoreShrapnelController>();
            if (shrapnelController != null)
            {
                shrapnelController.ActivateEffect(damage, direction, shrapnelRange);
            }
        }
    }
}