using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurretSpawner : TurretSpawnerBase
{

    protected override void SetupTurret(GameObject turretInstance)
    {
        RocketTurret rocketTurret = turretInstance.GetComponent<RocketTurret>();
        if (rocketTurret != null)
        {
            rocketTurret.weaponBaseReference = this;
        }
    }
 
}