using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IceOnEnemy : MonoBehaviour, IPoolMember, IEffect
{
    public int damage;
    public float cooldown = 1f; // Cooldown between ice applications.
    public float slowDuration = 3f; // Duration of the slow effect.
    public float movementSpeedReduction = 0.5f; // The amount of speed reduction while slowed.
    public float ttl = 10f; // Time to live for ice effect in seconds.
    PoolMember poolMember;
    private bool isCooldown;
    private bool _isSlowed;

    public bool isSlowed
    {
        get { return _isSlowed; }
        private set
        {
            if (_isSlowed != value) // Only trigger if the state actually changes
            {
                _isSlowed = value;
                OnEffectStateChanged.Invoke(_isSlowed);
            }
        }
    }

    // Implement the IEffect interface
    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isSlowed;

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
                element.StartSlowing(slowDuration, 0, movementSpeedReduction); // Starting the slow on the enemy.
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Enemy component not found on enemy object!", other);
            }
        }
    }

    private IEnumerator ApplyIceEffect(ElementalEffectsEnemyManager enemy)
    {
        if (isSlowed) yield break;

        isSlowed = true;
        enemy.StartSlowing(slowDuration, 0, movementSpeedReduction); // Let enemy handle the slow
        yield return new WaitForSeconds(slowDuration);
        isSlowed = false;
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
        DeactivateIce();
    }

    private void DeactivateIce()
    {
        poolMember.ReturnToPool();
    }
}