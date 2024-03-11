using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTurretSpawner : TurretSpawnerBase
{

    [SerializeField] private GameObject comboEngineerSummoner; // Reference to the ComboMecha child GameObject
    protected override void SetupTurret(GameObject turretInstance)
    {
        PlantTurret plantTurret = turretInstance.GetComponent<PlantTurret>();
        if (plantTurret != null)
        {
            plantTurret.weaponBaseReference = this;
        }
    }


    // Enables the weapon, allowing it to attack.
    public void EnableWeapon()
    {
        if (comboEngineerSummoner != null)
        {
            comboEngineerSummoner.SetActive(true);
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
        if (comboEngineerSummoner != null)
        {
            comboEngineerSummoner.SetActive(false);
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