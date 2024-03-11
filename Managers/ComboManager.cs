using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // Required for Button
using TMPro;
using System.Diagnostics;

public class ComboManager : MonoBehaviour
{
    [Header("Top Screen Combo UI")]
    [SerializeField] private TMP_Text comboNameText; // TextMeshPro text for the combo name
    [SerializeField] private GameObject comboDetailsPanelTop; // The panel on top screen for combo details

    [Header("Menu Combo Details UI")]
    [SerializeField] private GameObject comboDetailsPanel; // The panel in the menu
    [SerializeField] private TMP_Text comboDetailsTitleText; // TextMeshPro text for "COMBO!"
    [SerializeField] private TMP_Text comboDetailsNameText; // TextMeshPro text for Name
    [SerializeField] private TMP_Text comboDetailsDescriptionText; // TextMeshPro text for combo description
    [SerializeField] private Button comboButton; // The 'combo' button in the menu
    public List<ComboData> allComboData; // Assign in inspector with all ComboData SOs
    [SerializeField] private DataContainer dataContainer; // Reference to the DataContainer for saving/loading

    public List<WeaponData> weaponDataList = new List<WeaponData>();
    //
    private SpikeBallCombo spikeBallScript;
    private BombDropCombo bombDropComboScript;
    private WeaponManager weaponControl;
    private EngineerMechaCombo engineerMechaComboScript;
    private TotemSpiritCombo totemSpiritComboScript;
    private EngineerMeleeCombo engineerMeleeComboScript;
    private EngineerExplosivesCombo engineerExplosivesComboScript;
    private EngineerMagicCombo engineerMagicComboScript;
    private EngineerRangedCombo engineerRangedComboScript;
    private EngineerSummonerCombo engineerSummonerComboScript;
    private FourDifferentElementCombo fourDifferentElementCombo;
    private ComboRangedMeleeShotgun comboRangedMeleeShotgun;
    private MechaGauntletCombo mechaGauntletCombo;
    private MagicTargetingCombo magicTargetingCombo;
    private SummonerSpiritCombo comboSummonerSpirit;
    private ChakramCombo chakramCombo;
    private LaserCombo laserCombo;
    private AmmoPixieCombo ammoPixieCombo;
    private CatSniperCombo catSniperCombo;
    private KangarooBoxerCombo kangarooBoxerCombo;
    private WhiteTigerCombo whiteTigerCombo;
    private RobogoblinCombo robogoblinCombo;
    private ZuluMaskCombo zuluMaskCombo;
    private FoxBomberCombo foxBomberCombo;
    private BlacksmithRamCombo blacksmithRamCombo;
    private GrenadeDemonCombo grenadeDemonCombo;
    private ManaCreatureCombo manaCreatureCombo;
    private GravityHoleCombo gravityHoleCombo;
    private ShrapnelGrenadeCombo shrapnelGrenadeCombo;
    private ClaymoreMineCombo claymoreMineCombo;
    private FireBombCombo fireBombCombo;
    private RocketHammerCombo rocketHammerCombo;
    private CirclingSpiritsCombo circlingSpiritsCombo;
    private ExplosiveMagicCombo explosiveMagicCombo;
    private GrenadeLauncherCombo grenadeLauncherCombo;
    private MeteorTotemCombo meteorTotemCombo;
    private HunterSpiritCombo hunterSpiritCombo;
    private ExplodingPlantsCombo explodingPlantsCombo;
    private TauntingTotemCombo tauntingTotemCombo;
    //
    private void Awake()
    {
        InitializeComboScripts();
    }

