using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPlantPlayer : MonoBehaviour, IPoolMember
{
    private float ttl;
    private float damageRadius;
    private int damage;
    private PoolMember poolMember;

    public void Initialize(float timeToLive, float explosionRadius, int damageAmount)
    {
        ttl = timeToLive;
        damageRadius = explosionRadius;
        damage = damageAmount;
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(ttl);
        Explode();
    }

    private void Explode()
    {
        // Area damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                PostDamage(damage, hitCollider.transform.position);
            }
        }

        // Optionally, spawn an explosion effect here

        DeactivatePlant();
    }

    private void PostDamage(int damageDealt, Vector3 targetPosition)
    {
        // Assuming you have a MessageSystem to post damage messages
        MessageSystem.instance.PostMessage(damageDealt.ToString(), targetPosition);
    }

    private void DeactivatePlant()
    {
        if (poolMember == null)
            Destroy(gameObject);
        else
            poolMember.ReturnToPool();
    }

    public void SetPoolMember(PoolMember member)
    {
        poolMember = member;
    }
}