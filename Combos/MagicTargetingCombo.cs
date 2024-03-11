using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagicTargetingCombo : MonoBehaviour
{
    public bool isComboActive = false; 
    public bool isComboActivated = false;
    [SerializeField] private WeaponData magicTargetingWeaponData; // Assign this in the inspector
    private WeaponManager weaponManager;
    private Level levelInstance;

    private Character playerCharacter;

    private void Start()
    {
        levelInstance = Level.Instance;

        playerCharacter = GetComponent<Character>();


        if (playerCharacter == null)
        {
            UnityEngine.Debug.LogError("Character script not found on this object.");
        }
        if (magicTargetingWeaponData == null) UnityEngine.Debug.LogError("mecha gauntlet script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {

        if (isComboActive == true && !isComboActivated)
        {
            ActivateCombo();
        }
        else
        {
            return;
        }
    }

    public void SetComboActiveState(bool state)
    {
        isComboActive = state;
    }


    private void ActivateCombo()
    {
        // Add the sawblade weapon to the player's arsenal
        isComboActivated = true;
        weaponManager.AddWeapon(magicTargetingWeaponData);
        levelInstance.equippedWeapons.Add(magicTargetingWeaponData);
        // You can also handle other combo activation logic here

    }
   
}