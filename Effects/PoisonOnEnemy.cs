using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoisonOnEnemy : MonoBehaviour, IPoolMember, IEffect
{
    public int damage;
    public float cooldown = 1f; // Cooldown between poison applications.
    public int poisonDamage = 5; // Damage per second while poisoned.
    public float poisonDuration = 3f; // Duration of the poison effect.
    public int armorReduction = 2; // The amount of armor reduction while poisoned.
    public float ttl = 10f; // Time to live for poison in seconds.
    PoolMember poolMember;
    private bool isCooldown;
    private bool _isPoisoned;

    public bool isPoisoned
    {
        get { return _isPoisoned; }
        private set
        {
            if (_isPoisoned != value) // Only trigger if the state actually changes
            {
                _isPoisoned = value;
                OnEffectStateChanged.Invoke(_isPoisoned);
            }
        }
    }

    // Implement the IEffect interface
    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isPoisoned;

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
                element.StartPoisoning(poisonDuration, poisonDamage, armorReduction); // Starting the poison on the enemy.
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Enemy component not found on enemy object!", other);
            }
        }
    }

    private IEnumerator ApplyPoisonEffect(ElementalEffectsEnemyManager enemy)
    {
        if (isPoisoned) yield break;

        isPoisoned = true;
        enemy.StartPoisoning(poisonDuration, poisonDamage, armorReduction); // Let enemy handle the poison
        yield return new WaitForSeconds(poisonDuration);
        isPoisoned = false;
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
        DeactivatePoison();
    }

    private void DeactivatePoison()
    {
        poolMember.ReturnToPool();
    }
}