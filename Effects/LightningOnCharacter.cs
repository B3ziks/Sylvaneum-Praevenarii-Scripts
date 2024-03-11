using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class LightningOnCharacter : MonoBehaviour, IPoolMember, IEffect
{
    public SpecialAttack specialAttack;
    public int damage;
    public float cooldown = 5f;
    public float regenReductionAmount = 0.5f; // Reduction factor for HP regeneration.
    public float regenReductionDuration = 4f;
    public float ttl = 10f;
    PoolMember poolMember;
    private bool isCooldown;

    private bool _isShocked;
    public bool isShocked
    {
        get { return _isShocked; }
        private set
        {
            if (_isShocked != value)
            {
                _isShocked = value;
                OnEffectStateChanged.Invoke(_isShocked);
            }
        }
    }

    public event UnityAction<bool> OnEffectStateChanged = delegate { };
    public bool IsEffectActive => isShocked;

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
                effectManager.StartRegenReduction(regenReductionDuration, regenReductionAmount);
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
                effectManager.StartRegenReduction(regenReductionDuration, regenReductionAmount);
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
        DeactivateLightning();
    }

    private void DeactivateLightning()
    {
        poolMember.ReturnToPool();
    }
}