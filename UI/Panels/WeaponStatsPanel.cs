using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponStatsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI timeToAttackText;
    [SerializeField] private TextMeshProUGUI numOfHitsText;
    [SerializeField] private TextMeshProUGUI numOfAttacksText;
    [SerializeField] private TextMeshProUGUI projectileSpeedText;
    [SerializeField] private TextMeshProUGUI elementTypeText;
    [SerializeField] private TextMeshProUGUI elementalPotencyText;
    [SerializeField] private TextMeshProUGUI damageOverTimeText;
    [SerializeField] private TextMeshProUGUI critChanceText;
    [SerializeField] private TextMeshProUGUI critMultiplierText;
    [SerializeField] private TextMeshProUGUI maxSummonsText;
    [SerializeField] private TextMeshProUGUI maxRicochetsText;


    private Dictionary<ElementType, Color> elementTypeColors = new Dictionary<ElementType, Color>
    {
        { ElementType.Normal, Color.gray },
        { ElementType.Fire, Color.red },
        { ElementType.Poison, new Color(0, 0.5f, 0) }, // Darker green
        { ElementType.Ice, Color.blue },
        { ElementType.Lightning, new Color(1, 0.5f, 0) } // Darker yellow
    };
    // Helper method to format the stat text
    private string FormatStatText(string label, float baseValue, float currentValue)
    {
        float upgradeValue = currentValue - baseValue;
        string baseColor = ColorUtility.ToHtmlStringRGBA(Color.black); // Black for base stats
        string upgradeColor = upgradeValue >= 0 ? ColorUtility.ToHtmlStringRGBA(Color.green) : ColorUtility.ToHtmlStringRGBA(Color.red); // Green for positive upgrades, red for negative

        // Bold the upgrade value text
        string upgradeText = upgradeValue >= 0 ? $"<b>+{upgradeValue:F2}</b>" : $"<b>{upgradeValue:F2}</b>";

        // Bold the entire sum expression
        return $"{label} <b>{currentValue:F2}</b> (<color=#{baseColor}>{baseValue:F2}</color> + <color=#{upgradeColor}>{upgradeText}</color>)";
    }

    // Call this method to update the panel with weapon stats
    public void ShowWeaponStats(WeaponData weaponData)
    {
        if (weaponData != null)
        {
            nameText.text = weaponData.Name;
            damageText.text = FormatStatText("Damage: ", weaponData.baseStats.damage, weaponData.stats.damage);
            timeToAttackText.text = FormatStatText("Time to Attack: ", weaponData.baseStats.timeToAttack, weaponData.stats.timeToAttack);
            numOfHitsText.text = FormatStatText("Number of Hits: ", weaponData.baseStats.numberOfHits, weaponData.stats.numberOfHits);
            numOfAttacksText.text = FormatStatText("Number of Attacks: ", weaponData.baseStats.numberOfAttacks, weaponData.stats.numberOfAttacks);
            projectileSpeedText.text = FormatStatText("Projectile Speed: ", weaponData.baseStats.projectileSpeed, weaponData.stats.projectileSpeed);
            elementTypeText.text = "Element Type: " + weaponData.stats.elementType.ToString(); // Element type may not need formatting
            elementTypeText.color = elementTypeColors.ContainsKey(weaponData.stats.elementType) ? elementTypeColors[weaponData.stats.elementType] : Color.white;
            elementalPotencyText.text = FormatStatText("Elemental Potency: ", weaponData.baseStats.elementalPotency, weaponData.stats.elementalPotency);
            damageOverTimeText.text = FormatStatText("Damage Over Time: ", weaponData.baseStats.damageOverTime, weaponData.stats.damageOverTime);
            critChanceText.text = FormatStatText("Crit Chance: ", weaponData.baseStats.critStrikeChance, weaponData.stats.critStrikeChance);
            critMultiplierText.text = FormatStatText("Crit Multiplier: ", weaponData.baseStats.critStrikeMultiplier, weaponData.stats.critStrikeMultiplier);
            maxRicochetsText.text = FormatStatText("Max Ricochets: ", weaponData.baseStats.maxRicochets, weaponData.stats.maxRicochets);
            maxSummonsText.text = FormatStatText("Max Summoned Instances: ", weaponData.baseStats.maxNumberOfSummonedInstances, weaponData.stats.maxNumberOfSummonedInstances);
        }
    }


    // Call this to hide the panel or reset it
    public void HideWeaponStats()
    {
        nameText.text = "";
        damageText.text = "";
        timeToAttackText.text = "";
        numOfHitsText.text = "";
        numOfAttacksText.text = "";
        projectileSpeedText.text = "";
        elementTypeText.text = "";
        elementTypeText.color = Color.white; // Reset to default color
        elementalPotencyText.text = "";
        damageOverTimeText.text = "";
        critChanceText.text = "";
        critMultiplierText.text = "";
        maxRicochetsText.text = "";
        maxSummonsText.text = "";
    }
}