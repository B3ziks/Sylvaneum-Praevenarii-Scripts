using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PermanentUpgradeUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerPersistentUpgrades upgradeType; // Identifier for the upgrade this UI element represents

    private string description; // Cache the description

    [SerializeField] private DataContainer dataContainer;

    private void Start()
    {


        // Fetch the description for this upgrade from the DataContainer
        PlayerUpgrades playerUpgrade = dataContainer.upgrades.Find(u => u.persistentUpgrades == upgradeType);
        if (playerUpgrade != null)
        {
            description = GetDescriptionForUpgrade(playerUpgrade); // Fetch or set the description accordingly
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PermanentUpgradeDescriptionManager.instance.ShowDescription(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PermanentUpgradeDescriptionManager.instance.HideDescription();
    }

    // Assuming you want a method to get a description based on the upgrade data
    private string GetDescriptionForUpgrade(PlayerUpgrades upgrade)
    {
        // Here you can format your description based on the upgrade data
        // For example:
        return $"Upgrades your {upgrade.persistentUpgrades} by: {upgrade.increasingValue} * {upgrade.level} \n (Your upgrade level) \n {upgrade.description} \n <b>Cost: {upgrade.costToUpgrade} coins </b> ";
    }
}