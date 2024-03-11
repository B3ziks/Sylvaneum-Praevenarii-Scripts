using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FrostTurretSpawner : TurretSpawnerBase
{
    protected override void SetupTurret(GameObject turretInstance)
    {
        FrostTurret frostTurret = turretInstance.GetComponent<FrostTurret>();
        if (frostTurret != null)
        {
            frostTurret.weaponBaseReference = this;
        }
    }

    // If you need to override the Attack method, you can do so here.
    // Otherwise, the implementation in TurretSpawnerBase will be used.
}