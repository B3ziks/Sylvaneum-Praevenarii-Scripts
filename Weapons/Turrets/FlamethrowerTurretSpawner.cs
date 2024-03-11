using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerTurretSpawner : TurretSpawnerBase
{
    protected override void SetupTurret(GameObject turretInstance)
    {
        FlamethrowerTurret flamethrowerTurret = turretInstance.GetComponent<FlamethrowerTurret>();
        if (flamethrowerTurret != null)
        {
            flamethrowerTurret.weaponBaseReference = this;
        }
    }

    // If you need to override the Attack method, you can do so here.
    // Otherwise, the implementation in TurretSpawnerBase will be used.
}