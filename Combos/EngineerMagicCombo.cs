using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerMagicCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isMagicTurretActive = false;

    [SerializeField] private WeaponData magicTurretWeaponData; // Assign this in the inspector
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
        if (magicTurretWeaponData == null) UnityEngine.Debug.LogError("mecha gauntlet script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {

        if (isComboActive == true && isMagicTurretActive == false)
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
        weaponManager.AddWeapon(magicTurretWeaponData);
        levelInstance.equippedWeapons.Add(magicTurretWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivateSawblade()
    {
        isMagicTurretActive = true;
    }
}