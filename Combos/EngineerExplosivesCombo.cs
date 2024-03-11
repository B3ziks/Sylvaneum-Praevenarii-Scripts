using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerExplosivesCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isRocketTurretActive;
    [SerializeField] private WeaponData rocketTurretWeaponData; // Assign this in the inspector
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
        if (rocketTurretWeaponData == null) UnityEngine.Debug.LogError("SawbladeWeapon script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }
    private void Update()
    {

        if (isComboActive == true && isRocketTurretActive == false)
        {
            ActivateCombo();
            ActivateRocket();
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
        weaponManager.AddWeapon(rocketTurretWeaponData);
        levelInstance.equippedWeapons.Add(rocketTurretWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivateRocket()
    {
        isRocketTurretActive = true;
    }

}