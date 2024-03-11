using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquippedItemsUI : MonoBehaviour
{
    [Header("Equipped Item Prefabs")]
    [SerializeField] private GameObject equippedItemPrefab;
    [SerializeField] private GameObject comboWeaponPrefab; // Assign this in the inspector

    [Header("UI References")]
    [SerializeField] private Transform equippedItemsContainer;
    [SerializeField] private WeaponStatsPanel weaponStatsPanel; // Reference to the WeaponStatsPanel script

    private List<Image> equippedItemImages = new List<Image>();
    private List<WeaponData> equippedWeapons = new List<WeaponData>();
    private Level levelInstance;

    private Image comboWeaponImage; // For the combo weapon slot

    private void Start()
    {
        levelInstance = Level.Instance;

        // Initialize the equipped item images
        for (int i = 0; i < 4; i++)
        {
            GameObject itemInstance = Instantiate(equippedItemPrefab, equippedItemsContainer);
            Image itemImage = itemInstance.GetComponent<Image>();
            EventTrigger trigger = itemInstance.AddComponent<EventTrigger>();
            SetupEventTrigger(trigger, i);

            equippedItemImages.Add(itemImage);
            itemInstance.SetActive(false);
        }

        // Initialize the combo weapon slot
        GameObject comboItemInstance = Instantiate(comboWeaponPrefab, equippedItemsContainer);
        comboWeaponImage = comboItemInstance.GetComponent<Image>();
        EventTrigger comboTrigger = comboItemInstance.AddComponent<EventTrigger>();
        SetupEventTrigger(comboTrigger, 4); // 4 is the index for the combo weapon
        comboItemInstance.SetActive(false);
    }

    private void Update()
    {
        RefreshEquippedItems();
    }

    private void SetupEventTrigger(EventTrigger trigger, int index)
    {
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { OnPointerEnterItem(index); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => { OnPointerExitItem(); });
        trigger.triggers.Add(entryExit);
    }

    private void OnPointerEnterItem(int index)
    {
        // Check if the WeaponStatsPanel is active
        if (weaponStatsPanel.gameObject.activeInHierarchy && index >= 0 && index < equippedWeapons.Count)
        {
            weaponStatsPanel.ShowWeaponStats(equippedWeapons[index]);
        }
    }

    private void OnPointerExitItem()
    {
        weaponStatsPanel.HideWeaponStats();
    }

    public void UpdateEquippedItems(List<Sprite> equippedSprites, List<WeaponData> weapons)
    {
        equippedWeapons = weapons;

        for (int i = 0; i < equippedItemImages.Count; i++)
        {
            if (i < equippedSprites.Count && equippedSprites[i] != null)
            {
                equippedItemImages[i].sprite = equippedSprites[i];
                equippedItemImages[i].gameObject.SetActive(true);
            }
            else
            {
                equippedItemImages[i].gameObject.SetActive(false);
            }
        }

        // If there's a combo weapon in the list, activate its image and set the sprite
        if (equippedWeapons.Count == 5) // Assuming the fifth slot is reserved for the combo weapon
        {
            comboWeaponImage.sprite = equippedWeapons[4].Image; // The combo weapon should be the fifth weapon in the list
            comboWeaponImage.gameObject.SetActive(true);
        }
        else
        {
            comboWeaponImage.gameObject.SetActive(false);
        }
    }

    private void RefreshEquippedItems()
    {
        if (levelInstance == null) return;

        List<Sprite> equippedSprites = new List<Sprite>();
        List<WeaponData> weapons = new List<WeaponData>();

        foreach (var weapon in levelInstance.equippedWeapons)
        {
            equippedSprites.Add(weapon.Image);
            weapons.Add(weapon);
        }

        UpdateEquippedItems(equippedSprites, weapons);
    }
    //
    public void AddComboWeapon(WeaponData comboWeaponData)
    {
        if (!equippedWeapons.Contains(comboWeaponData))
        {
            equippedWeapons.Add(comboWeaponData);
            RefreshEquippedItems();
        }
    }

    public void RemoveComboWeapon(WeaponData comboWeaponData)
    {
        if (equippedWeapons.Contains(comboWeaponData))
        {
            equippedWeapons.Remove(comboWeaponData);
            RefreshEquippedItems();
        }
    }
}