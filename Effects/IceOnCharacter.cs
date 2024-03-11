using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class IceOnCharacter : MonoBehaviour, IPoolMember, IEffect
{
    public SpecialAttack specialAttack;
    public int damage;
    public float cooldown = 5f;
    public float slowAmount = 0.5f; // This is the factor by which the character's speed is reduced.
    public float slowDuration = 4f;
    public float ttl = 10f;
    PoolMember poolMember;
    private bool isCooldown;

    private bool _isSlowed;
    public bool isSlowed
    {
        get { return _isSlowed; }
        private set
        {
            if (_isSlowed != value)
            {
                _isSlowed = value;
                OnEffectStateChanged.Invoke(_isSlowed);
            }
        }
    }

    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isSlowed;

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
                effectManager.StartSlowing(slowDuration, slowAmount);
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Character component not found on player!", other);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isCooldown && other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            ElementalEffectManager effectManager = character.GetComponent<ElementalEffectManager>();
            if (character != null)
            {
                character.TakeDamage(damage);
                effectManager.StartSlowing(slowDuration, slowAmount);
                StartCoroutine(StartCooldown());
            }
            else
            {
                UnityEngine.Debug.LogError("Character component not found on player!", other);
            }
        }
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