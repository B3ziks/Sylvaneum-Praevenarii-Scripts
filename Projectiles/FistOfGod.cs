using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfGod : MonoBehaviour, IPoolMember
{
    public int damage = 50; // The damage dealt by the Fist of God
    public float areaOfEffectRadius = 2f; // The radius within which the fist will deal damage
    public float fallSpeed = 10f; // Speed of the fist's descent
    private PoolMember poolMember;

    // Call this method from the FistOfGodAttack when spawning this object
    public void Initialize(Vector3 targetPosition)
    {
        StartCoroutine(FallToTarget(targetPosition));
    }

    private IEnumerator FallToTarget(Vector3 targetPosition)
    {
        // Start above the target and move down
        while (transform.position.y > targetPosition.y)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // Impact effect and damage
        Impact();

        // After a short delay, deactivate or return to pool
        yield return new WaitForSeconds(0.5f); // Wait half a second to simulate the fist on the ground
        DeactivateOrReturnToPool();
    }

    private void Impact()
    {
        // Apply damage to player if they are within the area of effect
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, areaOfEffectRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Character playerCharacter = hitCollider.GetComponent<Character>();
                if (playerCharacter != null)
                {
                    playerCharacter.TakeDamage(damage);
                    // You can also apply a knockback effect here if desired
                }
            }
        }
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    private void DeactivateOrReturnToPool()
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

    // Optional: If you need to visualize the area of effect in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaOfEffectRadius);
    }
}