using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUpgradeUIElement : MonoBehaviour
{
    [SerializeField] PlayerPersistentUpgrades upgrade;
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI price;

    [SerializeField] DataContainer dataContainer;

    private void Start()
    {
        UpdateElement();
    }

    public void Ugrade()
    {
        PlayerUpgrades playerUpgrades = dataContainer.upgrades[(int)upgrade];

        if(playerUpgrades.level >= playerUpgrades.max_level) { return; }
        if(dataContainer.coins >= playerUpgrades.costToUpgrade)
        {
            dataContainer.coins -= playerUpgrades.costToUpgrade;
            playerUpgrades.level += 1;
            if (playerUpgrades.level == 0)
            {
                playerUpgrades.costToUpgrade = 100;
            }
            else if (playerUpgrades.level == 1)
            {
                playerUpgrades.costToUpgrade = 200;
            }
            else
            {
                playerUpgrades.costToUpgrade *= (playerUpgrades.level);
            }
            UpdateElement();
        }
    }

    void UpdateElement()
    {
        PlayerUpgrades playerUpgrade = dataContainer.upgrades[(int)upgrade];


        upgradeName.text = upgrade.ToString();
        level.text = playerUpgrade.level.ToString();
        price.text =  "Cost: " + playerUpgrade.costToUpgrade.ToString();
    }
}
