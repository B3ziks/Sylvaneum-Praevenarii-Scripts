using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        StormTotem stormTotem = totemInstance.GetComponent<StormTotem>();
        if (stormTotem != null)
        {
            stormTotem.weaponBaseReference = this;
        }
    }

}