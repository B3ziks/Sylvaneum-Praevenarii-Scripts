using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTurretSpawner : TurretSpawnerBase
{
    protected override void SetupTurret(GameObject turretInstance)
    {
        LightningTurret lightningTurret = turretInstance.GetComponent<LightningTurret>();
        if (lightningTurret != null)
        {
            lightningTurret.weaponBaseReference = this;
        }
    }

    // If you need to override the Attack method, you can do so here.
    // Otherwise, the implementation in TurretSpawnerBase will be used.
}