using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EngineerMeleeCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isSawbladeActive = false;

    [SerializeField] private WeaponData sawbladeWeaponData; // Assign this in the inspector
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
        if (sawbladeWeaponData == null) UnityEngine.Debug.LogError("SawbladeWeapon script not found on the child object.");
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
        weaponManager.AddWeapon(sawbladeWeaponData);
        levelInstance.equippedWeapons.Add(sawbladeWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivateSawblade()
    {
            isSawbladeActive = true;
    }

 
}