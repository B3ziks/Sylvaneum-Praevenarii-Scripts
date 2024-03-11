using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UpgradesEncyclopediaPanel : MonoBehaviour
{
    [SerializeField] private UpgradesUITable upgradesTable;
    [SerializeField] private WeaponsUITable weaponsTable;
    [SerializeField] private GameObject upgradeDisplayPrefab;
    [SerializeField] private GameObject weaponDisplayPrefab;  // new field for weapon prefab
    [SerializeField] private Transform upgradesContainer;
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private TextMeshProUGUI descriptionPanelText;  // reference to the global description panel
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button weaponsButton;
    //
    [SerializeField] private TMP_Dropdown filterDropdown; // Reference to the TMP Dropdown component
    [SerializeField] private GameObject upgradesScrollView; // Reference to the upgrades scroll view
    [SerializeField] private GameObject weaponsScrollView;  // Reference to the weapons scroll view

    private void Start()
    {
        PopulateDropdown();
        PopulateItems(); // Single method for both upgrades and weapons

        upgradesButton.onClick.AddListener(() => DisplayItems(upgradesContainer));
        weaponsButton.onClick.AddListener(() => DisplayItems(weaponsContainer));

        // Default display - Weapons are displayed by default
        DisplayItems(weaponsContainer);

        // Filter items based on the first weapon type in the dropdown list
        FilterItemsByType(filterDropdown.options[0].text);
    }

    private void PopulateDropdown()
    {
        // Assuming that you have a list of all weapon types
        List<string> weaponTypes = weaponsTable.weapons.Select(w => w.weaponType.ToString()).Distinct().ToList();

        filterDropdown.ClearOptions(); // Clear existing options
        filterDropdown.AddOptions(weaponTypes); // Add new options

        // Add listener for dropdown changes
        filterDropdown.onValueChanged.AddListener(delegate {
            FilterItemsByType(filterDropdown.options[filterDropdown.value].text);
        });
    }
    private void FilterItemsByType(string weaponType)
    {
        // Weapons
        foreach (Transform item in weaponsContainer)
        {
            WeaponsEncyclopediaDisplay weaponDisplay = item.GetComponent<WeaponsEncyclopediaDisplay>();
            if (weaponDisplay != null)
            {
                bool shouldDisplay = weaponType == "All" || weaponDisplay.WeaponData.weaponType.ToString() == weaponType;
                item.gameObject.SetActive(shouldDisplay);
            }
        }

        // Upgrades
        foreach (Transform item in upgradesContainer)
        {
            UpgradesEncyclopediaDisplay upgradeDisplay = item.GetComponent<UpgradesEncyclopediaDisplay>();
            if (upgradeDisplay != null)
            {
                // If your UpgradeData has a field that relates to weapon types, use that for filtering
                // For example, assuming upgradeData.ApplicableWeaponTypes is a List<string> of weapon types
                bool shouldDisplay = weaponType == "All" || upgradeDisplay.UpgradeData.weaponType.ToString() == weaponType;
                item.gameObject.SetActive(shouldDisplay);
            }
        }
    }

    //
    private void PopulateItems()
    {
        // Populating Upgrades
        foreach (var upgrade in upgradesTable.upgrades)
        {
            GameObject upgradeObj = Instantiate(upgradeDisplayPrefab, upgradesContainer);
            UpgradesEncyclopediaDisplay display = upgradeObj.GetComponent<UpgradesEncyclopediaDisplay>();
            if (display != null)
            {
                display.Setup(upgrade);
                display.SetDescriptionPanelText(descriptionPanelText);
            }
        }

        // Populating Weapons
        foreach (var weapon in weaponsTable.weapons)
        {
            GameObject weaponObj = Instantiate(weaponDisplayPrefab, weaponsContainer);
            WeaponsEncyclopediaDisplay display = weaponObj.GetComponent<WeaponsEncyclopediaDisplay>();
            if (display != null)
            {
                display.Setup(weapon);
                display.SetDescriptionPanelText(descriptionPanelText);
            }
        }
    }

    private void DisplayItems(Transform itemsContainer)
    {
        // Hide all scroll views and containers first
        upgradesContainer.gameObject.SetActive(false);
        weaponsContainer.gameObject.SetActive(false);
        upgradesScrollView.SetActive(false);  // Hide upgrades scroll view
        weaponsScrollView.SetActive(false);   // Hide weapons scroll view

        // Now display the chosen one
        itemsContainer.gameObject.SetActive(true);

        // Activate the corresponding scroll view
        if (itemsContainer == upgradesContainer)
        {
            upgradesScrollView.SetActive(true);
        }
        else if (itemsContainer == weaponsContainer)
        {
            weaponsScrollView.SetActive(true);
        }
    }
}
