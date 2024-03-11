using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        MeteorTotem meteorTotem = totemInstance.GetComponent<MeteorTotem>();
        if (meteorTotem != null)
        {
            meteorTotem.weaponBaseReference = this;
        }
    }
}
