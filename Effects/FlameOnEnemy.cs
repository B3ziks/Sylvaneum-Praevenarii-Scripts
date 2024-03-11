using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class FlameOnEnemy : MonoBehaviour, IPoolMember, IEffect
{
    public int damage;
    public float cooldown = 1f; // Cooldown between damage applications.
    public int burningDamage = 5; // Damage per second while burning.
    public float burningDuration = 3f; // Duration of the burning effect.
    public float ttl = 10f; // Time to live for flames in seconds.
    PoolMember poolMember;
    private bool isCooldown;
    private bool _isBurning;
    public ElementType elementType; // This should be set to the element type of the weapon that spawned this flame

    public bool isBurning
    {
        get { return _isBurning; }
        private set
        {
            if (_isBurning != value) // Only trigger if the state actually changes
            {
                _isBurning = value;
                OnEffectStateChanged.Invoke(_isBurning);
            }
        }
    }

    // Implement the IEffect interface
    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isBurning;

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
            ApplyEffectToEnemy(other);
            StartCoroutine(StartCooldown());
        }
    }

    private void ApplyEffectToEnemy(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        ElementalEffectsEnemyManager element = other.GetComponent<ElementalEffectsEnemyManager>();
        if (enemy != null && element != null)
        {
            enemy.TakeDamage(damage);
            element.StartBurning(burningDuration, burningDamage); // Starting the burning on the enemy.
            PostDamage(damage, enemy.transform.position); // Call PostDamage to show the damage message
        }
        else
        {
            UnityEngine.Debug.LogError("Enemy or ElementalEffectsEnemyManager component not found on enemy object!", other);
        }
    }
    private IEnumerator TTLRoutine()
    {
        yield return new WaitForSeconds(ttl);
        DeactivateFlame();
    }

    private void DeactivateFlame()
    {
        poolMember.ReturnToPool();
    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }
    //
    public void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(elementType);
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
}