    private void InitializeComboScripts()
    {
        spikeBallScript = GetComponent<SpikeBallCombo>();
        bombDropComboScript = GetComponent<BombDropCombo>();
        weaponControl = GetComponent<WeaponManager>();
        engineerMechaComboScript = GetComponent<EngineerMechaCombo>();
        totemSpiritComboScript = GetComponent<TotemSpiritCombo>();
        engineerMeleeComboScript = GetComponent<EngineerMeleeCombo>();
        engineerExplosivesComboScript = GetComponent<EngineerExplosivesCombo>();
        engineerMagicComboScript = GetComponent<EngineerMagicCombo>();
        engineerRangedComboScript = GetComponent<EngineerRangedCombo>();
        engineerSummonerComboScript = GetComponent<EngineerSummonerCombo>();
        fourDifferentElementCombo = GetComponent<FourDifferentElementCombo>();
        comboRangedMeleeShotgun = GetComponent<ComboRangedMeleeShotgun>();
        mechaGauntletCombo = GetComponent<MechaGauntletCombo>();
        magicTargetingCombo = GetComponent<MagicTargetingCombo>();
        comboSummonerSpirit = GetComponent<SummonerSpiritCombo>();
        chakramCombo = GetComponent<ChakramCombo>();
        laserCombo = GetComponent<LaserCombo>();
        ammoPixieCombo = GetComponent<AmmoPixieCombo>();
        catSniperCombo = GetComponent<CatSniperCombo>(); 
        kangarooBoxerCombo = GetComponent<KangarooBoxerCombo>(); 
        whiteTigerCombo = GetComponent<WhiteTigerCombo>(); 
        robogoblinCombo = GetComponent<RobogoblinCombo>(); 
        zuluMaskCombo = GetComponent<ZuluMaskCombo>();
        foxBomberCombo = GetComponent<FoxBomberCombo>();
        blacksmithRamCombo = GetComponent<BlacksmithRamCombo>();
        grenadeDemonCombo = GetComponent<GrenadeDemonCombo>();
        manaCreatureCombo = GetComponent<ManaCreatureCombo>();
        gravityHoleCombo = GetComponent<GravityHoleCombo>();
        shrapnelGrenadeCombo = GetComponent<ShrapnelGrenadeCombo>();
        grenadeLauncherCombo = GetComponent<GrenadeLauncherCombo>();
        claymoreMineCombo = GetComponent<ClaymoreMineCombo>();
        explosiveMagicCombo = GetComponent<ExplosiveMagicCombo>();
        circlingSpiritsCombo = GetComponent<CirclingSpiritsCombo>();
        rocketHammerCombo = GetComponent<RocketHammerCombo>();
        fireBombCombo = GetComponent<FireBombCombo>();
        meteorTotemCombo = GetComponent<MeteorTotemCombo>();
        hunterSpiritCombo = GetComponent<HunterSpiritCombo>();
        explodingPlantsCombo = GetComponent<ExplodingPlantsCombo>();
        tauntingTotemCombo = GetComponent<TauntingTotemCombo>();
    }
    public void SetWeaponDataList(List<WeaponData> weapons)
    {
        UnityEngine.Debug.Log("Setting weapons in ComboManager: " + weapons.Count);
        weaponDataList = weapons;
        ComboType combo = DetermineCombo(weapons);
        if (combo != ComboType.None)
        {
            ActivateCombo(combo);
        }
        else
        {
            HideComboOnTopScreen();
        }
    }

    private void ShowComboOnTopScreen(string comboName)
    {
        comboDetailsPanelTop.SetActive(true); // Show the top panel
        comboNameText.text = comboName;
        StartCoroutine(HideComboAfterDelay(3)); // Hide after 3 seconds
    }

