using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UpgradeDescriptionPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] upgradeNameTexts = new TextMeshProUGUI[3];
    [SerializeField] TextMeshProUGUI[] upgradeDescriptions = new TextMeshProUGUI[3];
    [SerializeField] TextMeshProUGUI[] upgradeTags = new TextMeshProUGUI[3];

    private Dictionary<ElementType, Color> elementTypeColors = new Dictionary<ElementType, Color>
    {
        { ElementType.Normal, Color.gray },
        { ElementType.Fire, Color.red },
        { ElementType.Poison, new Color(0, 0.5f, 0) }, // Darker green
        { ElementType.Ice, Color.blue },
        { ElementType.Lightning, new Color(1, 0.5f, 0) } // Darker yellow
        // Add more colors for other element types if necessary
    };

    public void Set(List<UpgradeData> upgrades)
    {
        for (int i = 0; i < 3; i++) // Handle all three text arrays
        {
            if (i < upgrades.Count) // If there's upgrade data for this index
            {
                upgradeNameTexts[i].text = upgrades[i].Name;
                upgradeDescriptions[i].text = upgrades[i].description;
                if (upgrades[i].weaponData != null)
                {
                    ElementType elementType = upgrades[i].weaponData.stats.elementType;
                    upgradeTags[i].text = string.Format("{0}, {1}", upgrades[i].weaponData.weaponType, elementType);
                    upgradeTags[i].color = elementTypeColors.ContainsKey(elementType) ? elementTypeColors[elementType] : Color.white; // Default to white if not defined
                }
            }
            else // If there's no upgrade data for this index, clear the texts
            {
                upgradeNameTexts[i].text = "";
                upgradeDescriptions[i].text = "";
                upgradeTags[i].text = "";
                upgradeTags[i].color = Color.white; // Reset color to default
            }
        }
    }
}