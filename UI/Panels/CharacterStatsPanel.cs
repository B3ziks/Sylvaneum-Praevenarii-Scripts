using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterStatsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI hpRegenText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI movementSpeedText;
    [SerializeField] private TextMeshProUGUI goldGainText;

    [SerializeField] private Character character; // Reference to the Character script

    private void OnEnable()
    {
        UpdateStatsDisplay();
    }

    public void UpdateStatsDisplay()
    {
        if (character != null)
        {
            hpText.text = FormatStatText("HP: ", character.baseMaxHp, character.maxHp);
            hpRegenText.text = FormatStatText("HP Regen: ", character.baseHpRegenerationRate, character.hpRegenerationRate);
            armorText.text = FormatStatText("Armor: ", character.baseArmor, character.armor);
            movementSpeedText.text = FormatStatText("Movement Speed: ", character.baseMovementSpeed, character.movementSpeed);
            goldGainText.text = FormatStatText("Gold gain: ", character.baseGoldGain, character.goldGain);
        }
    }
    private string FormatStatText(string label, float baseValue, float currentValue)
    {
        float upgradeValue = currentValue - baseValue;
        string baseColor = ColorUtility.ToHtmlStringRGBA(Color.black); // Black for base stats
        string upgradeColor = upgradeValue >= 0 ? ColorUtility.ToHtmlStringRGBA(Color.green) : ColorUtility.ToHtmlStringRGBA(Color.red); // Green for positive upgrades, red for negative

        string upgradeText = upgradeValue >= 0 ? $"+{upgradeValue:F2}" : $"{upgradeValue:F2}";
        // Bold the sum part
        return $"{label} <b>{currentValue:F2}</b> (<color=#{baseColor}>{baseValue:F2}</color> + <color=#{upgradeColor}>{upgradeText}</color>)";
    }
}