    private IEnumerator HideComboAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideComboOnTopScreen();
    }
    private void HideComboOnTopScreen()
    {
        comboDetailsPanelTop.SetActive(false); // Hide the top panel
        comboNameText.text = "";
    }

    public ComboType DetermineCombo(List<WeaponData> equippedWeapons)
    {
        if (weaponDataList.Count != 4) return ComboType.None;

        int meleeCount = weaponDataList.Count(w => w.weaponType == WeaponType.Melee);
        int rangedCount = weaponDataList.Count(w => w.weaponType == WeaponType.Ranged);
        int magicCount = weaponDataList.Count(w => w.weaponType == WeaponType.Magic);
        int summonerCount = weaponDataList.Count(w => w.weaponType == WeaponType.Summoner);
        int engineerCount = weaponDataList.Count(w => w.weaponType == WeaponType.Engineer);
        int explosivesCount = weaponDataList.Count(w => w.weaponType == WeaponType.Explosives);
        int totemCount = weaponDataList.Count(w => w.weaponType == WeaponType.Totem);


        if (meleeCount == 4) return ComboType.FourMelee;
        if (rangedCount == 4) return ComboType.FourRanged;
        if (magicCount == 4) return ComboType.FourMagic;
        if (summonerCount == 4) return ComboType.FourSummoner;
        if (engineerCount == 4) return ComboType.FourEngineer;
        if (explosivesCount == 4) return ComboType.FourExplosives;
        if (totemCount == 4) return ComboType.FourTotem;
        //
        if (engineerCount == 3 && meleeCount==1) return ComboType.ThreeEngineerPlusMelee;
        if (engineerCount == 3 && explosivesCount == 1) return ComboType.ThreeEngineerPlusExplosives;
        if (engineerCount == 3 && summonerCount == 1) return ComboType.ThreeEngineerPlusSummoner;
        if (engineerCount == 3 && rangedCount == 1) return ComboType.ThreeEngineerPlusRanged;
        if (engineerCount == 3 && magicCount == 1) return ComboType.ThreeEngineerPlusMagic;
        //melee
        if (meleeCount == 3 && magicCount == 1) return ComboType.ThreeMeleePlusMagic;
        if (meleeCount == 3 && engineerCount == 1) return ComboType.ThreeMeleePlusEngineer;
        if (meleeCount == 3 && rangedCount == 1) return ComboType.ThreeMeleePlusRanged;
        if (meleeCount == 3 && summonerCount == 1) return ComboType.ThreeMeleePlusSummoner;
        if (meleeCount == 3 && explosivesCount == 1) return ComboType.ThreeMeleePlusExplosives;
        //ranged
        if (rangedCount == 3 && magicCount == 1) return ComboType.ThreeRangedPlusMagic;
        if (rangedCount == 3 && meleeCount == 1) return ComboType.ThreeRangedPlusMelee;
        if (rangedCount == 3 && engineerCount == 1) return ComboType.ThreeRangedPlusEngineer;
        if (rangedCount == 3 && summonerCount == 1) return ComboType.ThreeRangedPlusSummoner;
        if (rangedCount == 3 && explosivesCount == 1) return ComboType.ThreeRangedPlusExplosives;
        //magic
        if (magicCount == 3 && engineerCount == 1) return ComboType.ThreeMagicPlusEngineer;
        if (magicCount == 3 && explosivesCount == 1) return ComboType.ThreeMagicPlusExplosives;
        if (magicCount == 3 && rangedCount == 1) return ComboType.ThreeMagicPlusRanged;
        if (magicCount == 3 && summonerCount == 1) return ComboType.ThreeMagicPlusSummoner;
        //summoner
        if (summonerCount == 3 && meleeCount == 1) return ComboType.ThreeSummonerPlusMelee;
        if (summonerCount == 3 && explosivesCount == 1) return ComboType.ThreeSummonerPlusExplosives;
        if (summonerCount == 3 && engineerCount == 1) return ComboType.ThreeSummonerPlusEngineer;
        if (summonerCount == 3 && rangedCount == 1) return ComboType.ThreeSummonerPlusRanged;
        if (summonerCount == 3 && magicCount == 1) return ComboType.ThreeSummonerPlusMagic;
        if (summonerCount == 3 && totemCount == 1) return ComboType.ThreeSummonerPlusTotem;
        //explosives
        if (explosivesCount == 3 && magicCount == 1) return ComboType.ThreeExplosivesPlusMagic;
        if (explosivesCount == 3 && meleeCount == 1) return ComboType.ThreeExplosivesPlusMelee;
        if (explosivesCount == 3 && rangedCount == 1) return ComboType.ThreeExplosivesPlusRanged;
        if (explosivesCount == 3 && summonerCount == 1) return ComboType.ThreeExplosivesPlusSummoner;
        //totems
        if (totemCount == 3 && magicCount == 1) return ComboType.ThreeTotemPlusMagic;
        if (totemCount == 3 && meleeCount == 1) return ComboType.ThreeTotemPlusMelee;
        if (totemCount == 3 && explosivesCount == 1) return ComboType.ThreeTotemPlusExplosives;
        if (totemCount == 3 && rangedCount == 1) return ComboType.ThreeTotemPlusRanged;

        //
        List<ElementType> elements = equippedWeapons.Select(w => w.stats.elementType).ToList();

        if (elements.Distinct().Count() == 1 && elements[0] != ElementType.Normal)
        {
            return ComboType.FourSameElemental; // All four weapons have the same element
        }

        if (elements.Distinct().Count() == 4 && elements[0] != ElementType.Normal)
        {
            return ComboType.FourDifferentElemental; // All four weapons have different elements
        }
        // ... continue with similar checks for other combo types

        return ComboType.None;
    }
    public void DiscoverCombo(ComboType comboType)
    {
        // Find the ComboData for the discovered combo.
        var comboData = allComboData.Find(c => c.comboType == comboType);
        if (comboData != null && !comboData.isDiscovered)
        {
            comboData.isDiscovered = true;
            // Add the discovered combo to the DataContainer's list
            dataContainer.AddDiscoveredCombo(comboType);
            // Logic to update the menu can go here if needed
        }
    }
    public void ActivateCombo(ComboType comboType)
    {
        

        ComboData comboData = allComboData.Find(data => data.comboType == comboType);
        if (comboData == null)
        {
            UnityEngine.Debug.LogError($"No ComboData found for combo type: {comboType}");
            return;
        }

        // Now use comboData to activate combos
        string comboName = comboData.comboName;
        string comboDescription = comboData.comboDescription;
        DiscoverCombo(comboType);

        switch (comboType)
        {
            case ComboType.FourMelee:
                spikeBallScript.SetComboActiveState(true);

                break;
            case ComboType.FourRanged:
                // Enable rapid fire - assuming there is a method in your WeaponManager script
                if (weaponControl != null)
                {
                    // Activate the ranged combo
                    weaponControl.ActivateRangedCombo(true);
                }
                break;
            case ComboType.FourExplosives:
                bombDropComboScript.SetExplosiveComboActive(true);
                break;
            case ComboType.FourEngineer:
                if (engineerMechaComboScript != null)
                {
                    engineerMechaComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
                }
                break;
            case ComboType.FourTotem:
                if (totemSpiritComboScript != null)
                {
                    totemSpiritComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("TotemSpiritCombo script not found on this object.");
                }
                break;
            case ComboType.ThreeEngineerPlusMelee:
                if (engineerMeleeComboScript != null)
                {
                    engineerMeleeComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerMeleeCombo script not found on this object.");
                }
                break;
            case ComboType.ThreeEngineerPlusExplosives:
                if (engineerExplosivesComboScript != null)
                {
                    engineerExplosivesComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerExplosiveCombo script not found on this object.");
                }
                break;
            case ComboType.ThreeEngineerPlusRanged:
                if (engineerRangedComboScript != null)
                {
                    engineerRangedComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerRangedCombo script not found on this object.");
                }
                break;
            case ComboType.ThreeEngineerPlusMagic:
                if (engineerMagicComboScript != null)
                {
                    engineerMagicComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerMagicCombo script not found on this object.");
                }
                break;
            case ComboType.ThreeEngineerPlusSummoner:
                if (engineerSummonerComboScript != null)
                {
                    engineerSummonerComboScript.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("EngineerSummonerCombo script not found on this object.");
                }
                break;
            case ComboType.FourMagic:
                foreach (var weaponData in weaponDataList)
                {
                    // Find the WeaponBase component associated with this WeaponData
                    WeaponBase weaponBase = FindWeaponBaseByData(weaponData);
                    if (weaponBase != null)
                    {
                        weaponBase.isComboActive = true;   // Set the combo flag to true
                    }
                }
                break;
            case ComboType.FourDifferentElemental:
                if (fourDifferentElementCombo!= null)
                {
                    fourDifferentElementCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("4 diff ele script not found on this object.");
                }
                break;
            case ComboType.ThreeRangedPlusMelee:
                if (comboRangedMeleeShotgun != null)
                {
                    comboRangedMeleeShotgun.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("shotgun script not found on this object.");
                }
                break;
            case ComboType.ThreeRangedPlusMagic:
                foreach (var weaponData in weaponDataList)
                {
                    // Find the WeaponBase component associated with this WeaponData
                    WeaponBase weaponBase = FindWeaponBaseByData(weaponData);
                    if (weaponBase != null)
                    {
                        weaponBase.isTargetingComboActivated = true;   // Set the combo flag to true
                        magicTargetingCombo.SetComboActiveState(true);
                    }
                }
                break;
            case ComboType.ThreeRangedPlusSummoner:
                if (ammoPixieCombo != null)
                {
                    ammoPixieCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("ammo pixie component not found!");
                }
                break;
            case ComboType.ThreeRangedPlusEngineer:
                if (laserCombo != null)
                {
                    laserCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("laser combo component not found!");
                }
                break;
            case ComboType.ThreeRangedPlusExplosives:
                if (grenadeLauncherCombo != null)
                {
                    grenadeLauncherCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("grenade launcher component not found!");
                }
                break;
            case ComboType.FourSummoner:
                SummonerSpiritCombo comboSummonerSpirit = GetComponent<SummonerSpiritCombo>();
                if (comboSummonerSpirit != null)
                {
                    comboSummonerSpirit.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("SummonerSpiritCombo component not found!");
                }
                break;
            case ComboType.ThreeMeleePlusEngineer:
                if (mechaGauntletCombo != null)
                {
                    mechaGauntletCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("mecha gauntlet script not found on this object.");
                }
                break;
            case ComboType.ThreeMeleePlusSummoner:
                if (blacksmithRamCombo != null)
                {
                    blacksmithRamCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("blacksmith ram script not found on this object.");
                }
                break;
            case ComboType.ThreeMeleePlusExplosives:
                if (rocketHammerCombo != null)
                {
                    rocketHammerCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("rocket hammer o script not found on this object.");
                }
                break;
            case ComboType.ThreeMeleePlusRanged:
                if (chakramCombo != null)
                {
                    chakramCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("chakram script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusMelee:
                if (kangarooBoxerCombo != null)
                {
                    kangarooBoxerCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("kangaroo script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusRanged:
                if (catSniperCombo != null)
                {
                    catSniperCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("cat sniper script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusEngineer:
                if (robogoblinCombo != null)
                {
                    robogoblinCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("robogoblin script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusMagic:
                if (whiteTigerCombo != null)
                {
                    whiteTigerCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("whitetiger script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusExplosives:
                if (foxBomberCombo != null)
                {
                    foxBomberCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("foxbomber script not found on this object.");
                }
                break;
            case ComboType.ThreeSummonerPlusTotem:
                if (zuluMaskCombo != null)
                {
                    zuluMaskCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("zulumask script not found on this object.");
                }
                break;
            case ComboType.ThreeExplosivesPlusSummoner:
                if (grenadeDemonCombo != null)
                {
                    grenadeDemonCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("grenade demon script not found on this object.");
                }
                break;
            case ComboType.ThreeExplosivesPlusMelee:
                if (claymoreMineCombo != null)
                {
                    claymoreMineCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("claymore mine script not found on this object.");
                }
                break;
            case ComboType.ThreeExplosivesPlusRanged:
                if (shrapnelGrenadeCombo != null)
                {
                    shrapnelGrenadeCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("shrapnel script not found on this object.");
                }
                break;
            case ComboType.ThreeMagicPlusEngineer:
                if (gravityHoleCombo != null)
                {
                    gravityHoleCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("gravity hole script not found on this object.");
                }
                break;
            case ComboType.ThreeMagicPlusSummoner:
                if (manaCreatureCombo != null)
                {
                    manaCreatureCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("mana creature script not found on this object.");
                }
                break;
            case ComboType.ThreeMagicPlusExplosives:
                if (explosiveMagicCombo != null)
                {
                    explosiveMagicCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("explosive magic script not found on this object.");
                }
                break;
            case ComboType.ThreeMagicPlusMelee:
                if (circlingSpiritsCombo!= null)
                {
                    circlingSpiritsCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("circling spirit script not found on this object.");
                }
                break;
            case ComboType.ThreeMagicPlusRanged:
                if (fireBombCombo != null)
                {
                    fireBombCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("firebomb script not found on this object.");
                }
                break;
            //totems
            case ComboType.ThreeTotemPlusRanged:
                if (hunterSpiritCombo != null)
                {
                    hunterSpiritCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("firebomb script not found on this object.");
                }
                break;
            case ComboType.ThreeTotemPlusMelee:
                if (tauntingTotemCombo != null)
                {
                    tauntingTotemCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("firebomb script not found on this object.");
                }
                break;
            case ComboType.ThreeTotemPlusMagic:
                if (meteorTotemCombo != null)
                {
                    meteorTotemCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("firebomb script not found on this object.");
                }
                break;
            case ComboType.ThreeTotemPlusExplosives:
                if (explodingPlantsCombo != null)
                {
                    explodingPlantsCombo.SetComboActiveState(true);
                }
                else
                {
                    UnityEngine.Debug.LogError("firebomb script not found on this object.");
                }
                break;
            default:
                HideComboOnTopScreen();
                comboButton.interactable = false;
                comboName = ""; // Reset combo name
                comboDescription = ""; // Reset combo description
                break;
        }

        if (!string.IsNullOrEmpty(comboName))
        {
            comboButton.interactable = true;
            comboDetailsNameText.text = comboName;
            comboDetailsDescriptionText.text = comboDescription; // Set the combo description
            ShowComboOnTopScreen(comboName);
        }
    }

    // Method to find the WeaponBase by WeaponData
    private WeaponBase FindWeaponBaseByData(WeaponData weaponData)
    {
        // You need to implement this method based on how your game associates WeaponData with WeaponBase
        // This is just a placeholder example
        foreach (var weaponBase in FindObjectsOfType<WeaponBase>())
        {
            if (weaponBase.weaponData == weaponData)
            {
                return weaponBase;
            }
        }
        return null;
    }
    public void ResetComboState()
    {
        // Reset all combos
        // Assuming you have a method to deactivate each combo
        DeactivateAllCombos();

        // Reset UI elements
        comboDetailsNameText.text = "";
        comboDetailsDescriptionText.text = "";
        comboButton.interactable = false;
        HideComboOnTopScreen();
    }

    private void DeactivateAllCombos()
    {
        // Add logic to deactivate all combos here

        if (spikeBallScript != null)
        {
            spikeBallScript.SetComboActiveState(false);
        }

            if (weaponControl != null)
            {
                // Activate the ranged combo
                weaponControl.ActivateRangedCombo(false);
            }

            bombDropComboScript.SetExplosiveComboActive(false);

            if (engineerMechaComboScript != null)
            {
                engineerMechaComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (totemSpiritComboScript != null)
            {
                totemSpiritComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("TotemSpiritCombo script not found on this object.");
            }
            if (engineerMeleeComboScript != null)
            {
                engineerMeleeComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (engineerExplosivesComboScript != null)
            {
                engineerExplosivesComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (engineerRangedComboScript != null)
            {
                engineerRangedComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (engineerMagicComboScript != null)
            {
                engineerMagicComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (engineerSummonerComboScript != null)
            {
                engineerSummonerComboScript.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            foreach (var weaponData in weaponDataList)
            {
                // Find the WeaponBase component associated with this WeaponData
                WeaponBase weaponBase = FindWeaponBaseByData(weaponData);
                if (weaponBase != null)
                {
                    weaponBase.isComboActive = false;   // Set the combo flag to true
                }
            }
            if (fourDifferentElementCombo != null)
            {
                fourDifferentElementCombo.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
            if (comboRangedMeleeShotgun != null)
            {
                comboRangedMeleeShotgun.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }

            foreach (var weaponData in weaponDataList)
            {
                // Find the WeaponBase component associated with this WeaponData
                WeaponBase weaponBase = FindWeaponBaseByData(weaponData);
                if (weaponBase != null)
                {
                    weaponBase.isTargetingComboActivated = false;   // Set the combo flag to true
                    magicTargetingCombo.SetComboActiveState(false);
            }
            }
            if (comboSummonerSpirit != null)
            {
                comboSummonerSpirit.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("SummonerSpiritCombo component not found!");
            }
            if (mechaGauntletCombo != null)
            {
                mechaGauntletCombo.SetComboActiveState(false);
            }
            else
            {
                UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
            }
        //laser
        if (laserCombo != null)
        {
            laserCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("LaserCombo script not found on this object.");
        }
        //
        if (kangarooBoxerCombo != null)
        {
            kangarooBoxerCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("KangarooCombo script not found on this object.");
        }
        //
        if (catSniperCombo != null)
        {
            catSniperCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("CatSniperCombo script not found on this object.");
        }
        //
        if (whiteTigerCombo != null)
        {
            whiteTigerCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("WhiteTigerCombo script not found on this object.");
        }
        //
        if (robogoblinCombo != null)
        {
            robogoblinCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("RobogoblinCombo script not found on this object.");
        }
        //
        if (zuluMaskCombo != null)
        {
            zuluMaskCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("ZuluCombo script not found on this object.");
        }
        //
        if (foxBomberCombo != null)
        {
            foxBomberCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("FoxBomberCombo script not found on this object.");
        }
        //
        if (chakramCombo != null)
        {
            chakramCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("ChakramCombo script not found on this object.");
        }
        //
        if (gravityHoleCombo != null)
        {
            gravityHoleCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }

        if (manaCreatureCombo != null)
        {
            manaCreatureCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (blacksmithRamCombo != null)
        {
            blacksmithRamCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (grenadeDemonCombo != null)
        {
            grenadeDemonCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (claymoreMineCombo != null)
        {
            claymoreMineCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (shrapnelGrenadeCombo != null)
        {
            shrapnelGrenadeCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (explosiveMagicCombo != null)
        {
            explosiveMagicCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (circlingSpiritsCombo != null)
        {
            circlingSpiritsCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (rocketHammerCombo != null)
        {
            rocketHammerCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //
        if (grenadeLauncherCombo != null)
        {
            grenadeLauncherCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("SummonerSpiritCombo component not found!");
        }
        //
        if (fireBombCombo != null)
        {
            fireBombCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("EngineerMechaCombo script not found on this object.");
        }
        //      
        if (explodingPlantsCombo != null)
        {
            explodingPlantsCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("firebomb script not found on this object.");
        }
        //
        if (meteorTotemCombo != null)
        {
            meteorTotemCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("firebomb script not found on this object.");
        }
        //      
        if (tauntingTotemCombo != null)
        {
            tauntingTotemCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("firebomb script not found on this object.");
        }
        //
        if (hunterSpiritCombo != null)
        {
            hunterSpiritCombo.SetComboActiveState(false);
        }
        else
        {
            UnityEngine.Debug.LogError("firebomb script not found on this object.");
        }
    }

}

public enum ComboType
{
    None, // Default, no combo
    FourMelee,
    FourRanged,
    FourMagic,
    FourSummoner,
    FourEngineer,
    FourExplosives,
    FourTotem,
    //
    FourSameElemental, //elemental purity
    FourDifferentElemental, // Jack of All Elemental Trades
    //
    ThreeSummonerPlusMelee,
    ThreeSummonerPlusRanged,
    ThreeSummonerPlusMagic,
    ThreeSummonerPlusExplosives,
    ThreeSummonerPlusEngineer,
    ThreeSummonerPlusTotem,
    //
    ThreeMeleePlusRanged,
    ThreeMeleePlusMagic,
    ThreeMeleePlusSummoner,
    ThreeMeleePlusEngineer,
    ThreeMeleePlusExplosives,
    ThreeMeleePlusTotem,
    //
    ThreeEngineerPlusMelee,
    ThreeEngineerPlusRanged,
    ThreeEngineerPlusMagic,
    ThreeEngineerPlusExplosives,
    ThreeEngineerPlusSummoner,
    ThreeEngineerPlusTotem,
    //
    ThreeRangedPlusMelee,
    ThreeRangedPlusMagic,
    ThreeRangedPlusSummoner,
    ThreeRangedPlusEngineer,
    ThreeRangedPlusExplosives,
    ThreeRangedPlusTotem,
    //
    ThreeMagicPlusMelee,
    ThreeMagicPlusRanged,
    ThreeMagicPlusSummoner,
    ThreeMagicPlusExplosives,
    ThreeMagicPlusEngineer,
    ThreeMagicPlusTotem,
    //
    ThreeExplosivesPlusMelee,
    ThreeExplosivesPlusRanged,
    ThreeExplosivesPlusMagic,
    ThreeExplosivesPlusSummoner,
    ThreeExplosivesPlusEngineer,
    ThreeExplosivesPlusTotem,
    //
    ThreeTotemPlusMagic,
    ThreeTotemPlusExplosives,
    ThreeTotemPlusMelee,
    ThreeTotemPlusRanged,
    ThreeTotemPlusEngineer,
    ThreeTotemPlusSummoner,
}