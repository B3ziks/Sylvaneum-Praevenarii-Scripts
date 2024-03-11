using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;  // Added namespace for UnityEvent.

public class FlameOnCharacter : MonoBehaviour, IPoolMember, IEffect
{
    public SpecialAttack specialAttack;
    public int damage;
    public float cooldown = 5f; // Cooldown between damage applications.
    public int burningDamage = 5; // Damage per second while burning.
    public float burningDuration = 3f; // Duration of the burning effect.
    public float ttl = 10f;  // Time to live for flames in seconds.
    PoolMember poolMember;
    private bool isCooldown;
    //
    private bool _isBurning;
    public bool isBurning
    {
        get { return _isBurning; }
        private set
        {
            if (_isBurning != value)  // Only trigger if the state actually changes
            {
                _isBurning = value;
                OnEffectStateChanged.Invoke(_isBurning);
            }
        }
    }
    //  
    // Implement the IEffect interface
    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isBurning;


    private void Awake()
    {
        if (specialAttack != null)
        {
            damage = specialAttack.damage;
        }
        else
        {
            UnityEngine.Debug.LogError("SpecialAttack reference is missing!", this);
        }
    }

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
        if (!isCooldown && other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            ElementalEffectManager effectManager = character.GetComponent<ElementalEffectManager>();
            if (character != null && effectManager != null)
            {
                character.TakeDamage(damage);
                effectManager.StartBurning(burningDuration, burningDamage); // Starting the burning on the character.
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Character component not found on player!", other);
            }
        }
    }
    private IEnumerator ApplyBurningEffect(ElementalEffectManager effectManager)
    {
        if (isBurning) yield break;

        isBurning = true;
        effectManager.StartBurning(burningDuration, burningDamage); // Let character handle the burning
        yield return new WaitForSeconds(burningDuration);
        isBurning = false;
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
        DeactivateFlame();
    }

    private void DeactivateFlame()
    {
        // TODO: Add your deactivation logic here, e.g., returning to object pool or simply deactivating.
        poolMember.ReturnToPool();
    }
}