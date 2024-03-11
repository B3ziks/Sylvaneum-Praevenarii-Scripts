using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerSpiritCombo : MonoBehaviour
{
    public PoolObjectData creaturePoolData; // Assign the prefab data for the creature
    public AudioClip roarSound; // Assign the roar sound clip
    public float comboRadius = 5f; // The radius within which the combo takes effect
    public int damageAmount = 100; // The amount of damage to deal
    public float buffDuration = 10f; // How long the buff lasts
    public float damageBuffMultiplier = 1.5f; // Multiplier to the damage
    public float attackSpeedBuffMultiplier = 0.8f; // Multiplier to the attack speed (less than 1 to increase speed)
    public float comboCooldown = 10f; // Cooldown duration in seconds
    public bool isComboActive = false;
    private PoolManager poolManager;
    public AudioSource audioSource;
    private bool isInCooldown = false;

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }
    private void Start()
    {
       // audioSource = GetComponent<AudioSource>(); // Get the audio source component
        if (audioSource == null)
        {
            // If AudioSource doesn't exist, add one
           // audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Check if the combo is active, if "R" is pressed, and if the cooldown has elapsed
        if (isComboActive && Input.GetKeyDown(KeyCode.R) && !isInCooldown)
        {
            StartCoroutine(PerformCombo());
        }
    }

    public void SetComboActiveState(bool active)
    {
        isComboActive = active; // Set the combo as active/inactive.
    }

    private IEnumerator PerformCombo()
    {
        isInCooldown = true;

        // Instantiate the creature prefab at the summoner's position
        GameObject creature = poolManager.GetObject(creaturePoolData);
        creature.transform.position = transform.position;
        creature.SetActive(true); // Make sure the creature is active


            audioSource.PlayOneShot(roarSound);

        // Deal damage in the radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, comboRadius);
        foreach (Collider2D collider in hitColliders)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
            }
        }

        // Apply buffs to summons
        SummonComboBuff[] allSummons = FindObjectsOfType<SummonComboBuff>(); // Assuming you have a Summon script attached to your summons
        foreach (SummonComboBuff summon in allSummons)
        {
            summon.ApplyBuff(damageBuffMultiplier, attackSpeedBuffMultiplier, buffDuration);
        }

        // Wait for 3 seconds before hiding the creature
        yield return new WaitForSeconds(3f);
        creature.SetActive(false);

        yield return new WaitForSeconds(comboCooldown); // Adjust the remaining cooldown time
        isInCooldown = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the combo's effect radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, comboRadius);
    }
}