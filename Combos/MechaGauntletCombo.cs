using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MechaGauntletCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isSawbladeActive = false;

    [SerializeField] private WeaponData mechaGauntletWeaponData; // Assign this in the inspector
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
        if (mechaGauntletWeaponData == null) UnityEngine.Debug.LogError("mecha gauntlet script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {

        if (isComboActive == true && isSawbladeActive == false)
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
        weaponManager.AddWeapon(mechaGauntletWeaponData);
        levelInstance.equippedWeapons.Add(mechaGauntletWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivateSawblade()
    {
        isSawbladeActive = true;
    }
}