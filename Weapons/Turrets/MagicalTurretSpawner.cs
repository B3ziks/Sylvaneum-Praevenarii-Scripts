using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MagicalTurretSpawner : TurretSpawnerBase
{

    protected override void SetupTurret(GameObject turretInstance)
    {
        MagicalTurret magicalTurret = turretInstance.GetComponent<MagicalTurret>();
        if (magicalTurret != null)
        {
            magicalTurret.weaponBaseReference = this;
        }
    }


}