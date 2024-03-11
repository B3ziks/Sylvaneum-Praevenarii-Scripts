using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Base Stats
    public int baseMaxHp = 100;
    public float baseHpRegenerationRate = 3f;
    public int baseArmor = 0;
    public float baseMovementSpeed = 3f;
    public float baseGoldGain = 1f;

    // Current Stats (including upgrades)
    public int maxHp = 100;
    public int currentHp = 100;
    public int armor = 0;
    public float hpRegenerationRate = 3f;
    public float movementSpeed;
    public float goldGain;

    public float hpRegenerationTimer =1f;



    public bool IsRooted { get; set; }

    [HideInInspector] public Level level;
    [HideInInspector] public Coins coins;

    private bool isDead;

    public GameObject hpRegenSlowPrefab;
    public GameObject movementSlowPrefab;
    public GameObject reduceArmorPrefab;
    private Dictionary<IEffect, GameObject> activeEffects = new Dictionary<IEffect, GameObject>();
    // Additional References for the effects and UI elements
    public Transform effectsUI; // Drag your EffectsUI object here in the Inspector.
  //  public GameObject burningEffectPrefab;
 //   public GameObject poisonEffectPrefab;
    public GameObject magicBarrierActivePrefab;
    public GameObject magicBarrierDisabledPrefab;
    [SerializeField] GameObject magicBarrier; // Reference to your MagicBarrier GameObject.
    public bool barrierActive;
    [SerializeField] float barrierCooldown = 5f; // Time in seconds before the barrier can be reactivated.

    private int originalMovementSpeed;
    // Additional Dictionary to track instantiated effect objects
    private Dictionary<string, GameObject> instantiatedEffects = new Dictionary<string, GameObject>();
    //debuffs aura
    private List<DebuffAuras> activeDebuffs = new List<DebuffAuras>();
    //private GameRestartManager restartManager;
    private int normalArmor; // To store the regular armor value.

    [SerializeField] DataContainer dataContainer;
    // [SerializeField] CharacterData selectedCharacter;
    [SerializeField] private float invincibilityDuration = 0.25f; // Duration of invincibility in seconds
    private float invincibilityTimer = 0; // Timer to track invincibility frames


    private void Awake()
    {
        level = GetComponent<Level>();
        coins = GetComponent<Coins>();
        // Ensure magicBarrier is not null before using it.
        if (magicBarrier != null)
        {
            magicBarrier.SetActive(false); // Ensuring it's hidden by default.
        }

        movementSpeed = baseMovementSpeed;
      //  restartManager = FindObjectOfType<GameRestartManager>();
        normalArmor = baseArmor; // Set to base armor initially.


    }

    private void Start()
    {

        LoadSelectedCharacter(dataContainer.selectedCharacter);
        ApplyPersistentUpgrades();
        //
        normalArmor = armor; // Update to the modified armor including upgrades and traits.

        originalMovementSpeed = (int)(movementSpeed);

        UpdateBarrierEffectUI(barrierActive);
        currentHp = maxHp;

    }

    private void LoadSelectedCharacter(CharacterData selectedCharacter)
    {
        InitAnimation(selectedCharacter.spritePrefab);
        GetComponent<WeaponManager>().AddWeapon(selectedCharacter.startingWeapon);

        ApplyCharacterTraits(selectedCharacter.traits);

    }

 
    private void ApplyCharacterTraits(CharacterTraits traits)
    {
        Debug.Log($"Applying Traits - Extra HP: {traits.extraHp}, Extra Armor: {traits.extraArmor}, Extra Speed: {traits.extraSpeed}");

        maxHp += traits.extraHp;
        armor += traits.extraArmor;
        movementSpeed += traits.extraSpeed;
        hpRegenerationRate += traits.extraHpRegen;
        //... apply other traits
    }

    private void InitAnimation(GameObject spritePrefab)
    {
        GameObject animObject = Instantiate(spritePrefab, transform);
        GetComponent<Animate>().SetAnimate(animObject);
    }

    private void ApplyPersistentUpgrades()
    {
        // Apply HP upgrade
        maxHp += (int)(dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.HP) *
                                         dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.HP));
        // Apply HPRegen upgrade
        hpRegenerationRate +=  dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.HPRegen) *
                                  dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.HPRegen);
        // Apply Armor upgrade
        armor += (int)(dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.Armor) *
                                  dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Armor));
        // Apply MovementSpeed upgrade (assuming you have a movementSpeed property)
        movementSpeed +=  dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.MovementSpeed) *
                                dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.MovementSpeed);

        // Apply GoldGain upgrade (assuming you have a method or property for gold gain)
        goldGain += 1 + dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.GoldGain) *
                    dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.GoldGain);

        // After applying all upgrades, call this to set the normal armor.
        SetNormalArmor();
    }

    private void Update()
    {
        HandleHpRegeneration();
        if (magicBarrier != null)
        {
            magicBarrier.SetActive(barrierActive);
        }
        if (GameRestartManager.IsRestarting)
        {
            armor = 999; // Temporarily set to an invincible state.
            currentHp = maxHp;
        }
        else
        {
            armor = normalArmor; // Reset to the normal armor value.
        }
        // Update the invincibility timer
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    private void HandleHpRegeneration()
    {
        hpRegenerationTimer += Time.deltaTime * hpRegenerationRate;

        if (hpRegenerationTimer > 1f)
        {
            Heal(1);
            hpRegenerationTimer -= 1f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invincibilityTimer > 0) return;

        if (barrierActive)
        {
            // Barrier takes the hit, player takes no damage.
            // Deactivate barrier and start the cooldown coroutine.
            barrierActive = false;
            StartCoroutine(BarrierCooldown());
            return;
        }

        // If we get here, the barrier is not active, so apply damage as normal.
        ApplyArmor(ref damage);
        currentHp -= damage;
        // After taking damage, start the invincibility timer
        invincibilityTimer = invincibilityDuration;
        // Post the damage message with a color
        MessageSystem.instance.PostMessage(damage.ToString(), transform.position, Color.red);

        if (currentHp <= 0)
        {
            GetComponent<CharacterGameOver>().GameOver();
            isDead = true;
        }

    }

    private void SetNormalArmor()
    {
        // Call this after applying traits and persistent upgrades to update the normal armor value.
        normalArmor = armor;
    }
    private IEnumerator BarrierCooldown()
    {
        // Before waiting, set the barrier to disabled
        UpdateBarrierEffectUI(false);
        yield return new WaitForSeconds(barrierCooldown);

        // Reactivate the barrier and set the barrier UI to active
        barrierActive = true;
        UpdateBarrierEffectUI(true);
    }
    private void UpdateBarrierEffectUI(bool active = true)
    {
        if (active)
        {
            // Instantiate active barrier UI and destroy the disabled one (if it exists)
            if (!instantiatedEffects.ContainsKey("barrierActive"))
            {
                GameObject effect = Instantiate(magicBarrierActivePrefab, effectsUI);
                instantiatedEffects["barrierActive"] = effect;
            }
            if (instantiatedEffects.ContainsKey("barrierDisabled"))
            {
                Destroy(instantiatedEffects["barrierDisabled"]);
                instantiatedEffects.Remove("barrierDisabled");
            }
        }
        else
        {
            // Instantiate disabled barrier UI and destroy the active one (if it exists)
            if (!instantiatedEffects.ContainsKey("barrierDisabled"))
            {
                GameObject effect = Instantiate(magicBarrierDisabledPrefab, effectsUI);
                instantiatedEffects["barrierDisabled"] = effect;
            }
            if (instantiatedEffects.ContainsKey("barrierActive"))
            {
                Destroy(instantiatedEffects["barrierActive"]);
                instantiatedEffects.Remove("barrierActive");
            }
        }
    }
    private void ApplyArmor(ref int damage)
    {
        damage -= armor;
        if (damage < 0) { damage = 0; }
    }

    public void Heal(int amount)
    {
        if (currentHp <= 0)
        {
            return;
        }
        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

   
  
    //debuffs
    public void ApplyDebuff(DebuffAuras buffType)
    {
        if (activeDebuffs.Contains(buffType)) // Check if buff is already active
            return; // If already active, just return

        switch (buffType)
        {
            case DebuffAuras.HPRegenerationRateSlowAura:
                hpRegenerationRate *= 0.8f; // Reducing to 80% for example
                activeDebuffs.Add(buffType); 

                // Instantiate the UI effect
                if (!instantiatedEffects.ContainsKey("hpRegenSlow"))
                {
                    GameObject effect = Instantiate(hpRegenSlowPrefab, effectsUI);
                    instantiatedEffects["hpRegenSlow"] = effect;
                }
                break;

            case DebuffAuras.MovementSlowAura:
                movementSpeed = (int)(movementSpeed * 0.8f); // 80% movement speed
                activeDebuffs.Add(buffType); 

                // Instantiate the UI effect
                if (!instantiatedEffects.ContainsKey("movementSlow"))
                {
                    GameObject effect = Instantiate(movementSlowPrefab, effectsUI);
                    instantiatedEffects["movementSlow"] = effect;
                }
                break;

            case DebuffAuras.ReduceArmorAura:
                armor = (int)(armor * 0.80f); // Reduce armor by 20%
                activeDebuffs.Add(buffType); 

                // Instantiate the UI effect
                if (!instantiatedEffects.ContainsKey("reduceArmor"))
                {
                    GameObject effect = Instantiate(reduceArmorPrefab, effectsUI);
                    instantiatedEffects["reduceArmor"] = effect;
                }
                break;
        }
    }

    public void RemoveDebuff(DebuffAuras buffType, float originalValue)
    {
        if (!activeDebuffs.Contains(buffType)) // Check if buff is active
            return; // If not active, just return

        switch (buffType)
        {
            case DebuffAuras.HPRegenerationRateSlowAura:
                hpRegenerationRate = originalValue;
                activeDebuffs.Remove(buffType); // Remove buff from active list

                // Destroy the UI effect
                if (instantiatedEffects.ContainsKey("hpRegenSlow"))
                {
                    Destroy(instantiatedEffects["hpRegenSlow"]);
                    instantiatedEffects.Remove("hpRegenSlow");
                }
                break;

            case DebuffAuras.MovementSlowAura:
                movementSpeed = (int)(originalMovementSpeed); // Restore the original value
                activeDebuffs.Remove(buffType); // Remove buff from active list

                // Destroy the UI effect
                if (instantiatedEffects.ContainsKey("movementSlow"))
                {
                    Destroy(instantiatedEffects["movementSlow"]);
                    instantiatedEffects.Remove("movementSlow");
                }
                break;

            case DebuffAuras.ReduceArmorAura:
                armor = (int)originalValue;
                activeDebuffs.Remove(buffType); // Remove buff from active list

                // Destroy the UI effect
                if (instantiatedEffects.ContainsKey("reduceArmor"))
                {
                    Destroy(instantiatedEffects["reduceArmor"]);
                    instantiatedEffects.Remove("reduceArmor");
                }
                break;
        }
    }

}
