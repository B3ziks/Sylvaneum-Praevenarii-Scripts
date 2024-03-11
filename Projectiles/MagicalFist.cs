using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalFist : MonoBehaviour, IPoolMember
{
    private WeaponBase weaponBase;
    private int damage;
    private float areaOfEffectRadius;
    private PoolMember poolMember;
    private float fallSpeed = 10f; // Speed of the fist's descent

    // Set the stats from the WeaponBase
    public void SetStats(WeaponBase weaponBase)
    {
        this.weaponBase = weaponBase;
        damage = weaponBase.GetDamage();
        areaOfEffectRadius = weaponBase.weaponData.stats.explosionRadius;
    }

    // Initialize the Magical Fist
    public void Initialize(Vector3 targetPosition)
    {
        StartCoroutine(FallToTarget(targetPosition));
    }

    private IEnumerator FallToTarget(Vector3 targetPosition)
    {
        // Start above the target and move down
        Vector3 startPosition = targetPosition + Vector3.up * 3.0f;
        transform.position = startPosition;

        while (transform.position.y > targetPosition.y)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // Apply area damage once the fist reaches the target
        ApplyAreaDamage(targetPosition);
        StartCoroutine(FadeAndDisable());
    }


    private void ApplyAreaDamage(Vector3 center)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, areaOfEffectRadius);
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                PostDamage(damage, enemy.transform.position);
            }
        }
    }

    public void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(weaponBase.weaponData.stats.elementType);
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }

    private Color GetMessageColor(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return new Color(1f, 0.8f, 0f, 1f);
            case ElementType.Poison:
                return Color.green;
            case ElementType.Ice:
                return Color.cyan;
            case ElementType.Lightning:
                return Color.yellow;
            default:
                return new Color(0.8f, 0.8f, 0.8f, 1f);
        }
    }

    private IEnumerator FadeAndDisable()
    {
        // Insert fade out logic here (e.g., adjust the sprite's alpha or use an animation)

        yield return new WaitForSeconds(1f); // Wait for the effect to complete

        // Deactivate or return to pool
        if (poolMember == null)
        {
            Destroy(gameObject);
        }
        else
        {
            poolMember.ReturnToPool();
        }
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }
}