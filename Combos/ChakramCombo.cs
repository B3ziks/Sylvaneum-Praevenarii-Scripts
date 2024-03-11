using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isChakramActive = false;

    [SerializeField] private WeaponData chakramWeaponData; // Assign this in the inspector
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
        if (chakramWeaponData == null) UnityEngine.Debug.LogError("mecha gauntlet script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {

        if (isComboActive == true && isChakramActive == false)
        {
            ActivateCombo();
            ActivateSawblade();
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
        weaponManager.AddWeapon(chakramWeaponData);
        levelInstance.equippedWeapons.Add(chakramWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivateSawblade()
    {
        isChakramActive = true;
    }
}