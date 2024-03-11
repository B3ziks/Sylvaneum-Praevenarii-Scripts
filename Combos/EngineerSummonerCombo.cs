using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerSummonerCombo : MonoBehaviour
{
    public bool isComboActive = false;

    private bool isPlantActive;
    [SerializeField] private WeaponData plantTurretWeaponData; // Assign this in the inspector
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
        if (plantTurretWeaponData == null) UnityEngine.Debug.LogError("SawbladeWeapon script not found on the child object.");
    }

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update()
    {

        if (isComboActive == true && isPlantActive == false)
        {
            ActivateCombo();
            ActivatePlant();
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
        weaponManager.AddWeapon(plantTurretWeaponData);
        levelInstance.equippedWeapons.Add(plantTurretWeaponData);
        // You can also handle other combo activation logic here

    }
    private void ActivatePlant()
    {
        isPlantActive = true;
    }
}