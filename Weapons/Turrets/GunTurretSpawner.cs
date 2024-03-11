using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GunTurretSpawner : TurretSpawnerBase
{
    protected override void SetupTurret(GameObject turretInstance)
    {
        GunTurret gunTurret = turretInstance.GetComponent<GunTurret>();
        if (gunTurret != null)
        {
            gunTurret.weaponBaseReference = this;
        }
    }

    // If you need to override the Attack method, you can do so here.
    // Otherwise, the implementation in TurretSpawnerBase will be used.
}