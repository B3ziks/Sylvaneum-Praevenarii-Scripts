using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionMagicStrike : MonoBehaviour, IPoolMember
{
    private WeaponBase weaponBase;
    private int damage;
    private float explosionRadius;
    protected PoolMember poolMember;

    // This method is now used to set the stats from the WeaponBase
    public void SetStats(WeaponBase weaponBase)
    {
        this.weaponBase = weaponBase;
        this.damage = weaponBase.GetDamage();
        this.explosionRadius = weaponBase.weaponData.stats.explosionRadius;
    }

    // Call this method to initialize the explosion strike
    public void Initialize(Enemy targetEnemy)
    {
        // Area damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                PostDamage(damage, hitCollider.transform.position);
            }
        }

        // Start the coroutine to fade and disable the explosion strike
        StartCoroutine(FadeAndDisable());
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
    private IEnumerator FadeAndDisable()
    {
        // Insert fade out logic here, you can adjust the sprite's alpha or use an animation

        yield return new WaitForSeconds(1f); // Wait for 1 second to simulate the strike

        // Deactivate or return to pool after fading
        if (poolMember == null)
        {
            Destroy(gameObject);
        }
        else
        {
            poolMember.ReturnToPool();
        }
        // If using pooling, return to the pool instead
    }
    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }
}