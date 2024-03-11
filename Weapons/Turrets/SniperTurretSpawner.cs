using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTurretSpawner : TurretSpawnerBase
{
    protected override void SetupTurret(GameObject turretInstance)
    {
        SniperTurret sniperTurret = turretInstance.GetComponent<SniperTurret>();
        if (sniperTurret != null)
        {
            sniperTurret.weaponBaseReference = this;
        }
    }

    // If you need to override the Attack method, you can do so here.
    // Otherwise, the implementation in TurretSpawnerBase will be used.
}