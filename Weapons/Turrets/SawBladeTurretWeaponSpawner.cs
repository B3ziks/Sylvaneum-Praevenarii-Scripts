using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeTurretWeaponSpawner : TurretSpawnerBase
{
    [SerializeField] private GameObject comboEngineerMelee; // Reference to the ComboMecha child GameObject

    protected override void SetupTurret(GameObject turretInstance)
    {
        SawBladeTurret sawbladeTurret = turretInstance.GetComponent<SawBladeTurret>();
        if (sawbladeTurret != null)
        {
            sawbladeTurret.weaponBaseReference = this;
        }
    }
    // Enables the weapon, allowing it to attack.
    public void EnableWeapon()
    {
        if (comboEngineerMelee != null)
        {
            comboEngineerMelee.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("comboMecha reference is null in EnableWeapon");
        }
        // Loop through each turret instance and activate them
        foreach (var turretInstance in turretInstances)
        {
            turretInstance.SetActive(true);
        }
    }

    // Disables the weapon, preventing it from attacking.
    public void DisableWeapon()
    {
        if (comboEngineerMelee != null)
        {
            comboEngineerMelee.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("comboMecha reference is null in DisableWeapon");
        }
        // Loop through each turret instance and deactivate them
        foreach (var turretInstance in turretInstances)
        {
            turretInstance.SetActive(false);
        }
    }
}