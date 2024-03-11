using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class EffectData
{
    public float Duration { get; private set; }
    private float damageTimer = 0f; // Timer to track damage application intervals.
    public int DamagePerTick { get; private set; }
    public int ArmorReduction { get; private set; }
    public float MovementSpeedReduction { get; private set; }
    public float DamageReceiveIncrease { get; private set; }
    public GameObject EffectPrefab { get; private set; }
    public Transform EffectTransform { get; private set; }
    public GameObject EffectInstance { get; private set; }
    public bool IsActive { get; private set; }

    // Delegate for applying additional effect specifics.
    public Action<Enemy> ApplyAdditionalEffect { get; private set; }
    public Action<Enemy> RevertAdditionalEffect { get; private set; }

    public EffectData(GameObject prefab, Transform effectTransform)
    {
        EffectPrefab = prefab;
        EffectTransform = effectTransform;
        IsActive = false;
    }

    public void StartEffect(Enemy enemy, float duration, int damagePerTick, int armorReduction = 0, float movementSpeedReduction = 0f, float damageReceiveIncrease = 0f, Action<Enemy> apply = null, Action<Enemy> revert = null)
    {
        IsActive = true;
        Duration = duration;
        DamagePerTick = damagePerTick;
        ArmorReduction = armorReduction;
        MovementSpeedReduction = movementSpeedReduction;
        DamageReceiveIncrease = damageReceiveIncrease;
        ApplyAdditionalEffect = apply;
        RevertAdditionalEffect = revert;

        if (EffectInstance == null)
        {
            EffectInstance = GameObject.Instantiate(EffectPrefab, EffectTransform.position, Quaternion.identity, EffectTransform);
        }
        else
        {
            // Reactivate the existing effect instance and reset its position if necessary.
            EffectInstance.transform.position = EffectTransform.position;
            EffectInstance.SetActive(true);
        }

        ApplyAdditionalEffect?.Invoke(enemy); // Apply additional effects once at the start.
    }

    public void ProcessEffect(Enemy enemy)
    {
        if (IsActive && Duration > 0)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f) // Apply damage once per second.
            {
                enemy.TakeDamage(DamagePerTick);
                PostMessage(DamagePerTick, enemy.transform.position); // Post the damage message.
                damageTimer -= 1f; // Decrease the timer by one second.
            }
            Duration -= Time.deltaTime;
        }
        else
        {
            StopEffect(enemy);
        }
    }

    private void PostMessage(int damage, Vector3 position)
    {
        // Assuming you have a static instance for your message system
        if (MessageSystem.instance != null)
        {
            MessageSystem.instance.PostMessage(damage.ToString(), position);
        }
        else
        {
            UnityEngine.Debug.LogWarning("MessageSystem instance is not found. Cannot post message.");
        }
    }

    public void StopEffect(Enemy enemy)
    {
        IsActive = false;
        RevertAdditionalEffect?.Invoke(enemy); // Revert additional effects when stopping.
        if (EffectInstance != null)
        {
            // Instead of destroying, deactivate the instance for later reuse.
            EffectInstance.SetActive(false);
        }
    }
}