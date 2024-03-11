using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ElementalEffectManager : MonoBehaviour
{
    private Character character; // Reference to the character this effect manager will be affecting
    public Transform effectsTransform;

    //burning
    private bool isBurning = false;
    private Coroutine burningCoroutine = null;
    private GameObject activeFireEffect;
    private bool shouldStopBurning;
    private float timeLeftToBurn;
    private float burningDamageOverTime;
    public GameObject firePrefab;
    // Ice
    private bool isSlowed = false;
    private float originalSpeed;
    private float slowAmount;
    private float timeLeftToSlow;
    public GameObject iceEffectPrefab;
    private Coroutine slowCoroutine = null;
    private GameObject activeIceEffect;
    // Lightning
    private bool isRegenReduced = false;
    private float originalRegenRate;
    private float regenReduction;
    private float timeLeftToRegenReduce;
    public GameObject lightningEffectPrefab;
    private Coroutine regenReduceCoroutine = null;
    private GameObject activeLightningEffect;
    // Poison
    private bool isPoisoned = false;
    private Coroutine poisonCoroutine = null;
    private GameObject activePoisonEffect;
    private bool shouldStopPoisoning;
    private float timeLeftToPoison;
    private float poisonDamageOverTime;
    public GameObject poisonPrefab;

    // Additional Dictionary to track instantiated effect objects
    private Dictionary<string, GameObject> instantiatedEffects = new Dictionary<string, GameObject>();
    //debuffs aura
    private List<DebuffAuras> activeDebuffs = new List<DebuffAuras>();
    private Dictionary<IEffect, GameObject> activeEffects = new Dictionary<IEffect, GameObject>();
    public GameObject burningEffectPrefab;
    public GameObject poisonEffectPrefab;
    public Transform effectsUI;

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    //fire
    public void StartBurning(float duration, float damageOverTime)
    {
        shouldStopBurning = false;
        timeLeftToBurn = duration;
        burningDamageOverTime = damageOverTime;
        if (!isBurning)
        {
            burningCoroutine = StartCoroutine(BurningEffect());
            if (!instantiatedEffects.ContainsKey("burning"))
            {
                GameObject effect = Instantiate(burningEffectPrefab, effectsUI);
                instantiatedEffects["burning"] = effect;
            }
        }
    }

    private IEnumerator BurningEffect()
    {
        isBurning = true;
        activeFireEffect = Instantiate(firePrefab, effectsTransform.position, Quaternion.identity, effectsTransform);

        while (timeLeftToBurn > 0)
        {
            yield return new WaitForSeconds(1f);
            character.TakeDamage((int)(burningDamageOverTime)); // Or any suitable damage value
            if (shouldStopBurning)
            {
                timeLeftToBurn -= 1f;
            }
        }

        if (shouldStopBurning)
        {
            Destroy(activeFireEffect);
            activeFireEffect = null;
            isBurning = false;

            if (instantiatedEffects.ContainsKey("burning"))
            {
                Destroy(instantiatedEffects["burning"]);
                instantiatedEffects.Remove("burning");
            }
        }
    }

    //poison
    public void StartPoisoning(float duration, float damageOverTime)
    {
        shouldStopPoisoning = false;
        timeLeftToPoison = duration;
        poisonDamageOverTime = damageOverTime;

        if (!isPoisoned)
        {
            poisonCoroutine = StartCoroutine(PoisonEffect());

            // Instantiating the UI effect.
            if (!instantiatedEffects.ContainsKey("poisoned"))
            {
                GameObject effect = Instantiate(poisonEffectPrefab, effectsUI);
                instantiatedEffects["poisoned"] = effect;
            }


        }
    }

    private IEnumerator PoisonEffect()
    {
        isPoisoned = true;
        activePoisonEffect = Instantiate(poisonPrefab, effectsTransform.position, Quaternion.identity, effectsTransform);

        while (timeLeftToPoison > 0)
        {
            yield return new WaitForSeconds(1f);
            character.TakeDamage((int)(poisonDamageOverTime));
            timeLeftToPoison -= 1f;  // decrementing every second
        }

        StopPoisoning();  // you can make a separate method for cleanup
    }

    private void StopPoisoning()
    {
        isPoisoned = false;
        Destroy(activePoisonEffect);
        activePoisonEffect = null;
        if (instantiatedEffects.ContainsKey("poisoned"))
        {
            Destroy(instantiatedEffects["poisoned"]);
            instantiatedEffects.Remove("poisoned");
        }
    }
    //ice
    public void StartSlowing(float duration, float slowFactor)
    {
        if (!isSlowed)
        {
            originalSpeed = character.movementSpeed; // Ensure this property exists in the Character script
            slowAmount = slowFactor;
            timeLeftToSlow = duration;
            slowCoroutine = StartCoroutine(SlowEffect());
            if (!instantiatedEffects.ContainsKey("slowed"))
            {
                GameObject effect = Instantiate(iceEffectPrefab, effectsUI);
                instantiatedEffects["slowed"] = effect;
            }
        }
    }

    private IEnumerator SlowEffect()
    {
        isSlowed = true;
        character.movementSpeed *= (1 - slowAmount); // Apply the slow effect

        while (timeLeftToSlow > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeftToSlow -= 1f;
        }

        StopSlowing(); // Ensure to reset the speed when done
    }

    public void StopSlowing()
    {
        if (isSlowed)
        {
            isSlowed = false;
            character.movementSpeed = originalSpeed; // Reset to original speed
            if (instantiatedEffects.ContainsKey("slowed"))
            {
                Destroy(instantiatedEffects["slowed"]);
                instantiatedEffects.Remove("slowed");
            }
        }
    }
    //lightning
    public void StartRegenReduction(float duration, float reduction)
    {
        if (!isRegenReduced)
        {
            originalRegenRate = character.hpRegenerationRate; // Ensure this property exists in the Character script
            regenReduction = reduction;
            timeLeftToRegenReduce = duration;
            regenReduceCoroutine = StartCoroutine(RegenReduceEffect());
            if (!instantiatedEffects.ContainsKey("regenReduced"))
            {
                GameObject effect = Instantiate(lightningEffectPrefab, effectsUI);
                instantiatedEffects["regenReduced"] = effect;
            }
        }
    }

    private IEnumerator RegenReduceEffect()
    {
        isRegenReduced = true;
        character.hpRegenerationRate -= regenReduction; // Apply the reduction effect

        while (timeLeftToRegenReduce > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeftToRegenReduce -= 1f;
        }

        StopRegenReduction(); // Ensure to reset the HP regen rate when done
    }

    public void StopRegenReduction()
    {
        if (isRegenReduced)
        {
            isRegenReduced = false;
            character.hpRegenerationRate = originalRegenRate; // Reset to original regen rate
            if (instantiatedEffects.ContainsKey("regenReduced"))
            {
                Destroy(instantiatedEffects["regenReduced"]);
                instantiatedEffects.Remove("regenReduced");
            }
        }
    }
    //
    public void ApplyElementalEffect(ElementType elementType, float duration, float damageOverTime)
    {
       // UnityEngine.Debug.Log($"Applying Elemental Effect: ElementType = {elementType}, Duration = {duration}, DamageOverTime = {damageOverTime}");

        switch (elementType)
        {
            case ElementType.Fire:
                StartBurning(duration, damageOverTime);
                break;
            case ElementType.Poison:
                StartPoisoning(duration, damageOverTime);
                break;
            case ElementType.Ice:
                StartSlowing(duration, slowAmount);
                break;
            case ElementType.Lightning:
                StartRegenReduction(duration, regenReduction);
                break;
                // ... handle other elemental types
        }
    }
    // Adjust OnTriggerStay2D and OnTriggerExit2D
    private void OnTriggerStay2D(Collider2D other)
    {
        FlameOnCharacter flame = other.gameObject.GetComponent<FlameOnCharacter>();
        PoisonOnCharacter poison = other.gameObject.GetComponent<PoisonOnCharacter>();
        IceOnCharacter ice = other.gameObject.GetComponent<IceOnCharacter>();
        LightningOnCharacter lightning = other.gameObject.GetComponent<LightningOnCharacter>();

        if (other.CompareTag("Fire"))
        {
            StartBurning(flame.burningDuration, burningDamageOverTime);
        }
        else if (other.CompareTag("Poison"))
        {
            StartPoisoning(poison.poisonDuration, poisonDamageOverTime);
        }
        else if (other.CompareTag("Ice"))
        {
            StartSlowing(ice.slowDuration, ice.slowAmount);
        }
        else if (other.CompareTag("Lightning"))
        {
            StartRegenReduction(lightning.regenReductionDuration, lightning.regenReductionAmount);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            shouldStopBurning = true;
        }
        else if (other.CompareTag("Poison"))
        {
            shouldStopPoisoning = true;
        }

    }
}