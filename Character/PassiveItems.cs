using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    [SerializeField] public List<Item> items;

    Character character;


    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        
    }

    public void Equip(Item itemToEquip)
    {
        if (items == null)
        {
            items = new List<Item>();
        }

        // Assuming Item has a copy constructor or a method to make a deep copy
        Item newItemInstance = Instantiate(itemToEquip); // This is a Unity method that clones ScriptableObjects.

        items.Add(newItemInstance);
        newItemInstance.Equip(character);
    }


    internal void UpgradeItem(UpgradeData upgradeData)
    {
        foreach (var item in items)
        {
            UnityEngine.Debug.Log("Item in list: " + item.Name);
        }
        Item itemToUpgrade = items.Find(id => id.Name == upgradeData.item.Name);
        if (itemToUpgrade != null)
        {
            itemToUpgrade.stats.Sum(upgradeData.itemStats);
            itemToUpgrade.Equip(character);
        }
        else
        {
            UnityEngine.Debug.LogError("No item found to upgrade with name: " + upgradeData.item.Name);
        }
    }

}
