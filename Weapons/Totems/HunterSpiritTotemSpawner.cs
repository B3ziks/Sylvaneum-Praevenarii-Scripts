using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSpiritTotemSpawner : TotemSpawnerBase
{
    protected override void SetupTotem(GameObject totemInstance)
    {
        HunterSpiritTotem hunterSpiritTotem = totemInstance.GetComponent<HunterSpiritTotem>();
        if (hunterSpiritTotem != null)
        {
            hunterSpiritTotem.weaponBaseReference = this;
        }
    }
}
