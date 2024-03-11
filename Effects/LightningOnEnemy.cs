using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightningOnEnemy : MonoBehaviour, IPoolMember, IEffect
{
    public int damage;
    public float cooldown = 1f; // Cooldown between lightning applications.
    public float lightningDuration = 3f; // Duration of the lightning effect.
    public float damageReceivedIncrease = 0.2f; // The amount of damage increase while affected by lightning.
    public float ttl = 10f; // Time to live for lightning effect in seconds.
    PoolMember poolMember;
    private bool isCooldown;
    private bool _isAffectedByLightning;

    public bool isAffectedByLightning
    {
        get { return _isAffectedByLightning; }
        private set
        {
            if (_isAffectedByLightning != value) // Only trigger if the state actually changes
            {
                _isAffectedByLightning = value;
                OnEffectStateChanged.Invoke(_isAffectedByLightning);
            }
        }
    }

    // Implement the IEffect interface
    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isAffectedByLightning;

    private void OnEnable()
    {
        StartCoroutine(TTLRoutine());
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCooldown && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            ElementalEffectsEnemyManager element = other.GetComponent<ElementalEffectsEnemyManager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                element.StartIncreasingDamageReceived(lightningDuration, 0, damageReceivedIncrease); // Starting the lightning effect on the enemy.
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Enemy component not found on enemy object!", other);
            }
        }
    }

    private IEnumerator ApplyLightningEffect(ElementalEffectsEnemyManager enemy)
    {
        if (isAffectedByLightning) yield break;

        isAffectedByLightning = true;
        enemy.StartIncreasingDamageReceived(lightningDuration, 0, damageReceivedIncrease); // Let enemy handle the lightning
        yield return new WaitForSeconds(lightningDuration);
        isAffectedByLightning = false;
    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }

    private IEnumerator TTLRoutine()
    {
        yield return new WaitForSeconds(ttl);
        DeactivateLightning();
    }

    private void DeactivateLightning()
    {
        poolMember.ReturnToPool();
    }
}